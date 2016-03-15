using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Pex.Framework.Packages;
using Microsoft.ExtendedReflection.Coverage;
using Microsoft.Pex.Engine;
using Microsoft.ExtendedReflection.Metadata;
using Microsoft.ExtendedReflection.Symbols;
using Microsoft.Pex.Engine.ComponentModel;
using System.IO;

namespace Covana.CoverageExtractor
{
    public class PUTCoverageObserver : PexExplorationPackageAttributeBase
    {
        private Method PUT;
        private IPexExplorationComponent host;



        protected override object BeforeExploration(Microsoft.Pex.Engine.ComponentModel.IPexExplorationComponent host)
        {
            //File.WriteAllText(@"C:\tempSeqex\debug2.txt", "enter");
            PUT = host.ExplorationServices.CurrentExploration.Exploration.Method;
            host.GetService<ProblemTrackDatabase>().CurrentPUTMethod = PUT;
            //File.WriteAllText(@"C:\tempSeqex\debug1.txt", "PUT name: "+PUT.FullName);
            this.host = host;
            host.ExplorationServices.CoverageManager.BeforePublishExplorationCoverage += new Microsoft.ExtendedReflection.Logging.RemoteEventHandler<Microsoft.ExtendedReflection.Logging.RemoteEventArgs>(CoverageManager_BeforePublishExplorationCoverage);
            return base.BeforeExploration(host);
        }

        void CoverageManager_BeforePublishExplorationCoverage(Microsoft.ExtendedReflection.Logging.RemoteEventArgs e)
        {
            var manager = host.ExplorationServices.CoverageManager;
            TaggedBranchCoverageBuilder<PexGeneratedTestName> cov;
            StringBuilder sb = new StringBuilder("PUT coverage: \n");
            Dictionary<MethodDefinition, HashSet<BranchCoverageDetail>> branchCoverageDetails = new Dictionary<MethodDefinition, HashSet<BranchCoverageDetail>>();
            try
            {
                if (manager.TryGetExplorationCoverageBuilder(out cov))
                {
                    IEnumerable<MethodDefinition> definitions = cov.Methods;
                    sb.AppendLine("methods: " + definitions.Count());

                    var entry = CoverageHelper.GetBlockCoverage(CoverageDomain.UserCodeUnderTest, cov);
                    var sb2 = new StringBuilder();
                    sb2.AppendLine(PUT.ShortName+"," + (entry.Covered +"/"+entry.Total));

                    foreach (var method in definitions)
                    {
                        CoverageDomain domain;
                        if (!branchCoverageDetails.ContainsKey(method))
                        {
                            branchCoverageDetails.Add(method, new HashSet<BranchCoverageDetail>());
                        }
                        int[] hits;
                        if (cov.TryGetMethodHits(method, out domain, out hits))
                        {
                            for (int branchLabel = 0; branchLabel < hits.Length; branchLabel++)
                            {
                                CodeLocation location = method.GetBranchLabelSource(branchLabel);
                                MethodDefinitionBodyInstrumentationInfo info;
                                if (method.TryGetBodyInstrumentationInfo(out info))
                                {
                                    ISymbolManager sm = host.Services.SymbolManager;
                                    SequencePoint sp;
                                    sm.TryGetSequencePoint(method, location.Offset, out sp);

                                    foreach (
                                        var outgoingBranchLabel in
                                            info.GetOutgoingBranchLabels(location.Offset))
                                    {
                                        CodeBranch codeBranch = new CodeBranch(location.Method,
                                                                               outgoingBranchLabel);
                                        if (!codeBranch.IsBranch && !codeBranch.IsSwitch && !codeBranch.IsCheck) // is explicit branch?
                                        {
                                            host.Log.Dump("coverage", "coverage",
                                                          "CodeBranch: " + codeBranch +
                                                          " is not explicit");
                                            sb.AppendLine("CodeBranch: " + codeBranch +
                                                          " is not explicit");

                                            continue; // if not, we don't log it                         
                                        }

                                        var fromMethod = method.FullName + "," +
                                                            sp.Document + "," +
                                                            sp.Line + ", column: " + sp.Column + " outgoing label: " +
                                                            outgoingBranchLabel;
                                        BranchInfo branchInfo = new BranchInfo(sp.Document, sp.Line, sp.Column, sp.EndColumn, method.FullName, location.Offset);

                                        int branchhit = 0;
                                        if (outgoingBranchLabel < hits.Length &&
                                            hits[outgoingBranchLabel] != 0)
                                        {
                                            branchhit = hits[outgoingBranchLabel];
                                        }

                                        string type = "";
                                        if (codeBranch.IsBranch)
                                        {
                                            type = "Explicit";
                                        }
                                        else if (codeBranch.IsCheck)
                                        {
                                            type = "Implicit";
                                        }


                                        BranchCoverageDetail coverageDetail = new BranchCoverageDetail(branchInfo, branchhit, null, 0, type);
                                        coverageDetail.CopyBranchProperties(codeBranch);
                                        coverageDetail.OutgoingLabel = outgoingBranchLabel;
                                        coverageDetail.BranchLabel = branchLabel;
                                        int targetOffset;
                                        if (info.TryGetTargetOffset(outgoingBranchLabel, out targetOffset))
                                        {
                                            sm.TryGetSequencePoint(method, targetOffset, out sp);
                                            var targetCovTimes =
                                                info.GetInstructionCoverage(hits).Invoke(targetOffset);
                                            BranchInfo targetInfo = new BranchInfo(sp.Document, sp.Line, sp.Column, sp.EndColumn, method.FullName, targetOffset);
                                            coverageDetail.TargetLocation = targetInfo;
                                            coverageDetail.targetCoveredTimes = targetCovTimes;
                                            if (!branchCoverageDetails[method].Contains(coverageDetail))
                                            {
                                                branchCoverageDetails[method].Add(coverageDetail);
                                            }
                                            else
                                            {
                                                foreach (BranchCoverageDetail detail in branchCoverageDetails[method])
                                                {
                                                    if (detail.Equals(coverageDetail))
                                                    {
                                                        if (coverageDetail.CoveredTimes > detail.CoveredTimes)
                                                        {
                                                            detail.CoveredTimes = coverageDetail.CoveredTimes;
                                                        }
                                                    }
                                                }
                                            }


                                        }

                                        if (outgoingBranchLabel < hits.Length &&
                                            hits[outgoingBranchLabel] == 0)
                                        {
                                            continue;

                                        }
                                    }
                                }
                            }
                        }
                    }

                    host.GetService<ProblemTrackDatabase>().PUTCoverageSummaryLog.AppendLine(sb2.ToString());
                }
                else
                {
                    sb.AppendLine("manager.TryGetAssemblyCoverageBuilder is null!");
                }

                host.GetService<ProblemTrackDatabase>().PUTBranchCoverageDetails.Add(PUT.Definition, branchCoverageDetails);
            }
            catch (Exception ex)
            {
                host.Log.Dump("coverage", "cov ex", ex.Message);
                host.GetService<ProblemTrackDatabase>().ErrorLog.AppendLine("exception in PUTCoverageObserver: " + ex);
            }
        }

        protected override void AfterExploration(Microsoft.Pex.Engine.ComponentModel.IPexExplorationComponent host, object data)
        {

            base.AfterExploration(host, data);
        }

    }
}
