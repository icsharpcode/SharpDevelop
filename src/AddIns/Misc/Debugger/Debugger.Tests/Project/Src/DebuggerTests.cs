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
	[TestFixture]
	public class DebuggerTests: DebuggerTestsBase
	{
		[Test]
		public void SimpleProgram()
		{
			StartTest("SimpleProgram");
			process.WaitForExit();
		}
		
		[Test]
		public void HelloWorld()
		{
			StartTest("HelloWorld");
			process.WaitForExit();
		}
		
		[Test]
		public void Break()
		{
			StartTest("Break");
			WaitForPause(PausedReason.Break, null);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void Breakpoint()
		{
			Breakpoint breakpoint = debugger.AddBreakpoint(@"F:\SharpDevelopTrunk\src\AddIns\Misc\Debugger\Debugger.Tests\Project\Src\TestPrograms\Breakpoint.cs", 18);
			
			StartTest("Breakpoint");
			WaitForPause(PausedReason.Break, null);
			
			ObjectDump(breakpoint);
			
			process.Continue();
			WaitForPause(PausedReason.Breakpoint, "Mark 1");
			
			process.Continue();
			WaitForPause(PausedReason.Break, "Mark 2");
			
			process.Continue();
			process.WaitForExit();
			
			ObjectDump(breakpoint);
		}
		
//		[Test]
//		public void FileRelease()
//		{
//			
//		}
				
//		[Test]
//		public void DebuggeeKilled()
//		{
//			StartTest("DebuggeeKilled");
//			WaitForPause(PausedReason.Break);
//			Assert.AreNotEqual(null, lastLogMessage);
//			System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(int.Parse(lastLogMessage));
//			p.Kill();
//			process.WaitForExit();
//		}
		
		[Test]
		public void Stepping()
		{
			StartTest("Stepping");
			WaitForPause(PausedReason.Break, null);
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepOver(); // Debugger.Break
			WaitForPause(PausedReason.StepComplete, null);
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepOver(); // Debug.WriteLine 1
			WaitForPause(PausedReason.StepComplete, "1");
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepInto(); // Method Sub
			WaitForPause(PausedReason.StepComplete, "1");
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepInto(); // '{'
			WaitForPause(PausedReason.StepComplete, "1");
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepInto(); // Debug.WriteLine 2
			WaitForPause(PausedReason.StepComplete, "2");
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepOut(); // Method Sub
			WaitForPause(PausedReason.StepComplete, "4");
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepOver(); // Method Sub
			WaitForPause(PausedReason.StepComplete, "4");
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepOver(); // Method Sub2
			WaitForPause(PausedReason.StepComplete, "5");
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void Callstack()
		{
			StartTest("Callstack");
			WaitForPause(PausedReason.Break, null);
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.StepOut();
			WaitForPause(PausedReason.StepComplete, null);
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.StepOut();
			WaitForPause(PausedReason.StepComplete, null);
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void FunctionArgumentVariables()
		{
			StartTest("FunctionArgumentVariables");
			WaitForPause(PausedReason.Break, null);
			
			for(int i = 0; i < 6; i++) {
				process.Continue();
				WaitForPause(PausedReason.Break, null);
				ObjectDump("SelectedFunction", process.SelectedFunction);
			}
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void FunctionLocalVariables()
		{
			StartTest("FunctionLocalVariables");
			WaitForPause(PausedReason.Break, null);
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void FunctionLifetime()
		{
			Function function;
			
			StartTest("FunctionLifetime");
			WaitForPause(PausedReason.Break, null);
			function = process.SelectedFunction;
			ObjectDump("Function", function);
			
			process.Continue(); // Go to the SubFunction
			WaitForPause(PausedReason.Break, null);
			ObjectDump("Function", function);
			ObjectDump("SubFunction", process.SelectedFunction);
			
			process.Continue(); // Go back to Function
			WaitForPause(PausedReason.Break, null);
			Assert.AreEqual(function, process.SelectedFunction);
			ObjectDump("Function", function);
			
			process.Continue(); // Setp out of function
			WaitForPause(PausedReason.Break, null);
			ObjectDump("Main", process.SelectedFunction);
			ObjectDump("Function", function);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void FunctionVariablesLifetime()
		{
			NamedValue argument = null;
			NamedValue local    = null;
			NamedValue localInSubFunction = null;
			NamedValue @class   = null;
			
			StartTest("FunctionVariablesLifetime"); // 1 - Enter program
			WaitForPause(PausedReason.Break, null);
			argument = process.SelectedFunction.GetArgument(0);
			local = process.SelectedFunction.LocalVariables["local"];
			@class = process.SelectedFunction.ContaingClassVariables["class"];
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			
			process.Continue(); // 2 - Go to the SubFunction
			WaitForPause(PausedReason.Break, null);
			localInSubFunction = process.SelectedFunction.LocalVariables["localInSubFunction"];
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.Continue(); // 3 - Go back to Function
			WaitForPause(PausedReason.Break, null);
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.Continue(); // 4 - Go to the SubFunction
			WaitForPause(PausedReason.Break, null);
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			localInSubFunction = process.SelectedFunction.LocalVariables["localInSubFunction"];
			ObjectDump("localInSubFunction(new)", @localInSubFunction);
			
			process.Continue(); // 5 - Setp out of both functions
			WaitForPause(PausedReason.Break, null);
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void ArrayValue()
		{
			StartTest("ArrayValue");
			WaitForPause(PausedReason.Break, null);
			NamedValue array = process.SelectedFunction.LocalVariables["array"];
			ObjectDump("array", array);
			ObjectDump("array elements", array.GetArrayElements());
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void ObjectValue()
		{
			NamedValue val = null;
			
			StartTest("ObjectValue");
			WaitForPause(PausedReason.Break, null);
			val = process.SelectedFunction.LocalVariables["val"];
			ObjectDump("val", val);
			ObjectDump("val members", val.GetMembers(null, Debugger.BindingFlags.All));
			//ObjectDump("typeof(val)", val.Type);
			
			process.Continue();
			WaitForPause(PausedReason.Break, null);
			ObjectDump("val", val);
			ObjectDump("val members", val.GetMembers(null, Debugger.BindingFlags.All));
			
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
			StartTest("SetIP");
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
		
		[Test]
		public void GenericDictionary()
		{
			StartTest("GenericDictionary");
			WaitForPause(PausedReason.Break, null);
			ObjectDump("dict", process.SelectedFunction.LocalVariables["dict"]);
			ObjectDump("dict members", process.SelectedFunction.LocalVariables["dict"].GetMembers(null, BindingFlags.All));
			
			process.Continue();
			process.WaitForExit();
		}
		
		[Test]
		public void Exception()
		{
			StartTest("Exception");
			WaitForPause(PausedReason.Exception, null);
			process.Continue();
			process.WaitForExit();
		}
	}
}
