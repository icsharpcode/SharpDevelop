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
        IsArray="False"
        IsInteger="False"
        IsInvalid="False"
        IsNull="False"
        IsObject="True"
        IsPrimitive="False"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="Debugger.Tests.ValueType">
        <Type>
          <DebugType
            BaseType="System.ValueType"
            FullName="Debugger.Tests.ValueType"
            HasElementType="False"
            Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
            IsArray="False"
            IsClass="False"
            IsGenericType="False"
            IsInteger="False"
            IsInterface="False"
            IsPrimitive="False"
            IsValueType="True"
            ManagedType="null"
            Module="ValueType.exe">
            <BaseType>
              <DebugType
                BaseType="System.Object"
                FullName="System.ValueType"
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
        </Type>
      </Value>
    </this>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT