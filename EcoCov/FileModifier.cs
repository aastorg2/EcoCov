using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Covana;
using Covana.EconomicAnalyzer;
using Covana.CoverageExtractor;

namespace EcoCov
{
    public class FileModifier
    {
        public static void CopyDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            if (sourceDir[sourceDir.Length - 1] != '\\')
            {
                sourceDir += '\\';
            }

            if (destDir[destDir.Length - 1] != '\\')
            {
                destDir += '\\';
            }

            DirectoryInfo sourceDirInfo = new DirectoryInfo(sourceDir);
            DirectoryInfo destDirInfo = new DirectoryInfo(destDir);

            foreach (var fileInfo in sourceDirInfo.GetFiles())
            {
                string destPath = Path.Combine(destDir + fileInfo.Name);
                fileInfo.CopyTo(destPath, true);
            }

            foreach (var dirInfo in sourceDirInfo.GetDirectories())
            {
                string destPath = Path.Combine(destDir + dirInfo.Name);
                CopyDirectory(dirInfo.FullName, destPath);
            }
        }

        public static void AddUsingStatements(string file, string[] referenceNames)
        {
            List<string> existingReferences = new List<string>();
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                string[] tokens = line.Split();
                if (tokens[0] == "namespace")
                {
                    break;
                }
                if (tokens[0] == "using")
                {
                    for (int j = 1; j < tokens.Length; j++)
                    {
                        if (tokens[j].Length >= 1 && tokens[j][tokens[j].Length - 1] == ';')
                        {
                            existingReferences.Add(tokens[j].Substring(0, tokens[j].Length - 1));
                            break;
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            bool added = false;
            foreach (string reference in referenceNames)
            {
                if (!existingReferences.Contains(reference))
                {
                    added = true;
                    sb.AppendLine("using " + reference + ";");
                }
            }
            if (added)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    sb.AppendLine(lines[i]);
                }
                File.WriteAllText(file, sb.ToString());
            }
        }

        public static void AddPexAttribute(string file)
        {
            string[] lines = File.ReadAllLines(file);
            StringBuilder sb = new StringBuilder();
            bool added=false;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                string[] tokens = line.Split();
                if (tokens[0]=="public")
                {
                    string start = lines[i].Substring(0, lines[i].IndexOf("public"));
                    if (tokens.Contains("class"))
                    {
                        string className="";
                        bool flag=false;
                        for (int j = 0; j < tokens.Length; j++)
                        {
                            if (tokens[j] == "class")
                            {
                                flag = true;
                                continue;
                            }
                            if (flag && tokens[j] != "")
                            {
                                className = tokens[j];
                                break;
                            }
                        }

                        List<string> existingAttributes = new List<string>();
                        int index = i - 1;
                        while (index>=0)
                        {
                            string l = lines[index].Trim();
                            if (l.Length > 2 
                                && l[0] == '[' && l[l.Length - 1] == ']')
                            {
                                string attr=l.Substring(1, l.Length-2).Trim();
                                existingAttributes.Add(attr);
                            }
                            else if (l != "")
                            {
                                break;
                            }
                            index--;
                        }
                        bool contains = false;
                        foreach (string attr in existingAttributes)
                        {
                            if(attr.Contains("PexClass"))
                            {
                                contains = true;
                                break;
                            }
                        }
                        if (!contains)
                        {
                            added = true;
                            sb.AppendLine(start+"[PexClass(typeof("+className+"))]");
                        }
                    }else if(line.Contains('('))
                    {
                        List<string> existingAttributes = new List<string>();
                        int index = i - 1;
                        while (index >= 0)
                        {
                            string l = lines[index].Trim();
                            if (l.Length > 2
                                && l[0] == '[' && l[l.Length - 1] == ']')
                            {
                                string attr = l.Substring(1, l.Length - 2).Trim();
                                existingAttributes.Add(attr);
                            }
                            else if (l != "")
                            {
                                break;
                            }
                            index--;
                        }
                        bool contains = false;
                        foreach (string attr in existingAttributes)
                        {
                            if (attr.Contains("PexMethod"))
                            {
                                contains = true;
                                break;
                            }
                        }
                        if (!contains)
                        {
                            added = true;
                            sb.AppendLine(start + "[PexMethod]");
                        }
                    }
                }
                sb.AppendLine(lines[i]);
            }
            if (added)
            {
                File.WriteAllText(file, sb.ToString());
            }
        }

        public static void AddPexAttribute(string file, string[] classes, string[] methods)
        {
            if((classes==null||classes.Length==0) && (methods==null||methods.Length==0)){
                AddPexAttribute(file);
                return;
            }
            string[] lines = File.ReadAllLines(file);
            StringBuilder sb = new StringBuilder();
            bool added = false;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                string[] tokens = line.Split();
                if (tokens[0] == "public")
                {
                    string start = lines[i].Substring(0, lines[i].IndexOf("public"));
                    if (classes!=null && classes.Length!=0 && tokens.Contains("class"))
                    {
                        string className = "";
                        bool flag = false;
                        for (int j = 0; j < tokens.Length; j++)
                        {
                            if (tokens[j] == "class")
                            {
                                flag = true;
                                continue;
                            }
                            if (flag && tokens[j] != "")
                            {
                                className = tokens[j];
                                break;
                            }
                        }

                        if (classes.Contains(className))
                        {
                            List<string> existingAttributes = new List<string>();
                            int index = i - 1;
                            while (index >= 0)
                            {
                                string l = lines[index].Trim();
                                if (l.Length > 2
                                    && l[0] == '[' && l[l.Length - 1] == ']')
                                {
                                    string attr = l.Substring(1, l.Length - 2).Trim();
                                    existingAttributes.Add(attr);
                                }
                                else if (l != "")
                                {
                                    break;
                                }
                                index--;
                            }
                            bool contains = false;
                            foreach (string attr in existingAttributes)
                            {
                                if (attr.Contains("PexClass"))
                                {
                                    contains = true;
                                    break;
                                }
                            }
                            if (!contains)
                            {
                                added = true;
                                sb.AppendLine(start + "[PexClass(typeof(" + className + "))]");
                            }
                        }
                    }
                    else if (methods != null && methods.Length != 0 && line.Contains('('))
                    {
                        string methodName = "";
                        string temp = line.Substring(0, line.IndexOf('(')).Trim();
                        string[] tks = temp.Split();
                        methodName = tks[tks.Length-1]; 

                        if(methods.Contains(methodName)){
                            List<string> existingAttributes = new List<string>();
                            int index = i - 1;
                            while (index >= 0)
                            {
                                string l = lines[index].Trim();
                                if (l.Length > 2
                                    && l[0] == '[' && l[l.Length - 1] == ']')
                                {
                                    string attr = l.Substring(1, l.Length - 2).Trim();
                                    existingAttributes.Add(attr);
                                }
                                else if (l != "")
                                {
                                    break;
                                }
                                index--;
                            }
                            bool contains = false;
                            foreach (string attr in existingAttributes)
                            {
                                if (attr.Contains("PexMethod"))
                                {
                                    contains = true;
                                    break;
                                }
                            }
                            if (!contains)
                            {
                                added = true;
                                sb.AppendLine(start + "[PexMethod]");
                            }
                        }
                    }
                }
                sb.AppendLine(lines[i]);
            }
            if (added)
            {
                File.WriteAllText(file, sb.ToString());
            }
        }

        public static void SynthesizeCodeForEMCP(List<PUT> PUTtoModify, EMCPSimuationData emcp, string projectDir, string destDir)
        {
            Dictionary<string, Dictionary<int, List<string>>> codeToAdd =
                new Dictionary<string, Dictionary<int, List<string>>>();
            foreach (var put in PUTtoModify)
            {
                string code = SynthesizeFixSimulationForEMCP(emcp, put);
                string newDocument = put.Document.Replace(projectDir, destDir);
                if (codeToAdd.ContainsKey(newDocument))
                {
                    Dictionary<int, List<string>> placeHolder = codeToAdd[newDocument];
                    if (placeHolder.ContainsKey(put.FirstLine))
                    {
                        placeHolder[put.FirstLine].Add(code);
                    }
                    else
                    {
                        List<string> list = new List<string>();
                        list.Add(code);
                        placeHolder.Add(put.FirstLine, list);
                    }
                }
                else
                {
                    Dictionary<int, List<string>> placeHolder = new Dictionary<int, List<string>>();
                    List<string> list = new List<string>();
                    list.Add(code);
                    placeHolder.Add(put.FirstLine, list);
                    codeToAdd.Add(newDocument, placeHolder);
                }
            }

            string problemType = emcp.Problem.Type;
            string moledType = problemType.Substring(0, problemType.LastIndexOf('.'));
            foreach (var document in codeToAdd.Keys)
            {
                Dictionary<int, List<string>> placeHolder = codeToAdd[document];
                List<string> content = new List<string>();
                string[] lines = File.ReadAllLines(document);
                bool assemblyAdded = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    string[] tokens = line.Split();
                    line = line.Replace(" ", "");
                    if (line == "[assembly:MoledType(typeof("+
                        moledType+
                        "))]")
                    {
                        assemblyAdded = true;
                        break;
                    }
                    if (tokens[0] == "namespace")
                    {
                        break;
                    }
                }
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (!assemblyAdded)
                    {
                        string[] tokens = line.Split();
                        if (tokens[0] == "namespace")
                        {
                            content.Add("[assembly: MoledType(typeof(" + moledType + "))]");
                            assemblyAdded = true;
                        }
                    }
                    content.Add(lines[i]);
                    
                    if (placeHolder.ContainsKey(i + 1))
                    {
                        int idxOfStart = -1;
                        bool passedStart = false;
                        //bool moldHostAdded = false;
                        for (int j = content.Count-1; j>=0; j--)
                        {
                            string ln = content[j].Trim();
                            if (ln.StartsWith("public"))
                            {
                                idxOfStart=j;
                                passedStart = true;
                            }
                            if (passedStart)
                            {
                                if (ln != "")
                                {
                                    string temp = ln.Replace(" ", "");
                                    if (temp == "[HostType(\"Moles\")]")
                                    {
                                        break;
                                    }
                                    else if (ln[0] != '[')
                                    {
                                        content.Insert(j, "[HostType(\"Moles\")]");
                                        break;
                                    }
                                }
                            }
                        }
                        foreach (var code in placeHolder[i + 1])
                        {
                            content.Add(code);
                        }
                    }
                }
                StringBuilder sb = new StringBuilder();
                foreach (var line in content)
                {
                    sb.AppendLine(line);
                }
                File.WriteAllText(document, sb.ToString());
            }
        }

        public static void SynthesizeCodeForOCP(List<PUT> PUTtoModify, OCPSimulationData ocp, string projectDir, string destDir)
        {
            Dictionary<string, Dictionary<int, List<string>>> codeToAdd = 
                new Dictionary<string, Dictionary<int, List<string>>>();
            foreach (var put in PUTtoModify)
            {
                string code = SynthesizeFixSimulationForOCP(ocp, put);
                string newDocument = put.Document.Replace(projectDir, destDir);
                if (codeToAdd.ContainsKey(newDocument))
                {
                    Dictionary<int, List<string>> placeHolder = codeToAdd[newDocument];
                    if (placeHolder.ContainsKey(put.FirstLine))
                    {
                        placeHolder[put.FirstLine].Add(code);
                    }
                    else
                    {
                        List<string> list = new List<string>();
                        list.Add(code);
                        placeHolder.Add(put.FirstLine, list);
                    }
                }
                else
                {
                    Dictionary<int, List<string>> placeHolder = new Dictionary<int, List<string>>();
                    List<string> list = new List<string>();
                    list.Add(code);
                    placeHolder.Add(put.FirstLine, list);
                    codeToAdd.Add(newDocument, placeHolder);
                }
            }

            foreach (var document in codeToAdd.Keys)
            {
                Dictionary<int, List<string>> placeHolder = codeToAdd[document];
                StringBuilder sb = new StringBuilder();
                string[] lines = File.ReadAllLines(document);
                for (int i = 0; i < lines.Length; i++)
                {
                    sb.AppendLine(lines[i]);
                    if (placeHolder.ContainsKey(i + 1))
                    {
                        foreach (var code in placeHolder[i + 1])
                        {
                            sb.AppendLine(code);
                        }
                    }
                }
                File.WriteAllText(document, sb.ToString());
            }
            
        }

        private static string SynthesizeFixSimulationForOCP(OCPSimulationData data, PUT put)
        {
            var ocpsymboliccopyTemplate = File.ReadAllText(
                @"C:\Users\LSH\Desktop\covana\GuidanceSimulator\OCPSymbolicCopyTemplate.txt");
            var ocpsetfieldTemplate = File.ReadAllText(
                @"C:\Users\LSH\Desktop\covana\GuidanceSimulator\OCPSetFieldTemplate.txt");
            var pexsetfieldTemplate = File.ReadAllText(
                @"C:\Users\LSH\Desktop\covana\GuidanceSimulator\PexSetFieldTemplate.txt");

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
                    var genericTypesString = split[1].Split(new[] { '[' })[1].Replace("]", "");
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

        private static string SynthesizeFixSimulationForEMCP(EMCPSimuationData data, PUT put)
        {
            var emcpMoleTemplate = File.ReadAllText(
                @"C:\Users\LSH\Desktop\covana\GuidanceSimulator\EMCPMoleTemplate.txt");
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
    }
}
