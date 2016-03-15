using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using __Auxiliary;
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ExtendedReflection.Interpretation;
using Microsoft.ExtendedReflection.Interpretation.Visitors;
using Microsoft.ExtendedReflection.Logging;
using Microsoft.ExtendedReflection.Metadata;
using Microsoft.ExtendedReflection.Reasoning;
using Microsoft.ExtendedReflection.Symbols;
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;
using Microsoft.Pex.Engine.ComponentModel;
using Microsoft.Pex.Framework.Packages;
using Covana.Analyzer;
using StringBuilder = System.Text.StringBuilder;

namespace Covana.ProblemExtractor
{
    [__DoNotInstrument]
    public class ProblemObserverAttribute :
        PexExecutionPackageAttributeBase
    {
        protected override object BeforeExecution(IPexComponent host)
        {
            SafeDebug.AssumeNotNull(host, "host");

            return new NewTestLogger(host);
        }

        protected override sealed void AfterExecution(Microsoft.Pex.Engine.ComponentModel.IPexComponent host,
                                                      object data)
        {
        }

        [__DoNotInstrument]
        private class NewTestLogger : PexComponentElementBase
        {
            private string InfoFileDirectory = "c:\\tempSeqex\\";
            private readonly string outputFile = "";
            private Method currentMethod = null;


            public NewTestLogger(IPexComponent host)
                : base(host)
            {
                Host.Log.ProblemHandler += Log_ProblemHandler;
                outputFile = InfoFileDirectory + Host.Services.CurrentAssembly.Assembly.Assembly.ShortName +
                             ".problem.txt";
                Host.Log.BeforePexExplorationHandler += new RemoteEventHandler<Microsoft.Pex.Engine.Logging.PexExplorationEventArgs>(Log_BeforePexExplorationHandler);
                //                this.outputFile = Path.Combine(host.Services.ReportManager.ReportPath, "newtests.txt");
            }

            void Log_BeforePexExplorationHandler(Microsoft.Pex.Engine.Logging.PexExplorationEventArgs e)
            {
                currentMethod = e.Exploration.Method;
            }

            private void Log_ProblemHandler(Microsoft.ExtendedReflection.Logging.ProblemEventArgs e)
            {
                RecordFlipCount(e);


                if (e.Result == TryGetModelResult.Success)
                {
                    Host.GetService<ProblemTrackDatabase>().SimpleLog.AppendLine("flipped result: " + e.Result);
                    Host.GetService<ProblemTrackDatabase>().CurrentSuccessfulFlippedPathCondition = e;
                    return;
                }
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (e.Result != TryGetModelResult.Success)
                {

                    Host.GetService<ProblemTrackDatabase>().SimpleLog.AppendLine("********************************************** ");
                    Host.GetService<ProblemTrackDatabase>().SimpleLog.AppendLine("flipped result: " + e.Result);
                    //                    return;
                }
                try
                {
                    CodeLocation location = e.FlippedLocation;
                    var database = Host.GetService<ProblemTrackDatabase>();
                    database.PexProblemEventArgsList.Add(e);

                    //                e.Result == TryGetModelResult.
                    this.Host.Log.Dump("My Category", "flipped location: " + location, null);
                    SequencePoint sp;
                    Host.Services.SymbolManager.TryGetSequencePoint(location.Method, location.Offset, out sp);
                    sb.AppendLine("flipped location: " + sp.Document + " line: " + sp.Line + " offset: " +
                                  location.Offset);
                    sb.AppendLine("e.ParentOfFlipped.InCodeBranch: " + e.ParentOfFlipped.InCodeBranch);

                    var flippedmethod = location.Method;
                    MethodDefinitionBodyInstrumentationInfo info;
                    if (flippedmethod.TryGetBodyInstrumentationInfo(out info))
                    {
                        foreach (var outbranch in e.ParentOfFlipped.OutCodeBranches)
                        {
                            int targetOffset = 0;
                            info.TryGetTargetOffset(outbranch.BranchLabel, out targetOffset);
                            SequencePoint outsp;
                            Host.Services.SymbolManager.TryGetSequencePoint(flippedmethod, targetOffset, out outsp);
                            sb.AppendLine("going to: " + outsp.Document + " line: " + outsp.Line + " offset: " +
                                 outsp.Offset);
                        }
                    }

                    var flippedCondition = e.Suffix;
                    var parentOfFlipped = e.ParentOfFlipped.CodeLocation;
                    Host.Services.SymbolManager.TryGetSequencePoint(parentOfFlipped.Method, parentOfFlipped.Offset,
                                                                    out sp);
                    sb.AppendLine("parent flipped location: " + sp.Document + " line: " + sp.Line + " offset: " +
                                  parentOfFlipped.Offset);

                    var stringWriter = new StringWriter();
                    var bodyWriter = this.Host.Services.LanguageManager.DefaultLanguage.CreateBodyWriter(stringWriter,
                                                                                                         VisibilityContext
                                                                                                             .
                                                                                                             Private);
                    var emitter = new TermEmitter(e.TermManager, new NameCreator());
                    if (emitter.TryEvaluate(Indexable.One(flippedCondition), 1000, bodyWriter))
                    {
                        bodyWriter.Return(SystemTypes.Bool);
                    }
                    string flippedTerm = stringWriter.ToString();
                    stringWriter.Close();
                    var prefixWriter = new StringWriter();
                    prefixWriter.WriteLine("Feasible prefixes:");
                    if (e.FeasiblePrefix != null && e.FeasiblePrefix.Length > 0)
                    {
                        var bodyWriter2 =
                            this.Host.Services.LanguageManager.DefaultLanguage.CreateBodyWriter(prefixWriter,
                                                                                                VisibilityContext
                                                                                                    .
                                                                                                    Private);
                        foreach (Term prefix in e.FeasiblePrefix)
                        {
                            if (emitter.TryEvaluate(Indexable.One(prefix), 1000, bodyWriter2))
                            {
                                bodyWriter2.Return(SystemTypes.Bool);
                            }
                        }
                    }
                    else
                    {
                        prefixWriter.WriteLine("No feasible prefixes.");
                    }


                    string feasiblePrefix = prefixWriter.ToString();
                    this.Host.Log.Dump("My Category", "condition", stringWriter.ToString());
                    sb.AppendLine(prefixWriter.ToString());
                    prefixWriter.Close();
                    sb.AppendLine("flipped term: " + flippedTerm);
                    int returnIndex = flippedTerm.IndexOf("return");
                    string targetObjectType = "";
                    bool infeasible = false;
                    var flippedTermReturn = flippedTerm.Substring(returnIndex, flippedTerm.LastIndexOf(";") - returnIndex + 1);
                    int objlen = flippedTermReturn.IndexOf(")") - flippedTermReturn.IndexOf("(") - 1;
                    if (objlen > 0)
                    {
                        targetObjectType = flippedTermReturn.
                       Substring(flippedTermReturn.IndexOf("(") + 1, objlen);
                    }

                    sb.AppendLine("targetObjectType: " + targetObjectType);
                    sb.AppendLine("flippedReturnTerm: " + flippedTermReturn);
                    if (flippedTermReturn.Contains("==") && flippedTermReturn.Contains("null"))
                    {
                        var conflictTerm = flippedTermReturn.Replace("==", "!=");
                        if (feasiblePrefix.Contains(conflictTerm))
                        {
                            sb.AppendLine("found conflict constraints in feasiblePrefix: " + conflictTerm);
                            infeasible = true;
                        }
                    }

                    if (flippedTermReturn.Contains("==") && flippedTermReturn.EndsWith("null;"))
                    {
                        sb.AppendLine("found equal to null constraints in feasiblePrefix: " + flippedTermReturn);
                        infeasible = true;

                    }

                    if (flippedTermReturn.Contains("!=") && flippedTermReturn.Contains("null"))
                    {
                        var conflictTerm = flippedTermReturn.Replace("!=", "==");
                        if (feasiblePrefix.Contains(conflictTerm))
                        {
                            sb.AppendLine("found conflict constraints in feasiblePrefix: " + conflictTerm);
                            infeasible = true;
                        }
                    }


                    if (flippedTerm.Contains("!=") && flippedTerm.Contains("null"))
                    {
                        int index = flippedTerm.IndexOf("!=") - 2;
                        int length = 0;
                        while (flippedTerm[index] != ' ')
                        {
                            index--;
                            length++;
                        }
                        string variable = flippedTerm.Substring(index + 1, length);
                        sb.AppendLine("variable for targetObjectType: " + variable);

                        string infeasibleCheck = ".GetType() != typeof(" + targetObjectType + ")";
                        string conflictCheck = variable + " == (" + targetObjectType + ")null";
                        sb.AppendLine("test for infeasible: " + infeasibleCheck);
                        if (feasiblePrefix.Contains(infeasibleCheck))
                        {
                            sb.AppendLine("found infeasible constraint: " + infeasibleCheck);
                            infeasible = true;
                        }
                        //else if (feasiblePrefix.Contains(conflictCheck))
                        //{
                        //    sb.AppendLine("found conflict constraint: " + conflictCheck);
                        //    infeasible = true;
                        //}
                        else if (targetObjectType.Contains("(") || targetObjectType.Contains(")") || targetObjectType.Contains("="))
                        {
                            sb.AppendLine("found wrong object type: " + targetObjectType);
                            infeasible = true;
                        }
                        else
                        {
                            var branchInfo = new BranchInfo(sp.Document, sp.Line, sp.Column, sp.EndColumn,
                                                            location.Method.FullName, location.Offset);
                            if (database.TargetObjectTypes.ContainsKey(targetObjectType))
                            {
                                database.TargetObjectTypes[targetObjectType].Add(branchInfo);
                            }
                            else
                            {
                                database.TargetObjectTypes.Add(targetObjectType,
                                                               new HashSet<BranchInfo> { branchInfo });
                            }
                        }
                    }

                    IEnumerable<Field> fields = GetInvolvedFields(e.TermManager, flippedCondition);
                    IEnumerable<TypeEx> types = GetInvolvedObjectTypes(e.TermManager, flippedCondition);
                    var simpleLog = database.SimpleLog;
                    var errorLog = database.ErrorLog;
                    Field target;
                    TypeEx declaringType;
                    simpleLog.AppendLine("============Log Problem================");
                    simpleLog.AppendLine("result: " + e.Result);
                    simpleLog.AppendLine(sb.ToString());

                    //   fields = ReorganizeFieldSequence(fields.Reverse());

                    bool canAnalysis = true;
                    TypeEx targetType;
                    if (
                        !ObjectCreationProblemAnalyzer.GetTargetExplorableField(fields.Reverse(), out target,
                                                                              out declaringType,
                                                                              Host, out targetType))
                    {
                        simpleLog.AppendLine("can not analyze");
                        canAnalysis = false;
                    }


                    simpleLog.AppendLine("failed term: \n" + flippedTerm.ToString());
                    fields.ToList().ForEach(x => simpleLog.AppendLine("involved field: " + x));
                    foreach (var f in fields)
                    {
                        simpleLog.AppendLine("involved field: ");
                        simpleLog.AppendLine("f.FullName:" + f.FullName);
                        simpleLog.AppendLine("f.Definition.FullName" + f.Definition.FullName);
                        simpleLog.AppendLine("f.InstanceFieldMapType:" + f.InstanceFieldMapType.FullName);
                        TypeEx type;
                        //                        type.
                        f.TryGetDeclaringType(out type);
                        simpleLog.AppendLine("f.TryGetDeclaringType: " + type.FullName);
                    }

                    types.ToList().ForEach(x => simpleLog.AppendLine("found object type: " + x));
                    types.ToList().ForEach(x => Host.GetService<ProblemTrackDatabase>().FoundTypes.Add(x.FullName));
                    fields.ToList().ForEach(x => Host.GetService<ProblemTrackDatabase>().FoundTypes.Add(x.Type.FullName));
                    fields.ToList().ForEach(x =>
                    {
                        TypeEx decType;
                        if (x.TryGetDeclaringType(out decType))
                        {
                            Host.GetService<ProblemTrackDatabase>().FoundTypes.Add(
                                decType.FullName);
                        }
                        ;
                    });
                    simpleLog.AppendLine("target field: " + target);

                    if (fields != null && fields.Count() > 0 && !infeasible && sp.Line != -1)
                    {
                        simpleLog.AppendLine("found candidate OCP fields > 0");
                        CreateCandidateObjectCreationProblem(database, location, sp, stringWriter, simpleLog, fields,
                                                           target,
                                                           errorLog, targetType, targetObjectType);
                    }

                    if (!infeasible && fields.Count() == 0 && types.Count() == 1 && !canAnalysis)
                    {
                        simpleLog.AppendLine("found candidate OCP with one type: " + types.First().FullName);
                        if (!isPrimitive(types.First()))
                        {
                            CreateCandidateObjectCreationProblemForSingleType(stringWriter, sp, location, types.First().FullName,
                                                                      database, simpleLog, errorLog);
                        }
                        else
                        {
                            simpleLog.AppendLine("type is primitive: " + types.First().FullName);
                        }
                    }
                    else if (fields == null || fields.Count() == 0 && targetObjectType != null && !infeasible)
                    {
                        simpleLog.AppendLine("found candidate OCP");
                        CreateCandidateObjectCreationProblemForSingleType(stringWriter, sp, location, targetObjectType,
                                                                        database, simpleLog, errorLog);
                    }

                    simpleLog.AppendLine("============end Log Problem================");
                    simpleLog.AppendLine();
                }
                catch (Exception ex)
                {
                    Host.GetService<ProblemTrackDatabase>().ErrorLog.AppendLine("Error in problem observer: " + ex);
                }
                //                DumpInfoToDebugFile(sb.ToString(),outputFile);
            }

            private IEnumerable<Field> ReorganizeFieldSequence(IEnumerable<Field> fields)
            {
                List<Field> result = new List<Field>();
                bool error = false;
                bool reorder = ValidateFieldSequence(fields, out error);

                while (reorder)
                {

                }

                return result;
            }

            private static bool ValidateFieldSequence(IEnumerable<Field> fields, out bool error)
            {
                Field prevField = null;
                bool reorder = false;
                foreach (var field in fields)
                {
                    if (prevField == null)
                    {
                        prevField = field;
                    }
                    else
                    {
                        TypeEx declaringType = null;
                        if (!field.TryGetDeclaringType(out declaringType))
                        {
                            error = true;
                            return false;
                        }
                        var prevFieldType = prevField.Type;
                        if (prevField.Type == declaringType ||
                        prevFieldType.IsAssignableTo(declaringType)
                        || declaringType.IsAssignableTo(prevFieldType))
                        {
                            continue;
                        }
                        else
                        {
                            reorder = true;
                            break;
                        }
                    }
                }
                error = false;
                return reorder;
            }

            private String[] primitiveTypes = new[] { "System.Int32", "System.Int16", "System.Int64", "System.String",
                        "System.Boolean", "System.Guid", "System.Char", "System.Byte", 
                        "System.UInt32", "System.UInt16", "System.UInt64", "System.Decimal",
             "System.Double", "System.Single", "System.DateTime", "System.TimeSpan",};

            private bool isPrimitive(TypeEx typeEx)
            {
                if (primitiveTypes.Contains(typeEx.FullName))
                {
                    return true;
                }

                return false;
            }

            private void RecordFlipCount(ProblemEventArgs e)
            {
                CodeLocation location = e.FlippedLocation;
                SequencePoint sp;
                if (Host.Services.SymbolManager.TryGetSequencePoint(location.Method, location.Offset, out sp))
                {
                    Dictionary<BranchInfo, int> branchFlipCounts =
                        Host.GetService<ProblemTrackDatabase>().BranchFlipCounts;
                    //                sb.AppendLine("flipped location: " + sp.Document + " line: " + sp.Line + " offset: " + location.Offset);
                    var info = new BranchInfo(sp.Document, sp.Line, sp.Column, sp.EndColumn, location.Method.FullName,
                                              location.Offset);
                    if (!branchFlipCounts.ContainsKey(info))
                    {
                        branchFlipCounts.Add(info, 0);
                    }
                    branchFlipCounts[info]++;
                }
            }

            private CandidateObjectCreationProblem CreateCandidateObjectCreationProblemForSingleType(StringWriter stringWriter, SequencePoint sp,
                                                                         CodeLocation location, string type,
                                                                         ProblemTrackDatabase database,
                                                                         StringBuilder simpleLog, StringBuilder errorLog)
            {
                try
                {
                    var issue = new CandidateObjectCreationProblem();
                    var fieldInfos = new HashSet<FieldInfo>();


                    issue.InvolvedFieldInfos = fieldInfos;

                    issue.DetailDescription = " add for target object type: " + stringWriter.ToString();
                    issue.BranchLocation = new BranchInfo(sp.Document, sp.Line, sp.Column, sp.EndColumn,
                                                          location.Method.FullName,
                                                          location.Offset);
                    issue.TargetType = type;
                    issue.TargetObjectType = type;
                    database.CandidateObjectCreationProblems.Add(issue);
                    simpleLog.AppendLine("add candidate objection issue succeed: " + issue);
                    return issue;
                }
                catch (Exception ex)
                {
                    simpleLog.AppendLine("add candidate objection issue faile: ");
                    simpleLog.AppendLine(ex.ToString());
                    return null;
                }
            }

            private IEnumerable<TypeEx> GetInvolvedObjectTypes(TermManager manager, Term term)
            {
                using (var ofc = new ObjectFieldCollector(manager))
                {
                    ofc.VisitTerm(default(TVoid), term);
                    return ofc.Types;
                }
            }

            private CandidateObjectCreationProblem CreateCandidateObjectCreationProblem(ProblemTrackDatabase database, CodeLocation location,
                                                            SequencePoint sp, StringWriter stringWriter,
                                                            StringBuilder simpleLog, IEnumerable<Field> fields,
                                                            Field target, StringBuilder errorLog, TypeEx type,
                                                            string targetObjectType)
            {
                TypeEx declaringType;
                try
                {
                    var issue = new CandidateObjectCreationProblem();
                    var fieldInfos = new HashSet<FieldInfo>();
                    foreach (var field in fields)
                    {
                        if (!field.TryGetDeclaringType(out declaringType))
                        {
                            errorLog.AppendLine("fail to get declaring type of field: " + field.FullName);
                            continue;
                        }
                        fieldInfos.Add(new FieldInfo(field.FullName, field.Type.FullName, declaringType.FullName));
                    }
                    issue.Fields = new List<Field>(fields);
                    issue.Method = currentMethod;
                    issue.InvolvedFieldInfos = fieldInfos;

                    if (target != null)
                    {
                        if (!target.TryGetDeclaringType(out declaringType))
                        {
                            errorLog.AppendLine("fail to get declaring type of field: " + target.FullName);
                        }
                        else
                        {
                            issue.TargetField = new FieldInfo(target.FullName, target.Type.FullName,
                                                              declaringType.FullName);
                        }
                    }

                    issue.DetailDescription = stringWriter.ToString();
                    issue.BranchLocation = new BranchInfo(sp.Document, sp.Line, sp.Column, sp.EndColumn,
                                                          location.Method.FullName,
                                                          location.Offset);
                    issue.TargetType = type != null ? type.FullName : "null";
                    issue.TargetObjectType = targetObjectType;
                    database.CandidateObjectCreationProblems.Add(issue);
                    simpleLog.AppendLine("add candidate objection issue succeed: " + issue);
                    return issue;
                }
                catch (Exception ex)
                {
                    simpleLog.AppendLine("add candidate objection issue faile: ");
                    simpleLog.AppendLine(ex.ToString());
                    return null;
                }
            }

            private void DumpInfoToDebugFile(string s, string fileName)
            {
                try
                {
                    if (!Directory.Exists(InfoFileDirectory))
                        Directory.CreateDirectory(InfoFileDirectory);
                    TextWriter tw = new StreamWriter(fileName);
                    tw.WriteLine(s);
                    tw.Close();
                }
                catch (Exception e)
                {
                    this.Host.Log.Dump("problems", "wrting problem info to " + fileName, e.ToString());
                }
            }

            private IEnumerable<Field> GetInvolvedFields(TermManager termManager, Term t)
            {
                using (var ofc = new ObjectFieldCollector(termManager))
                {
                    ofc.VisitTerm(default(TVoid), t);
                    return ofc.Fields;
                }
            }

            private sealed class ObjectFieldCollector : TermInternalizingRewriter<TVoid>
            {
                public List<Field> Fields = new List<Field>();
                public List<TypeEx> Types = new List<TypeEx>();

                public ObjectFieldCollector(TermManager termManager)
                    : base(termManager, TermInternalizingRewriter<TVoid>.OnCollection.Fail)
                {
                }

                public override Term VisitSymbol(TVoid parameter, Term term, ISymbolId key)
                {
                    Field instanceField;
                    if (this.TermManager.TryGetInstanceField(term, out instanceField))
                        this.Fields.Add(instanceField);


                    TypeEx objectType;
                    if (key is ISymbolIdWithType)
                    {
                        var type = key as ISymbolIdWithType;
                        Types.Add(type.Type);
                    }
                    return base.VisitSymbol(parameter, term, key);
                }
            }
        }
    }
}