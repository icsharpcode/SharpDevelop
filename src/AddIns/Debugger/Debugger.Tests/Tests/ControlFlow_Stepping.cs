// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		public static int Property {
			get {
				return 1;
			}
		}
		
		static int field = 1;
		
		public static int FieldProperty {
			get {
				return field;
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
			int p = Property;
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
		[NUnit.Framework.Test, NUnit.Framework.Ignore("Broken on .NET 4.5.2 due to #472")]
		public void ControlFlow_Stepping()
		{
			StartTest();
			
			SequencePoint start = this.CurrentStackFrame.NextStatement;
			
			foreach (bool jmcEnabled in new bool[] {true, true, false}) {
				ObjectDump("Log", "Starting run with JMC=" + jmcEnabled.ToString());
				
				this.CurrentStackFrame.SetIP(start.Filename, start.StartLine + 1, start.StartColumn, false);
				
				process.Options.EnableJustMyCode = jmcEnabled;
				process.Options.StepOverFieldAccessProperties = true;
				
				this.CurrentStackFrame.StepInto(); // 42.ToString()
				Assert.AreEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
				
				if (jmcEnabled) {
					this.CurrentStackFrame.StepInto(); // StepRoot
					Assert.AreEqual("StepRight", this.CurrentStackFrame.MethodInfo.Name);
					this.CurrentStackFrame.StepOut();
					this.CurrentStackFrame.StepOver(); // Finish the step out
					Assert.AreEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
				} else {
					this.CurrentStackFrame.StepInto(); // StepRoot
					Assert.AreEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
				}
				
				this.CurrentStackFrame.StepInto(); // Generated default constructor
				Assert.AreEqual("Target", this.CurrentStackFrame.MethodInfo.Name);
				this.CurrentStackFrame.StepOut();
				this.CurrentStackFrame.StepOver(); // Finish the step out
				Assert.AreEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
				
				this.CurrentStackFrame.StepInto(); // Property
				Assert.AreNotEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
				this.CurrentStackFrame.StepOut();
				this.CurrentStackFrame.StepOver(); // Finish the step out
				Assert.AreEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
				
				this.CurrentStackFrame.StepInto(); // FieldProperty
				Assert.AreEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
				
				this.CurrentStackFrame.StepInto(); // CatchExcpetion
				Assert.AreEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
				
				if (jmcEnabled) {
					this.CurrentStackFrame.StepInto(); // ZigZag1
					this.CurrentStackFrame.StepOver();
					this.CurrentStackFrame.StepOver();
					Assert.AreEqual("ZigZag2", this.CurrentStackFrame.MethodInfo.Name);
					this.CurrentStackFrame.StepInto();
					Assert.AreEqual("ZigZag2", this.CurrentStackFrame.MethodInfo.Name);
					Assert.AreEqual(3, this.CurrentStackFrame.NextStatement.StartColumn);
					this.CurrentStackFrame.StepOut();
					this.CurrentStackFrame.StepOver(); // Finish the step out
					Assert.AreEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
				} else {
					this.CurrentStackFrame.StepInto(); // ZigZag1
					Assert.AreEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
				}
				
				this.CurrentStackFrame.StepInto(); // MyEvent
				Assert.AreEqual("Event2", this.CurrentStackFrame.MethodInfo.Name);
				this.CurrentStackFrame.StepOut();
				Assert.AreEqual("Event4", this.CurrentStackFrame.MethodInfo.Name);
				this.CurrentStackFrame.StepOut();
				this.CurrentStackFrame.StepOver(); // Finish the step out
				Assert.AreEqual("Main", this.CurrentStackFrame.MethodInfo.Name);
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
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_Stepping.exe (Has symbols)</ModuleLoaded>
    <Paused>ControlFlow_Stepping.cs:113,4-113,40</Paused>
    <Log>Starting run with JMC=True</Log>
    <Paused>ControlFlow_Stepping.cs:115,4-115,15</Paused>
    <Paused>ControlFlow_Stepping.cs:38,27-38,28</Paused>
    <Paused>ControlFlow_Stepping.cs:115,4-115,15</Paused>
    <Paused>ControlFlow_Stepping.cs:116,4-116,36</Paused>
    <Paused>ControlFlow_Stepping.cs:41,25-41,26</Paused>
    <Paused>ControlFlow_Stepping.cs:116,4-116,36</Paused>
    <Paused>ControlFlow_Stepping.cs:117,4-117,21</Paused>
    <Paused>ControlFlow_Stepping.cs:45,8-45,9</Paused>
    <Paused>ControlFlow_Stepping.cs:117,4-117,21</Paused>
    <Paused>ControlFlow_Stepping.cs:118,4-118,26</Paused>
    <Paused>ControlFlow_Stepping.cs:119,4-119,21</Paused>
    <Paused>ControlFlow_Stepping.cs:120,4-120,14</Paused>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <Paused>ControlFlow_Stepping.cs:73,3-73,4</Paused>
    <Paused>ControlFlow_Stepping.cs:74,4-74,46</Paused>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>ZigZag2</LogMessage>
    <Paused>ControlFlow_Stepping.cs:75,4-75,14</Paused>
    <Paused>ControlFlow_Stepping.cs:73,3-73,4</Paused>
    <LogMessage>ZigZag2</LogMessage>
    <Paused>ControlFlow_Stepping.cs:120,4-120,14</Paused>
    <Paused>ControlFlow_Stepping.cs:121,4-121,35</Paused>
    <Paused>ControlFlow_Stepping.cs:102,50-102,51</Paused>
    <Paused>ControlFlow_Stepping.cs:105,50-105,51</Paused>
    <Paused>ControlFlow_Stepping.cs:121,4-121,35</Paused>
    <Paused>ControlFlow_Stepping.cs:122,3-122,4</Paused>
    <Log>Starting run with JMC=True</Log>
    <Paused>ControlFlow_Stepping.cs:115,4-115,15</Paused>
    <Paused>ControlFlow_Stepping.cs:38,27-38,28</Paused>
    <Paused>ControlFlow_Stepping.cs:115,4-115,15</Paused>
    <Paused>ControlFlow_Stepping.cs:116,4-116,36</Paused>
    <Paused>ControlFlow_Stepping.cs:41,25-41,26</Paused>
    <Paused>ControlFlow_Stepping.cs:116,4-116,36</Paused>
    <Paused>ControlFlow_Stepping.cs:117,4-117,21</Paused>
    <Paused>ControlFlow_Stepping.cs:45,8-45,9</Paused>
    <Paused>ControlFlow_Stepping.cs:117,4-117,21</Paused>
    <Paused>ControlFlow_Stepping.cs:118,4-118,26</Paused>
    <Paused>ControlFlow_Stepping.cs:119,4-119,21</Paused>
    <Paused>ControlFlow_Stepping.cs:120,4-120,14</Paused>
    <Paused>ControlFlow_Stepping.cs:73,3-73,4</Paused>
    <Paused>ControlFlow_Stepping.cs:74,4-74,46</Paused>
    <LogMessage>ZigZag2</LogMessage>
    <Paused>ControlFlow_Stepping.cs:75,4-75,14</Paused>
    <Paused>ControlFlow_Stepping.cs:73,3-73,4</Paused>
    <LogMessage>ZigZag2</LogMessage>
    <Paused>ControlFlow_Stepping.cs:120,4-120,14</Paused>
    <Paused>ControlFlow_Stepping.cs:121,4-121,35</Paused>
    <Paused>ControlFlow_Stepping.cs:102,50-102,51</Paused>
    <Paused>ControlFlow_Stepping.cs:105,50-105,51</Paused>
    <Paused>ControlFlow_Stepping.cs:121,4-121,35</Paused>
    <Paused>ControlFlow_Stepping.cs:122,3-122,4</Paused>
    <Log>Starting run with JMC=False</Log>
    <Paused>ControlFlow_Stepping.cs:115,4-115,15</Paused>
    <Paused>ControlFlow_Stepping.cs:116,4-116,36</Paused>
    <Paused>ControlFlow_Stepping.cs:41,25-41,26</Paused>
    <Paused>ControlFlow_Stepping.cs:116,4-116,36</Paused>
    <Paused>ControlFlow_Stepping.cs:117,4-117,21</Paused>
    <Paused>ControlFlow_Stepping.cs:45,8-45,9</Paused>
    <Paused>ControlFlow_Stepping.cs:117,4-117,21</Paused>
    <Paused>ControlFlow_Stepping.cs:118,4-118,26</Paused>
    <Paused>ControlFlow_Stepping.cs:119,4-119,21</Paused>
    <Paused>ControlFlow_Stepping.cs:120,4-120,14</Paused>
    <LogMessage>ZigZag2</LogMessage>
    <LogMessage>ZigZag2</LogMessage>
    <Paused>ControlFlow_Stepping.cs:121,4-121,35</Paused>
    <Paused>ControlFlow_Stepping.cs:102,50-102,51</Paused>
    <Paused>ControlFlow_Stepping.cs:105,50-105,51</Paused>
    <Paused>ControlFlow_Stepping.cs:121,4-121,35</Paused>
    <Paused>ControlFlow_Stepping.cs:122,3-122,4</Paused>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
