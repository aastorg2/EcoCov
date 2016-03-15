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
    public class Algorithms
    {
        public static int BenefitEstimation(string projectDir, string problemKind, string problemType,
            string assemblyFile, string nameSpace, string type, string[] methods)
        {
            //copy the entire project to a new directory
            string destDir="";
            if (projectDir[projectDir.Length - 1] == '\\')
            {
                projectDir = projectDir.Substring(0, projectDir.Length - 1);
            }
            destDir += projectDir + "-Copy";
            FileModifier.CopyDirectory(projectDir, destDir);
            
            //run pex on the copied project
            string newAssemblyFile=assemblyFile.Replace(projectDir, destDir);
            //string newProjectFile = projecFile.Replace(projectDir, destDir);
            string projectFile = "";
            foreach (var file in Directory.GetFiles(destDir))
            {
                if(file.EndsWith(".csproj"))
                {
                    projectFile = file;
                    break;
                }
            }
            CommandExecutor.ExecuteCommand(
                CommandGenerator.GenerateRunPexCommand(
                newAssemblyFile, nameSpace, type, methods));

            //get coverage report and problems
            string covPath = @"C:\tempSeqex\covdetails.xml";
            string ocpPath = @"C:\tempSeqex\ocpproblems.xml";
            string emcpPath = @"C:\tempSeqex\emcpproblems.xml";
            string putPath = @"C:\tempSeqex\puts.xml";
            List<BranchCoverageDetail> covReport = RetrieveResults.GetCoverageReport(covPath);
            List<OCPSimulationData> OCPs = RetrieveResults.GetOCPs(ocpPath);
            List<EMCPSimuationData> EMCPs = RetrieveResults.GetEMCPs(emcpPath);
            List<PUT> PUTs = RetrieveResults.GetPUTs(putPath);

            if (problemKind == "ObjectCreation")
            {
                //OCP
                OCPSimulationData selectedOCP = null;
                foreach (OCPSimulationData ocp in OCPs)
                {
                    if (ocp.Problem.Type == problemType)
                    {
                        selectedOCP = ocp;
                        break;
                    }
                }
                List<PUT> PUTtoModify=new List<PUT>();
                foreach (var put in PUTs)
                {
                    foreach (var para in put.Paras)
                    {
                        if (para.Type == selectedOCP.ParaType)
                        {
                            PUTtoModify.Add(put);
                            break;
                        }
                    }
                }
                FileModifier.SynthesizeCodeForOCP(PUTtoModify, selectedOCP, projectDir, destDir);
            }
            else
            {
                //EMCP
                EMCPSimuationData selectedEMCP = null;
                foreach (EMCPSimuationData emcp in EMCPs)
                {
                    if (emcp.Problem.Type == problemType)
                    {
                        selectedEMCP = emcp;
                        break;
                    }
                }
                List<PUT> PUTtoModify = new List<PUT>();
                foreach (var put in PUTs)
                {
                    if (selectedEMCP.PUTs.Contains(put.Name))
                    {
                        PUTtoModify.Add(put);
                    }
                }
                FileModifier.SynthesizeCodeForEMCP(PUTtoModify, selectedEMCP, projectDir, destDir);
            }
            
            //rebuild
            CommandExecutor.ExecuteCommand(CommandGenerator.GenerateBuildCommand(projectFile));
            //rerun
            CommandExecutor.ExecuteCommand(
                CommandGenerator.GenerateRunPexCommand(
                newAssemblyFile, nameSpace, type, methods));

            List<BranchCoverageDetail> newCovReport = RetrieveResults.GetCoverageReport(covPath);
            DisplayCovResults(newCovReport);
            return CompareCovReport(covReport, newCovReport);
        }

        public static void PathSelection()
        {
            
        }

        private static void DisplayCovResults(List<BranchCoverageDetail> covReport)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var branchCov in covReport)
            {
                BranchInfo from = branchCov.BranchInfo;
                BranchInfo to = branchCov.TargetLocation;
                sb.AppendLine(from.Line + " - " + to.Line + "\t" + branchCov.CoveredTimes + 
                    "\t" + branchCov.targetCoveredTimes+"\t"+branchCov.Type);
            }
            File.WriteAllText(@"C:\tempSeqex\output.txt", sb.ToString());
        }

        private static int CompareCovReport(
            List<BranchCoverageDetail> covReport, List<BranchCoverageDetail> newCovReport)
        {
            
            return 0;
        }
    }
}
