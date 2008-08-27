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
    <DebuggingPaused>Break ArrayValue.cs:20,4-20,40</DebuggingPaused>
    <array>
      <Value
        ArrayDimensions="{5}"
        ArrayLength="5"
        ArrayRank="1"
        AsString="{System.Int32[]}"
        Expression="array"
        IsInvalid="False"
        IsNull="False"
        IsReference="True"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="System.Int32[]" />
    </array>
    <array_elements>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0"
          Expression="array[0]"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="0"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="1"
          Expression="array[1]"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="1"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="2"
          Expression="array[2]"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="2"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="3"
          Expression="array[3]"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="4"
          Expression="array[4]"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="4"
          Type="System.Int32" />
      </Item>
    </array_elements>
    <type>
      <DebugType
        BaseType="System.Array"
        ElementType="System.Int32"
        FullName="System.Int32[]"
        GenericArguments="{}"
        Interfaces="{}"
        Kind="Array"
        Module="{Exception: The type is not a class or value type.}"
        Name="Int32[]">
        <BaseType>
          <DebugType
            BaseType="System.Object"
            ElementType="null"
            FullName="System.Array"
            GenericArguments="{}"
            Interfaces="{System.ICloneable, System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable}"
            Kind="Class"
            Module="mscorlib.dll"
            Name="Array">
            <BaseType>
              <DebugType
                BaseType="null"
                ElementType="null"
                FullName="System.Object"
                GenericArguments="{}"
                Interfaces="{}"
                Kind="Class"
                Module="mscorlib.dll"
                Name="Object">
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
        ArrayLength="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="5"
        Expression="array.Length"
        IsInvalid="False"
        IsNull="False"
        IsReference="False"
        PrimitiveValue="5"
        Type="System.Int32" />
    </array.Length>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT