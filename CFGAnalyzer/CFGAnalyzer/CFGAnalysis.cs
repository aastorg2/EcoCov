using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phx;
using Phx.Graphs;
using Phx.BitVector;
using Phx.IR;

namespace CFGAnalyzer
{


    public class CFGAnalysis
    {
        private string file;
        private string method;
        private uint line1;
        private uint line2;
        private Phx.Graphs.FlowGraph flowGraph;
        private Phx.PEModuleUnit moduleUnit;
        private Phx.FunctionUnit functionUnit;

        private uint target;
        private HashSet<Problem> problems;

        public HashSet<Problem> Problems
        {
            get { return this.problems; }
        }

        public uint Target
        {
            get { return target; }
            //set { target = value; }
        }

        private BasicBlock targetBlock;

        public BasicBlock TargetBlock
        {
            get { return targetBlock; }
            //set { targetBlock = value; }
        }

        public string File
        {
            get { return file; }
            set { file = value; }
        }

        public string Method
        {
            get { return method; }
            set { method = value; }
        }

        public uint Line1
        {
            get { return line1; }
            set { line1 = value; }
        }

        public uint Line2
        {
            get { return line2; }
            set { line2 = value; }
        }

        public Phx.Graphs.FlowGraph CFG
        {
            get { return this.flowGraph; }
        }

        public Phx.PEModuleUnit Module
        {
            get { return this.moduleUnit; }
        }

        public Phx.FunctionUnit MethodDef
        {
            get { return this.functionUnit; }
        }

        public CFGAnalysis(string file, string method, HashSet<Problem> problems = null, uint target = 0)
        {
            this.file = file;
            this.method = method;
            this.line1 = 0;
            this.line2 = 0;
            if (target != 0)
                this.SetTarget(target);
            if (problems != null)
                this.SetProblems(problems);

            //initialize phoenix
            this.InitializePhx();
            this.SetModule(this.file);
            this.SetMethod(this.method);
            this.BuildCFG();
            this.flowGraph.BuildPostDominators();
        }

        public void SetProblems(HashSet<Problem> problems)
        {
            this.problems = problems;
        }

        private void InitializePhx()
        {
            Phx.Targets.Architectures.Architecture arch = Phx.Targets.Architectures.Msil.Architecture.New();
            Phx.Targets.Runtimes.Runtime runtime = Phx.Targets.Runtimes.Vccrt.Win.Msil.Runtime.New(arch);

            GlobalData.RegisterTargetArchitecture(arch);
            GlobalData.RegisterTargetRuntime(runtime);

            Initialize.BeginInitialization();
            string[] argv = new string[] { };
            Initialize.EndInitialization("PHX|*|_PHX_|", argv);

        }

        public void SetModule(string file)
        {
            this.file = file;
            this.moduleUnit = PEModuleUnit.Open(
                //@"D:\Research\MSRA Internship\TryPhoenix\ClassLibrary1\bin\Debug\ClassLibrary1.dll"
                this.file
                );
            this.moduleUnit.RaiseMsilOnly = true;
            this.moduleUnit.LoadGlobalSymbols();
            this.moduleUnit.LoadEncodedIRUnitList();
            this.moduleUnit.LayoutAggregateType();
        }

        public void SetMethod(string method)
        {
            this.method = method;
            //this.functionUnit = null;
            for (Phx.ContributionUnitIterator contribUnitIterator = this.moduleUnit.GetEnumerableContributionUnit().GetEnumerator(); contribUnitIterator.MoveNext(); )
            {
                if (contribUnitIterator.Current.IsFunctionUnit)
                {
                    this.functionUnit = contribUnitIterator.Current.AsFunctionUnit;
                    if (this.functionUnit.Name.NameString.Equals(this.method))
                        break;
                }
            }
        }

        public void BuildCFG()
        {
            //functionUnit.DisassembleToBeforeLayout();
            this.functionUnit.ParentPEModuleUnit.Raise(this.functionUnit.UnitSymbol.AsFunctionSymbol, Phx.FunctionUnit.HighLevelIRFunctionUnitState);

            this.functionUnit.BuildFlowGraphWithoutEH();

            if (this.functionUnit.SsaInfo == null)
            {
                this.functionUnit.SsaInfo = Phx.SSA.Info.New(
                                             this.functionUnit.Lifetime,
                                             this.functionUnit,
                                             Phx.SSA.BuildOptions.DefaultNotAliased
                                          );
                this.functionUnit.SsaInfo.Build();
            }

            //functionUnit.DumpIR();
            this.flowGraph = this.functionUnit.FlowGraph;
        }

        public void DumpCFG()
        {

            foreach (Node node in flowGraph.Nodes)
            {
                BasicBlock block = node.AsBasicBlock;

            }
            this.flowGraph.Dump();
        }

