// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3230 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger.Tests.TestPrograms
{
	public class CompilerGeneratedClasses
	{
		delegate void IntDelegate(int i);
		
		public static void Main()
		{
			new List<object>(MyEnum());
		}
		
		static IEnumerable<object> MyEnum()
		{
			int stateFullVar = 101;
			int stateFullVar_DelegRef = 102;
			int stateFullVar_NestedDelegRef = 103;
			
			yield return stateFullVar + stateFullVar_DelegRef + stateFullVar_NestedDelegRef;
			
			{
				int stateLessVar = 201;
				int stateLessVar_DelegRef = 202;
				int stateLessVar_NestedDelegRef = 203;
				
				System.Diagnostics.Debugger.Break();
				
				IntDelegate deleg = delegate (int delegArg_NestedDelegRef) {
					int delegVar = 301;
					int delegVar_NestedDelegRef = 302;
					Console.WriteLine(stateFullVar_DelegRef);
					Console.WriteLine(stateLessVar_DelegRef);
					
					IntDelegate nestedDeleg = delegate (int nestedDelegArg) {
						int nestedDelegVar = 303;
						Console.WriteLine(delegArg_NestedDelegRef);
						Console.WriteLine(delegVar_NestedDelegRef);
						Console.WriteLine(stateFullVar_NestedDelegRef);
						Console.WriteLine(stateLessVar_NestedDelegRef);
					};
					
					System.Diagnostics.Debugger.Break();
				};
				deleg(401);
			}
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using System.Linq;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void CompilerGeneratedClasses()
		{
			StartTest("CompilerGeneratedClasses.cs");
			ObjectDump("YieldLocalVariables",
			           process.SelectedStackFrame.MethodInfo.LocalVariables.
			           Select(v => new { v.Name, Value = v.GetValue(process.SelectedStackFrame) })
			          );
			process.Continue();
			ObjectDump("DelegateLocalVariables",
			           process.SelectedStackFrame.MethodInfo.LocalVariables.
			           Select(v => new { v.Name, Value = v.GetValue(process.SelectedStackFrame) })
			          );
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="CompilerGeneratedClasses.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>CompilerGeneratedClasses.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break CompilerGeneratedClasses.cs:35,5-35,41</DebuggingPaused>
    <YieldLocalVariables>
      <Item>
        <AnonymousType
          Name="stateLessVar"
          Value="stateLessVar = 201" />
      </Item>
      <Item>
        <AnonymousType
          Name="deleg"
          Value="deleg = null" />
      </Item>
    </YieldLocalVariables>
    <DebuggingPaused>Break CompilerGeneratedClasses.cs:51,6-51,42</DebuggingPaused>
    <DelegateLocalVariables>
      <Item>
        <AnonymousType
          Name="delegVar"
          Value="delegVar = 301" />
      </Item>
      <Item>
        <AnonymousType
          Name="nestedDeleg"
          Value="nestedDeleg = {IntDelegate}" />
      </Item>
    </DelegateLocalVariables>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
