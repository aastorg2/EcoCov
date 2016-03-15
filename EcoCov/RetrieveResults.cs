using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Covana;
using Covana.EconomicAnalyzer;
using Covana.CoverageExtractor;

namespace EcoCov
{
    public class RetrieveResults
    {
        public static List<OCPSimulationData> GetOCPs(string xmlPath)
        {
            XmlSerializer ocpSerializer = new XmlSerializer(typeof(List<OCPSimulationData>));
            List<OCPSimulationData> OCPs = (List<OCPSimulationData>)ocpSerializer.Deserialize(File.OpenRead(xmlPath));
            return OCPs;
        }

        public static List<EMCPSimuationData> GetEMCPs(string xmlPath)
        {
            XmlSerializer emcpSerializer = new XmlSerializer(typeof(List<EMCPSimuationData>));
            List<EMCPSimuationData> EMCPs = (List<EMCPSimuationData>)emcpSerializer.Deserialize(File.OpenRead(xmlPath));
            return EMCPs;
        }

        public static List<BranchCoverageDetail> GetCoverageDetails(string xmlPath)
        {
            XmlSerializer covSerializer = new XmlSerializer(typeof(List<BranchCoverageDetail>));
            List<BranchCoverageDetail> covDetails = (List<BranchCoverageDetail>)covSerializer.Deserialize(File.OpenRead(xmlPath));
            return covDetails;
        }

        public static List<PUT> GetPUTs(string xmlPath)
        {
            XmlSerializer putSerializer = new XmlSerializer(typeof(List<PUT>));
            List<PUT> PUTs = (List<PUT>)putSerializer.Deserialize(File.OpenRead(xmlPath));
            return PUTs;
        }

        public static List<BranchCoverageDetail> GetCoverageReport(string xmlPath)
        {
            List<BranchCoverageDetail> covCandidates = GetCoverageDetails(xmlPath);
            List<BranchCoverageDetail> covReport = new List<BranchCoverageDetail>();
            foreach (BranchCoverageDetail candidate in covCandidates)
            {
                BranchInfo from = candidate.BranchInfo;
                BranchInfo to = candidate.TargetLocation;
                if (from.Line == -1 || to.Line == -1||candidate.Type=="implicit")
                {
                    continue;
                }
                else
                {
                    //sb.AppendLine(from.Line + " - " + to.Line + "\t" +
                    //    candidate.CoveredTimes + "\t" + candidate.targetCoveredTimes+"\t"+
                    //    candidate.Type);
                    covReport.Add(candidate);
                }
            }
            return covReport;
        }
    }
}
