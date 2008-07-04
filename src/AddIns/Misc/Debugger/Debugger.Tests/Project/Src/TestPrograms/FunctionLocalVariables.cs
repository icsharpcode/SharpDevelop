// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class FunctionLocalVariables
	{
		public static void Main()
		{
			int i = 0;
			string s = "S";
			string[] args = new string[] {"p1"};
			object n = null;
			object o = new object();
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void FunctionLocalVariables()
		{
			StartTest("FunctionLocalVariables.cs");
			
			ObjectDump("LocalVariables", process.SelectedStackFrame.GetLocalVariableValues());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="FunctionLocalVariables.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>FunctionLocalVariables.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <LocalVariables>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0"
          Expression="i"
          IsArray="False"
          IsInteger="True"
          IsInvalid="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="0"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="S"
          Expression="s"
          IsArray="False"
          IsInteger="False"
          IsInvalid="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="S"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="[1]"
          ArrayLenght="1"
          ArrayRank="1"
          AsString="{System.String[]}"
          Expression="args"
          IsArray="True"
          IsInteger="False"
          IsInvalid="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="null"
          Expression="n"
          IsArray="False"
          IsInteger="False"
          IsInvalid="False"
          IsNull="True"
          IsObject="False"
          IsPrimitive="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Object}"
          Expression="o"
          IsArray="False"
          IsInteger="False"
          IsInvalid="False"
          IsNull="False"
          IsObject="True"
          IsPrimitive="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object" />
      </Item>
    </LocalVariables>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT