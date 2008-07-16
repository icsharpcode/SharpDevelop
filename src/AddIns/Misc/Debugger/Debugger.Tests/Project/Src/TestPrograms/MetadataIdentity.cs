// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class MetadataIdentity
	{
		public static void Main()
		{
			new MetadataIdentity().Func();
		}
		
		public void Func()
		{
			System.Diagnostics.Debugger.Break();
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using NUnit.Framework;
	using Debugger.MetaData;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void MetadataIdentity()
		{
			StartTest("MetadataIdentity.cs");
			
			DebugType type = process.SelectedStackFrame.GetThisValue().Type;
			MethodInfo mainMethod = process.SelectedStackFrame.MethodInfo;
			process.Continue();
			
			Assert.AreEqual(type, process.SelectedStackFrame.GetThisValue().Type);
			Assert.AreEqual(mainMethod, process.SelectedStackFrame.MethodInfo);
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="MetadataIdentity.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>MetadataIdentity.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break MetadataIdentity.cs:21,4-21,40</DebuggingPaused>
    <DebuggingPaused>Break MetadataIdentity.cs:22,4-22,40</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT