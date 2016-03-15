using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Pex.Framework.Packages;
using System.IO;
using Microsoft.ExtendedReflection.Symbols;
using Microsoft.ExtendedReflection.Metadata;
using Microsoft.ExtendedReflection.Collections;
using System.Xml.Serialization;

namespace Covana.EconomicAnalyzer
{
    [__DoNotInstrument]
    public class PUTObserver :
        PexExplorationPackageAttributeBase
    {
        private StreamWriter writer;

        protected override object BeforeExploration(Microsoft.Pex.Engine.ComponentModel.IPexExplorationComponent host)
        {
            writer = File.CreateText(@"d:\tempseqex.txt");
            return base.BeforeExploration(host);
        }

        protected override void AfterExploration(Microsoft.Pex.Engine.ComponentModel.IPexExplorationComponent host, object data)
        {
            var exploration = host.ExplorationServices.CurrentExploration.Exploration;
            var method = host.ExplorationServices.CurrentExploration.Exploration.Method;
            writer.WriteLine("Test Class Name: " + host.ExplorationServices.CurrentExploration.Exploration.FullName);
            writer.WriteLine("PUT name: " + method);
            writer.WriteLine("Full name: " + exploration.FullName);
            var body = method.Definition.Body;
            writer.WriteLine("Body Type: " + body.ToString());
            var paras = method.Parameters;
            foreach (var para in paras)
            {
                writer.WriteLine("para type {0} para name {1} ", para.Type.FullName, para.Name);
            }
            ISymbolManager sm = host.Services.SymbolManager;
            IIndexable<SequencePoint> sps;
            sm.TryGetSequencePoints(method.Definition, out sps);

            if (sps.Count > 0)
            {
                var firstIns = sps[0];
                writer.WriteLine("file: " + firstIns.Document + " line: " + firstIns.Line);

                //XmlSerializer serializer = new XmlSerializer(typeof(PUT));
                var put = new PUT();
                put.Document = firstIns.Document;
                put.Name = method.FullName;
                put.Class = host.ExplorationServices.CurrentExploration.Exploration.FullName;
                put.FirstLine = firstIns.Line;
                foreach (var para in paras)
                {
                    put.Paras.Add(new Para(para.Type.FullName,para.Name));
                }

                host.GetService<ProblemTrackDatabase>().PUTs.Add(put);
                //TextWriter xmlwriter = new StreamWriter(@"c:\tempseqex\puts.xml");
                //serializer.Serialize(xmlwriter, put);
                //xmlwriter.Close();
            }
            //foreach (var sp in sps)
            //{

            //writer.WriteLine("Intruction: " + sp.ToSourceLocation() + " file: " + sp.Document + " line: " + 
            //    sp.Line + " column: " + sp.Column + " endcolumn: " + sp.EndColumn);
            //}
            writer.Close();


            base.AfterExploration(host, data);
        }
    }
}
