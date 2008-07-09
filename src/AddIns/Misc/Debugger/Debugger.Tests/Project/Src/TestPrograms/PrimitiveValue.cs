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
    <locals
      Capacity="4"
      Count="4">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="True"
          Expression="b"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="True"
          Type="System.Boolean">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="System.Boolean"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="False"
              IsPrimitive="True"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <BaseType>
                <DebugType
                  BaseType="null"
                  ElementType="null"
                  FullName="System.Object"
                  GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="True"
                  IsInteger="False"
                  IsInterface="False"
                  IsPointer="False"
                  IsPrimitive="False"
                  IsString="False"
                  IsValueType="False"
                  IsVoid="False"
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
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="5"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="System.Int32"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="True"
              IsInterface="False"
              IsPointer="False"
              IsPrimitive="True"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <BaseType>
                <DebugType
                  BaseType="null"
                  ElementType="null"
                  FullName="System.Object"
                  GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="True"
                  IsInteger="False"
                  IsInterface="False"
                  IsPointer="False"
                  IsPrimitive="False"
                  IsString="False"
                  IsValueType="False"
                  IsVoid="False"
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
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="five"
          Type="System.String">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="System.String"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="False"
              IsPrimitive="True"
              IsString="True"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <BaseType>
                <DebugType
                  BaseType="null"
                  ElementType="null"
                  FullName="System.Object"
                  GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="True"
                  IsInteger="False"
                  IsInterface="False"
                  IsPointer="False"
                  IsPrimitive="False"
                  IsString="False"
                  IsValueType="False"
                  IsVoid="False"
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
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="5.5"
          Type="System.Double">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="System.Double"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="False"
              IsPrimitive="True"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <BaseType>
                <DebugType
                  BaseType="null"
                  ElementType="null"
                  FullName="System.Object"
                  GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="True"
                  IsInteger="False"
                  IsInterface="False"
                  IsPointer="False"
                  IsPrimitive="False"
                  IsString="False"
                  IsValueType="False"
                  IsVoid="False"
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