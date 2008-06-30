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
			System.Diagnostics.Debug.WriteLine("Start");
			System.Diagnostics.Debugger.Break();
			Internal();
			IgnoredClass.Internal();
			System.Diagnostics.Debug.WriteLine("End");
		}
		
		[DebuggerStepThrough]
		static void Internal()
		{
			System.Diagnostics.Debug.WriteLine("Internal");
		}
		
		[DebuggerNonUserCode]
		public class IgnoredClass
		{
			public static void Internal()
			{
				System.Diagnostics.Debug.WriteLine("Internal");
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
			
			process.SelectedStackFrame.StepInto();
			process.SelectedStackFrame.StepInto();
			Assert.AreEqual(process.SelectedStackFrame.MethodInfo.Name, "Main");
			process.SelectedStackFrame.StepInto();
			Assert.AreEqual(process.SelectedStackFrame.MethodInfo.Name, "Main");
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="DebuggerAttributes.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">DebuggerAttributes.exe</ModuleLoaded>
    <ModuleLoaded symbols="False">System.dll</ModuleLoaded>
    <ModuleLoaded symbols="False">System.Configuration.dll</ModuleLoaded>
    <ModuleLoaded symbols="False">System.Xml.dll</ModuleLoaded>
    <LogMessage>Start\r\n</LogMessage>
    <DebuggingPaused>Break</DebuggingPaused>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <LogMessage>Internal\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <LogMessage>Internal\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <LogMessage>End\r\n</LogMessage>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT