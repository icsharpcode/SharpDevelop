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
  <Test
    name="PrimitiveValue.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>PrimitiveValue.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <locals>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="True"
          Expression="b"
          IsArray="False"
          IsInteger="False"
          IsInvalid="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="True"
          Type="System.Boolean">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Boolean"
              HasElementType="False"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsGenericType="False"
              IsInteger="False"
              IsInterface="False"
              IsPrimitive="True"
              IsValueType="False"
              ManagedType="System.Boolean"
              Module="{Exception: The type is not a class or value type.}">
              <BaseType>
                <DebugType
                  BaseType="null"
                  FullName="System.Object"
                  HasElementType="False"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="True"
                  IsGenericType="False"
                  IsInteger="False"
                  IsInterface="False"
                  IsPrimitive="False"
                  IsValueType="False"
                  ManagedType="null"
                  Module="mscorlib.dll">
                  <BaseType>null</BaseType>
                </DebugType>
              </BaseType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="5"
          Expression="i"
          IsArray="False"
          IsInteger="True"
          IsInvalid="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="5"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Int32"
              HasElementType="False"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsGenericType="False"
              IsInteger="True"
              IsInterface="False"
              IsPrimitive="True"
              IsValueType="False"
              ManagedType="System.Int32"
              Module="{Exception: The type is not a class or value type.}">
              <BaseType>
                <DebugType
                  BaseType="null"
                  FullName="System.Object"
                  HasElementType="False"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="True"
                  IsGenericType="False"
                  IsInteger="False"
                  IsInterface="False"
                  IsPrimitive="False"
                  IsValueType="False"
                  ManagedType="null"
                  Module="mscorlib.dll">
                  <BaseType>null</BaseType>
                </DebugType>
              </BaseType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="five"
          Expression="s"
          IsArray="False"
          IsInteger="False"
          IsInvalid="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="five"
          Type="System.String">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.String"
              HasElementType="False"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsGenericType="False"
              IsInteger="False"
              IsInterface="False"
              IsPrimitive="True"
              IsValueType="False"
              ManagedType="System.String"
              Module="{Exception: The type is not a class or value type.}">
              <BaseType>
                <DebugType
                  BaseType="null"
                  FullName="System.Object"
                  HasElementType="False"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="True"
                  IsGenericType="False"
                  IsInteger="False"
                  IsInterface="False"
                  IsPrimitive="False"
                  IsValueType="False"
                  ManagedType="null"
                  Module="mscorlib.dll">
                  <BaseType>null</BaseType>
                </DebugType>
              </BaseType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="5.5"
          Expression="d"
          IsArray="False"
          IsInteger="False"
          IsInvalid="False"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="5.5"
          Type="System.Double">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Double"
              HasElementType="False"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsGenericType="False"
              IsInteger="False"
              IsInterface="False"
              IsPrimitive="True"
              IsValueType="False"
              ManagedType="System.Double"
              Module="{Exception: The type is not a class or value type.}">
              <BaseType>
                <DebugType
                  BaseType="null"
                  FullName="System.Object"
                  HasElementType="False"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="True"
                  IsGenericType="False"
                  IsInteger="False"
                  IsInterface="False"
                  IsPrimitive="False"
                  IsValueType="False"
                  ManagedType="null"
                  Module="mscorlib.dll">
                  <BaseType>null</BaseType>
                </DebugType>
              </BaseType>
            </DebugType>
          </Type>
        </Value>
      </Item>
    </locals>
    <b_as_string>True</b_as_string>
    <i_as_string>5</i_as_string>
    <s_as_string>five</s_as_string>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT