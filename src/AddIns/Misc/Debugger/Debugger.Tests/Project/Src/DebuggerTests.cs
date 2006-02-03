// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger;
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
		string log;
		string lastLogMessage;
		string assemblyFilename;
		string assemblyDir;
		string symbolsFilename;
		
		public DebuggerTests()
		{
			assemblyFilename = Assembly.GetExecutingAssembly().Location;
			assemblyDir = Path.GetDirectoryName(assemblyFilename);
			symbolsFilename = Path.Combine(assemblyDir, Path.GetFileNameWithoutExtension(assemblyFilename) + ".pdb");
			
			debugger = new NDebugger();
			debugger.MTA2STA.CallMethod = CallMethod.Manual;
			debugger.LogMessage += delegate(object sender, MessageEventArgs e) {
				log += e.Message;
				lastLogMessage = e.Message;
			};
		}
		
		[TearDown]
		void TearDown()
		{
			debugger.Terminate();
		}
		
		void StartProgram(string programName)
		{
			StartProgram(assemblyFilename, programName);
		}
		
		void StartProgram(string exeFilename, string programName)
		{
			log = "";
			lastLogMessage = null;
			debugger.Terminate();
			debugger.Start(exeFilename, Path.GetDirectoryName(exeFilename), programName);
		}
		
		void WaitForPause(PausedReason expectedReason)
		{
			debugger.WaitForPause();
			Assert.AreEqual(true, debugger.IsPaused);
			Assert.AreEqual(expectedReason, debugger.PausedReason);
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
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void HelloWorld()
		{
			StartProgram("HelloWorld");
			debugger.WaitForPrecessExit();
			Assert.AreEqual("Hello world!\r\n", log);
		}
		
		[Test]
		public void Break()
		{
			StartProgram("Break");
			WaitForPause(PausedReason.Break, null);
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void Symbols()
		{
			Assert.AreEqual("debugger.tests.exe", Path.GetFileName(assemblyFilename).ToLower());
			Assert.IsTrue(File.Exists(symbolsFilename), "Symbols file not found (.pdb)");
			
			StartProgram("Symbols");
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual(true, debugger.GetModule(Path.GetFileName(assemblyFilename)).SymbolsLoaded, "Module symbols not loaded");
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void Breakpoint()
		{
			Breakpoint b = debugger.AddBreakpoint(@"D:\corsavy\SharpDevelop\src\AddIns\Misc\Debugger\Debugger.Tests\Project\Src\TestPrograms\Breakpoint.cs", 18);
			
			StartProgram("Breakpoint");
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual(true, b.Enabled);
			Assert.AreEqual(true, b.HadBeenSet, "Breakpoint is not set");
			Assert.AreEqual(18, b.SourcecodeSegment.StartLine);
			
			debugger.Continue();
			WaitForPause(PausedReason.Breakpoint, "Mark 1");
			
			debugger.Continue();
			WaitForPause(PausedReason.Break, "Mark 2");
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
			Assert.AreEqual("Mark 1\r\nMark 2\r\n", log);
		}
		
		[Test, Ignore("Works only if run alone")]
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
			debugger.WaitForPrecessExit();
			
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
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void Stepping()
		{
			StartProgram("Stepping");
			WaitForPause(PausedReason.Break, null);
			
			debugger.StepOver(); // Debugger.Break
			WaitForPause(PausedReason.StepComplete, null);
			
			debugger.StepOver(); // Debug.WriteLine 1
			WaitForPause(PausedReason.StepComplete, "1");
			
			debugger.StepInto(); // Method Sub
			WaitForPause(PausedReason.StepComplete, "1");
			
			debugger.StepInto(); // '{'
			WaitForPause(PausedReason.StepComplete, "1");
			
			debugger.StepInto(); // Debug.WriteLine 2
			WaitForPause(PausedReason.StepComplete, "2");
			
			debugger.StepOut(); // Method Sub
			WaitForPause(PausedReason.StepComplete, "4");
			
			debugger.StepOver(); // Method Sub
			WaitForPause(PausedReason.StepComplete, "4");
			
			debugger.StepOver(); // Method Sub2
			WaitForPause(PausedReason.StepComplete, "5");
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void Callstack()
		{
			List<Function> callstack;
			
			StartProgram("Callstack");
			WaitForPause(PausedReason.Break, null);
			callstack = new List<Function>(debugger.CurrentThread.Callstack);
			Assert.AreEqual("Sub2", callstack[0].Name);
			Assert.AreEqual("Sub1", callstack[1].Name);
			Assert.AreEqual("Main", callstack[2].Name);
			
			debugger.StepOut();
			WaitForPause(PausedReason.StepComplete, null);
			callstack = new List<Function>(debugger.CurrentThread.Callstack);
			Assert.AreEqual("Sub1", callstack[0].Name);
			Assert.AreEqual("Main", callstack[1].Name);
			
			debugger.StepOut();
			WaitForPause(PausedReason.StepComplete, null);
			callstack = new List<Function>(debugger.CurrentThread.Callstack);
			Assert.AreEqual("Main", callstack[0].Name);
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void FunctionArgumentVariables()
		{
			List<Variable> args;
			
			StartProgram("FunctionArgumentVariables");
			WaitForPause(PausedReason.Break, null);
			
			for(int i = 0; i < 2; i++) {
				debugger.Continue();
				WaitForPause(PausedReason.Break, null);
				args = new List<Variable>(debugger.CurrentFunction.ArgumentVariables);
				// names
				Assert.AreEqual("i", args[0].Name);
				Assert.AreEqual("s", args[1].Name);
				Assert.AreEqual("args", args[2].Name);
				// types
				Assert.AreEqual(typeof(PrimitiveValue), args[0].Value.GetType());
				Assert.AreEqual(typeof(PrimitiveValue), args[1].Value.GetType());
				Assert.AreEqual(typeof(ArrayValue),     args[2].Value.GetType());
				// values
				Assert.AreEqual("0", args[0].Value.AsString);
				Assert.AreEqual("S", args[1].Value.AsString);
				Assert.AreEqual(0 ,((ArrayValue)args[2].Value).Lenght);
				
				debugger.Continue();
				WaitForPause(PausedReason.Break, null);
				args = new List<Variable>(debugger.CurrentFunction.ArgumentVariables);
				// types
				Assert.AreEqual(typeof(PrimitiveValue), args[0].Value.GetType());
				Assert.AreEqual(typeof(PrimitiveValue), args[1].Value.GetType());
				Assert.AreEqual(typeof(ArrayValue),     args[2].Value.GetType());
				// values
				Assert.AreEqual("1", args[0].Value.AsString);
				Assert.AreEqual("S", args[1].Value.AsString);
				Assert.AreEqual(1 ,((ArrayValue)args[2].Value).Lenght);
				
				debugger.Continue();
				WaitForPause(PausedReason.Break, null);
				args = new List<Variable>(debugger.CurrentFunction.ArgumentVariables);
				// types
				Assert.AreEqual(typeof(PrimitiveValue), args[0].Value.GetType());
				Assert.AreEqual(typeof(NullValue), args[1].Value.GetType());
				Assert.AreEqual(typeof(ArrayValue),     args[2].Value.GetType());
				// values
				Assert.AreEqual("2", args[0].Value.AsString);
				Assert.IsNotNull(args[1].Value.AsString);
				Assert.AreEqual(2 ,((ArrayValue)args[2].Value).Lenght);
			}
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void FunctionLocalVariables()
		{
			List<Variable> args;
			
			StartProgram("FunctionLocalVariables");
			WaitForPause(PausedReason.Break, null);
			args = new List<Variable>(debugger.CurrentFunction.LocalVariables);
			// names
			Assert.AreEqual("i", args[0].Name);
			Assert.AreEqual("s", args[1].Name);
			Assert.AreEqual("args", args[2].Name);
			Assert.AreEqual("n", args[3].Name);
			Assert.AreEqual("o", args[4].Name);
			// types
			Assert.AreEqual(typeof(PrimitiveValue), args[0].Value.GetType());
			Assert.AreEqual(typeof(PrimitiveValue), args[1].Value.GetType());
			Assert.AreEqual(typeof(ArrayValue),     args[2].Value.GetType());
			Assert.AreEqual(typeof(NullValue),     args[3].Value.GetType());
			Assert.AreEqual(typeof(ObjectValue),     args[4].Value.GetType());
			// values
			Assert.AreEqual("0", args[0].Value.AsString);
			Assert.AreEqual("S", args[1].Value.AsString);
			Assert.AreEqual(1 ,((ArrayValue)args[2].Value).Lenght);
			Assert.IsNotNull(args[3].Value.AsString);
			Assert.AreEqual("{System.Object}", args[4].Value.AsString);
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void FunctionLifetime()
		{
			Function function;
			
			StartProgram("FunctionLifetime");
			WaitForPause(PausedReason.Break, null);
			function = debugger.CurrentFunction;
			Assert.IsNotNull(function);
			Assert.AreEqual("Function", function.Name);
			Assert.AreEqual(false, function.HasExpired);
			Assert.AreEqual("1", function.GetArgumentVariable(0).Value.AsString);
			
			debugger.Continue(); // Go to the SubFunction
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("SubFunction", debugger.CurrentFunction.Name);
			Assert.AreEqual(false, function.HasExpired);
			Assert.AreEqual("1", function.GetArgumentVariable(0).Value.AsString);
			
			debugger.Continue(); // Go back to Function
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("Function", debugger.CurrentFunction.Name);
			Assert.AreEqual(false, function.HasExpired);
			Assert.AreEqual("1", function.GetArgumentVariable(0).Value.AsString);
			
			debugger.Continue(); // Setp out of function
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("Main", debugger.CurrentFunction.Name);
			Assert.AreEqual(true, function.HasExpired);
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void FunctionVariablesLifetime()
		{
			Function function = null;
			Variable argument = null;
			Variable local    = null;
			Variable @class   = null;
			
			StartProgram("FunctionVariablesLifetime");
			WaitForPause(PausedReason.Break, null);
			function = debugger.CurrentFunction;
			Assert.IsNotNull(function);
			Assert.AreEqual("Function", function.Name);
			argument = function.GetArgumentVariable(0);
			foreach(Variable var in function.LocalVariables) {
				local = var;
			}
			foreach(Variable var in function.ContaingClassVariables) {
				@class = var;
			}
			Assert.IsNotNull(argument);
			Assert.IsNotNull(local);
			Assert.IsNotNull(@class);
			Assert.AreEqual("argument", argument.Name);
			Assert.AreEqual("local", local.Name);
			Assert.AreEqual("class", @class.Name);
			Assert.AreEqual("1", argument.Value.AsString);
			Assert.AreEqual("2", local.Value.AsString);
			Assert.AreEqual("3", @class.Value.AsString);
			
			debugger.Continue(); // Go to the SubFunction
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("1", argument.Value.AsString);
			Assert.AreEqual("2", local.Value.AsString);
			Assert.AreEqual("3", @class.Value.AsString);
			
			debugger.Continue(); // Go back to Function
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual("1", argument.Value.AsString);
			Assert.AreEqual("2", local.Value.AsString);
			Assert.AreEqual("3", @class.Value.AsString);
			
			debugger.Continue(); // Setp out of function
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual(typeof(UnavailableValue), argument.Value.GetType());
			Assert.AreEqual(typeof(UnavailableValue), local.Value.GetType());
			Assert.AreEqual(typeof(UnavailableValue), @class.Value.GetType());
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void ArrayValue()
		{
			Variable local = null;
			List<Variable> subVars = new List<Variable>();
			
			StartProgram("ArrayValue");
			WaitForPause(PausedReason.Break, null);
			foreach(Variable var in debugger.CurrentFunction.LocalVariables) {
				local = var; break;
			}
			Assert.AreEqual("array", local.Name);
			Assert.AreEqual(true, local.MayHaveSubVariables);
			Assert.AreEqual(typeof(ArrayValue), local.Value.GetType());
			Assert.AreEqual("{System.Int32[5]}", local.Value.AsString);
			foreach(Variable var in local.SubVariables) {
				subVars.Add(var);
			}
			for(int i = 0; i < 5; i++) {
				Assert.AreEqual("[" + i.ToString() + "]", subVars[i].Name);
				Assert.AreEqual(i.ToString(), subVars[i].Value.AsString);
			}
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void ObjectValue()
		{
			Variable local = null;
			Variable baseClass;
			List<Variable> subVars = new List<Variable>();
			
			StartProgram("ObjectValue");
			WaitForPause(PausedReason.Break, null);
			foreach(Variable var in debugger.CurrentFunction.LocalVariables) {
				local = var;
			}
			Assert.AreEqual("val", local.Name);
			Assert.AreEqual(true, local.MayHaveSubVariables);
			Assert.AreEqual(typeof(ObjectValue), local.Value.GetType());
			Assert.AreEqual("{Debugger.Tests.TestPrograms.ObjectValue}", local.Value.AsString);
			foreach(Variable var in local.SubVariables) {
				subVars.Add(var);
			}
			Assert.AreEqual("privateField", subVars[1].Name);
			Assert.AreEqual("publicFiled", subVars[2].Name);
			Assert.AreEqual("PublicProperty", subVars[3].Name);
			Assert.AreEqual(typeof(ClassVariable), subVars[1].GetType());
			Assert.AreEqual(typeof(ClassVariable), subVars[2].GetType());
			Assert.AreEqual(typeof(PropertyVariable), subVars[3].GetType());
			Assert.AreEqual(false, ((ClassVariable)subVars[1]).IsPublic);
			Assert.AreEqual(true, ((ClassVariable)subVars[2]).IsPublic);
			Assert.AreEqual(true, ((ClassVariable)subVars[3]).IsPublic);
			Assert.AreEqual(true, ((ObjectValue)local.Value).HasBaseClass);
			baseClass = subVars[0];
			Assert.AreEqual(typeof(ObjectValue), baseClass.Value.GetType());
			Assert.AreEqual(false, baseClass.Value.IsExpired);
			Assert.AreEqual("{Debugger.Tests.TestPrograms.BaseClass}", baseClass.Value.AsString);
			
			debugger.Continue();
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual(typeof(ObjectValue), baseClass.Value.GetType());
			Assert.AreEqual(false, baseClass.Value.IsExpired);
			Assert.AreEqual("{Debugger.Tests.TestPrograms.BaseClass}", baseClass.Value.AsString);
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void PropertyVariable()
		{
			Variable local = null;
			List<Variable> subVars = new List<Variable>();
			
			StartProgram("PropertyVariable");
			WaitForPause(PausedReason.Break, null);
			foreach(Variable var in debugger.CurrentFunction.LocalVariables) {
				local = var;
			}
			foreach(Variable var in local.SubVariables) {
				subVars.Add(var);
			}
			Assert.AreEqual("PrivateProperty", subVars[1].Name);
			Assert.AreEqual("PublicProperty", subVars[2].Name);
			Assert.AreEqual("ExceptionProperty", subVars[3].Name);
			Assert.AreEqual("StaticProperty", subVars[4].Name);
			
			Assert.AreEqual(typeof(UnavailableValue), subVars[1].Value.GetType());
			debugger.StartEvaluation();
			WaitForPause(PausedReason.AllEvalsComplete, null);
			Assert.AreEqual("private", subVars[1].Value.AsString);
			
			Assert.AreEqual(typeof(UnavailableValue), subVars[2].Value.GetType());
			debugger.StartEvaluation();
			WaitForPause(PausedReason.AllEvalsComplete, null);
			Assert.AreEqual("public", subVars[2].Value.AsString);
			
			Assert.AreEqual(typeof(UnavailableValue), subVars[3].Value.GetType());
			debugger.StartEvaluation();
			WaitForPause(PausedReason.AllEvalsComplete, null);
			Assert.AreEqual(typeof(UnavailableValue), subVars[3].Value.GetType());
			
			Assert.AreEqual(typeof(UnavailableValue), subVars[4].Value.GetType());
			debugger.StartEvaluation();
			WaitForPause(PausedReason.AllEvalsComplete, null);
			Assert.AreEqual("static", subVars[4].Value.AsString);
			
			debugger.Continue();
			WaitForPause(PausedReason.Break, null);
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void PropertyVariableForm()
		{
			Variable local = null;
			
			StartProgram("PropertyVariableForm");
			WaitForPause(PausedReason.Break, null);
			foreach(Variable var in debugger.CurrentFunction.LocalVariables) {
				local = var;
			}
			Assert.AreEqual("form", local.Name);
			Assert.AreEqual(typeof(Variable), local.GetType());
			
			foreach(Variable var in local.SubVariables) {
				if (var is PropertyVariable) {
					Assert.AreEqual(typeof(UnavailableValue), var.Value.GetType(), "Variable name: " + var.Name);
					debugger.StartEvaluation();
					WaitForPause(PausedReason.AllEvalsComplete, null);
					Assert.AreEqual(false, var.Value.IsExpired, "Variable name: " + var.Name);
					Assert.AreNotEqual(null, var.Value.AsString, "Variable name: " + var.Name);
				}
			}
			
			debugger.Continue();
			WaitForPause(PausedReason.Break, null);
			
			foreach(Variable var in local.SubVariables) {
				if (var is PropertyVariable) {
					Assert.AreEqual(typeof(UnavailableValue), var.Value.GetType(), "Variable name: " + var.Name);
				}
			}
			debugger.StartEvaluation();
			WaitForPause(PausedReason.AllEvalsComplete, null);
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void SetIP()
		{
			StartProgram("SetIP");
			WaitForPause(PausedReason.Break, "1");
			
			Assert.IsNotNull(debugger.CurrentFunction.CanSetIP("SetIP.cs", 16, 0));
			Assert.IsNull(debugger.CurrentFunction.CanSetIP("SetIP.cs", 100, 0));
			debugger.CurrentFunction.SetIP("SetIP.cs", 16, 0);
			debugger.Continue();
			WaitForPause(PausedReason.Break, "1");
			Assert.AreEqual("1\r\n1\r\n", log);
			
			debugger.Continue();
			debugger.WaitForPrecessExit();
		}
	}
}
