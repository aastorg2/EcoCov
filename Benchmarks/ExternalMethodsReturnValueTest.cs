using System;
using System.IO;
using System.Text;
using ExternalLib;
using FieldAccessExtractor;
using Microsoft.Pex.Framework;
using Covana.CoverageExtractor;
using Covana.ResultTrackingExtrator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Moles.Framework;
//[assembly: MoledType(typeof(ExternalLib.ExternalObj))]

namespace Benchmarks
{
    [PexClass(typeof(ExternalMethodsReturnValueTest))]
    public partial class ExternalMethodsReturnValueTest
    {
        [PexMethod]
        [HostType("Moles")]
        public void ExternalMethodReturnValueTest(int y)
        {
            //Deq.Replace<System.Int32>(() => ExternalLib.ExternalObj.Compute(0), () => PexChoose.Value<System.Int32>("mock"));

            
            int value = ExternalObj.Compute(y);

            Console.Out.WriteLine("after computation, value is: " + value);

            if (value > 5)
            {
                Console.WriteLine("value > 5!");
            }
        }
    }
}