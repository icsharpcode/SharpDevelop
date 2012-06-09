// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

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
		[NUnit.Framework.Test, NUnit.Framework.Ignore]
		public void AppDomain_Tests()
		{
			StartTest();
			
//			IType type1 = this.CurrentStackFrame.GetLocalVariableValue("one").Type;
//			IType type1b = this.CurrentStackFrame.GetLocalVariableValue("one").Type;
//			ObjectDump("SameDomainEqual", type1.Equals(type1b));
//			process.Continue();
//			ObjectDump("AppDomainName", this.CurrentStackFrame.GetLocalVariableValue("appDomainName").AsString());
//			IType type2 = this.CurrentStackFrame.GetLocalVariableValue("two").Type;
//			ObjectDump("OtherDomainEqual", type1.Equals(type2));
//			ObjectDump("AppDomainsEqual", type1.AppDomain == type2.AppDomain);
//			ObjectDump("AppDomainIDsEqual", type1.AppDomain.ID == type2.AppDomain.ID);
			
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
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>AppDomain_Tests.exe (Has symbols)</ModuleLoaded>
    <Paused>AppDomain_Tests.cs:13,4-13,40</Paused>
    <SameDomainEqual>True</SameDomainEqual>
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>AppDomain_Tests.exe (Has symbols)</ModuleLoaded>
    <Paused>AppDomain_Tests.cs:26,4-26,40</Paused>
    <AppDomainName>myDomain Id=2</AppDomainName>
    <OtherDomainEqual>False</OtherDomainEqual>
    <AppDomainsEqual>False</AppDomainsEqual>
    <AppDomainIDsEqual>False</AppDomainIDsEqual>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
