using System;
using System.IO;
using Microsoft.Pex.Framework;
using Covana.ProblemExtractor;
using Covana.ResultTrackingExtrator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Moles.Framework;

[assembly: MoledType(typeof(System.IO.Path))]
namespace Benchmarks
{
    public partial class CombinePathUtility
    {
        public static void CombinePath(string path)
        {
            if (System.IO.Path.Combine(path, "quick.png") == "ok")
            {
                Console.WriteLine("combine succeessfully");
            }
            else
            {
                Console.WriteLine("failure");
            }

            Console.WriteLine("after combining " + path + " with quick.png");
        }
    }

    [PexClass(typeof(CombinePathUtility))]
    public partial class ExceptionThrownbyExternalMethodsTest
    {
        [PexMethod]
        [HostType("Moles")]
        public void TestPrintPath(string filename)
        {
            Deq.Replace<System.String>(() => System.IO.Path.Combine(null, null), () => PexChoose.Value<System.String>("mock"));
            CombinePathUtility.CombinePath(filename);
        }

    }
}