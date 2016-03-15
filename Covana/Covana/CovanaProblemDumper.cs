using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Pex.Framework;
using Covana.Analyzer;
using Covana.ResultTrackingExtrator;
using System.IO;
using Microsoft.ExtendedReflection.Metadata;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ExtendedReflection.Logging;

namespace Covana
{
    public class CovanaProblemDumper
    {
        public Dictionary<Problem, HashSet<BranchInfo>> ProblemWithBranches = new Dictionary<Problem, HashSet<BranchInfo>>();
        public Dictionary<Problem, HashSet<Method>> ProblemWithPUTs = new Dictionary<Problem, HashSet<Method>>();
        public Dictionary<Problem, CandidateObjectCreationProblem> ProblemMap = new Dictionary<Problem, CandidateObjectCreationProblem>();
        public HashSet<CandidateObjectCreationProblem> _candidateObjectCreationProblems;
        public NonCoveredBranchInfo _nonCoveredBranchInfo;
        public ResultTrackingInfo resultTrackingInfo;
        public Dictionary<string, HashSet<BranchInfo>> uninstrumentedMethodsFoundInObj =
            new Dictionary<string, HashSet<BranchInfo>>();
        public HashSet<ExceptionExternalMethod> ExceptionExternalMethods = new HashSet<ExceptionExternalMethod>();
        public StringBuilder SimpleLog = new StringBuilder("CovanaProblemDumper: \n");

        public Dictionary<string, List<string>> extToPUTs;
        public Dictionary<string, List<string>> ecpToPUTs;
        public SafeSet<Microsoft.ExtendedReflection.Logging.UninstrumentedMethod> ExternalMethods { get; set; }
        public Dictionary<Method, HashSet<CodeLocation>> ExternalMethodInBranch { get; set; }

     

