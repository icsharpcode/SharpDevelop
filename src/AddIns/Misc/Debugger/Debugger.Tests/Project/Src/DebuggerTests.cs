// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger;
using Debugger.Interop;
using Microsoft.CSharp;
using NUnit.Framework;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Debugger.Tests
{
	/// <summary>
	/// This class contains methods that test the debugger
	/// </summary>
	[TestFixture]
	public class DebuggerTests
	{
		NDebugger debugger;
		string assemblyFilename;
		string assemblyDir;
		string symbolsFilename;
		
		Process process;
		string log;
		string lastLogMessage;
		
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			assemblyFilename = Assembly.GetExecutingAssembly().Location;
			assemblyDir = Path.GetDirectoryName(assemblyFilename);
			symbolsFilename = Path.Combine(assemblyDir, Path.GetFileNameWithoutExtension(assemblyFilename) + ".pdb");
			
			debugger = new NDebugger();
			debugger.MTA2STA.CallMethod = CallMethod.Manual;
		}
		
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			
		}
		
		[TearDown]
		public void TearDown()
		{
			while(debugger.Processes.Count > 0) {
				debugger.Processes[0].Terminate();
				debugger.Processes[0].WaitForExit();
			}
		}
		
		void StartProgram(string programName)
		{
			StartProgram(assemblyFilename, programName);
		}
		
		void StartProgram(string exeFilename, string programName)
		{
			log = "";
			lastLogMessage = null;
			process = debugger.Start(exeFilename, Path.GetDirectoryName(exeFilename), programName);
			process.LogMessage += delegate(object sender, MessageEventArgs e) {
				log += e.Message;
				lastLogMessage = e.Message;
			};
		}
		
		void WaitForPause(PausedReason expectedReason)
		{
			process.WaitForPause();
			Assert.AreEqual(true, process.IsPaused);
			Assert.AreEqual(expectedReason, process.PausedReason);
		}	
		
		void WaitForPause(PausedReason expectedReason, string expectedLastLogMessage)
		{
			WaitForPause(expectedReason);
			if (expectedLastLogMessage != null) expectedLastLogMessage += "\r\n";
			Assert.AreEqual(expectedLastLogMessage, lastLogMessage);
		}
		
		
		[Test]
		public void SimpleProgram()
		{
			StartProgram("SimpleProgram");
			process.WaitForExit();
		}
		
		[Test]
		public void HelloWorld()
		{
			StartProgram("HelloWorld");
			process.WaitForExit();
			Assert.AreEqual("Hello world!\r\n", log);
		}
		
		[Test]
		public void Break()
		{
			StartProgram("Break");
			WaitForPause(PausedReason.Break, null);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void Symbols()
		{
			Assert.AreEqual("debugger.tests.exe", Path.GetFileName(assemblyFilename).ToLower());
			Assert.IsTrue(File.Exists(symbolsFilename), "Symbols file not found (.pdb)");
			
			StartProgram("Symbols");
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual(true, process.GetModule(Path.GetFileName(assemblyFilename)).SymbolsLoaded, "Module symbols not loaded");
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void Breakpoint()
		{
			Breakpoint b = debugger.AddBreakpoint(@"F:\SharpDevelopTrunk\src\AddIns\Misc\Debugger\Debugger.Tests\Project\Src\TestPrograms\Breakpoint.cs", 18);
			
			StartProgram("Breakpoint");
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual(true, b.Enabled);
			Assert.AreEqual(true, b.HadBeenSet, "Breakpoint is not set");
			Assert.AreEqual(18, b.SourcecodeSegment.StartLine);
			
			process.Continue();
			WaitForPause(PausedReason.Breakpoint, "Mark 1");
			
			process.Continue();
			WaitForPause(PausedReason.Break, "Mark 2");
			
			process.Continue();
			process.WaitForExit();
			Assert.AreEqual("Mark 1\r\nMark 2\r\n", log);
		}
		
		[Test]
		public void FileRelease()
		{
			Assert.IsTrue(File.Exists(assemblyFilename), "Assembly file not found");
			Assert.IsTrue(File.Exists(symbolsFilename), "Symbols file not found (.pdb)");
			
			string tempPath = Path.Combine(Path.GetTempPath(), Path.Combine("DebeggerTest", new Random().Next().ToString()));
			Directory.CreateDirectory(tempPath);
			
			string newAssemblyFilename = Path.Combine(tempPath, Path.GetFileName(assemblyFilename));
			string newSymbolsFilename = Path.Combine(tempPath, Path.GetFileName(symbolsFilename));
			
			File.Copy(assemblyFilename, newAssemblyFilename);
			File.Copy(symbolsFilename, newSymbolsFilename);
			
			Assert.IsTrue(File.Exists(newAssemblyFilename), "Assembly file copying failed");
			Assert.IsTrue(File.Exists(newSymbolsFilename), "Symbols file copying failed");
			
			StartProgram(newAssemblyFilename, "FileRelease");
			process.WaitForExit();
			
			try {
				File.Delete(newAssemblyFilename);
			} catch (System.Exception e) {
				Assert.Fail("Assembly file not released\n" + e.ToString());
			}
			
			try {
				File.Delete(newSymbolsFilename);
			} catch (System.Exception e) {
				Assert.Fail("Symbols file not released\n" + e.ToString());
			}
		}
		
		[Test]
		public void DebuggeeKilled()
		{
			StartProgram("DebuggeeKilled");
			WaitForPause(PausedReason.Break);
			Assert.AreNotEqual(null, lastLogMessage);
			System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(int.Parse(lastLogMessage));
			p.Kill();
			process.WaitForExit();
		}
		
		[Test]
		public void Stepping()
		{
			StartProgram("Stepping");
			WaitForPause(PausedReason.Break, null);
			
			process.StepOver(); // Debugger.Break
			WaitForPause(PausedReason.StepComplete, null);
			
			process.StepOver(); // Debug.WriteLine 1
			WaitForPause(PausedReason.StepComplete, "1");
			
			process.StepInto(); // Method Sub
			WaitForPause(PausedReason.StepComplete, "1");
			
			process.StepInto(); // '{'
			WaitForPause(PausedReason.StepComplete, "1");
			
			process.StepInto(); // Debug.WriteLine 2
			WaitForPause(PausedReason.StepComplete, "2");
			
			process.StepOut(); // Method Sub
			WaitForPause(PausedReason.StepComplete, "4");
			
			process.StepOver(); // Method Sub
			WaitForPause(PausedReason.StepComplete, "4");
			
			process.StepOver(); // Method Sub2
			WaitForPause(PausedReason.StepComplete, "5");
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void Callstack()
		{
			List<Function> callstack;
			
			StartProgram("Callstack");
			WaitForPause(PausedReason.Break, null);
			callstack = new List<Function>(process.SelectedThread.Callstack);
			Assert.AreEqual("Sub2", callstack[0].Name);
			Assert.AreEqual("Sub1", callstack[1].Name);
			Assert.AreEqual("Main", callstack[2].Name);
			
			process.StepOut();
			WaitForPause(PausedReason.StepComplete, null);
			callstack = new List<Function>(process.SelectedThread.Callstack);
			Assert.AreEqual("Sub1", callstack[0].Name);
			Assert.AreEqual("Main", callstack[1].Name);
			
			process.StepOut();
			WaitForPause(PausedReason.StepComplete, null);
			callstack = new List<Function>(process.SelectedThread.Callstack);
			Assert.AreEqual("Main", callstack[0].Name);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void FunctionArgumentVariables()
		{
			StartProgram("FunctionArgumentVariables");
			WaitForPause(PausedReason.Break, null);
			
			for(int i = 0; i < 2; i++) {
				NamedValueCollection args;
				
				process.Continue();
				WaitForPause(PausedReason.Break, null);
				args = process.SelectedFunction.Arguments;
				NamedValue args_i = args["i"];
				NamedValue args_s = args["s"];
				NamedValue args_args = args["args"];
				// names
				Assert.AreEqual("i", args_i.Name);
				Assert.AreEqual("s", args_s.Name);
				Assert.AreEqual("args", args_args.Name);
				// types
				Assert.IsTrue(args_i.IsPrimitive);
				Assert.IsTrue(args_s.IsPrimitive);
				Assert.IsTrue(args_args.IsArray);
				// values
				Assert.AreEqual("0", args_i.AsString);
				Assert.AreEqual("S", args_s.AsString);
				Assert.AreEqual(0, args_args.ArrayLenght);
				
				process.Continue();
				WaitForPause(PausedReason.Break, null);
				args = process.SelectedFunction.Arguments;
				// values
				Assert.AreEqual("1", args["i"].AsString);
				Assert.AreEqual("S", args["s"].AsString);
				Assert.AreEqual(1, args["args"].ArrayLenght);
				
				process.Continue();
				WaitForPause(PausedReason.Break, null);
				args = process.SelectedFunction.Arguments;
				// values
				Assert.AreEqual("2", args["i"].AsString);
				Assert.IsTrue(args["s"].IsNull);
				Assert.AreEqual(2, args["args"].ArrayLenght);
			}
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void FunctionLocalVariables()
		{
			StartProgram("FunctionLocalVariables");
			WaitForPause(PausedReason.Break, null);
			NamedValueCollection vars = process.SelectedFunction.LocalVariables;
			// types
			Assert.IsTrue(vars["i"].IsPrimitive);
			Assert.IsTrue(vars["s"].IsPrimitive);
			Assert.IsTrue(vars["args"].IsArray);
			Assert.IsTrue(vars["n"].IsNull);
			Assert.IsTrue(vars["o"].IsObject);
			// values
			Assert.AreEqual("0", vars["i"].AsString);
			Assert.AreEqual("S", vars["s"].AsString);
			Assert.AreEqual(1, vars["args"].ArrayLenght);
			Assert.IsTrue(vars["n"].IsNull);
			Assert.AreEqual("{System.Object}", vars["o"].AsString);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void FunctionLifetime()
		{
			Function function;
			
			StartProgram("FunctionLifetime");
			WaitForPause(PausedReason.Break, null);
			function = process.SelectedFunction;
			Assert.IsNotNull(function);
			Assert.AreEqual("Function", function.Name);
			Assert.AreEqual(false, function.HasExpired);
			Assert.AreEqual("1", function.GetArgument(0).AsString);
			
			process.Continue(); // Go to the SubFunction
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("SubFunction", process.SelectedFunction.Name);
			Assert.AreEqual(false, function.HasExpired);
			Assert.AreEqual("1", function.GetArgument(0).AsString);
			
			process.Continue(); // Go back to Function
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("Function", process.SelectedFunction.Name);
			Assert.AreEqual(false, function.HasExpired);
			Assert.AreEqual("1", function.GetArgument(0).AsString);
			
			process.Continue(); // Setp out of function
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("Main", process.SelectedFunction.Name);
			Assert.AreEqual(true, function.HasExpired);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void FunctionVariablesLifetime()
		{
			Function function = null;
			NamedValue argument = null;
			NamedValue local    = null;
			NamedValue localInSubFunction = null;
			NamedValue @class   = null;
			
			StartProgram("FunctionVariablesLifetime"); // 1 - Enter program
			WaitForPause(PausedReason.Break, null);
			function = process.SelectedFunction;
			Assert.IsNotNull(function);
			Assert.AreEqual("Function", function.Name);
			argument = function.GetArgument(0);
			local = function.LocalVariables["local"];
			@class = function.ContaingClassVariables["class"];
			Assert.IsNotNull(argument);
			Assert.IsNotNull(local);
			Assert.IsNotNull(@class);
			Assert.AreEqual("argument", argument.Name);
			Assert.AreEqual("local", local.Name);
			Assert.AreEqual("class", @class.Name);
			Assert.AreEqual("1", argument.AsString);
			Assert.AreEqual("2", local.AsString);
			Assert.AreEqual("3", @class.AsString);
			
			process.Continue(); // 2 - Go to the SubFunction
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("1", argument.AsString);
			Assert.AreEqual("2", local.AsString);
			Assert.AreEqual("3", @class.AsString);
			// Check localInSubFunction variable
			localInSubFunction = process.SelectedFunction.LocalVariables["localInSubFunction"];
			Assert.AreEqual("4", localInSubFunction.AsString);
			
			process.Continue(); // 3 - Go back to Function
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("1", argument.AsString);
			Assert.AreEqual("2", local.AsString);
			Assert.AreEqual("3", @class.AsString);
			// localInSubFunction should be dead now
			Assert.IsTrue(localInSubFunction.HasExpired);
			
			process.Continue(); // 4 - Go to the SubFunction
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("1", argument.AsString);
			Assert.AreEqual("2", local.AsString);
			Assert.AreEqual("3", @class.AsString);
			// localInSubFunction should be still dead...
			Assert.IsTrue(localInSubFunction.HasExpired);
			// ... , but we should able to get new one
			localInSubFunction = process.SelectedFunction.LocalVariables["localInSubFunction"];
			Assert.AreEqual("4", localInSubFunction.AsString);
			
			process.Continue(); // 5 - Setp out of both functions
			WaitForPause(PausedReason.Break, null);
			Assert.IsTrue(argument.HasExpired);
			Assert.IsTrue(local.HasExpired);
			Assert.IsTrue(@class.HasExpired);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void ArrayValue()
		{
			StartProgram("ArrayValue");
			WaitForPause(PausedReason.Break, null);
			NamedValue array = process.SelectedFunction.LocalVariables["array"];
			Assert.AreEqual("array", array.Name);
			Assert.IsTrue(array.IsArray);
			Assert.AreEqual("{System.Int32[5]}", array.AsString);
			NamedValueCollection elements = array.GetArrayElements();
			Assert.AreEqual(5, elements.Count);
			for(int i = 0; i < 5; i++) {
				Assert.AreEqual("[" + i.ToString() + "]", elements[i].Name);
				Assert.AreEqual(i.ToString(), elements[i].AsString);
			}
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void ObjectValue()
		{
			NamedValue local = null;
			
			StartProgram("ObjectValue");
			WaitForPause(PausedReason.Break, null);
			local = process.SelectedFunction.LocalVariables["val"];
			Assert.AreEqual("val", local.Name);
			Assert.IsTrue(local.IsObject);
			Assert.AreEqual("{Debugger.Tests.TestPrograms.ObjectValue}", local.AsString);
			Assert.AreEqual("Debugger.Tests.TestPrograms.ObjectValue", local.Type.Name);
			NamedValueCollection subVars = local.GetMembers(null, Debugger.BindingFlags.All);
			Assert.IsTrue(subVars["privateField"].IsPrimitive);
			Assert.IsTrue(subVars["publicFiled"].IsPrimitive);
			Assert.IsTrue(subVars["PublicProperty"].IsPrimitive);
			Assert.IsTrue(((MemberValue)subVars["privateField"]).MemberInfo.IsPrivate);
			Assert.IsTrue(((MemberValue)subVars["publicFiled"]).MemberInfo.IsPublic);
			Assert.IsTrue(((MemberValue)subVars["PublicProperty"]).MemberInfo.IsPublic);
			DebugType baseClass = local.Type.BaseType;
			Assert.AreEqual("Debugger.Tests.TestPrograms.BaseClass", baseClass.Name);
			Assert.AreEqual("private", subVars["privateField"].AsString);
			
			process.Continue();
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("new private", subVars["privateField"].AsString);
			
			process.Continue();
			process.WaitForExit();
		}
		
		/*
		[Test]
		public void PropertyVariable()
		{
			StartProgram("PropertyVariable");
			WaitForPause(PausedReason.Break, null);
			NamedValueCollection props = process.SelectedFunction.LocalVariables["var"].GetMembers(null, Debugger.BindingFlags.All);
			
			Assert.AreEqual(typeof(UnavailableValue), props["PrivateProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause(PausedReason.EvalComplete, null);
			Assert.AreEqual("private", props["PrivateProperty"].AsString);
			
			Assert.AreEqual(typeof(UnavailableValue), props["PublicProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause(PausedReason.EvalComplete, null);
			Assert.AreEqual("public", props["PublicProperty"].AsString);
			
			Assert.AreEqual(typeof(UnavailableValue), props["ExceptionProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause(PausedReason.EvalComplete, null);
			Assert.AreEqual(typeof(UnavailableValue), props["ExceptionProperty"].Value.GetType());
			
			Assert.AreEqual(typeof(UnavailableValue), props["StaticProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause(PausedReason.EvalComplete, null);
			Assert.AreEqual("static", props["StaticProperty"].AsString);
			
			process.Continue();
			WaitForPause(PausedReason.Break, null);
			
			process.Continue();
			process.WaitForPrecessExit();
		}
		*/
		
		/*
		[Test]
		public void PropertyVariableForm()
		{
			Variable local = null;
			
			StartProgram("PropertyVariableForm");
			WaitForPause(PausedReason.Break, null);
			foreach(Variable var in process.SelectedFunction.LocalVariables) {
				local = var;
			}
			Assert.AreEqual("form", local.Name);
			Assert.AreEqual(typeof(Variable), local.GetType());
			
			foreach(Variable var in local.Value.SubVariables) {
				Assert.AreEqual(typeof(UnavailableValue), var.Value.GetType(), "Variable name: " + var.Name);
				process.StartEvaluation();
				WaitForPause(PausedReason.EvalComplete, null);
				Assert.AreNotEqual(null, var.Value.AsString, "Variable name: " + var.Name);
			}
			
			process.Continue();
			WaitForPause(PausedReason.Break, null);
			
			foreach(Variable var in local.Value.SubVariables) {
				Assert.AreEqual(typeof(UnavailableValue), var.Value.GetType(), "Variable name: " + var.Name);
			}
			process.StartEvaluation();
			WaitForPause(PausedReason.EvalComplete, null);
			
			process.Continue();
			process.WaitForPrecessExit();
		}
		*/
		
		[Test]
		public void SetIP()
		{
			StartProgram("SetIP");
			WaitForPause(PausedReason.Break, "1");
			
			Assert.IsNotNull(process.SelectedFunction.CanSetIP("SetIP.cs", 16, 0));
			Assert.IsNull(process.SelectedFunction.CanSetIP("SetIP.cs", 100, 0));
			process.SelectedFunction.SetIP("SetIP.cs", 16, 0);
			process.Continue();
			WaitForPause(PausedReason.Break, "1");
			Assert.AreEqual("1\r\n1\r\n", log);
			
			process.Continue();
			process.WaitForExit();
		}
	}
}
