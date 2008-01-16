// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class ArrayValue
	{
		public static void Main()
		{
			int[] array = new int[5];
			for(int i = 0; i < 5; i++) {
				array[i] = i;
			}
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void ArrayValue()
		{
			StartTest("ArrayValue.cs");
			WaitForPause();
			Value array = process.SelectedStackFrame.LocalVariables["array"];
			ObjectDump("array", array);
			ObjectDump("array elements", array.GetArrayElements());
			
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
  <Test name="ArrayValue.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">ArrayValue.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <Object name="array" Type="Value" ToString="array = {System.Int32[]}">
      <IsArray>True</IsArray>
      <ArrayLenght>5</ArrayLenght>
      <ArrayRank>1</ArrayRank>
      <ArrayDimensions>[5]</ArrayDimensions>
      <IsObject>False</IsObject>
      <IsPrimitive>False</IsPrimitive>
      <IsInteger>False</IsInteger>
      <PrimitiveValue exception="Value is not a primitive type" />
      <Expression>array</Expression>
      <Name>array</Name>
      <IsNull>False</IsNull>
      <AsString>{System.Int32[]}</AsString>
      <HasExpired>False</HasExpired>
      <Type>System.Int32[]</Type>
    </Object>
    <Object name="array elements" Type="ValueCollection" ToString="[ValueCollection Count=5]">
      <Count>5</Count>
      <Item Type="Value" ToString="array[0] = 0">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>True</IsInteger>
        <PrimitiveValue>0</PrimitiveValue>
        <Expression>array[0]</Expression>
        <Name>[0]</Name>
        <IsNull>False</IsNull>
        <AsString>0</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="array[1] = 1">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>True</IsInteger>
        <PrimitiveValue>1</PrimitiveValue>
        <Expression>array[1]</Expression>
        <Name>[1]</Name>
        <IsNull>False</IsNull>
        <AsString>1</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="array[2] = 2">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>True</IsInteger>
        <PrimitiveValue>2</PrimitiveValue>
        <Expression>array[2]</Expression>
        <Name>[2]</Name>
        <IsNull>False</IsNull>
        <AsString>2</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="array[3] = 3">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>True</IsInteger>
        <PrimitiveValue>3</PrimitiveValue>
        <Expression>array[3]</Expression>
        <Name>[3]</Name>
        <IsNull>False</IsNull>
        <AsString>3</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="array[4] = 4">
        <IsArray>False</IsArray>
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <ArrayDimensions exception="Value is not an array" />
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <IsInteger>True</IsInteger>
        <PrimitiveValue>4</PrimitiveValue>
        <Expression>array[4]</Expression>
        <Name>[4]</Name>
        <IsNull>False</IsNull>
        <AsString>4</AsString>
        <HasExpired>False</HasExpired>
        <Type>System.Int32</Type>
      </Item>
    </Object>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT