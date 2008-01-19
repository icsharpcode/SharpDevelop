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
  <Test name="FunctionArgumentVariables.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">FunctionArgumentVariables.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="i = 1">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>1</AsString>
        <Expression>i</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>1</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="s = A">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>A</AsString>
        <Expression>s</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>A</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
      <Item Type="Value" ToString="s_null = &lt;null&gt;">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>&lt;null&gt;</AsString>
        <Expression>s_null</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>True</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Type>System.Object</Type>
      </Item>
      <Item Type="Value" ToString="ref_i = 2">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>2</AsString>
        <Expression>ref_i</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>2</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="out_i = 3">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>3</AsString>
        <Expression>out_i</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>3</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="out_i2 = 0">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>0</AsString>
        <Expression>out_i2</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>0</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="ref_s = B">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>B</AsString>
        <Expression>ref_s</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>B</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
      <Item Type="Value" ToString="iNull = {System.Nullable&lt;System.Int32&gt;}">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>{System.Nullable&lt;System.Int32&gt;}</AsString>
        <Expression>iNull</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>True</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Type>System.Nullable&lt;System.Int32&gt;</Type>
      </Item>
      <Item Type="Value" ToString="iNull_null = {System.Nullable&lt;System.Int32&gt;}">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>{System.Nullable&lt;System.Int32&gt;}</AsString>
        <Expression>iNull_null</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>True</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Type>System.Nullable&lt;System.Int32&gt;</Type>
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="args = {System.String[]}">
        <ArrayDimensions>[0]</ArrayDimensions>
        <ArrayLenght>0</ArrayLenght>
        <ArrayRank>1</ArrayRank>
        <AsString>{System.String[]}</AsString>
        <Expression>args</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>True</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Type>System.String[]</Type>
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="args = {System.String[]}">
        <ArrayDimensions>[1]</ArrayDimensions>
        <ArrayLenght>1</ArrayLenght>
        <ArrayRank>1</ArrayRank>
        <AsString>{System.String[]}</AsString>
        <Expression>args</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>True</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Type>System.String[]</Type>
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="args = {System.String[]}">
        <ArrayDimensions>[2]</ArrayDimensions>
        <ArrayLenght>2</ArrayLenght>
        <ArrayRank>1</ArrayRank>
        <AsString>{System.String[]}</AsString>
        <Expression>args</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>True</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Type>System.String[]</Type>
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="i = 1">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>1</AsString>
        <Expression>i</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>1</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="s = A">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>A</AsString>
        <Expression>s</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>A</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
    </Arguments>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT