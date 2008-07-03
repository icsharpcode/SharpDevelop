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
  <Test
    name="ArrayValue.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ArrayValue.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <array>
      <Value
        ArrayDimensions="[5]"
        ArrayLenght="5"
        ArrayRank="1"
        AsString="{System.Int32[]}"
        Expression="array"
        HasExpired="False"
        IsArray="True"
        IsInteger="False"
        IsNull="False"
        IsObject="False"
        IsPrimitive="False"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="System.Int32[]" />
    </array>
    <array_elements>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0"
          Expression="array[0]"
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
          AsString="1"
          Expression="array[1]"
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
          AsString="2"
          Expression="array[2]"
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
          Expression="array[3]"
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
          AsString="4"
          Expression="array[4]"
          HasExpired="False"
          IsArray="False"
          IsInteger="True"
          IsNull="False"
          IsObject="False"
          IsPrimitive="True"
          PrimitiveValue="4"
          Type="System.Int32" />
      </Item>
    </array_elements>
    <type>
      <DebugType
        BaseType="System.Array"
        FullName="System.Int32[]"
        HasElementType="True"
        Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
        IsArray="True"
        IsClass="False"
        IsGenericType="False"
        IsInteger="False"
        IsInterface="False"
        IsPrimitive="False"
        IsValueType="False"
        ManagedType="System.Array"
        Module="{Exception: The type is not a class or value type.}">
        <BaseType>
          <DebugType
            BaseType="System.Object"
            FullName="System.Array"
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
        </BaseType>
      </DebugType>
    </type>
    <array.Length>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLenght="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="5"
        Expression="array.Length"
        HasExpired="False"
        IsArray="False"
        IsInteger="True"
        IsNull="False"
        IsObject="False"
        IsPrimitive="True"
        PrimitiveValue="5"
        Type="System.Int32" />
    </array.Length>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT