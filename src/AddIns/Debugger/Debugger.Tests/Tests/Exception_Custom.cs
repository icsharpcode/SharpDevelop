// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Tests
{
	class MyException: System.Exception
	{
		public MyException(string msg, System.Exception inner) : base (msg, inner)
		{
			
		}
	}
	
	public class Exception_Custom
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
		public void Exception_Custom()
		{
			StartTest();
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
    name="Exception_Custom.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Exception_Custom.exe (Has symbols)</ModuleLoaded>
    <ExceptionThrown>Debugger.Tests.MyException: test2 ---&gt; Debugger.Tests.MyException: test1</ExceptionThrown>
    <DebuggingPaused>ExceptionIntercepted Exception_Custom.cs:23,5-23,39</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
