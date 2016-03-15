using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Microsoft.ExtendedReflection.Monitoring;
using System.Reflection;
using Microsoft.Moles.Framework.Moles;
using Microsoft.Moles.Framework;
using Microsoft.Pex.Framework;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//[assembly: MoledType(typeof(DateTime))]
//[assembly: MoledType(typeof(System.IO.File))]

namespace Benchmarks
{
    #region snippet Deq
    /// <summary>
    /// Deq is a minimalistic example that shows how a mock framework
    /// could leverage the Moles detour facilities to support
    /// mocking any .NET method.
    /// </summary>
    [__DoNotInstrument]
    public class Deq
    {
        /// <summary>
        /// A method that allows to replace any static method
        /// matching the Func&lt;T&gt; signature with a delegate
        /// in a strongly typed fashion.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="detour"></param>
        public static void Replace<T>(
            Expression<Func<T>> method,
            Func<T> detour)
        {
            using (_ProtectingThreadContext.Acquire())
            {
                // ExtractCall walks the expression tree 
                // and extracts the method being called
                MethodInfo call = ExtractCall(method.Body);
                MoleRuntime.SetMole(detour, null, call);
            }
        }

        /// <summary>
        /// Extracts the method info from the expression. Throws
        /// if the expression is not a static method call.
        /// </summary>
        private static MethodInfo ExtractCall(Expression expression)
        {
            // This is a sample. It is probably not the most efficient
            // or clean way to extract the method call.
            var callExpression = expression as MethodCallExpression;
            if (callExpression != null)
                return callExpression.Method;

            var memberAccess = expression as MemberExpression;
            if (memberAccess != null)
            {
                var property = memberAccess.Member as PropertyInfo;
                if (property != null)
                    return property.GetGetMethod();
            }
            
            throw new ArgumentException("expression only support static method calls");
        }
    }
    #endregion

    /// <summary>
    /// This example uses the Deq helper method and does not require
    /// compiled Moles assemblies.
    /// </summary>
    [PexClass(typeof(CantStubMeWithDeqTest))]
    public partial class CantStubMeWithDeqTest
    {
        #region snippet DeqY2k
        [TestMethod, PexMethod]
        [HostType("Moles")] // moles require the Moles host type
        public void StaticMethodUnitTestWithDeq()
        {
            // we set up a mole to return 123.
            Deq.Replace(() => DateTime.Now, () => new DateTime(2000, 1, 1));
            Assert.AreEqual(new DateTime(2000, 1, 1), DateTime.Now);
        }
        #endregion

        [TestMethod, PexMethod]
        [HostType("Moles")] // moles require the Moles host type
        public void StaticMethodUnitTestWithDeqFile(string name)
        {
            // we set up a mole to return 123.
            Deq.Replace<bool>(() => File.Exists("yes"), () => PexChoose.Value<bool>("test"));
            if (File.Exists(name))
            {
                Console.WriteLine("yes");
            }
            else
            {
                Console.WriteLine("No");
            }
        }
    }
}
