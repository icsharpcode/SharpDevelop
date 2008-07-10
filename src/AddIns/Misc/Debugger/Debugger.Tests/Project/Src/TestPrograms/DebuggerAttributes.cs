// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;

namespace Debugger.Tests.TestPrograms
{
	public class DebuggerAttributes
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			Internal();
			IgnoredClass.Internal();
			StepOut1();
			IgnoredClass.Internal_Pass();
			new DefaultCtorClass().Target();
		}
		
		[DebuggerStepThrough]
		static void StepOut1()
		{
			StepOut2();
		}
		
		static void StepOut2()
		{
		}
		
		[DebuggerStepThrough]
		static void Internal()
		{
		}
		
		[DebuggerNonUserCode]
		public class IgnoredClass
		{
			public static void Internal()
			{
			}
			
			public static void Internal_Pass()
			{
				NotIgnoredClass.Target();
			}
		}
		
		public class NotIgnoredClass
		{
			public static void Target()
			{
			}
		}
		
		public class DefaultCtorClass
		{
			public void Target()
			{
				
			}
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using NUnit.Framework;
	using Debugger.Wrappers.CorDebug;
	using Debugger.Wrappers.MetaData;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void DebuggerAttributes()
		{
			StartTest("DebuggerAttributes.cs");
			
			process.SelectedStackFrame.StepInto(); // Break command
			process.SelectedStackFrame.StepInto(); // Internal
			Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
			process.SelectedStackFrame.StepInto(); // IgnoredClass.Internal
			Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
			process.SelectedStackFrame.StepInto(); // StepOut1
			Assert.AreEqual("StepOut2", process.SelectedStackFrame.MethodInfo.Name);
			process.SelectedStackFrame.StepOut();
			Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
			process.SelectedStackFrame.StepOver(); // Finish the step out
			Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
			process.SelectedStackFrame.StepInto(); // IgnoredClass.Internal_Pass
			Assert.AreEqual("Target", process.SelectedStackFrame.MethodInfo.Name);
			process.SelectedStackFrame.StepOut();
			Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
			process.SelectedStackFrame.StepOver(); // Finish the step out
			Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
			process.SelectedStackFrame.StepInto(); // Generated default constructor
			Assert.AreEqual("Target", process.SelectedStackFrame.MethodInfo.Name);
			process.SelectedStackFrame.StepOut();
			Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
			process.SelectedStackFrame.StepOver(); // Finish the step out
			Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="DebuggerAttributes.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>DebuggerAttributes.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT