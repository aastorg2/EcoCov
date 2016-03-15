using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Moles.Framework;
using Microsoft.Pex.Framework;
using System.IO.Moles;
using System.Moles;
using System.Collections.Generic.Moles;

[assembly: MoledType(typeof(System.IO.File))]
[assembly: MoledType(typeof(System.IO.DirectoryInfo))]
namespace Benchmarks
{
    [PexClass]
    public partial class TryMoles
    {
        [PexMethod]
        [HostType("moles")]
        public static void TestFileExists(String path)
        {
            //MFile.ExistsString = (s) => PexChoose.Value<System.Boolean>("mock");
            MFile.ExistsString = delegate(String s)
            {
                return PexChoose.Value<System.Boolean>("mock");
            };
            if (File.Exists(path))
                Console.WriteLine(true);
            else
                Console.WriteLine(false);    
        }

        [PexMethod]
        [HostType("Moles")]
        public static void TestStringTolower(String str)
        {
            //info = new MDirectoryInfo();
            DirectoryInfo info = new DirectoryInfo("s");
            MDirectoryInfo minfo = new MDirectoryInfo(info);
            minfo.ExistsGet = () => PexChoose.Value<System.Boolean>("mock");
            //MDirectoryInfo.AllInstances.ExistsGet = dirInfo => true;
            if (info.Exists)
                Console.WriteLine(true);
            else
                Console.WriteLine(false);
        }

        [PexMethod]
        public static void Parse(string fullName)
        {
            //if (fullName == null)
            //    throw new ArgumentNullException("fullName");
            //if (fullName.Length == 0)
            //    throw new ArgumentException("Name can not be empty");

            ////var name = new AssemblyNameReference();
            //var tokens = fullName.Split(',');
            //for (int i = 0; i < tokens.Length; i++)
            //{
            //    var token = tokens[i].Trim();

            //    if (i == 0)
            //    {
            //        //name.Name = token;
            //        continue;
            //    }

            //    var parts = token.Split('=');
            //    if (parts.Length != 2)
            //        throw new ArgumentException("Malformed name");

                switch (fullName.ToLowerInvariant())
                {
                    case "version":
                        //name.Version = new Version(parts[1]);
                        break;
                    case "culture":
                        //name.Culture = parts[1];
                        break;
                    case "publickeytoken":
                        

                        break;
                }
            //}

            //return name;
        }
    }
}
