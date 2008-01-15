// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class HelloWorld
	{
		public static void Main()
		{
			System.Diagnostics.Debug.WriteLine("Hello world!");
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void HelloWorld()
		{
			StartTest("HelloWorld");
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="HelloWorld">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">HelloWorld.exe</ModuleLoaded>
    <ModuleLoaded symbols="False">System.dll</ModuleLoaded>
    <ModuleLoaded symbols="False">System.Configuration.dll</ModuleLoaded>
    <ModuleLoaded symbols="False">System.Xml.dll</ModuleLoaded>
    <LogMessage>Hello world!\r\n</LogMessage>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT