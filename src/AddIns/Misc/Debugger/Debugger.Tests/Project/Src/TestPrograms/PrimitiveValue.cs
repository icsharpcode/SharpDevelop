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
          <BaseType exception="Value does not fall within the expected range." />
          <FullName>System.Boolean</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>False</IsGenericType>
          <IsInteger>False</IsInteger>
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
          <BaseType exception="Value does not fall within the expected range." />
          <FullName>System.Int32</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>False</IsGenericType>
          <IsInteger>True</IsInteger>
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
          <BaseType exception="Value does not fall within the expected range." />
          <FullName>System.String</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>False</IsGenericType>
          <IsInteger>False</IsInteger>
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
          <BaseType exception="Value does not fall within the expected range." />
          <FullName>System.Double</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>False</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsPrimitive>True</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>System.Double</ManagedType>
          <Module exception="The type is not a class or value type." />
        </Type>
      </Item>
    </locals>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT