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
using NIgnore = NUnit.Framework.IgnoreAttribute;

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
			CheckXmlOutput();
		}
		
		[Test]
		public void HelloWorld()
		{
			StartTest("HelloWorld");
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void Break()
		{
			StartTest("Break");
			WaitForPause();
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void Breakpoint()
		{
			Breakpoint breakpoint = debugger.AddBreakpoint(@"F:\SharpDevelopTrunk\src\AddIns\Misc\Debugger\Debugger.Tests\Project\Src\TestPrograms\Breakpoint.cs", 18);
			
			StartTest("Breakpoint");
			WaitForPause();
			
			ObjectDump(breakpoint);
			
			process.Continue();
			WaitForPause();
			
			process.Continue();
			WaitForPause();
			
			process.Continue();
			process.WaitForExit();
			
			ObjectDump(breakpoint);
			CheckXmlOutput();
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
//			WaitForPause();
//			Assert.AreNotEqual(null, lastLogMessage);
//			System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(int.Parse(lastLogMessage));
//			p.Kill();
//			process.WaitForExit();
//		}
		
		[Test]
		public void Stepping()
		{
			StartTest("Stepping");
			WaitForPause();
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepOver(); // Debugger.Break
			WaitForPause();
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepOver(); // Debug.WriteLine 1
			WaitForPause();
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepInto(); // Method Sub
			WaitForPause();
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepInto(); // '{'
			WaitForPause();
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepInto(); // Debug.WriteLine 2
			WaitForPause();
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepOut(); // Method Sub
			WaitForPause();
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepOver(); // Method Sub
			WaitForPause();
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.StepOver(); // Method Sub2
			WaitForPause();
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void Callstack()
		{
			StartTest("Callstack");
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.StepOut();
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.StepOut();
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void FunctionArgumentVariables()
		{
			StartTest("FunctionArgumentVariables");
			WaitForPause();
			
			for(int i = 0; i < 6; i++) {
				process.Continue();
				WaitForPause();
				ObjectDump("SelectedFunction", process.SelectedFunction);
			}
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void FunctionLocalVariables()
		{
			StartTest("FunctionLocalVariables");
			WaitForPause();
			ObjectDump("SelectedFunction", process.SelectedFunction);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void FunctionLifetime()
		{
			Function function;
			
			StartTest("FunctionLifetime");
			WaitForPause();
			function = process.SelectedFunction;
			ObjectDump("Function", function);
			
			process.Continue(); // Go to the SubFunction
			WaitForPause();
			ObjectDump("Function", function);
			ObjectDump("SubFunction", process.SelectedFunction);
			
			process.Continue(); // Go back to Function
			WaitForPause();
			Assert.AreEqual(function, process.SelectedFunction);
			ObjectDump("Function", function);
			
			process.Continue(); // Setp out of function
			WaitForPause();
			ObjectDump("Main", process.SelectedFunction);
			ObjectDump("Function", function);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void FunctionVariablesLifetime()
		{
			Value argument = null;
			Value local    = null;
			Value localInSubFunction = null;
			Value @class   = null;
			
			StartTest("FunctionVariablesLifetime"); // 1 - Enter program
			WaitForPause();
			argument = process.SelectedFunction.GetArgument(0);
			local = process.SelectedFunction.LocalVariables["local"];
			@class = process.SelectedFunction.ContaingClassVariables["class"];
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			
			process.Continue(); // 2 - Go to the SubFunction
			WaitForPause();
			localInSubFunction = process.SelectedFunction.LocalVariables["localInSubFunction"];
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.Continue(); // 3 - Go back to Function
			WaitForPause();
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.Continue(); // 4 - Go to the SubFunction
			WaitForPause();
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			localInSubFunction = process.SelectedFunction.LocalVariables["localInSubFunction"];
			ObjectDump("localInSubFunction(new)", @localInSubFunction);
			
			process.Continue(); // 5 - Setp out of both functions
			WaitForPause();
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void ArrayValue()
		{
			StartTest("ArrayValue");
			WaitForPause();
			Value array = process.SelectedFunction.LocalVariables["array"];
			ObjectDump("array", array);
			ObjectDump("array elements", array.GetArrayElements());
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test, NIgnore]
		public void ObjectValue()
		{
			Value val = null;
			
			StartTest("ObjectValue");
			WaitForPause();
			val = process.SelectedFunction.LocalVariables["val"];
			ObjectDump("val", val);
			ObjectDump("val members", val.GetMembers(null, Debugger.BindingFlags.All));
			//ObjectDump("typeof(val)", val.Type);
			
			process.Continue();
			WaitForPause();
			ObjectDump("val", val);
			ObjectDump("val members", val.GetMembers(null, Debugger.BindingFlags.All));
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		/*
		[Test]
		public void PropertyVariable()
		{
			StartProgram("PropertyVariable");
			WaitForPause();
			NamedValueCollection props = process.SelectedFunction.LocalVariables["var"].GetMembers(null, Debugger.BindingFlags.All);
			
			Assert.AreEqual(typeof(UnavailableValue), props["PrivateProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause();
			Assert.AreEqual("private", props["PrivateProperty"].AsString);
			
			Assert.AreEqual(typeof(UnavailableValue), props["PublicProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause();
			Assert.AreEqual("public", props["PublicProperty"].AsString);
			
			Assert.AreEqual(typeof(UnavailableValue), props["ExceptionProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause();
			Assert.AreEqual(typeof(UnavailableValue), props["ExceptionProperty"].Value.GetType());
			
			Assert.AreEqual(typeof(UnavailableValue), props["StaticProperty"].Value.GetType());
			process.StartEvaluation();
			WaitForPause();
			Assert.AreEqual("static", props["StaticProperty"].AsString);
			
			process.Continue();
			WaitForPause(PausedReason.Break, null);
			
			process.Continue();
			process.WaitForPrecessExit();
			CheckXmlOutput();
		}
		*/
		
		/*
		[Test]
		public void PropertyVariableForm()
		{
			Variable local = null;
			
			StartProgram("PropertyVariableForm");
			WaitForPause();
			foreach(Variable var in process.SelectedFunction.LocalVariables) {
				local = var;
			}
			Assert.AreEqual("form", local.Name);
			Assert.AreEqual(typeof(Variable), local.GetType());
			
			foreach(Variable var in local.Value.SubVariables) {
				Assert.AreEqual(typeof(UnavailableValue), var.Value.GetType(), "Variable name: " + var.Name);
				process.StartEvaluation();
				WaitForPause();
				Assert.AreNotEqual(null, var.Value.AsString, "Variable name: " + var.Name);
			}
			
			process.Continue();
			WaitForPause();
			
			foreach(Variable var in local.Value.SubVariables) {
				Assert.AreEqual(typeof(UnavailableValue), var.Value.GetType(), "Variable name: " + var.Name);
			}
			process.StartEvaluation();
			WaitForPause();
			
			process.Continue();
			process.WaitForPrecessExit();
			CheckXmlOutput();
		}
		*/
		
		[Test]
		public void SetIP()
		{
			StartTest("SetIP");
			WaitForPause();
			
			Assert.IsNotNull(process.SelectedFunction.CanSetIP("SetIP.cs", 16, 0));
			Assert.IsNull(process.SelectedFunction.CanSetIP("SetIP.cs", 100, 0));
			process.SelectedFunction.SetIP("SetIP.cs", 16, 0);
			process.Continue();
			WaitForPause();
			Assert.AreEqual("1\r\n1\r\n", log);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test, NIgnore]
		public void GenericDictionary()
		{
			StartTest("GenericDictionary");
			WaitForPause();
			ObjectDump("dict", process.SelectedFunction.LocalVariables["dict"]);
			ObjectDump("dict members", process.SelectedFunction.LocalVariables["dict"].GetMembers(null, BindingFlags.All));
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void Exception()
		{
			StartTest("Exception");
			WaitForPause();
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void ExceptionCustom()
		{
			StartTest("ExceptionCustom");
			WaitForPause();
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test, NIgnore]
		public void Expressions()
		{
			StartTest("Expressions");
			WaitForPause();
			
			ObjectDump("Variables", process.SelectedFunction.Variables);
			ObjectDump("array", process.SelectedFunction.Variables["array"].GetArrayElements());
			ObjectDump("array2", process.SelectedFunction.Variables["array2"].GetArrayElements());
			ObjectDump("this", process.SelectedFunction.ThisValue.GetMembers());
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
		
		[Test]
		public void Generics()
		{
			StartTest("Generics");
			
			for(int i = 0; i < 8; i++) {
				WaitForPause();
				ObjectDump("SelectedFunction", process.SelectedFunction);
				process.Continue();
			}
			
			WaitForPause();
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
