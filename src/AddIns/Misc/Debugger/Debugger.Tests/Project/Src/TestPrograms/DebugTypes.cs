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
		public struct Point {
			int x;
			int y;
		}
		
		public static unsafe void Main()
		{
			object nullObject = null;
			string nullString = null;
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
				"DebubType.BaseType",
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
    <DebuggingPaused>Break</DebuggingPaused>
    <LocalVariables
      Capacity="16"
      Count="15">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is null}"
          ArrayLenght="{Exception: Value is null}"
          ArrayRank="{Exception: Value is null}"
          AsString="null"
          Expression="nullObject"
          IsInvalid="False"
          IsNull="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
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
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is null}"
          ArrayLenght="{Exception: Value is null}"
          ArrayRank="{Exception: Value is null}"
          AsString="null"
          Expression="nullString"
          IsInvalid="False"
          IsNull="True"
          PrimitiveValue="null"
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
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="42"
          Expression="loc"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="42"
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
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="43"
          Expression="locByRef"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="43"
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
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0x0012F470"
          Expression="locPtr"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
              ElementType="System.Int32"
              FullName="System.Int32*"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="True"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0x0012F46C"
          Expression="locPtrByRef"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
              ElementType="System.Int32"
              FullName="System.Int32*"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="True"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0x0012F468"
          Expression="locPtrPtr"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32**">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
              ElementType="System.Int32*"
              FullName="System.Int32**"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="True"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
                <DebugType
                  BaseType="{Exception: Value does not fall within the expected range.}"
                  ElementType="System.Int32"
                  FullName="System.Int32*"
                  GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="False"
                  IsInteger="False"
                  IsInterface="False"
                  IsPointer="True"
                  IsPrimitive="False"
                  IsString="False"
                  IsValueType="False"
                  IsVoid="False"
                  Module="{Exception: The type is not a class or value type.}">
                  <ElementType>
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0x0012F464"
          Expression="locVoidPtr"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Void*">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
              ElementType="System.Void"
              FullName="System.Void*"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="True"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
                <DebugType
                  BaseType="{Exception: Value does not fall within the expected range.}"
                  ElementType="null"
                  FullName="System.Void"
                  GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="False"
                  IsInteger="False"
                  IsInterface="False"
                  IsPointer="False"
                  IsPrimitive="False"
                  IsString="False"
                  IsValueType="False"
                  IsVoid="True"
                  Module="{Exception: The type is not a class or value type.}">
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Object}"
          Expression="locObj"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
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
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Object}"
          Expression="locObjByRef"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
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
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="[4]"
          ArrayLenght="4"
          ArrayRank="1"
          AsString="{System.Char[]}"
          Expression="locSZArray"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[]"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="True"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="False"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Char"
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
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="[2, 2]"
          ArrayLenght="4"
          ArrayRank="2"
          AsString="{System.Char[,]}"
          Expression="locArray"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[,]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[,]"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="True"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="False"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Char"
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{Point}"
          Expression="locStruct"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="Point"
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
              Module="DebugTypes.exe">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0x0012F454"
          Expression="locStructPtr"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point*">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
              ElementType="Point"
              FullName="Point*"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="True"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
                <DebugType
                  BaseType="System.ValueType"
                  ElementType="null"
                  FullName="Point"
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
                  Module="DebugTypes.exe">
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32}"
          Expression="box"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="System.Int32"
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
              Module="mscorlib.dll">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
    </LocalVariables>
    <DebuggingPaused>Break</DebuggingPaused>
    <Arguments
      Capacity="16"
      Count="15">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="42"
          Expression="arg"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="42"
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
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="43"
          Expression="argByRef"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="43"
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
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0x0012F424"
          Expression="argPtr"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
              ElementType="System.Int32"
              FullName="System.Int32*"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="True"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0x0012F46C"
          Expression="argPtrByRef"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
              ElementType="System.Int32"
              FullName="System.Int32*"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="True"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0x0012F41C"
          Expression="argPtrPtr"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32**">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
              ElementType="System.Int32*"
              FullName="System.Int32**"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="True"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
                <DebugType
                  BaseType="{Exception: Value does not fall within the expected range.}"
                  ElementType="System.Int32"
                  FullName="System.Int32*"
                  GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="False"
                  IsInteger="False"
                  IsInterface="False"
                  IsPointer="True"
                  IsPrimitive="False"
                  IsString="False"
                  IsValueType="False"
                  IsVoid="False"
                  Module="{Exception: The type is not a class or value type.}">
                  <ElementType>
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0x0012F418"
          Expression="argVoidPtr"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Void*">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
              ElementType="System.Void"
              FullName="System.Void*"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="True"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
                <DebugType
                  BaseType="{Exception: Value does not fall within the expected range.}"
                  ElementType="null"
                  FullName="System.Void"
                  GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
                  IsArray="False"
                  IsClass="False"
                  IsInteger="False"
                  IsInterface="False"
                  IsPointer="False"
                  IsPrimitive="False"
                  IsString="False"
                  IsValueType="False"
                  IsVoid="True"
                  Module="{Exception: The type is not a class or value type.}">
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Object}"
          Expression="argObj"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
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
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Object}"
          Expression="argObjByRef"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
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
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="[4]"
          ArrayLenght="4"
          ArrayRank="1"
          AsString="{System.Char[]}"
          Expression="argSZArray"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[]"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="True"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="False"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Char"
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
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="[2, 2]"
          ArrayLenght="4"
          ArrayRank="2"
          AsString="{System.Char[,]}"
          Expression="argArray"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[,]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[,]"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="True"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="False"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  ElementType="null"
                  FullName="System.Char"
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{Point}"
          Expression="argStruct"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="Point"
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
              Module="DebugTypes.exe">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{Point}"
          Expression="argStructByRef"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="Point"
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
              Module="DebugTypes.exe">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0x0012F3F8"
          Expression="argStructPtr"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point*">
          <Type>
            <DebugType
              BaseType="{Exception: Value does not fall within the expected range.}"
              ElementType="Point"
              FullName="Point*"
              GenericArguments="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              Interfaces="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]"
              IsArray="False"
              IsClass="False"
              IsInteger="False"
              IsInterface="False"
              IsPointer="True"
              IsPrimitive="False"
              IsString="False"
              IsValueType="False"
              IsVoid="False"
              Module="{Exception: The type is not a class or value type.}">
              <ElementType>
                <DebugType
                  BaseType="System.ValueType"
                  ElementType="null"
                  FullName="Point"
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
                  Module="DebugTypes.exe">
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
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32}"
          Expression="argBox"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="System.Int32"
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
              Module="mscorlib.dll">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLenght="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Int32}"
          Expression="argBoxByRef"
          IsInvalid="False"
          IsNull="False"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              ElementType="null"
              FullName="System.Int32"
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
              Module="mscorlib.dll">
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