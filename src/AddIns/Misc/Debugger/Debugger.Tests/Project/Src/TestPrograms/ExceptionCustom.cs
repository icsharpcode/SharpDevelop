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
		public MyException(string msg, System.Exception inner) : base (msg, inner)
		{
			
		}
	}
	
	public class ExceptionCustom
	{
		public static void Main()
		{
			try {
				throw new MyException("test1", null);
			} catch(System.Exception e) {
				throw new MyException("test2", e);
			}
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
			process.Terminate();
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="ExceptionCustom.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ExceptionCustom.exe (Has symbols)</ModuleLoaded>
    <ExceptionThrown>Debugger.Tests.TestPrograms.MyException: test2 ---&gt; Debugger.Tests.TestPrograms.MyException: test1</ExceptionThrown>
    <DebuggingPaused>ExceptionIntercepted ExceptionCustom.cs:27,5-27,39</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT