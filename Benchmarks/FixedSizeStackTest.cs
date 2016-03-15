using System;
using System.Collections.Generic;
using Microsoft.Pex.Framework;
using Covana.ProblemExtractor;
using Microsoft.Pex.Framework.Explorable;

namespace Benchmarks
{
    public class Stack
    {
        private List<object> items;

        public Stack()
        {
            items = new List<object>();
        }


        public int Count
        {
            get { return items.Count; }
        }

        public void Push(object item)
        {
            items.Add(item);
        }

        public object Pop()
        {
            if (items.Count > 0)
            {
                object result = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                return result;
            }
            throw new Exception("empty");
        }
    }

    public class FixedSizeStack
    {
        private Stack stack;

        public FixedSizeStack(Stack stack)
        {
            this.stack = stack;
        }

        public void Push(object item)
        {
            if (stack.Count == 10)
            {
                throw new Exception("full");
            }

            stack.Push(item);
        }

        public void MultiPop()
        {
            if (stack.Count < 3)
            {
                throw new Exception("not enough items");
            }
            stack.Pop();
            stack.Pop();
            stack.Pop();
        }
    }

    [PexClass(typeof(FixedSizeStack))]
    public partial class FixedSizeStackTest
    {
        [PexMethod]
        public void TestPush(FixedSizeStack stack, object item)
        {
            
            stack.Push(item);
        }

        [PexMethod]
        public void TestMultiPop(FixedSizeStack stack)
        {
            if (stack != null)
            {
                var field1 = stack.GetType().GetField("stack", System.Reflection.BindingFlags.Public |
                                           System.Reflection.BindingFlags.NonPublic |
                                           System.Reflection.BindingFlags.Instance);
                var fieldobj1 = field1.GetValue(stack);

                if (fieldobj1 != null)
                {
                    var field2 = fieldobj1.GetType().GetField("items", System.Reflection.BindingFlags.Public |
                                               System.Reflection.BindingFlags.NonPublic |
                                               System.Reflection.BindingFlags.Instance);
                    var fieldobj2 = field2.GetValue(fieldobj1);
                    Benchmarks.FixedSizeStack symbolicObj = FixSimulation.Util.SymbolicCopyFields(stack);
                    PexInvariant.SetField<List<object>>(fieldobj1, "items", PexChoose.Value<List<object>>("ocpsymbolic"));

                    //if (fieldobj2 != null)
                    //{
                    //    Benchmarks.FixedSizeStack symbolicObj = FixSimulation.Util.SymbolicCopyFields(stack);
                    //    stack = symbolicObj;

                    //    PexInvariant.SetField<System.Int32>(fieldobj2, "_size", PexChoose.Value<System.Int32>("ocpsymbolic"));
                    //}

                }

            }
            stack.MultiPop();
        }
    }
}