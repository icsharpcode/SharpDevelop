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
			ExpandProperties(
				"DebugType.BaseType"
			);
			StartTest("ArrayValue.cs");
			
			Value array = process.SelectedStackFrame.GetLocalVariableValue("array");
			ObjectDump("array", array);
			ObjectDump("array elements", array.GetArrayElements());
			ObjectDump("type", array.Type);
			ObjectDump("array.Length", array.GetMemberValue("Length"));
			
			EndTest();
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
    <array Type="Value" ToString="array = {System.Int32[]}">
      <ArrayDimensions>[5]</ArrayDimensions>
      <ArrayLenght>5</ArrayLenght>
      <ArrayRank>1</ArrayRank>
      <AsString>{System.Int32[]}</AsString>
      <Expression>array</Expression>
      <HasExpired>False</HasExpired>
      <IsArray>True</IsArray>
      <IsInteger>False</IsInteger>
      <IsNull>False</IsNull>
      <IsObject>False</IsObject>
      <IsPrimitive>False</IsPrimitive>
      <PrimitiveValue exception="Value is not a primitive type" />
      <Type>System.Int32[]</Type>
    </array>
    <array_elements Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="array[0] = 0">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>0</AsString>
        <Expression>array[0]</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>0</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="array[1] = 1">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>1</AsString>
        <Expression>array[1]</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>1</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="array[2] = 2">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>2</AsString>
        <Expression>array[2]</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>2</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="array[3] = 3">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>3</AsString>
        <Expression>array[3]</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>3</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="array[4] = 4">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>4</AsString>
        <Expression>array[4]</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>4</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
    </array_elements>
    <type Type="DebugType" ToString="System.Int32[]">
      <BaseType Type="DebugType" ToString="System.Array">
        <BaseType Type="DebugType" ToString="System.Object">
          <BaseType>null</BaseType>
          <FullName>System.Object</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>True</IsClass>
          <IsGenericType>False</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>False</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>null</ManagedType>
          <Module>mscorlib.dll</Module>
        </BaseType>
        <FullName>System.Array</FullName>
        <HasElementType>False</HasElementType>
        <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
        <IsArray>False</IsArray>
        <IsClass>True</IsClass>
        <IsGenericType>False</IsGenericType>
        <IsInteger>False</IsInteger>
        <IsInterface>False</IsInterface>
        <IsPrimitive>False</IsPrimitive>
        <IsValueType>False</IsValueType>
        <ManagedType>null</ManagedType>
        <Module>mscorlib.dll</Module>
      </BaseType>
      <FullName>System.Int32[]</FullName>
      <HasElementType>True</HasElementType>
      <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
      <IsArray>True</IsArray>
      <IsClass>False</IsClass>
      <IsGenericType>False</IsGenericType>
      <IsInteger>False</IsInteger>
      <IsInterface>False</IsInterface>
      <IsPrimitive>False</IsPrimitive>
      <IsValueType>False</IsValueType>
      <ManagedType>System.Array</ManagedType>
      <Module exception="The type is not a class or value type." />
    </type>
    <array.Length Type="Value" ToString="array.Length = 5">
      <ArrayDimensions exception="Value is not an array" />
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <AsString>5</AsString>
      <Expression>array.Length</Expression>
      <HasExpired>False</HasExpired>
      <IsArray>False</IsArray>
      <IsInteger>True</IsInteger>
      <IsNull>False</IsNull>
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <PrimitiveValue>5</PrimitiveValue>
      <Type>System.Int32</Type>
    </array.Length>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT