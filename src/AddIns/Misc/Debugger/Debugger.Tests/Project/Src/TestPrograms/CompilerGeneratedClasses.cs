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
			new List<object>(new CompilerGeneratedClasses().MyEnum());
		}
		
		IEnumerable<object> MyEnum()
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
						Console.WriteLine(this);
						
						System.Diagnostics.Debugger.Break();
					};
					
					System.Diagnostics.Debugger.Break();
					nestedDeleg(402);
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
			ObjectDump("OutterDelegateLocalVariables",
			           process.SelectedStackFrame.MethodInfo.LocalVariables.
			           Select(v => new { v.Name, Value = v.GetValue(process.SelectedStackFrame) })
			          );
			process.Continue();
			ObjectDump("InnterDelegateLocalVariables",
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
      <Item>
        <AnonymousType
          Name="stateLessVar_DelegRef"
          Value="(()(CS$&lt;&gt;8__locals5)).stateLessVar_DelegRef = 202" />
      </Item>
      <Item>
        <AnonymousType
          Name="stateLessVar_NestedDelegRef"
          Value="(()(CS$&lt;&gt;8__locals5)).stateLessVar_NestedDelegRef = 203" />
      </Item>
    </YieldLocalVariables>
    <DebuggingPaused>Break CompilerGeneratedClasses.cs:54,6-54,42</DebuggingPaused>
    <OutterDelegateLocalVariables>
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
      <Item>
        <AnonymousType
          Name="delegVar_NestedDelegRef"
          Value="(()(CS$&lt;&gt;8__locals7)).delegVar_NestedDelegRef = 302" />
      </Item>
      <Item>
        <AnonymousType
          Name="delegArg_NestedDelegRef"
          Value="(()(CS$&lt;&gt;8__locals7)).delegArg_NestedDelegRef = 401" />
      </Item>
      <Item>
        <AnonymousType
          Name="stateLessVar_DelegRef"
          Value="(()(this)).stateLessVar_DelegRef = 202" />
      </Item>
      <Item>
        <AnonymousType
          Name="stateLessVar_NestedDelegRef"
          Value="(()(this)).stateLessVar_NestedDelegRef = 203" />
      </Item>
      <Item>
        <AnonymousType
          Name="stateFullVar_DelegRef"
          Value="(()((()(this)).CS$&lt;&gt;8__locals3)).stateFullVar_DelegRef = 102" />
      </Item>
      <Item>
        <AnonymousType
          Name="stateFullVar_NestedDelegRef"
          Value="(()((()(this)).CS$&lt;&gt;8__locals3)).stateFullVar_NestedDelegRef = 103" />
      </Item>
      <Item>
        <AnonymousType
          Name="this"
          Value="(()((()(this)).CS$&lt;&gt;8__locals3)).&lt;&gt;4__this = {Debugger.Tests.TestPrograms.CompilerGeneratedClasses}" />
      </Item>
    </OutterDelegateLocalVariables>
    <DebuggingPaused>Break CompilerGeneratedClasses.cs:51,7-51,43</DebuggingPaused>
    <InnterDelegateLocalVariables>
      <Item>
        <AnonymousType
          Name="nestedDelegVar"
          Value="nestedDelegVar = 303" />
      </Item>
      <Item>
        <AnonymousType
          Name="delegVar_NestedDelegRef"
          Value="(()(this)).delegVar_NestedDelegRef = 302" />
      </Item>
      <Item>
        <AnonymousType
          Name="delegArg_NestedDelegRef"
          Value="(()(this)).delegArg_NestedDelegRef = 401" />
      </Item>
      <Item>
        <AnonymousType
          Name="stateLessVar_DelegRef"
          Value="(()((()(this)).CS$&lt;&gt;8__locals5)).stateLessVar_DelegRef = 202" />
      </Item>
      <Item>
        <AnonymousType
          Name="stateLessVar_NestedDelegRef"
          Value="(()((()(this)).CS$&lt;&gt;8__locals5)).stateLessVar_NestedDelegRef = 203" />
      </Item>
      <Item>
        <AnonymousType
          Name="stateFullVar_DelegRef"
          Value="(()((()(this)).CS$&lt;&gt;8__locals3)).stateFullVar_DelegRef = 102" />
      </Item>
      <Item>
        <AnonymousType
          Name="stateFullVar_NestedDelegRef"
          Value="(()((()(this)).CS$&lt;&gt;8__locals3)).stateFullVar_NestedDelegRef = 103" />
      </Item>
      <Item>
        <AnonymousType
          Name="this"
          Value="(()((()(this)).CS$&lt;&gt;8__locals3)).&lt;&gt;4__this = {Debugger.Tests.TestPrograms.CompilerGeneratedClasses}" />
      </Item>
    </InnterDelegateLocalVariables>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
