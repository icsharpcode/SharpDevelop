// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class BaseClass
	{
		string name = "base name";
		
		public string Name {
			get { return name; }
		}
		
		public string Value = "base value";
	}
	
	public class TestClass: BaseClass
	{
		string name = "derived name";
		
		new public string Name {
			get { return name; }
		}
		
		new public string Value = "derived value";
		
		string field = "field value";
		string[] array = {"one", "two", "three"};
		
		public static void Main()
		{
			new TestClass().Test("argValue");
		}
		
		public void Test(string arg)
		{
			int i = 0;
			string[] array = {"one", "two", "three"};
			string[,] array2 = {{"A","B"},{"C","D"}};
			string BaseClass = "baseClassString";
			
			System.Diagnostics.Debugger.Break();
		}
		
		public void Test(int arg)
		{
			
		}
		
		public void Test(TestClass[] arg)
		{
			
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Expressions()
		{
			StartTest("Expressions.cs");
			
			ObjectDump("Arguments", process.SelectedStackFrame.GetArgumentValues());
			ObjectDump("LocalVariables", process.SelectedStackFrame.GetLocalVariableValues());
			ObjectDump("this", process.SelectedStackFrame.GetThisValue().GetMemberValues());
			ObjectDump("methods", process.SelectedStackFrame.MethodInfo.DeclaringType.GetMethods());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Expressions.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Expressions.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Expressions.cs:48,4-48,40</DebuggingPaused>
    <Arguments
      Capacity="4"
      Count="1">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="argValue"
          Expression="arg"
          IsReference="True"
          PrimitiveValue="argValue"
          Type="System.String" />
      </Item>
    </Arguments>
    <LocalVariables
      Capacity="4"
      Count="4">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0"
          Expression="i"
          PrimitiveValue="0"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{3}"
          ArrayLength="3"
          ArrayRank="1"
          AsString="{System.String[]}"
          Expression="array"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{2, 2}"
          ArrayLength="4"
          ArrayRank="2"
          AsString="{System.String[,]}"
          Expression="array2"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[,]" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="baseClassString"
          Expression="BaseClass"
          IsReference="True"
          PrimitiveValue="baseClassString"
          Type="System.String" />
      </Item>
    </LocalVariables>
    <this>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="derived name"
          Expression="((Debugger.Tests.TestPrograms.TestClass)(this)).name"
          IsReference="True"
          PrimitiveValue="derived name"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="derived value"
          Expression="((Debugger.Tests.TestPrograms.TestClass)(this)).Value"
          IsReference="True"
          PrimitiveValue="derived value"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="field value"
          Expression="((Debugger.Tests.TestPrograms.TestClass)(this)).field"
          IsReference="True"
          PrimitiveValue="field value"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{3}"
          ArrayLength="3"
          ArrayRank="1"
          AsString="{System.String[]}"
          Expression="((Debugger.Tests.TestPrograms.TestClass)(this)).array"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="base name"
          Expression="((Debugger.Tests.TestPrograms.BaseClass)(this)).name"
          IsReference="True"
          PrimitiveValue="base name"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="base value"
          Expression="((Debugger.Tests.TestPrograms.BaseClass)(this)).Value"
          IsReference="True"
          PrimitiveValue="base value"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="derived name"
          Expression="((Debugger.Tests.TestPrograms.TestClass)(this)).Name"
          IsReference="True"
          PrimitiveValue="derived name"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="base name"
          Expression="((Debugger.Tests.TestPrograms.BaseClass)(this)).Name"
          IsReference="True"
          PrimitiveValue="base name"
          Type="System.String" />
      </Item>
    </this>
    <methods
      Capacity="8"
      Count="6">
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.get_Name"
          IsPublic="True"
          IsSpecialName="True"
          Module="Expressions.exe"
          Name="get_Name"
          ReturnType="System.String"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.Main"
          IsPublic="True"
          IsStatic="True"
          Module="Expressions.exe"
          Name="Main" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.Test"
          IsPublic="True"
          LocalVariableNames="{array, array2, BaseClass, i}"
          Module="Expressions.exe"
          Name="Test"
          ParameterCount="1"
          ParameterTypes="{System.String}" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.Test"
          IsPublic="True"
          Module="Expressions.exe"
          Name="Test"
          ParameterCount="1"
          ParameterTypes="{System.Int32}" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.Test"
          IsPublic="True"
          Module="Expressions.exe"
          Name="Test"
          ParameterCount="1"
          ParameterTypes="{Debugger.Tests.TestPrograms.TestClass[]}" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass..ctor"
          IsPublic="True"
          IsSpecialName="True"
          Module="Expressions.exe"
          Name=".ctor" />
      </Item>
    </methods>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT