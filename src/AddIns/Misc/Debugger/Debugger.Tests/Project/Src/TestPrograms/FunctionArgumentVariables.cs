// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class FunctionArgumentVariables
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			int ref_i = 2;
			int out_i;
			int out_i2;
			string ref_s = "B";
			int? iNull = 5;
			int? iNull_null = null;
			StaticFunction(
				1,
				"A",
				null,
				ref ref_i,
				out out_i,
				out out_i2,
				ref ref_s,
				iNull,
				iNull_null
			);
			VarArgs();
			VarArgs("A");
			VarArgs("A", "B");
			new FunctionArgumentVariables().Function(1, "A");
		}
		
		static void StaticFunction(int i,
		                           string s,
		                           string s_null,
		                           ref int ref_i,
		                           out int out_i,
		                           out int out_i2,
		                           ref string ref_s,
		                           int? iNull,
		                           int? iNull_null)
		{
			out_i = 3;
			System.Diagnostics.Debugger.Break();
			out_i2 = 4;
		}
		
		static void VarArgs(params string[] args)
		{
			System.Diagnostics.Debugger.Break();
		}
		
		void Function(int i, string s)
		{
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void FunctionArgumentVariables()
		{
			StartTest("FunctionArgumentVariables.cs");
			
			for(int i = 0; i < 5; i++) {
				process.Continue();
				ObjectDump("Arguments", process.SelectedStackFrame.GetArgumentValues());
			}
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="FunctionArgumentVariables.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>FunctionArgumentVariables.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="1"
          Expression="i"
          HasExpired="False"
          IsArray="False"
          IsInteger="True"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="1"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="A"
          Expression="s"
          HasExpired="False"
          IsArray="False"
          IsInteger="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="A"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="null"
          Expression="s_null"
          HasExpired="False"
          IsArray="False"
          IsInteger="False"
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
          AsString="2"
          Expression="ref_i"
          HasExpired="False"
          IsArray="False"
          IsInteger="True"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="2"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="3"
          Expression="out_i"
          HasExpired="False"
          IsArray="False"
          IsInteger="True"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0"
          Expression="out_i2"
          HasExpired="False"
          IsArray="False"
          IsInteger="True"
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
          AsString="B"
          Expression="ref_s"
          HasExpired="False"
          IsArray="False"
          IsInteger="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="B"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Nullable&lt;System.Int32&gt;}"
          Expression="iNull"
          HasExpired="False"
          IsArray="False"
          IsInteger="False"
          IsNull="False"
          IsObject="True"
          IsPrimitive="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Nullable&lt;System.Int32&gt;" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Nullable&lt;System.Int32&gt;}"
          Expression="iNull_null"
          HasExpired="False"
          IsArray="False"
          IsInteger="False"
          IsNull="False"
          IsObject="True"
          IsPrimitive="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Nullable&lt;System.Int32&gt;" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments>
      <Item>
        <Value
          ArrayDimensions="[0]"
          ArrayLenght="0"
          ArrayRank="1"
          AsString="{System.String[]}"
          Expression="args"
          HasExpired="False"
          IsArray="True"
          IsInteger="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments>
      <Item>
        <Value
          ArrayDimensions="[1]"
          ArrayLenght="1"
          ArrayRank="1"
          AsString="{System.String[]}"
          Expression="args"
          HasExpired="False"
          IsArray="True"
          IsInteger="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments>
      <Item>
        <Value
          ArrayDimensions="[2]"
          ArrayLenght="2"
          ArrayRank="1"
          AsString="{System.String[]}"
          Expression="args"
          HasExpired="False"
          IsArray="True"
          IsInteger="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="1"
          Expression="i"
          HasExpired="False"
          IsArray="False"
          IsInteger="True"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="1"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="A"
          Expression="s"
          HasExpired="False"
          IsArray="False"
          IsInteger="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="A"
          Type="System.String" />
      </Item>
    </Arguments>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT