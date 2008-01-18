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
			WaitForPause();
			
			for(int i = 0; i < 5; i++) {
				process.Continue();
				WaitForPause();
				ObjectDump("Arguments", process.SelectedStackFrame.GetArgumentValues());
			}
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
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
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>i</Expression>
        <IsNull>False</IsNull>
        <AsString>1</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Int32</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>True</IsInteger>
        <PrimitiveValue>1</PrimitiveValue>
      </Item>
      <Item Type="Value" ToString="s = A">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>s</Expression>
        <IsNull>False</IsNull>
        <AsString>A</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.String</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue>A</PrimitiveValue>
      </Item>
      <Item Type="Value" ToString="s_null = &lt;null&gt;">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>s_null</Expression>
        <IsNull>True</IsNull>
        <AsString>&lt;null&gt;</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Object</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue exception="Value is not a primitive type" />
      </Item>
      <Item Type="Value" ToString="ref_i = 2">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>ref_i</Expression>
        <IsNull>False</IsNull>
        <AsString>2</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Int32</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>True</IsInteger>
        <PrimitiveValue>2</PrimitiveValue>
      </Item>
      <Item Type="Value" ToString="out_i = 3">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>out_i</Expression>
        <IsNull>False</IsNull>
        <AsString>3</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Int32</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>True</IsInteger>
        <PrimitiveValue>3</PrimitiveValue>
      </Item>
      <Item Type="Value" ToString="out_i2 = 0">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>out_i2</Expression>
        <IsNull>False</IsNull>
        <AsString>0</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Int32</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>True</IsInteger>
        <PrimitiveValue>0</PrimitiveValue>
      </Item>
      <Item Type="Value" ToString="ref_s = B">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>ref_s</Expression>
        <IsNull>False</IsNull>
        <AsString>B</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.String</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue>B</PrimitiveValue>
      </Item>
      <Item Type="Value" ToString="iNull = {System.Nullable&lt;System.Int32&gt;}">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>iNull</Expression>
        <IsNull>False</IsNull>
        <AsString>{System.Nullable&lt;System.Int32&gt;}</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Nullable&lt;System.Int32&gt;</Type>
        <IsObject>True</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue exception="Value is not a primitive type" />
      </Item>
      <Item Type="Value" ToString="iNull_null = {System.Nullable&lt;System.Int32&gt;}">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>iNull_null</Expression>
        <IsNull>False</IsNull>
        <AsString>{System.Nullable&lt;System.Int32&gt;}</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Nullable&lt;System.Int32&gt;</Type>
        <IsObject>True</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue exception="Value is not a primitive type" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="args = {System.String[]}">
        <IsArray>True</IsArray>
        <ArrayLenght>0</ArrayLenght>
        <ArrayRank>1</ArrayRank>
        <ArrayDimensions>[0]</ArrayDimensions>
        <Expression>args</Expression>
        <IsNull>False</IsNull>
        <AsString>{System.String[]}</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.String[]</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue exception="Value is not a primitive type" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="args = {System.String[]}">
        <IsArray>True</IsArray>
        <ArrayLenght>1</ArrayLenght>
        <ArrayRank>1</ArrayRank>
        <ArrayDimensions>[1]</ArrayDimensions>
        <Expression>args</Expression>
        <IsNull>False</IsNull>
        <AsString>{System.String[]}</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.String[]</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue exception="Value is not a primitive type" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="args = {System.String[]}">
        <IsArray>True</IsArray>
        <ArrayLenght>2</ArrayLenght>
        <ArrayRank>1</ArrayRank>
        <ArrayDimensions>[2]</ArrayDimensions>
        <Expression>args</Expression>
        <IsNull>False</IsNull>
        <AsString>{System.String[]}</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.String[]</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue exception="Value is not a primitive type" />
      </Item>
    </Arguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="i = 1">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>i</Expression>
        <IsNull>False</IsNull>
        <AsString>1</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Int32</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>True</IsInteger>
        <PrimitiveValue>1</PrimitiveValue>
      </Item>
      <Item Type="Value" ToString="s = A">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <Expression>s</Expression>
        <IsNull>False</IsNull>
        <AsString>A</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.String</Type>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>False</IsInteger>
        <PrimitiveValue>A</PrimitiveValue>
      </Item>
    </Arguments>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT