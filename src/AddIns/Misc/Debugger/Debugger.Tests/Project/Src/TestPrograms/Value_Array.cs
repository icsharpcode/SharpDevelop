// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Value_Array
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
		public void Value_Array()
		{
			ExpandProperties(
				"DebugType.BaseType"
			);
			StartTest("Value_Array.cs");
			
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
    name="Value_Array.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Value_Array.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Value_Array.cs:20,4-20,40</DebuggingPaused>
    <array>
      <Value
        ArrayDimensions="{5}"
        ArrayLength="5"
        ArrayRank="1"
        AsString="{System.Int32[]}"
        Expression="array"
        IsReference="True"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="System.Int32[]" />
    </array>
    <array_elements>
      <Item>
        <Value
          AsString="0"
          Expression="(array)[(System.Int32)0]"
          PrimitiveValue="0"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="1"
          Expression="(array)[(System.Int32)1]"
          PrimitiveValue="1"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="2"
          Expression="(array)[(System.Int32)2]"
          PrimitiveValue="2"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="3"
          Expression="(array)[(System.Int32)3]"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="4"
          Expression="(array)[(System.Int32)4]"
          PrimitiveValue="4"
          Type="System.Int32" />
      </Item>
    </array_elements>
    <type>
      <DebugType
        BaseType="System.Array"
        ElementType="System.Int32"
        FullName="System.Int32[]"
        Kind="Array"
        Module="{Exception: The type is not a class or value type.}"
        Name="Int32[]">
        <BaseType>
          <DebugType
            BaseType="System.Object"
            FullName="System.Array"
            Interfaces="{System.ICloneable, System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable}"
            Kind="Class"
            Module="mscorlib.dll"
            Name="Array">
            <BaseType>
              <DebugType
                FullName="System.Object"
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
        AsString="5"
        Expression="((System.Array)(array)).Length"
        PrimitiveValue="5"
        Type="System.Int32" />
    </array.Length>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT