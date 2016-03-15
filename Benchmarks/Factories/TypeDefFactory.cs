// <copyright file="TypeDefFactory.cs">Copyright ? 2009</copyright>

using System;
using Microsoft.Pex.Framework;
using Benchmarks;

namespace Benchmarks
{
    /// <summary>A factory for Benchmarks.TypeDef instances</summary>
    public static partial class TypeDefFactory
    {
        /// <summary>A factory for Benchmarks.TypeDef instances</summary>
        //[PexFactoryMethod(typeof(TypeDef))]
        public static TypeDef Create(TypeDef p_typeDef1, string name_s, string[] names)
        {
            TypeDef typeDef = new TypeDef(p_typeDef1, name_s);
            var ifs = typeDef.Interfaces;
            foreach (var i in names)
            {
                ifs.Add(new InterfaceDef(null, i));
            }

            return typeDef;

            // TODO: Edit factory method of TypeDef
            // This method should be able to configure the object in all possible ways.
            // Add as many parameters as needed,
            // and assign their values to each field by using the API.
        }
    }
}
