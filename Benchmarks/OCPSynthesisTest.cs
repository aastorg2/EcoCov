using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Settings;
using System.Reflection;
using Microsoft.Pex.Framework.Explorable;

namespace Benchmarks
{
    [PexClass(typeof(OCPSynthesisTest), TestEmissionFilter = PexTestEmissionFilter.All)]
    public partial class OCPSynthesisTest
    {
        [PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        public static void Test1(TypeDef input1, B b)
        {

            if (b.c.size > 10)
            {
                if (input1.Temps != null)
                {
                    Console.WriteLine("not null");
                }

                if (input1.Temps.Size > 0)
                {
                    Console.WriteLine("size > 0");
                }

                //var ifs = t.Interfaces;
                for (int i = 0; i < input1.Temps.Size; i++)
                {
                    Console.WriteLine("size > 0");
                    if (input1.Temps.Array[i] == 5)
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

        [PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        public static void Test2(TypeDef input2, B b)
        {
            //Benchmarks.TypeDef symbolicObj = ReflectionUtil.ReflectionUtil.SymbolicCopyFields(input2);
            //if (symbolicObj != null)
            //{
            //    input2 = symbolicObj;
            //}
            //var field0 = input2.GetType().GetField("temps", BindingFlags.Public |
            //                                 BindingFlags.NonPublic |
            //                                 BindingFlags.Instance);
            //var fieldobj0 = field0.GetValue(input2);
            //if (fieldobj0 != null)
            //{
            //    PexInvariant.SetField<System.Int32>(fieldobj0, "size", PexChoose.Value<System.Int32>("ocpsymbolic"));
            //}
            //PexInvariant.CreateInstance<OCPSynthesisTest>();
            if (b.c.size > 10)
            {
                if (input2.Temps != null)
                {
                    Console.WriteLine("not null");
                }

                if (input2.Temps.Size > 0)
                {
                    Console.WriteLine("size > 0");
                }



                //var ifs = t.Interfaces;
                for (int i = 0; i < input2.Temps.Size; i++)
                {
                    Console.WriteLine("size > 0");
                    if (input2.Temps.Array[i] == 5)
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


        public class Multi
        {
            private List<int> a;
            private List<int> b;

            public List<int> A { get { return a; } }
            public List<int> B { get { return b; } }
        }

        [PexMethod]
        public static void MultiProblemTest(Multi m)
        {
            //if (m != null)
            //{

            //    var symbolicObj = FixSimulation.Util.SymbolicCopyFields(m);
            //    m = symbolicObj;
            //    PexInvariant.SetField<List<int>>(m, "a", PexChoose.Value<List<int>>("ocpsymbolic"));
            //    //PexInvariant.SetField<List<int>>(m, "b", PexChoose.Value<List<int>>("ocpsymbolic"));

            //}

            //if (m != null)
            //{

            //    var symbolicObj = FixSimulation.Util.SymbolicCopyFields(m);
            //    m = symbolicObj;
            //    PexInvariant.SetField<List<int>>(m, "b", PexChoose.Value<List<int>>("ocpsymbolic"));

            //}

            //if (m != null) { PexInvariant.SetField<System.Collections.Generic.List<System.Int32>>(m, "a", PexChoose.Value<System.Collections.Generic.List<System.Int32>>("ocpsymbolic")); }

            if (m.A.Count > 5)
            {
                Console.WriteLine("a > 5");
                if (m.B.Count > 5)
                {
                    Console.WriteLine("a > 5 && b > 5");
                }
            }
        }
    }
}
