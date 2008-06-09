// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class PrimitiveValue
	{
		public static void Main()
		{
			bool b = true;
			int i = 5;
			string s = "five";
			double d = 5.5;
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void PrimitiveValue()
		{
			ExpandProperties(
				"Value.Type",
				"DebugType.BaseType"
			);
			StartTest("PrimitiveValue.cs");
			
			ObjectDump("locals", process.SelectedStackFrame.GetLocalVariableValues());
			// Test System.Object access
			ObjectDump("b as string", process.SelectedStackFrame.GetLocalVariableValue("b").InvokeToString());
			ObjectDump("i as string", process.SelectedStackFrame.GetLocalVariableValue("i").InvokeToString());
			ObjectDump("s as string", process.SelectedStackFrame.GetLocalVariableValue("s").InvokeToString());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="PrimitiveValue.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">PrimitiveValue.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <locals Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="b = True">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>True</AsString>
        <Expression>b</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>True</PrimitiveValue>
        <Type Type="DebugType" ToString="System.Boolean">
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
          <FullName>System.Boolean</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>False</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>True</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>System.Boolean</ManagedType>
          <Module exception="The type is not a class or value type." />
        </Type>
      </Item>
      <Item Type="Value" ToString="i = 5">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>5</AsString>
        <Expression>i</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>5</PrimitiveValue>
        <Type Type="DebugType" ToString="System.Int32">
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
          <FullName>System.Int32</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>False</IsGenericType>
          <IsInteger>True</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>True</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>System.Int32</ManagedType>
          <Module exception="The type is not a class or value type." />
        </Type>
      </Item>
      <Item Type="Value" ToString="s = five">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>five</AsString>
        <Expression>s</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>five</PrimitiveValue>
        <Type Type="DebugType" ToString="System.String">
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
          <FullName>System.String</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>False</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>True</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>System.String</ManagedType>
          <Module exception="The type is not a class or value type." />
        </Type>
      </Item>
      <Item Type="Value" ToString="d = 5.5">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>5.5</AsString>
        <Expression>d</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>5.5</PrimitiveValue>
        <Type Type="DebugType" ToString="System.Double">
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
          <FullName>System.Double</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>False</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>True</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>System.Double</ManagedType>
          <Module exception="The type is not a class or value type." />
        </Type>
      </Item>
    </locals>
    <b_as_string>True</b_as_string>
    <i_as_string>5</i_as_string>
    <s_as_string>five</s_as_string>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT