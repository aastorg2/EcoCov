using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CFGAnalysis
{
    public class TestCase
    {
        static void TryException()
        {
            int c = 5;
            bool a = File.Exists("aaa");
            c++;
        }

        static void Reduce(string[] args)
        {
            int a = 0;
            bool b = true;
            if (b)
            {
                a++;
            }
            else
            {
                a--;
            }
            while (a < 10)
            {
                a++;
            }
            for (int i = 0; i < 10; i++)
            {
                a++;
                if (a > 5)
                {
                    a-=16;
                    break;
                }
            }
            if (a > 1)
            {
                a += 64;
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    if (i % 2 == 0)
                        a++;
                    for (int j = 0; j < 5; j++)
                    {
                        a *= 2;
                    }
                    if (a > 100)
                    {
                        a += 10;
                    }
                    else
                    {
                        a -= 10;
                    }
                }
                List<int> list = new List<int>();
                foreach (var i in list)
                {
                    if (i % 2 == 0)
                        a++;
                    else
                    {
                        while (list.Count != 0)
                        {
                            a++;
                            if (a > 100)
                            {
                                a += 32;
                                a = list[i];
                                break;
                            }
                            a++;
                        }
                    }
                }
            }
            a -= 10;
        }
    }
}