        public void DumpProblems(AssemblyEx assemblyUnderTest)
        {
            SimpleLog.AppendLine("start dumping problems ... ");
            try
            {
                foreach (var candidateObjectCreationProblem in _candidateObjectCreationProblems)
                {
                    var branchInfo = candidateObjectCreationProblem.BranchLocation;
                    BranchInfo a = candidateObjectCreationProblem.BranchLocation;
                    if (_nonCoveredBranchInfo.BranchLocations.Contains(branchInfo.ToLocation))
                    {
                        var problem = new Problem(ProblemKind.ObjectCreation, candidateObjectCreationProblem.TargetType,
                                              candidateObjectCreationProblem.ToString());
                        if (!ProblemWithBranches.ContainsKey(problem))
                        {
                            ProblemWithBranches.Add(problem, new HashSet<BranchInfo>());
                            ProblemWithPUTs.Add(problem, new HashSet<Method>());
                            ProblemMap.Add(problem, candidateObjectCreationProblem);

                        }

                        ProblemWithBranches[problem].Add(branchInfo);
                        ProblemWithPUTs[problem].Add(candidateObjectCreationProblem.Method);

                    }

                    if (candidateObjectCreationProblem.BranchLocation.Line == -1)
                    {
                        if (candidateObjectCreationProblem.TargetObjectType != null)
                        {
                            var problem = new Problem(ProblemKind.ObjectCreation, candidateObjectCreationProblem.TargetType,
                                                  candidateObjectCreationProblem.ToString());
                            if (!ProblemWithBranches.ContainsKey(problem))
                            {
                                ProblemWithBranches.Add(problem, new HashSet<BranchInfo>());
                                ProblemWithPUTs.Add(problem, new HashSet<Method>());
                                ProblemMap.Add(problem, candidateObjectCreationProblem);
                            }

                            ProblemWithBranches[problem].Add(branchInfo);
                            ProblemWithPUTs[problem].Add(candidateObjectCreationProblem.Method);
                        }
                    }
                }

                foreach (KeyValuePair<Problem, HashSet<BranchInfo>> pair in ProblemWithBranches)
                {
                    List<BranchInfo> needToRemoves = new List<BranchInfo>();
                    if (pair.Value.Count > 1)
                    {

                        foreach (BranchInfo info in pair.Value)
                        {
                            if (info.Line == -1)
                            {
                                needToRemoves.Add(info);
                            }
                        }
                    }
                    foreach (BranchInfo info in needToRemoves)
                    {
                        pair.Value.Remove(info);
                    }
                }


                foreach (KeyValuePair<string, HashSet<BranchInfo>> pair in resultTrackingInfo.UnIntrumentedMethodInBranch)
                {
                    var methodName = pair.Key;
                    var infos = pair.Value;
                    foreach (var info in infos)
                    {
                        if (_nonCoveredBranchInfo.Branches.Contains(info))
                        {
                            var problem = new Problem(ProblemKind.UnInstrumentedMethod,
                                                  methodName, "External method found in branch: " + methodName);

                            if (!ProblemWithBranches.ContainsKey(problem))
                            {
                                ProblemWithBranches.Add(problem, new HashSet<BranchInfo>());
                                ProblemWithPUTs.Add(problem, new HashSet<Method>());
                            }

                            ProblemWithBranches[problem].Add(info);
                        }
                    }
                }

                foreach (var methodsFoundInObj in uninstrumentedMethodsFoundInObj)
                {
                    var branchInfo = methodsFoundInObj.Value;

                    string methodName = methodsFoundInObj.Key;
                    var problem = new Problem(ProblemKind.UnInstrumentedMethod,
                                          methodName, "External method found in object creation: " + methodName);

                    if (!ProblemWithBranches.ContainsKey(problem))
                    {
                        ProblemWithBranches.Add(problem, new HashSet<BranchInfo>());
                        ProblemWithPUTs.Add(problem, new HashSet<Method>());
                    }
                    foreach (var info in branchInfo)
                    {
                        ProblemWithBranches[problem].Add(info);
                    }
                }

                foreach (var exceptionExternalMethod in ExceptionExternalMethods)
                {
                    int line = 0;
                    try
                    {
                        line = Convert.ToInt32(exceptionExternalMethod.Line);
                    }
                    catch (Exception)
                    {
                    }
                    var branchInfo = new BranchInfo("", line, 0, 0, exceptionExternalMethod.CallerMethod, 0);

                    string methodName = exceptionExternalMethod.MethodName;
                    if (methodName.Contains("Pex") || methodName.Contains("GetObjectData"))
                    {
                        return;
                    }
                    var problem = new Problem(ProblemKind.UnInstrumentedMethod,
                                          methodName,
                                          "External method found in exception:\n" +
                                          exceptionExternalMethod.ExceptionString);

                    if (!ProblemWithBranches.ContainsKey(problem))
                    {
                        ProblemWithBranches.Add(problem, new HashSet<BranchInfo>());
                        ProblemWithPUTs.Add(problem, new HashSet<Method>());
                    }

                    ProblemWithBranches[problem].Add(branchInfo);
                }

                var ocpdatum = new List<OCPSimulationData>();
                var emcpdatum = new List<EMCPSimuationData>();
                foreach (var pair in ProblemWithBranches)
                {
                    var problem = pair.Key;
                    var branches = pair.Value;
                    if (problem.Kind == ProblemKind.ObjectCreation)
                    {
                        BuildOCPProblemFixSimulationData(ocpdatum, problem, branches);
                    }
                    else if (problem.Kind == ProblemKind.UnInstrumentedMethod)
                    {
                        SimpleLog.AppendLine("found emcp: " + problem);
                        var emcpdata = new EMCPSimuationData();
                        emcpdata.Problem = problem;
                        emcpdata.Branches = branches;

                        bool isexceptionMethods = false;
                        foreach (var exceptionMethod in ExceptionExternalMethods)
                        {
                            if (emcpdata.Problem.Type.Equals(exceptionMethod.MethodName))
                            {
                                emcpdata.ReturnType = exceptionMethod.ReturnType;
                                emcpdata.Paras = exceptionMethod.Paras;
                                
                                SimpleLog.AppendLine("ecpToPUTs count: " + ecpToPUTs.Count);
                                foreach (var method in ecpToPUTs[exceptionMethod.MethodName])
                                {
                                    SimpleLog.AppendLine("method: " + method + "\texception: " + exceptionMethod.MethodName);
                                    emcpdata.PUTs.Add(method);
                                }
                                isexceptionMethods = true;
                                break;
                            }
                        }

                        bool found = false;
                        if (!isexceptionMethods)
                        {

                            foreach (var method in ExternalMethods)
                            {
                                var methodname = method.Method.FullName;
                                SimpleLog.AppendLine("checking emcp: " + emcpdata.Problem.Type + " with external method " + method.Method.FullName);
                                if (emcpdata.Problem.Type.Equals(methodname))
                                {
                                    emcpdata.ReturnType = method.Method.Definition.ResultType.FullName;
                                    foreach (var paraDef in method.Method.Definition.Parameters)
                                    {
                                        var para = new Para(paraDef.Type.FullName, paraDef.Name);
                                        emcpdata.Paras.Add(para);
                                        SimpleLog.AppendLine("extToPUTs count: " + extToPUTs.Count);
                                        foreach (var m in extToPUTs[methodname])
                                        {
                                            //SimpleLog.AppendLine("m is null: "+(m==null));
                                            emcpdata.PUTs.Add(m);
                                        }
                                    }
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                throw new Exception("cannot find uninstrumented method data in branches!");
                            }
                        }

                        if (!isexceptionMethods && !found)
                        {
                            throw new Exception("cannot find uninstrumented method in excpetions or branches!");
                        }

                        emcpdatum.Add(emcpdata);
                    }
                }
                SimpleLog.AppendLine("emcps: " + emcpdatum.Count);
                XmlSerializer ocpSerializer = new XmlSerializer(typeof(List<OCPSimulationData>));
                XmlTextWriter ocpwriter = new XmlTextWriter(new FileStream(@"c:\tempseqex\ocpproblems.xml", FileMode.Create), Encoding.UTF8);
                ocpwriter.Formatting = Formatting.Indented;
                ocpSerializer.Serialize(ocpwriter, ocpdatum);
                ocpwriter.Close();

                XmlSerializer emcpSerializer = new XmlSerializer(typeof(List<EMCPSimuationData>));
                XmlTextWriter emcpwriter = new XmlTextWriter(new FileStream(@"c:\tempseqex\emcpproblems.xml", FileMode.Create), Encoding.UTF8);
                emcpwriter.Formatting = Formatting.Indented;
                emcpSerializer.Serialize(emcpwriter, emcpdatum);
                emcpwriter.Close();
            }
            catch (Exception e)
            {
                SimpleLog.AppendLine(e.ToString());
            }
            finally
            {
                var writer = File.CreateText(@"c:\tempseqex\covanaproblemdumper.txt");
                writer.WriteLine(SimpleLog.ToString());
                writer.Close();

            }
        }

        private void BuildOCPProblemFixSimulationData(List<OCPSimulationData> simulationdatum, Problem p, HashSet<BranchInfo> branches)
        {
            var ocp = new OCPSimulationData();
            ocp.Problem = p;
            ocp.Branches = branches;
            var methods = ProblemWithPUTs[p];
            SimpleLog.AppendLine("Problem: " + p.Type);
            var dp = ProblemMap[p];
            if (dp.Fields != null)
            {
                foreach (var field in dp.Fields)
                {
                    SimpleLog.AppendLine("Field " + field);
                    SimpleLog.AppendLine("Field Shortname " + field.ShortName);

                }
            }

            SimpleLog.AppendLine("PUTs count " + methods.Count);
            foreach (var method in methods)
            {
                if (method == null)
                {
                    continue;
                }
                SimpleLog.AppendLine("PUT: " + method.FullName);
                ocp.PUTs.Add(method.FullName);

            }
            var accesspath = new StringBuilder();
            var lastfieldname = "";
            TypeEx lastfieldType = null;
            var currentfieldname = "";
            if (dp.Fields == null)
            {
                SimpleLog.AppendLine("No fields found");
                ocp.ParaType = p.Type;
                ocp.LastField = "";
                ocp.LastFieldType = "";
                simulationdatum.Add(ocp);
                return;
            }

            for (int i = 0; i < dp.Fields.Count; i++)
            {
                var field = dp.Fields[i];
                var split = field.FullName.Split(new[] { '.' });
                if (i == 0)
                {
                    lastfieldname = split[split.Length - 1];
                    lastfieldType = field.Type;
                    currentfieldname = lastfieldname;
                }
                else
                {
                    Field temp = null;
                    if (field.Type.TryGetField(currentfieldname, out temp))
                    {
                        SimpleLog.AppendLine("found parent field " + field);
                        accesspath.Insert(0, "." + split[split.Length - 1]);
                        ocp.AccessPath.Add(split[split.Length - 1]);
                        currentfieldname = split[split.Length - 1];
                    }
                }

            }
            TypeEx declaringType;
            if (dp.Fields[dp.Fields.Count - 1].TryGetDeclaringType(out declaringType))
            {
                ocp.ParaType = declaringType.FullName;
                SimpleLog.AppendLine("access path: " + accesspath.ToString());
                SimpleLog.AppendLine("Last field name: " + lastfieldname);
                SimpleLog.AppendLine("Last field type: " + lastfieldType);
                ocp.LastField = lastfieldname;
                ocp.LastFieldType = lastfieldType.FullName;
            }
            simulationdatum.Add(ocp);
        }


    }
}
