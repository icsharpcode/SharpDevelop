// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests
{
	public struct ValueType
	{
		public static void Main()
		{
			new ValueType().Fun();
		}
		
		public void Fun()
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
		public void ValueType()
		{
			ExpandProperties(
				"Value.Type",
				"DebugType.BaseType"
			);
			StartTest("ValueType.cs");
			
			ObjectDump("this", process.SelectedStackFrame.GetThisValue());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="ValueType.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ValueType.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <this>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLenght="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="{Debugger.Tests.ValueType}"
        Expression="this"
        IsInvalid="False"
        IsNull="False"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="Debugger.Tests.ValueType">
        <Type>
          <DebugType
            BaseType="System.ValueType"
            ElementType="null"
            FullName="Debugger.Tests.ValueType"
            GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
            Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
            IsArray="False"
            IsClass="False"
            IsInteger="False"
            IsInterface="False"
            IsPointer="False"
            IsPrimitive="False"
            IsString="False"
            IsValueType="True"
            IsVoid="False"
            Module="ValueType.exe">
            <BaseType>
              <DebugType
                BaseType="System.Object"
                ElementType="null"
                FullName="System.ValueType"
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
            </BaseType>
          </DebugType>
        </Type>
      </Value>
    </this>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT