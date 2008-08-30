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
			
			System.Diagnostics.Debugger.Break();
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
    <DebuggingPaused>Break Expressions.cs:47,4-47,40</DebuggingPaused>
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
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="argValue"
          Type="System.String" />
      </Item>
    </Arguments>
    <LocalVariables
      Capacity="4"
      Count="3">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0"
          Expression="i"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
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
          IsInvalid="False"
          IsNull="False"
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
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[,]" />
      </Item>
    </LocalVariables>
    <this>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="derived name"
          Expression="this.name"
          IsInvalid="False"
          IsNull="False"
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
          Expression="this.Value"
          IsInvalid="False"
          IsNull="False"
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
          Expression="this.field"
          IsInvalid="False"
          IsNull="False"
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
          Expression="this.array"
          IsInvalid="False"
          IsNull="False"
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
          Expression="this.name"
          IsInvalid="False"
          IsNull="False"
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
          Expression="this.Value"
          IsInvalid="False"
          IsNull="False"
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
          Expression="this.Name"
          IsInvalid="False"
          IsNull="False"
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
          Expression="this.Name"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="base name"
          Type="System.String" />
      </Item>
    </this>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT