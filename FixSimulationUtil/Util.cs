using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Pex.Framework.Explorable;
using Microsoft.ExtendedReflection.Metadata;

namespace FixSimulation
{
    [__DoNotInstrument]
    public class Util
    {

        public static object GetField(object t, string name)
        {

            var fields = t.GetType().GetFields(BindingFlags.Public |
                                             BindingFlags.NonPublic |
                                             BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.Name == name)
                {
                    return field.GetValue(t);
                }
            }

            return null;
        }

        public static T SymbolicCopyFields<T>(T from) where T : class
        {
            if (from == null)
            {
                return null;
            }

            T to = PexInvariant.CreateInstance<T>();
            try
            {
                if (from != null)
                {
                    var tt = Metadata<T>.Type;
                    foreach (var field in tt.InstanceFields)
                    {
                        PexInvariant.SetField((object)to, field.ShortName, field.GetValue(from));
                    }
                }
            }
            catch (Exception e)
            {

                return null;
            }

            return to;
        }

        public static bool TryCopyFields<T>(T to, T from) where T : class
        {
            if (to.GetType() != from.GetType())
            {
                return false;
            }

            var fields = to.GetType().GetFields(BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance);
            try
            {
                foreach (var field in fields)
                {
                    //field.SetValue(to, field.GetValue(from));
                    PexInvariant.SetField(to, field.Name, field.GetValue(from));
                }
            }
            catch (Exception e)
            {

                return false;
            }

            return true;
        }

        public static bool TryCopyFields<T>(T to, T from, string name) where T : class
        {
            if (to.GetType() != from.GetType())
            {
                return false;
            }

            var fields = to.GetType().GetFields(BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance);
            try
            {
                foreach (var field in fields)
                {
                    if (field.Name == name)
                    {
                        continue;
                    }
                    field.SetValue(to, field.GetValue(from));
                }
            }
            catch (Exception e)
            {

                return false;
            }

            return true;
        }
    }
}
