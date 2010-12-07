// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Tests
{
	public class AppDomain_Tests
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
		public void AppDomain_Tests()
		{
			StartTest();
			
			DebugType type1 = process.SelectedStackFrame.GetLocalVariableValue("one").Type;
			DebugType type1b = process.SelectedStackFrame.GetLocalVariableValue("one").Type;
			ObjectDump("SameDomainEqual", type1 == type1b);
			process.Continue();
			ObjectDump("AppDomainName", process.SelectedStackFrame.GetLocalVariableValue("appDomainName").AsString());
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
    name="AppDomain_Tests.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>AppDomain_Tests.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break AppDomain_Tests.cs:13,4-13,40</DebuggingPaused>
    <SameDomainEqual>True</SameDomainEqual>
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>AppDomain_Tests.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break AppDomain_Tests.cs:26,4-26,40</DebuggingPaused>
    <AppDomainName>myDomain Id=2</AppDomainName>
    <OtherDomainEqual>False</OtherDomainEqual>
    <AppDomainsEqual>False</AppDomainsEqual>
    <AppDomainIDsEqual>False</AppDomainIDsEqual>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
