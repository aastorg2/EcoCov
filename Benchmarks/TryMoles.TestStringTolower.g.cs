// <copyright file="TryMoles.TestStringTolower.g.cs">Copyright ? 2009</copyright>
// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using Microsoft.Moles.Framework.Moles;
using Microsoft.Pex.Framework;

namespace Benchmarks
{
    public partial class TryMoles
    {
[TestMethod]
[PexGeneratedBy(typeof(TryMoles))]
public void TestStringTolower349()
{
    TryMoles.TestStringTolower((string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TryMoles))]
[Ignore]
[PexDescription("this test requires to run under the Pex profiler in order to reproduce")]
[PexRaisedException(typeof(MoleNotInstrumentedException))]
[PexNotReproducible]
[HostType("Moles")]
public void TestStringTolowerThrowsMoleNotInstrumentedException328()
{
    TryMoles.TestStringTolower((string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TryMoles))]
[HostType("Moles")]
public void TestStringTolower34901()
{
    TryMoles.TestStringTolower((string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TryMoles))]
[HostType("Moles")]
public void TestStringTolower34902()
{
    TryMoles.TestStringTolower((string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TryMoles))]
[HostType("Moles")]
public void TestStringTolower34903()
{
    TryMoles.TestStringTolower((string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TryMoles))]
[HostType("Moles")]
public void TestStringTolower34904()
{
    TryMoles.TestStringTolower((string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TryMoles))]
[HostType("Moles")]
public void TestStringTolower34905()
{
    IPexChoiceRecorder choices = PexChoose.Replay.Setup();
    choices.DefaultSession
        .At(0, "mock", (object)true);
    TryMoles.TestStringTolower((string)null);
}
    }
}
