// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class AppDomains
	{
		public static void Main()
		{
			int one = 1;
			System.Diagnostics.Debugger.Break();
			System.AppDomain appDomain = System.AppDomain.CreateDomain("myDomain");
			RemoteObj printer = (RemoteObj)appDomain.CreateInstanceAndUnwrap(typeof(RemoteObj).Assembly.FullName, typeof(RemoteObj).FullName);
			printer.Foo();
		}
	}
	
	public class RemoteObj: MarshalByRefObject
	{
		public void Foo()
		{
			int two = 2;
			string appDomainName = System.AppDomain.CurrentDomain.FriendlyName + " Id=" + System.AppDomain.CurrentDomain.Id;
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using Debugger.MetaData;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void AppDomains()
		{
			StartTest("AppDomains.cs");
			
			DebugType type1 = process.SelectedStackFrame.GetLocalVariableValue("one").Type;
			DebugType type1b = process.SelectedStackFrame.GetLocalVariableValue("one").Type;
			ObjectDump("SameDomainEqual", type1 == type1b);
			process.Continue();
			ObjectDump("AppDomainName", process.SelectedStackFrame.GetLocalVariableValue("appDomainName").AsString);
			DebugType type2 = process.SelectedStackFrame.GetLocalVariableValue("two").Type;
			ObjectDump("OtherDomainEqual", type1 == type2);
			ObjectDump("AppDomainsEqual", type1.AppDomain == type2.AppDomain);
			ObjectDump("AppDomainIDsEqual", type1.AppDomain.ID == type2.AppDomain.ID);
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="AppDomains.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>AppDomains.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break AppDomains.cs:17,4-17,40</DebuggingPaused>
    <SameDomainEqual>True</SameDomainEqual>
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>AppDomains.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break AppDomains.cs:30,4-30,40</DebuggingPaused>
    <AppDomainName>myDomain Id=2</AppDomainName>
    <OtherDomainEqual>False</OtherDomainEqual>
    <AppDomainsEqual>False</AppDomainsEqual>
    <AppDomainIDsEqual>False</AppDomainIDsEqual>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT