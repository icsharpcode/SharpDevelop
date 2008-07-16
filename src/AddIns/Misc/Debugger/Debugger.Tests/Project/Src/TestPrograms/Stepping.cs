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
	public class Stepping
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
	using Debugger.Wrappers.CorDebug;
	using Debugger.Wrappers.MetaData;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Stepping()
		{
			StartTest("Stepping.cs");
			
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
				
				// TODO: Does not work for static
				// process.SelectedStackFrame.StepInto(); // FieldProperty
				process.SelectedStackFrame.StepOver(); // FieldProperty
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
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Stepping.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Stepping.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Stepping.cs:102,4-102,40</DebuggingPaused>
    <Log>Starting run with JMC=True</Log>
    <DebuggingPaused>SetIP Stepping.cs:103,4-103,37</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:104,4-104,15</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:27,27-27,28</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:104,4-104,15</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:105,4-105,36</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:30,25-30,26</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:105,4-105,36</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:106,4-106,26</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:107,4-107,26</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:108,4-108,21</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:109,4-109,14</DebuggingPaused>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <DebuggingPaused>StepComplete Stepping.cs:62,3-62,4</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:63,4-63,46</DebuggingPaused>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>ZigZag2</LogMessage>
    <DebuggingPaused>StepComplete Stepping.cs:64,4-64,14</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:62,3-62,4</DebuggingPaused>
    <LogMessage>ZigZag2</LogMessage>
    <DebuggingPaused>StepComplete Stepping.cs:109,4-109,14</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:110,4-110,35</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:91,50-91,51</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:94,50-94,51</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:110,4-110,35</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:111,3-111,4</DebuggingPaused>
    <Log>Starting run with JMC=True</Log>
    <DebuggingPaused>SetIP Stepping.cs:103,4-103,37</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:104,4-104,15</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:27,27-27,28</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:104,4-104,15</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:105,4-105,36</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:30,25-30,26</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:105,4-105,36</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:106,4-106,26</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:107,4-107,26</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:108,4-108,21</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:109,4-109,14</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:62,3-62,4</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:63,4-63,46</DebuggingPaused>
    <LogMessage>ZigZag2</LogMessage>
    <DebuggingPaused>StepComplete Stepping.cs:64,4-64,14</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:62,3-62,4</DebuggingPaused>
    <LogMessage>ZigZag2</LogMessage>
    <DebuggingPaused>StepComplete Stepping.cs:109,4-109,14</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:110,4-110,35</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:91,50-91,51</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:94,50-94,51</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:110,4-110,35</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:111,3-111,4</DebuggingPaused>
    <Log>Starting run with JMC=False</Log>
    <DebuggingPaused>SetIP Stepping.cs:103,4-103,37</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:104,4-104,15</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:105,4-105,36</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:30,25-30,26</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:105,4-105,36</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:106,4-106,26</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:107,4-107,26</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:108,4-108,21</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:109,4-109,14</DebuggingPaused>
    <LogMessage>ZigZag2</LogMessage>
    <LogMessage>ZigZag2</LogMessage>
    <DebuggingPaused>StepComplete Stepping.cs:110,4-110,35</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:91,50-91,51</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:94,50-94,51</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:110,4-110,35</DebuggingPaused>
    <DebuggingPaused>StepComplete Stepping.cs:111,3-111,4</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT