using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ExtendedReflection.Metadata;

namespace Covana
{
    public class PUT
    {
        public string Name { get; set; }
        public string Document { get; set; }
        public int FirstLine { get; set; }
        public string Class { get; set; }
        public List<Para> Paras = new List<Para>();
    }

    [Serializable]
    public class Para
    {
        public HashSet<string> boolTypeNames = new HashSet<string>(new[] { "Boolean" });
        public HashSet<string> numericTypeNames = new HashSet<string>(new[] { "Byte", "Double", "Int16", "Int32", "Int64", "UInt16", "UInt32", "UInt64" });
        public HashSet<string> charTypeNames = new HashSet<string>(new[] { "Char" });

        public Para(string t, string n)
        {
            Type = t;
            Name = n;
            DecideTypeCategory();
        }

        public void DecideTypeCategory()
        {
            var typesplit = Type.Split(new[] { '.' });
            var typename = typesplit[typesplit.Length - 1];
            if (numericTypeNames.Contains(typename))
            {
                IsNumeric = true;
            }
            if (boolTypeNames.Contains(typename))
            {
                IsBool = true;
            }
            if (charTypeNames.Contains(typename))
            {
                IsChar = true;
            }
        }
        public Para() { }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool IsNumeric { get; set; }
        public bool IsBool { get; set; }
        public bool IsChar { get; set; }
        public bool IsObject
        {

            get
            {
                if (!IsNumeric && !IsBool && !IsChar)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
    }

    public class OCPSimulationData
    {
        public Problem Problem { get; set; }
        public HashSet<BranchInfo> Branches { get; set; }
        public List<string> PUTs = new List<string>();
        public List<string> AccessPath = new List<string>();
        public string LastField { get; set; }
        public string LastFieldType { get; set; }
        public string ParaType { get; set; }
    }

    public class EMCPSimuationData
    {
        public Problem Problem { get; set; }
        public HashSet<BranchInfo> Branches { get; set; }
        public List<Para> Paras = new List<Para>();
        public string ReturnType { get; set; }
        public List<string> PUTs=new List<string>();
    }
}
