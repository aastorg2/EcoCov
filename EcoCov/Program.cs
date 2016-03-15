using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcoCov
{
    class Program
    {
        static void Main(string[] args)
        {
            //Command command = new Command();
            //command.executable = "pex.x86.exe";

            //            CommandExecutor.ExecuteCommand(CommandGenerator.GenerateBuildCommand(
            //@"C:\Users\LSH\Desktop\MSBuild\TestProgram.csproj"
            //));

            //            string[] methods = { "Reverse", "GreatestCommonDenominator" };
            //            Command command = CommandGenerator.GenerateRunPexCommand(
            //                    @"C:\Users\LSH\Desktop\MSBuild\bin\Debug\TestProgram.exe",
            //                    "TestProgram", "Program", methods);
            //            CommandExecutor.ExecuteCommand(command);

            //FileModifier.CopyDirectory(@"C:\Users\LSH\Desktop\TestGenerationProblems\",
            //    @"C:\Users\LSH\Desktop\TestGenerationProblemsCopy\");

            //string[] references = { "System.Text", "System.IO" };
            //FileModifier.AddUsingStatements(
            //@"C:\Users\LSH\Desktop\TestGenerationProblems - Copy\TestGenerationProblems\Program.cs", references);
            //            string[] methods={"Reverse"};
            //            FileModifier.AddPexAttribute(
            //@"C:\Users\LSH\Desktop\TestProgram\TestProgram\Program.cs", null, methods);

            //string[] methods = { "TestMultiPop" };
            //Algorithms.BenefitEstimation(@"C:\Users\LSH\Desktop\EconomicAnalysis\covana\Benchmarks", "ObjectCreation", "Benchmarks.Stack",
            //    @"C:\Users\LSH\Desktop\EconomicAnalysis\covana\Benchmarks\bin\Debug\Benchmarks.dll", "Benchmarks", "FixedSizeStackTest",
            //    methods);

            //string[] methods = { "ExternalMethodReturnValueTest" };
            //Algorithms.BenefitEstimation(@"C:\Users\LSH\Desktop\covana\Benchmarks", "UnInstrumentedMethod", "ExternalLib.ExternalObj.Compute",
            //    @"C:\Users\LSH\Desktop\covana\Benchmarks\bin\Debug\Benchmarks.dll", "Benchmarks", "ExternalMethodsReturnValueTest",
            //    methods);

            //string[] methods = { "TestPrintPath" };
            //Algorithms.BenefitEstimation(@"C:\Users\LSH\Desktop\covana\Benchmarks", "UnInstrumentedMethod", "System.IO.Path.Combine",
            //    @"C:\Users\LSH\Desktop\covana\Benchmarks\bin\Debug\Benchmarks.dll", "Benchmarks", "ExceptionThrownbyExternalMethodsTest",
            //    methods);

            string[] methods = args[6].Split(';');
            Algorithms.BenefitEstimation(args[0], args[1], args[2], args[3], args[4], args[5], methods);
        }
    }
}
