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
    public enum Metric
    {
        NumOfNodes,
        NumOfBranches,
        NumOfRelevantBranches,
        NumOfProblems,
        NumOfRelevantProblems
    }

    public class Path
    {
        
        //the sequence of nodes in the paths;
        private BasicBlock[] nodes;
        public BasicBlock[] Nodes
        {
            get { return this.nodes; }
        }
        private CFGAnalysis owner;
        public CFGAnalysis Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }
        private int cost;
        public int Cost
        {
            get { return this.cost; }
        }

        private int numOfNodes=0;
        private int numOfBranches=0;
        private int numOfRelevantBranches=0;
        private int numOfProblems=0;
        private int numOfRelevantProblems=0;

        public Path(BasicBlock[] nodes)
        {
            this.nodes = nodes;
        }

        public Path(BasicBlock[] nodes, CFGAnalysis owner)
        {
            this.nodes = nodes;
            this.owner = owner;
        }

        public void CalculatePathCost(Metric metric)
        {
            if (metric == Metric.NumOfBranches)
            {
                this.cost = this.GetNumOfBranches();
            }
            else if (metric == Metric.NumOfRelevantBranches)
            {
                this.cost = this.GetNumOfRelevantBranches();
            }
            else if (metric == Metric.NumOfNodes)
            {
                this.cost = this.GetNumOfNodes();
            }
            else if (metric == Metric.NumOfProblems)
            {
                this.cost = this.GetNumOfProblems();
            }
            else if (metric == Metric.NumOfRelevantProblems)
            {
                this.cost = this.GetNumOfRelevantProblems();
            }
            throw new System.Exception("undefined metric");
        }

        public int GetNumOfNodes()
        {
            this.numOfNodes = this.nodes.Length-2;
            return this.numOfNodes;
        }

        public int GetNumOfBranches()
        {
            foreach (BasicBlock block in this.nodes)
            {
                if (block != nodes.First() && block != nodes.Last())
                {
                    if (block.SuccessorEdges.Count() > 1)
                    {
                        this.numOfBranches++;
                    }
                }
            }
            return this.numOfBranches;
        }

        public int GetNumOfRelevantBranches()
        {
            BasicBlock endOfPath = this.nodes.First();
            foreach (BasicBlock block in this.nodes)
            {
                if (block != nodes.First() && block != nodes.Last())
                {
                    if (block.SuccessorEdges.Count() > 1)
                    {
                        if (!endOfPath.PostDominates(block))
                        {
                            this.numOfRelevantBranches++;
                        }
                    }
                }
            }
            return this.numOfRelevantBranches;
        }

        public int GetNumOfProblems()
        {
            foreach (BasicBlock block in this.nodes)
            {
                if (block != nodes.First() && block != nodes.Last())
                {
                    foreach (Problem problem in this.owner.Problems)
                    {
                        BasicBlock problemBlock = this.owner.GetBlockOfInstruction(problem.LineNumber);
                        if (block == problemBlock)
                        {
                            this.numOfProblems++;
                        }
                    }
                }
            }
            return this.numOfProblems;
        }

        public int GetNumOfRelevantProblems()
        {
            BasicBlock endOfPath = this.nodes.First();
            foreach (BasicBlock block in this.nodes)
            {
                if (block != nodes.First() && block != nodes.Last())
                {
                    foreach (Problem problem in this.owner.Problems)
                    {
                        BasicBlock problemBlock = this.owner.GetBlockOfInstruction(problem.LineNumber);
                        if (block == problemBlock)
                        {
                            if (!endOfPath.PostDominates(block))
                            {
                                this.numOfRelevantProblems++;
                            }
                        }
                    }
                }
            }
            return this.numOfRelevantProblems;
        }
    }
}
