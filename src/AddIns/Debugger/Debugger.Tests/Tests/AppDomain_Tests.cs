// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
	using ICSharpCode.NRefactory.TypeSystem;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void AppDomain_Tests()
		{
			StartTest();
			
			IType type1 = this.CurrentStackFrame.GetLocalVariableValue("one").Type;
			process.Continue();
			ObjectDump("AppDomainName", this.CurrentStackFrame.GetLocalVariableValue("appDomainName").AsString());
			IType type2 = this.CurrentStackFrame.GetLocalVariableValue("two").Type;
			ObjectDump("OtherDomainEqual", type1.Equals(type2));
			ObjectDump("AppDomain1-ID", type1.GetDefinition().Compilation.GetAppDomain().ID);
			ObjectDump("AppDomain2-ID", type2.GetDefinition().Compilation.GetAppDomain().ID);
			
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
    <Paused>AppDomain_Tests.cs:28,4-28,40</Paused>
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>AppDomain_Tests.exe (Has symbols)</ModuleLoaded>
    <Paused>AppDomain_Tests.cs:41,4-41,40</Paused>
    <AppDomainName>myDomain Id=2</AppDomainName>
    <OtherDomainEqual>False</OtherDomainEqual>
    <AppDomain1-ID>1</AppDomain1-ID>
    <AppDomain2-ID>2</AppDomain2-ID>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
