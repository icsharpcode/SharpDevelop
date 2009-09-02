// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class StackFrame_Arguments
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
			new StackFrame_Arguments().Function(1, "A");
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
		public void StackFrame_Arguments()
		{
			StartTest("StackFrame_Arguments.cs");
			
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
    name="StackFrame_Arguments.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_Arguments.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break StackFrame_Arguments.cs:16,4-16,40</DebuggingPaused>
    <DebuggingPaused>Break StackFrame_Arguments.cs:51,4-51,40</DebuggingPaused>
    <Arguments
      Capacity="16"
      Count="9">
      <Item>
        <Value
          AsString="1"
          Expression="i"
          PrimitiveValue="1"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="A"
          Expression="s"
          IsReference="True"
          PrimitiveValue="A"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="null"
          Expression="s_null"
          IsNull="True"
          IsReference="True"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="2"
          Expression="ref_i"
          PrimitiveValue="2"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="3"
          Expression="out_i"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="0"
          Expression="out_i2"
          PrimitiveValue="0"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="B"
          Expression="ref_s"
          IsReference="True"
          PrimitiveValue="B"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="{System.Nullable&lt;System.Int32&gt;}"
          Expression="iNull"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Nullable&lt;System.Int32&gt;" />
      </Item>
      <Item>
        <Value
          AsString="{System.Nullable&lt;System.Int32&gt;}"
          Expression="iNull_null"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Nullable&lt;System.Int32&gt;" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break StackFrame_Arguments.cs:57,4-57,40</DebuggingPaused>
    <Arguments
      Capacity="4"
      Count="1">
      <Item>
        <Value
          ArrayDimensions="{0}"
          ArrayRank="1"
          AsString="{System.String[]}"
          Expression="args"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break StackFrame_Arguments.cs:57,4-57,40</DebuggingPaused>
    <Arguments
      Capacity="4"
      Count="1">
      <Item>
        <Value
          ArrayDimensions="{1}"
          ArrayLength="1"
          ArrayRank="1"
          AsString="{System.String[]}"
          Expression="args"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break StackFrame_Arguments.cs:57,4-57,40</DebuggingPaused>
    <Arguments
      Capacity="4"
      Count="1">
      <Item>
        <Value
          ArrayDimensions="{2}"
          ArrayLength="2"
          ArrayRank="1"
          AsString="{System.String[]}"
          Expression="args"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break StackFrame_Arguments.cs:62,4-62,40</DebuggingPaused>
    <Arguments
      Capacity="4"
      Count="2">
      <Item>
        <Value
          AsString="1"
          Expression="i"
          PrimitiveValue="1"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="A"
          Expression="s"
          IsReference="True"
          PrimitiveValue="A"
          Type="System.String" />
      </Item>
    </Arguments>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT