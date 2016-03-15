EcoCov: Economic Analysis for Problem Resolution in Structural Test Generation
==========================================================================

This is a prototype implementation for the EcoCov framework. EcoCov prioritzie problems encountered by test generation tools (specifically Pex in this implementation) to obtain the max return-on-investment in testing. Below is our project website,
https://sites.google.com/site/ecocov4testgeneration/

Instructions on how to use EcoCov

1. Install VS2010 and Pex.
2. Add EcoCov project into your project reference.
3. Add the following 7 properties in the AssemblyInfo.cs of your test project:
[assembly: IssueTrack]
[assembly: ProblemObserver]
[assembly: IssueObserver]
[assembly: AssemblyCoverageObserver]
[assembly: ResultTrackingObserver]
[assembly: PUTObserver]
[assembly: PUTCoverageObserver]
4. Right click the method or class under test and select "Create Parameterized Unit Tests".
5. Right click the generated PUT and then select "Run Pex Explorations".
6. Set EcoCov project as the startup project and Run EcoCov (sample usage in the code comments). 
