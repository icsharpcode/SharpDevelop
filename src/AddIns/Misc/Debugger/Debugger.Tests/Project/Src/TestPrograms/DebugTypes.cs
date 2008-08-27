// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class DebugTypes
	{
		public class MyClass
		{
			
		}
		
		public interface MyInterface
		{
			
		}
		
		public class MyInterfaceImpl: MyInterface
		{
			
		}
		
		public struct Point {
			int x;
			int y;
		}
		
		public static unsafe void Main()
		{
			// The nulls should be first to test for "Value does not fall within the expected range." exception of .BaseType
			MyClass nullMyClass = null;
			object nullObject = null;
			string nullString = null;
			object obj = new Object();
			int loc = 42;
			int locByRef = 43;
			int* locPtr = &loc;
			int* locPtrByRef = &loc;
			int** locPtrPtr = &locPtr;
			void* locVoidPtr = &loc;
			object locObj = new object();
			object locObjByRef = new object();
			char[] locSZArray = "Test".ToCharArray();
			char[,] locArray = new char[2,2];
			Point locStruct;
			Point* locStructPtr = &locStruct;
			object box = 40;
			MyInterfaceImpl myInterfaceImpl = new MyInterfaceImpl();
			MyInterface myInterface = myInterfaceImpl;
			
			System.Diagnostics.Debugger.Break();
			
			Fun(loc, ref locByRef, locPtr, ref locPtrByRef,
			    locPtrPtr, locVoidPtr,
			    locObj, ref locObjByRef,
			    locSZArray, locArray,
			    locStruct, ref locStruct, locStructPtr,
			    box, ref box);
		}
		
		static unsafe void Fun(int arg, ref int argByRef, int* argPtr, ref int* argPtrByRef,
		                       int** argPtrPtr, void* argVoidPtr,
		                       object argObj, ref object argObjByRef,
		                       char[] argSZArray, char[,] argArray,
		                       Point argStruct, ref Point argStructByRef, Point* argStructPtr,
		                       object argBox, ref object argBoxByRef)
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
		public void DebugTypes()
		{
			ExpandProperties(
				"Value.Type",
				"DebugType.ElementType"
			);
			StartTest("DebugTypes.cs");
			ObjectDump("LocalVariables", process.SelectedStackFrame.GetLocalVariableValues());
			process.Continue();
			ObjectDump("Arguments", process.SelectedStackFrame.GetArgumentValues());
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="DebugTypes.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>DebugTypes.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break DebugTypes.cs:57,4-57,40</DebuggingPaused>
    <LocalVariables
      Capacity="32"
      Count="19">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is null}"
          ArrayLength="{Exception: Value is null}"
          ArrayRank="{Exception: Value is null}"
          AsString="null"
          Expression="nullMyClass"
          IsInvalid="False"
          IsNull="True"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="MyClass">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="MyClass"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Class"
              Module="DebugTypes.exe"
              Name="MyClass">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is null}"
          ArrayLength="{Exception: Value is null}"
          ArrayRank="{Exception: Value is null}"
          AsString="null"
          Expression="nullObject"
          IsInvalid="False"
          IsNull="True"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="null"
              FullName="System.Object"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Class"
              Module="mscorlib.dll"
              Name="Object">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is null}"
          ArrayLength="{Exception: Value is null}"
          ArrayRank="{Exception: Value is null}"
          AsString="null"
          Expression="nullString"
          IsInvalid="False"
          IsNull="True"
          IsReference="True"
          PrimitiveValue="null"
          Type="System.String">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="System.String"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Primitive"
              Module="{Exception: The type is not a class or value type.}"
              Name="String">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Object}"
          Expression="obj"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="null"
              FullName="System.Object"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Class"
              Module="mscorlib.dll"
              Name="Object">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="42"
          Expression="loc"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="42"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="System.Int32"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Primitive"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="43"
          Expression="locByRef"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="43"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="System.Int32"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Primitive"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32*}"
          Expression="locPtr"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="System.Int32"
              FullName="System.Int32*"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32*">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Int32"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Primitive"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Int32">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32*}"
          Expression="locPtrByRef"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="System.Int32"
              FullName="System.Int32*"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32*">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Int32"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Primitive"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Int32">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32**}"
          Expression="locPtrPtr"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32**">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="System.Int32*"
              FullName="System.Int32**"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32**">
              <ElementType>
                <DebugType
                  BaseType="null"
                  ElementType="System.Int32"
                  FullName="System.Int32*"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Pointer"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Int32*">
                  <ElementType>
                    <DebugType
                      BaseType="System.Object"
                      ElementType="null"
                      FullName="System.Int32"
                      GenericArguments="{}"
                      Interfaces="{}"
                      Kind="Primitive"
                      Module="{Exception: The type is not a class or value type.}"
                      Name="Int32">
                      <ElementType>null</ElementType>
                    </DebugType>
                  </ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Void*}"
          Expression="locVoidPtr"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Void*">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="System.Void"
              FullName="System.Void*"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Void*">
              <ElementType>
                <DebugType
                  BaseType="null"
                  ElementType="null"
                  FullName="System.Void"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Void"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Void">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Object}"
          Expression="locObj"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="null"
              FullName="System.Object"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Class"
              Module="mscorlib.dll"
              Name="Object">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Object}"
          Expression="locObjByRef"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="null"
              FullName="System.Object"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Class"
              Module="mscorlib.dll"
              Name="Object">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{4}"
          ArrayLength="4"
          ArrayRank="1"
          AsString="{System.Char[]}"
          Expression="locSZArray"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[]"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Array"
              Module="{Exception: The type is not a class or value type.}"
              Name="Char[]">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Char"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Primitive"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Char">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{2, 2}"
          ArrayLength="4"
          ArrayRank="2"
          AsString="{System.Char[,]}"
          Expression="locArray"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[,]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[,]"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Array"
              Module="{Exception: The type is not a class or value type.}"
              Name="Char[,]">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Char"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Primitive"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Char">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{Point}"
          Expression="locStruct"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="Point"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="ValueType"
              Module="DebugTypes.exe"
              Name="Point">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{Point*}"
          Expression="locStructPtr"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point*">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="Point"
              FullName="Point*"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Point*">
              <ElementType>
                <DebugType
                  BaseType="System.ValueType"
                  ElementType="null"
                  FullName="Point"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="ValueType"
                  Module="DebugTypes.exe"
                  Name="Point">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32}"
          Expression="box"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="40"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="System.Int32"
              GenericArguments="{}"
              Interfaces="{System.IComparable, System.IFormattable, System.IConvertible}"
              Kind="ValueType"
              Module="mscorlib.dll"
              Name="Int32">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{MyInterfaceImpl}"
          Expression="myInterfaceImpl"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="MyInterfaceImpl">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="MyInterfaceImpl"
              GenericArguments="{}"
              Interfaces="{MyInterface}"
              Kind="Class"
              Module="DebugTypes.exe"
              Name="MyInterfaceImpl">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{MyInterfaceImpl}"
          Expression="myInterface"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="MyInterfaceImpl">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="MyInterfaceImpl"
              GenericArguments="{}"
              Interfaces="{MyInterface}"
              Kind="Class"
              Module="DebugTypes.exe"
              Name="MyInterfaceImpl">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
    </LocalVariables>
    <DebuggingPaused>Break DebugTypes.cs:74,4-74,40</DebuggingPaused>
    <Arguments
      Capacity="16"
      Count="15">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="42"
          Expression="arg"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="42"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="System.Int32"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Primitive"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="43"
          Expression="argByRef"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="43"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              ElementType="null"
              FullName="System.Int32"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Primitive"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32*}"
          Expression="argPtr"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="System.Int32"
              FullName="System.Int32*"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32*">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Int32"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Primitive"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Int32">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32*}"
          Expression="argPtrByRef"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="System.Int32"
              FullName="System.Int32*"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32*">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Int32"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Primitive"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Int32">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32**}"
          Expression="argPtrPtr"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32**">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="System.Int32*"
              FullName="System.Int32**"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32**">
              <ElementType>
                <DebugType
                  BaseType="null"
                  ElementType="System.Int32"
                  FullName="System.Int32*"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Pointer"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Int32*">
                  <ElementType>
                    <DebugType
                      BaseType="System.Object"
                      ElementType="null"
                      FullName="System.Int32"
                      GenericArguments="{}"
                      Interfaces="{}"
                      Kind="Primitive"
                      Module="{Exception: The type is not a class or value type.}"
                      Name="Int32">
                      <ElementType>null</ElementType>
                    </DebugType>
                  </ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Void*}"
          Expression="argVoidPtr"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Void*">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="System.Void"
              FullName="System.Void*"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Void*">
              <ElementType>
                <DebugType
                  BaseType="null"
                  ElementType="null"
                  FullName="System.Void"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Void"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Void">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Object}"
          Expression="argObj"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="null"
              FullName="System.Object"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Class"
              Module="mscorlib.dll"
              Name="Object">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Object}"
          Expression="argObjByRef"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="null"
              FullName="System.Object"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Class"
              Module="mscorlib.dll"
              Name="Object">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{4}"
          ArrayLength="4"
          ArrayRank="1"
          AsString="{System.Char[]}"
          Expression="argSZArray"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[]"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Array"
              Module="{Exception: The type is not a class or value type.}"
              Name="Char[]">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Char"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Primitive"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Char">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{2, 2}"
          ArrayLength="4"
          ArrayRank="2"
          AsString="{System.Char[,]}"
          Expression="argArray"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[,]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[,]"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Array"
              Module="{Exception: The type is not a class or value type.}"
              Name="Char[,]">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Char"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="Primitive"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Char">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{Point}"
          Expression="argStruct"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="Point"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="ValueType"
              Module="DebugTypes.exe"
              Name="Point">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{Point}"
          Expression="argStructByRef"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="Point"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="ValueType"
              Module="DebugTypes.exe"
              Name="Point">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{Point*}"
          Expression="argStructPtr"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point*">
          <Type>
            <DebugType
              BaseType="null"
              ElementType="Point"
              FullName="Point*"
              GenericArguments="{}"
              Interfaces="{}"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Point*">
              <ElementType>
                <DebugType
                  BaseType="System.ValueType"
                  ElementType="null"
                  FullName="Point"
                  GenericArguments="{}"
                  Interfaces="{}"
                  Kind="ValueType"
                  Module="DebugTypes.exe"
                  Name="Point">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32}"
          Expression="argBox"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="40"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="System.Int32"
              GenericArguments="{}"
              Interfaces="{System.IComparable, System.IFormattable, System.IConvertible}"
              Kind="ValueType"
              Module="mscorlib.dll"
              Name="Int32">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32}"
          Expression="argBoxByRef"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="40"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="System.Int32"
              GenericArguments="{}"
              Interfaces="{System.IComparable, System.IFormattable, System.IConvertible}"
              Kind="ValueType"
              Module="mscorlib.dll"
              Name="Int32">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
    </Arguments>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT