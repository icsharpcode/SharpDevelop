// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;

namespace Debugger.Tests
{
	public class ControlFlow_Stepping
	{
		[DebuggerStepThrough]
		static void StepRoot()
		{
			IgnoredClass.StepLeft();
			StepRight();
		}
		
		[DebuggerNonUserCode]
		public class IgnoredClass {
			public static void StepLeft() {}
		}
		
		static void StepRight() {}
		
		public class DefaultCtorClass {
			public void Target() {}
		}
		
		public static int ShortProperty {
			get {
				return 1;
			}
		}
		
		static int field = 1;
		
		public static int FieldProperty {
			get {
				return
					field;
			}
		}
		
		[DebuggerNonUserCode]
		static void ZigZag1()
		{
			try {
				ZigZag2();
			} catch (System.Exception) {
				try {
					ZigZag2();
				} catch (System.Exception) {
				}
			}
		}
		
		static void ZigZag2()
		{ // ... we end up here
			System.Diagnostics.Debug.Write("ZigZag2");
			ZigZag3(); // Stepping in here ...
		}
		
		[DebuggerNonUserCode]
		static void ZigZag3()
		{
			throw new System.Exception();
		}
		
		[DebuggerNonUserCode]
		static void CatchExcpetion()
		{
			try {
				ThrowExcpetion();
			} catch (System.Exception) {
			}
		}
		
		[DebuggerNonUserCode]
		static void ThrowExcpetion()
		{
			throw new System.Exception();
		}
		
		static event EventHandler MyEvent;
		[DebuggerNonUserCode]
		static void Event1(object sender, EventArgs e) {}
		static void Event2(object sender, EventArgs e) {}
		[DebuggerNonUserCode]
		static void Event3(object sender, EventArgs e) {}
		static void Event4(object sender, EventArgs e) {}
		
		public static void Main()
		{
			MyEvent += Event1;
			MyEvent += Event2;
			MyEvent += Event3;
			MyEvent += Event4;
			System.Diagnostics.Debugger.Break();
			string theasnwer = 42.ToString();
			StepRoot();
			new DefaultCtorClass().Target();
			int s = ShortProperty;
			int f = FieldProperty;
			CatchExcpetion();
			ZigZag1();
			MyEvent(null, EventArgs.Empty);
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using NUnit.Framework;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void ControlFlow_Stepping()
		{
			StartTest();
			
			SourcecodeSegment start = process.SelectedStackFrame.NextStatement;
			
			foreach (bool jmcEnabled in new bool[] {true, true, false}) {
				ObjectDump("Log", "Starting run with JMC=" + jmcEnabled.ToString());
				
				process.SelectedStackFrame.SetIP(start.Filename, start.StartLine + 1, start.StartColumn);
				
				process.Options.EnableJustMyCode = jmcEnabled;
				process.Options.StepOverSingleLineProperties = true;
				process.Options.StepOverFieldAccessProperties = true;
				
				process.SelectedStackFrame.StepInto(); // 42.ToString()
				Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
				
				if (jmcEnabled) {
					process.SelectedStackFrame.StepInto(); // StepRoot
					Assert.AreEqual("StepRight", process.SelectedStackFrame.MethodInfo.Name);
					process.SelectedStackFrame.StepOut();
					process.SelectedStackFrame.StepOver(); // Finish the step out
					Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
				} else {
					process.SelectedStackFrame.StepInto(); // StepRoot
					Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
				}
				
				process.SelectedStackFrame.StepInto(); // Generated default constructor
				Assert.AreEqual("Target", process.SelectedStackFrame.MethodInfo.Name);
				process.SelectedStackFrame.StepOut();
				process.SelectedStackFrame.StepOver(); // Finish the step out
				Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
				
				process.SelectedStackFrame.StepInto(); // ShortProperty
				Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
				
				process.SelectedStackFrame.StepInto(); // FieldProperty
				Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
				
				process.SelectedStackFrame.StepInto(); // CatchExcpetion
				Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
				
				if (jmcEnabled) {
					process.SelectedStackFrame.StepInto(); // ZigZag1
					process.SelectedStackFrame.StepOver();
					process.SelectedStackFrame.StepOver();
					Assert.AreEqual("ZigZag2", process.SelectedStackFrame.MethodInfo.Name);
					process.SelectedStackFrame.StepInto();
					Assert.AreEqual("ZigZag2", process.SelectedStackFrame.MethodInfo.Name);
					Assert.AreEqual(3, process.SelectedStackFrame.NextStatement.StartColumn);
					process.SelectedStackFrame.StepOut();
					process.SelectedStackFrame.StepOver(); // Finish the step out
					Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
				} else {
					process.SelectedStackFrame.StepInto(); // ZigZag1
					Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
				}
				
				process.SelectedStackFrame.StepInto(); // MyEvent
				Assert.AreEqual("Event2", process.SelectedStackFrame.MethodInfo.Name);
				process.SelectedStackFrame.StepOut();
				Assert.AreEqual("Event4", process.SelectedStackFrame.MethodInfo.Name);
				process.SelectedStackFrame.StepOut();
				process.SelectedStackFrame.StepOver(); // Finish the step out
				Assert.AreEqual("Main", process.SelectedStackFrame.MethodInfo.Name);
			}
			
			// Restore default state
			process.Options.EnableJustMyCode = true;
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="ControlFlow_Stepping.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_Stepping.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ControlFlow_Stepping.cs:98,4-98,40</DebuggingPaused>
    <Log>Starting run with JMC=True</Log>
    <DebuggingPaused>SetIP ControlFlow_Stepping.cs:99,4-99,37</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:100,4-100,15</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:23,27-23,28</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:100,4-100,15</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:101,4-101,36</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:26,25-26,26</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:101,4-101,36</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:102,4-102,26</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:103,4-103,26</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:104,4-104,21</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:105,4-105,14</DebuggingPaused>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:58,3-58,4</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:59,4-59,46</DebuggingPaused>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>ZigZag2</LogMessage>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:60,4-60,14</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:58,3-58,4</DebuggingPaused>
    <LogMessage>ZigZag2</LogMessage>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:105,4-105,14</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:106,4-106,35</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:87,50-87,51</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:90,50-90,51</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:106,4-106,35</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:107,3-107,4</DebuggingPaused>
    <Log>Starting run with JMC=True</Log>
    <DebuggingPaused>SetIP ControlFlow_Stepping.cs:99,4-99,37</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:100,4-100,15</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:23,27-23,28</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:100,4-100,15</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:101,4-101,36</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:26,25-26,26</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:101,4-101,36</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:102,4-102,26</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:103,4-103,26</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:104,4-104,21</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:105,4-105,14</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:58,3-58,4</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:59,4-59,46</DebuggingPaused>
    <LogMessage>ZigZag2</LogMessage>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:60,4-60,14</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:58,3-58,4</DebuggingPaused>
    <LogMessage>ZigZag2</LogMessage>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:105,4-105,14</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:106,4-106,35</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:87,50-87,51</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:90,50-90,51</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:106,4-106,35</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:107,3-107,4</DebuggingPaused>
    <Log>Starting run with JMC=False</Log>
    <DebuggingPaused>SetIP ControlFlow_Stepping.cs:99,4-99,37</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:100,4-100,15</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:101,4-101,36</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:26,25-26,26</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:101,4-101,36</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:102,4-102,26</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:103,4-103,26</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:104,4-104,21</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:105,4-105,14</DebuggingPaused>
    <LogMessage>ZigZag2</LogMessage>
    <LogMessage>ZigZag2</LogMessage>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:106,4-106,35</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:87,50-87,51</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:90,50-90,51</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:106,4-106,35</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_Stepping.cs:107,3-107,4</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