        //check whether there exists a directed path from line1 to line2
        //if the two line belong to the same basic block, then return true
        public bool ExistPath(uint line1, uint line2)
        {
            this.line1 = line1;
            this.line2 = line2;
            return this.ExistPath();
        }

        public BasicBlock GetBlockOfInstruction(uint targetInst)
        {
            foreach (Node node in this.flowGraph.Nodes)
            {
                BasicBlock block = node.AsBasicBlock;
                foreach (Instruction inst in block.Instructions)
                {
                    uint line = this.functionUnit.DebugInfo.GetLineNumber(inst.DebugTag);
                    if (line == targetInst)
                    {
                        return block;
                    }
                }
            }
            return null;
        }

        private bool ExistPath()
        {
            BasicBlock block1 = null;
            BasicBlock block2 = null;
            foreach (Node node in this.flowGraph.Nodes)
            {
                BasicBlock block = node.AsBasicBlock;
                foreach (Instruction inst in block.Instructions)
                {
                    uint line = this.functionUnit.DebugInfo.GetLineNumber(inst.DebugTag);
                    if (block1 == null && line == this.line1)
                    {
                        block1 = block;
                        if (block2 != null)
                            break;
                    }
                    if (block2 == null && line == this.line2)
                    {
                        block2 = block;
                        if (block1 != null)
                            break;
                    }
                }
            }

            Console.WriteLine(block1.Id);
            Console.WriteLine(block2.Id);
            if (block1 == block2)
            {
                if (this.line1 <= this.line2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return ExistPathBetweenBlocks(block1, block2);
            }
        }

        public bool ExistPathBetweenBlocks(BasicBlock block1, BasicBlock block2)
        {
            Stack<BasicBlock> stack = new Stack<BasicBlock>();
            List<BasicBlock> visitedBlocks = new List<BasicBlock>();
            stack.Push(block1);
            while (stack.Count != 0)
            {
                BasicBlock block = stack.Pop();
                if (visitedBlocks.Contains(block))
                {
                    continue;
                }
                else
                {
                    visitedBlocks.Add(block);
                }
                foreach (Edge edge in block.SuccessorEdges)
                {
                    BasicBlock successor = edge.SuccessorNode.AsBasicBlock;
                    if (successor == block2)
                    {
                        return true;
                    }
                    else
                    {
                        stack.Push(successor);
                    }
                }
            }
            return false;
        }

        //answer whether the block2 is control dependent on block1's last branch instruction
        //check condition 1: block1 is not postdominated by block2
        //check condition 2: There exists a directed path P from block1 to block2
        //with any node in P (excluding block1 and block2) postdominated by block2
        private bool IsControlDependent(BasicBlock block1, BasicBlock block2)
        {
            //check condition 1
            if (block1 == block2)
                return false;
            if (block2.PostDominates(block1))
                return false;

            //check condition 2
            Stack<BasicBlock> s = new Stack<BasicBlock>();
            s.Push(block2);
            HashSet<BasicBlock> visitedBlocks = new HashSet<BasicBlock>();
            while (s.Count != 0)
            {
                BasicBlock block = s.Pop();
                if (visitedBlocks.Contains(block))
                {
                    continue;
                }
                else
                {
                    visitedBlocks.Add(block);
                }
                foreach (Edge edge in block.PredecessorEdges)
                {
                    BasicBlock predecessor = edge.PredecessorNode.AsBasicBlock;
                    if (predecessor == block1)
                    {
                        //find the source block and return true
                        return true;
                    }
                    else
                    {
                        //if block2 does not dominates the node, then stop searching this branch
                        if (!block2.PostDominates(predecessor))
                        {
                            continue;
                        }
                        else
                        {
                            s.Push(predecessor);
                        }
                    }
                }
            }
            return false;
        }

        //A node n in a program’s CFG is v-control-dependent on a
        //node m iff n postdominates m’s v CFG successor, but does not postdominate m
        //(by definition, every node postdominates itself ).
        //in this case, m is the problem block, v is a certain successor of the problem block, n is the block to be checked
        private bool IsVControlDependent(BasicBlock m, BasicBlock v, BasicBlock n)
        {
            bool isSuccessor=false;
            //check whether v is a successor of m;
            foreach (Edge edge in m.SuccessorEdges)
            {
                BasicBlock block = edge.SuccessorNode.AsBasicBlock;
                if (block == v)
                {
                    isSuccessor = true;
                    break;
                }
            }
            if (!isSuccessor)
            {
                throw new System.Exception("v is not a successor of m");
            }

            if (n.PostDominates(v) && !n.PostDominates(m))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetEstimatedBenefit(Problem problem, uint lineNumOfVBlock, BasicBlock[] unCoveredBlock)
        {
            BasicBlock problemBlock = this.GetBlockOfInstruction(problem.LineNumber);
            BasicBlock vBlock = this.GetBlockOfInstruction(lineNumOfVBlock);
            int benefit = 0;
            foreach (BasicBlock block in unCoveredBlock)
            {
                if (IsVControlDependent(problemBlock, vBlock, block))
                {
                    benefit++;
                }
            }
            return benefit;
        }

        public void SetTarget(uint lineNumber)
        {
            this.target = lineNumber;
            this.targetBlock = this.GetBlockOfInstruction(this.target);
        }

        public bool IsControlDependent(uint line1, uint line2)
        {
            BasicBlock block1 = this.GetBlockOfInstruction(line1);
            BasicBlock block2 = this.GetBlockOfInstruction(line2);
            return this.IsControlDependent(block1, block2);
        }

        public Problem ChooseNextProblem(Metric metric)
        {
            //key is problem, value is the cost of solving the problem
            Dictionary<Problem, int> candidates = new Dictionary<Problem, int>();
            foreach (Problem problem in this.problems)
            {
                BasicBlock block = this.GetBlockOfInstruction(problem.LineNumber);
                //if problem and target are in the same block, 
                //then need to manually examine the case
                if (block == this.targetBlock)
                {
                    throw new System.Exception("Problem and target are in the same block");
                }
                //if there is no path from problem to the target, no need to handle the problem
                if (!this.ExistPath(problem.LineNumber, this.target))
                {
                    continue;
                }
                //if the target postdominate the problem, then the target will be reached anyway regardless of
                //whether the problem is solved or not. Hence, no need to handle the problem
                if (this.targetBlock.PostDominates(block))
                {
                    continue;
                }
                //otherwise, there exists a path from the problem to the target and 
                //the problem is not postdominated by the target, which would be our candidate set
                Path path;
                int cost = this.GetProblemCost(block, metric, out path);
                candidates.Add(problem, cost);
            }

            return this.FindEasiestProblem(candidates);
        }

        private Problem FindEasiestProblem(Dictionary<Problem, int> candidates)
        {
            if (candidates.Count == 0)
                return null;
            HashSet<Problem> ties = new HashSet<Problem>();
            int max = candidates.Values.Max<int>();
            foreach (var key in candidates.Keys)
            {
                if (candidates[key] == max)
                {
                    ties.Add(key);
                }
            }
            if (ties.Count > 1)
            {
                //break ties randomly
                Random random = new Random();
                int index = random.Next(ties.Count);
                return ties.ElementAt(index);
            }
            else
            {
                return ties.First();
            }
        }

        private void GetAllPathsToTarget(BasicBlock start, List<Path> allPaths, Stack<BasicBlock> stack)
        {
            //recursive depth-first search algorithm to find all simple paths between two nodes
            stack.Push(start);
            if (start == this.targetBlock)
            {
                allPaths.Add(new Path(stack.ToArray(), this));
            }
            else
            {
                foreach (Edge edge in start.SuccessorEdges)
                {
                    BasicBlock successor = edge.SuccessorNode.AsBasicBlock;
                    if (!stack.Contains(successor))
                    {
                        this.GetAllPathsToTarget(successor, allPaths, stack);
                    }
                }
            }
            stack.Pop();
        }

        private int GetProblemCost(BasicBlock problemBlock, Metric metric, out Path result)
        {
            List<Path> allPaths = new List<Path>();
            this.GetAllPathsToTarget(problemBlock, allPaths, new Stack<BasicBlock>());
            result = null;
            if (allPaths.Count == 0)
            {
                throw new System.Exception("No path found from problem to the target");
            }

            int minCost = Int32.MaxValue;
            foreach (Path path in allPaths)
            {
                path.CalculatePathCost(metric);
                if (path.Cost < minCost)
                    minCost = path.Cost;
            }
            HashSet<Path> ties = new HashSet<Path>();
            foreach (Path path in allPaths)
            {
                if (path.Cost == minCost)
                {
                    ties.Add(path);
                }
            }
            if (ties.Count == 1)
            {
                result = ties.First();
            }
            else
            {
                //break ties randomly
                Random random = new Random();
                int index = random.Next(ties.Count);
                result = ties.ElementAt(index);
            }
            return minCost;
        }

        /// <summary>
        /// arg0 file name, arg1 method name
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            //CFGAnalysis analyzer = new CFGAnalysis(args[0], args[1]);
            //analyzer.DumpCFG();
            //Console.WriteLine(analyzer.ExistPath());
            Stack<int> s=new Stack<int>();
            s.Push(1);
            s.Push(2);
            int[] array=s.ToArray();
            Console.WriteLine(array.First());
            Console.WriteLine(array.Last());
        }
    }
}
