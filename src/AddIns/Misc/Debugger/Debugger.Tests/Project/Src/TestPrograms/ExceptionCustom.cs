// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	class MyException: System.Exception
	{
		public MyException(string msg) : base (msg)
		{
			
		}
	}
	
	public class ExceptionCustom
	{
		public static void Main()
		{
			throw new MyException("test");
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void ExceptionCustom()
		{
			StartTest("ExceptionCustom.cs");
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="ExceptionCustom.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">ExceptionCustom.exe</ModuleLoaded>
    <DebuggingPaused>Exception</DebuggingPaused>
    <ExceptionThrown>test</ExceptionThrown>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT