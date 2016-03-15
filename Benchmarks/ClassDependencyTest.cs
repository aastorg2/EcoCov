using System;
using System.Collections.Generic;
using Microsoft.Pex.Framework;
using Covana.ProblemExtractor;
using System.Diagnostics.Contracts;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Explorable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ExternalLib;
using Microsoft.Moles.Framework;
using FixSimulation;
using Microsoft.ExtendedReflection.Metadata;
using System.Reflection;
//[assembly: MoledType(typeof(ExternalObj2))]


namespace Benchmarks
{
    public class InterfaceDef
    {
        private TypeDef parent;
        private string name;

        public InterfaceDef(TypeDef p, string name)
        {
            this.parent = p;
            this.name = name;
        }

        public string Name { get { return name; } }
    }


    public class OwnList
    {
        public int size;
        public int Size { get { return size; } }

        private int[] array;
        public int[] Array { get { return array; } }


        public OwnList() { }
        public OwnList(int[] a, int s) { array = a; size = s; }
    }

    public class TypeDef
    {
        private TypeDef parent;
        private string name;
        private List<InterfaceDef> interfaces;
        private OwnList temps;

        //  [ContractInvariantMethod]
        //private void Invariant()
        //{
        //  items array is not null
        //     Contract.Invariant(this.temps != null);
        //   count in [0, items.Length]
        //    Contract.Invariant(0 < this.temps.Size);
        //}

        public TypeDef()
        {


        }


        public void CreateTemp()
        {
            temps = new OwnList();
        }

        public OwnList Temps { get { return temps; } }

        public TypeDef(TypeDef p, string name)
        {
            this.parent = p;
            this.name = name;
        }

        public TypeDef Parent { get { return parent; } set { parent = value; } }

        public List<InterfaceDef> Interfaces
        {
            get
            {
                if (interfaces != null)
                {
                    return interfaces;
                }

                return interfaces = new List<InterfaceDef>();
            }
        }

        public void Resolve()
        {
            if (parent == null)
            {
                throw new Exception("excpetion");
            }
        }

        public bool HasInterfaces
        {
            get
            {
                if (interfaces != null)
                    return interfaces.Count > 0;

                return false;
            }
        }

        public string Name { get { return name; } }

        public List<InterfaceDef> EasyInterfaces { get; set; }
    }

    public class C
    {
        public int size;
    }

    public class B
    {
        public C c;
    }


    public class A
    {
        private B b;

        public B getB() { return b; }

        //    [ContractInvariantMethod]
        private void Invariant()
        {

            //Contract.Invariant(this.b != null);
            //   count in [0, items.Length]
            //    Contract.Invariant(0 < this.temps.Size);
        }
    }

    public class X
    {
        private B b;

        public B getB() { return b; }

        public X(B b) { this.b = b; }
    }



    [PexClass(typeof(ClassDependencyTest), TestEmissionFilter = PexTestEmissionFilter.All)]
    public partial class ClassDependencyTest
    {
        [PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        public static void TestABC(X x, B b)
        {
            //a = PexInvariant.CreateInstance<A>();
            //PexInvariant.SetField<B>((object)a, "b", b);
            if (b.c != null && b.c.size > 10)
            {
                var xb = x.getB();
                var xc = xb.c;
                if (xc.size > 5)
                {

                    Console.WriteLine("Find~!");

                }

            }


            PexObserve.ValueForViewing("pc", PexSymbolicValue.GetRawPathConditionString());
        }

        [PexMethod]
        public static void InfeasibleTest(int x)
        {
            if (x > 5)
            {
                Console.WriteLine("x > 5");
                if (x < 5)
                {
                    Console.WriteLine("infeasible");
                }
            }
        }

        [PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        public static void Test(TypeDef t, B b)
        {
            //TypeDef q = ReflectionUtil.ReflectionUtil.SymbolicCopyFields(t);
            //if (q != null)
            //{
            //    t = q;
            //}
            //var field = t.GetType().GetField("temps", BindingFlags.Public |
            //                                 BindingFlags.NonPublic |
            //                                 BindingFlags.Instance);
            //var ownlist = field.GetValue(t);
            //if (ownlist != null)
            //{
            //    PexInvariant.SetField<int>((object)ownlist, "size", PexChoose.Value<int>("test2"));
            //}

            if (b.c.size > 10)
            {
                if (t.Temps != null)
                {
                    Console.WriteLine("not null");
                }

                if (t.Temps.Size > 0)
                {
                    Console.WriteLine("size > 0");
                }



                //var ifs = t.Interfaces;
                for (int i = 0; i < t.Temps.Size; i++)
                {
                    Console.WriteLine("size > 0");
                    if (t.Temps.Array[i] == 5)
                    {
                        Console.WriteLine("covered");
                    }

                }
            }
            //if (t.Temps.Size > 5)
            //{
            //    Console.WriteLine("> 5");
            //}
            PexObserve.ValueForViewing("pc", PexSymbolicValue.GetPathConditionString());
        }

        [PexMethod]
        [HostType("Moles")] // moles require the Moles host type
        public void StaticMethodUnitTestWithDeqFile(string filename)
        {
            Deq.Replace<bool>(() => File.Exists(null), () => PexChoose.Value<bool>("mock"));
            if (File.Exists(filename))
            {
                Console.WriteLine("yes");
            }
            else
            {
                Console.WriteLine("No");
            }
        }

        [PexMethod]
        [HostType("Moles")] // moles require the Moles host type
        public void MultipleCauses(TypeDef t, string filename, int k, int i, int y, int z)
        {
            //   Deq.Replace<int>(() => ExternalObj2.Compute(0), () => PexChoose.Value<int>("test"));
            //   bool exists = File.Exists(filename);
            int value = ExternalObj.Compute(y);
            if (i > 5)
            {
                if (value < 5)
                {
                    Console.WriteLine("value > 5!");
                    return;
                }

            }
            else
            {
                int newvalue = ExternalObj2.Compute(z);

                if (newvalue < 5)
                {
                    Console.WriteLine("value > 5!");
                    return;
                }

                if (t.Temps.Size < 5)
                {
                    return;
                }
            }

            if (k > 100)
            {
                Console.WriteLine("Goal!!");
            }


        }
    }
}
