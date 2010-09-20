// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Tests
{
	public class _HelloWorldTest
	{
		public static void Main()
		{
			System.Diagnostics.Debug.WriteLine("Hello world!");
			
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void _HelloWorldTest()
		{
			StartTest();
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="_HelloWorldTest.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>_HelloWorldTest.exe (Has symbols)</ModuleLoaded>
    <ModuleLoaded>System.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Configuration.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>System.Xml.dll (No symbols)</ModuleLoaded>
    <LogMessage>Hello world!\r\n</LogMessage>
    <DebuggingPaused>Break _HelloWorldTest.cs:14,4-14,40</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
