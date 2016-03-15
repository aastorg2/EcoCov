using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using Covana;

namespace GuidanceSimulator
{
    public partial class GuidanceMainWindow : Form
    {
        List<OCPSimulationData> ocpdatum;
        Dictionary<string, PUT> puts;
        private List<EMCPSimuationData> emcpdatum;

        public GuidanceMainWindow()
        {
            InitializeComponent();
        }

        private void GuidanceMainWindow_Load(object sender, EventArgs e)
        {
            UpdateProblemTree();
            UpdatePUTTree();
        }

        private void UpdateProblemTree()
        {
            XmlSerializer ocpSerializer = new XmlSerializer(typeof(List<OCPSimulationData>));
            ocpdatum = (List<OCPSimulationData>)ocpSerializer.Deserialize(File.OpenRead(@"c:\tempseqex\ocpproblems.xml"));


            XmlSerializer emcpSerializer = new XmlSerializer(typeof(List<EMCPSimuationData>));
            emcpdatum = (List<EMCPSimuationData>)emcpSerializer.Deserialize(File.OpenRead(@"c:\tempseqex\emcpproblems.xml"));

            XmlSerializer putserializer = new XmlSerializer(typeof(List<PUT>));
            puts = new Dictionary<string, PUT>();

            var readputs = (List<PUT>)putserializer.Deserialize(File.OpenRead(@"c:\tempseqex\puts.xml"));
            foreach (var put in readputs)
            {
                puts.Add(put.Name, put);
            }

            problemTree.BeginUpdate();
            problemTree.Nodes.Clear();
            foreach (var data in ocpdatum)
            {
                var problem = data.Problem;

                var node = new TreeNode(problem.Kind + " : " + problem.Type);
                node.Tag = data;
                problemTree.Nodes.Add(node);
                foreach (var branch in data.Branches)
                {
                    string text = branch.Document + ":" + branch.Line + ", ILOffset: " + branch.ILOffset;
                    var treeNode = new TreeNode(text);
                    treeNode.Tag = node.Tag;
                    node.Nodes.Add(treeNode);
                }
            }

            foreach (var data in emcpdatum)
            {
                var problem = data.Problem;

                var node = new TreeNode(problem.Kind + " : " + problem.Type);
                node.Tag = data;
                problemTree.Nodes.Add(node);
                foreach (var branch in data.Branches)
                {
                    string text = branch.Document + ":" + branch.Line + ", ILOffset: " + branch.ILOffset;
                    var treeNode = new TreeNode(text);
                    treeNode.Tag = node.Tag;
                    node.Nodes.Add(treeNode);
                }
            }

            problemTree.EndUpdate();
            problemTree.ExpandAll();
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            UpdateProblemTree();
            UpdatePUTTree();
        }

        private void problemTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Get the node at the current mouse pointer location.
            TreeNode theNode = this.problemTree.GetNodeAt(e.X, e.Y);

            // Set a ToolTip only if the mouse pointer is actually paused on a node.
            if ((theNode != null))
            {
                // Verify that the tag property is not "null".
                if (theNode.Tag != null)
                {
                    if (theNode.Tag is OCPSimulationData)
                    {
                        var data = (OCPSimulationData)theNode.Tag;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Para Type: " + data.ParaType);
                        sb.Append("Access Path: para.");
                        var reverselist = new List<string>(data.AccessPath);
                        reverselist.Reverse();
                        reverselist.ForEach(x => sb.Append(x + "."));
                        sb.Remove(sb.Length - 1, 1);
                        sb.AppendLine();
                        sb.AppendLine("Last Field: " + data.LastField);
                        sb.AppendLine("Last Field Type: " + data.LastFieldType);
                        sb.AppendLine("Problem Detail: ");
                        sb.AppendLine(data.Problem.Description);
                        detailTextbox.Text = sb.ToString();
                    }

                    if (theNode.Tag is EMCPSimuationData)
                    {
                        var data = (EMCPSimuationData)theNode.Tag;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Result Type: " + data.ReturnType);
                        sb.Append("Paras: ");
                        data.Paras.ForEach(x => sb.Append(x.Type + " " + x.Name + ", "));
                        sb.Remove(sb.Length - 2, 2);
                        sb.AppendLine();
                        sb.AppendLine(data.Problem.Description);
                        detailTextbox.Text = sb.ToString();
                    }
                }
            }
        }

        private void UpdatePUTTree()
        {
            putTree.BeginUpdate();
            putTree.Nodes.Clear();
            foreach (var put in puts.Values)
            {
                var node = new TreeNode(put.Name);
                putTree.Nodes.Add(node);
                var node1 = new TreeNode(put.Document);
                node1.Tag = put;
                node.Nodes.Add(node1);
                var node2 = new TreeNode("FirstLine: " + put.FirstLine);
                node2.Tag = put;
                node.Nodes.Add(node2);
                node.Tag = put;
            }
            putTree.EndUpdate();
            putTree.ExpandAll();
        }

        private string SynthesizeFixSimulationForOCP(OCPSimulationData data, PUT put)
        {
            var ocpsymboliccopyTemplate = File.ReadAllText("OCPSymbolicCopyTemplate.txt");
            var ocpsetfieldTemplate = File.ReadAllText("OCPSetFieldTemplate.txt");
            var pexsetfieldTemplate = File.ReadAllText("PexSetFieldTemplate.txt");

            var textsb = new StringBuilder();

            string paraname = "";
            bool found = false;
            foreach (var para in put.Paras)
            {
                if (para.Type.Equals(data.ParaType))
                {
                    found = true;
                    paraname = para.Name;
                }
            }

            if (!found)
            {
                return "This PUT does not need synthesis simulation of OCP";
            }

            var copytext = string.Format(ocpsymboliccopyTemplate, data.ParaType, paraname);
            // textsb.AppendLine(copytext);

            string setfieldtext = string.Empty;
            int j = data.AccessPath.Count;
            if (string.IsNullOrEmpty(data.LastField))
            {
                textsb.AppendLine(copytext);
            }
            else
            {
                var lastfieldType = data.LastFieldType;
                if (lastfieldType.Contains("["))
                {
                    var split = lastfieldType.Split(new[] { '`' });
                    var mainType = split[0];
                    var nbrGenerics = split[1].Split(new[] { '[' })[0];
                    var genericTypesString = split[1].Split(new[] { '[' })[1].Replace("]","");
                    lastfieldType = mainType + "<" + genericTypesString + ">";

                }

                for (int i = 0; i < data.AccessPath.Count; i++)
                {
                    if (i == 0)
                    {
                        var lastfield = j - 1 == 0 ? paraname : "fieldobj" + (j - 1);
                        string pexsetfield = string.Format(pexsetfieldTemplate, lastfieldType, ("fieldobj" + j), data.LastField);

                        var textbody = "if (fieldobj" + j + " != null){" + copytext + "\r\n" + pexsetfield + "}";
                        setfieldtext = string.Format(ocpsetfieldTemplate, lastfield, data.AccessPath[i], j, textbody);
                    }
                    else if (i == data.AccessPath.Count - 1) // last
                    {
                        setfieldtext = string.Format(ocpsetfieldTemplate, paraname, data.AccessPath[i], j, setfieldtext);
                    }
                    else
                    {
                        var lastfield = "fieldobj" + (j - 1);
                        setfieldtext = string.Format(ocpsetfieldTemplate, lastfield, data.AccessPath[i], j, setfieldtext);
                    }
                    j--;
                }

                if (string.IsNullOrEmpty(setfieldtext)) // field of the program input
                {
                    var textbody = "if (" + paraname + " != null){" + string.Format(pexsetfieldTemplate, lastfieldType, paraname, data.LastField) + "}";
                    setfieldtext = textbody;
                }
                textsb.AppendLine(setfieldtext);
            }
            return textsb.ToString();
        }

        private void putTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Get the node at the current mouse pointer location.
            TreeNode theNode = this.putTree.GetNodeAt(e.X, e.Y);

            // Set a ToolTip only if the mouse pointer is actually paused on a node.
            if ((theNode != null))
            {
                // Verify that the tag property is not "null".
                if (theNode.Tag != null)
                {
                    if (problemTree.SelectedNode != null)
                    {
                        var put = (PUT)theNode.Tag;
                        var text = "";
                        if (problemTree.SelectedNode.Tag is OCPSimulationData)
                        {
                            var data = (OCPSimulationData)problemTree.SelectedNode.Tag;

                            text = SynthesizeFixSimulationForOCP(data, put);
                        }
                        if (problemTree.SelectedNode.Tag is EMCPSimuationData)
                        {
                            var data = (EMCPSimuationData)problemTree.SelectedNode.Tag;

                            text = SynthesizeFixSimulationForEMCP(data, put);
                        }

                        synthesistxtBox.Text = text;
                    }
                }
            }
        }

        private string SynthesizeFixSimulationForEMCP(EMCPSimuationData data, PUT put)
        {
            var emcpMoleTemplate = File.ReadAllText("EMCPMoleTemplate.txt");
            var paratext = "";
            for (int i = 0; i < data.Paras.Count; i++)
            {
                var para = data.Paras[i];
                if (para.IsNumeric)
                {
                    paratext += "0,";
                }
                else if (para.IsBool)
                {
                    paratext += "false,";
                }
                else if (para.IsChar)
                {
                    paratext += "'a',";
                }
                else
                {
                    paratext += "null,";
                }
            }
            paratext = paratext.Remove(paratext.Length - 1);

            return string.Format(emcpMoleTemplate, data.ReturnType, data.Problem.Type, paratext);
        }

        private void synthesisBtn_Click(object sender, EventArgs e)
        {
            TreeNode node = problemTree.SelectedNode;
            if (node == null)
            {
                return;
            }

            if (node.Tag != null)
            {

            }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void detailTextbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void synthesistxtBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void putTree_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
