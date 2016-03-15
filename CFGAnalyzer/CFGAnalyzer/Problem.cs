using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFGAnalyzer
{
    public class Problem
    {
        private static int idCounter = 0;
        private int id;
        private uint lineNumber;
        private string name;

        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public uint LineNumber
        {
            get { return this.lineNumber; }
            set { this.lineNumber = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public Problem(uint lineNumber, string name=null)
        {
            this.lineNumber = lineNumber;
            this.name = name;
            this.id = Problem.idCounter;
            Problem.idCounter++;
        }
    }
}
