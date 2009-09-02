// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger.Tests.TestPrograms
{
	public class DebugType
	{
		public unsafe class MyClass
		{
			public delegate int Add(byte b1, byte b2);
			
			public int i;
			public bool b;
			public char c;
			public IntPtr intPtr;
			public int* pInt;
			public void* pVoid;
			public int[] intArray;
			public int[,] intMultiArray;
			public List<int> intList;
			public List<int>[] intListArray;
			public Point point;
			public MyClass myClass;
			public Add fnPtr;
			
			public T Foo<T>(ref T tRef, T[] tArray) { return tRef; }
		}
		
		public interface MyInterface<K, V, T>
		{
			K ItemKey { get; }
		}
		
		public class MyInterfaceImpl<T> : MyInterface<string, IEnumerable<int>, MyClass>
		{
			public T Item { get { return default(T); } }
			public string ItemKey { get { return null; } }
		}
		
		public struct Point {
			int x;
			int y;
		}
		
		public static unsafe void Main()
		{
			// TODO
			// int? iNull = 5;
			// int? iNull_null = null;
			
			// The nulls should be first to test for "Value does not fall within the expected range." exception of .BaseType
			MyClass nullMyClass = null;
			object nullObject = null;
			string nullString = null;
			object obj = new Object();
			MyClass myClass = new MyClass();
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
			MyInterfaceImpl<int> myInterfaceImpl = new MyInterfaceImpl<int>();
			MyInterface<string, IEnumerable<int>, MyClass> myInterface = myInterfaceImpl;
			
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
		public void DebugType()
		{
			ExpandProperties(
				"Value.Type",
				"DebugType.ElementType"
			);
			StartTest("DebugType.cs");
			ObjectDump("MyClassMemberts", process.SelectedStackFrame.GetLocalVariableValue("myClass").Type.GetMembers());
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
    name="DebugType.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>DebugType.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break DebugType.cs:80,4-80,40</DebuggingPaused>
    <MyClassMemberts
      Capacity="16"
      Count="15">
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.i"
          IsPublic="True"
          Module="DebugType.exe"
          Name="i"
          Type="System.Int32" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.b"
          IsPublic="True"
          Module="DebugType.exe"
          Name="b"
          Type="System.Boolean" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.c"
          IsPublic="True"
          Module="DebugType.exe"
          Name="c"
          Type="System.Char" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.intPtr"
          IsPublic="True"
          Module="DebugType.exe"
          Name="intPtr"
          Type="System.IntPtr" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.pInt"
          IsPublic="True"
          Module="DebugType.exe"
          Name="pInt"
          Type="System.Int32*" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.pVoid"
          IsPublic="True"
          Module="DebugType.exe"
          Name="pVoid"
          Type="System.Void*" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.intArray"
          IsPublic="True"
          Module="DebugType.exe"
          Name="intArray"
          Type="System.Int32[]" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.intMultiArray"
          IsPublic="True"
          Module="DebugType.exe"
          Name="intMultiArray"
          Type="System.Int32[,]" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.intList"
          IsPublic="True"
          Module="DebugType.exe"
          Name="intList"
          Type="System.Collections.Generic.List&lt;System.Int32&gt;" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.intListArray"
          IsPublic="True"
          Module="DebugType.exe"
          Name="intListArray"
          Type="System.Collections.Generic.List&lt;System.Int32&gt;[]" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.point"
          IsPublic="True"
          Module="DebugType.exe"
          Name="point"
          Type="Point" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.myClass"
          IsPublic="True"
          Module="DebugType.exe"
          Name="myClass"
          Type="MyClass" />
      </Item>
      <Item>
        <FieldInfo
          DeclaringType="MyClass"
          FullName="MyClass.fnPtr"
          IsPublic="True"
          Module="DebugType.exe"
          Name="fnPtr"
          Type="Add" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="MyClass"
          FullName="MyClass.Foo"
          IsPublic="True"
          Module="DebugType.exe"
          Name="Foo"
          ParameterCount="2"
          ParameterTypes="{System.Object, System.Object[]}"
          ReturnType="System.Object" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="MyClass"
          FullName="MyClass..ctor"
          IsPublic="True"
          IsSpecialName="True"
          Module="DebugType.exe"
          Name=".ctor"
          StepOver="True" />
      </Item>
    </MyClassMemberts>
    <LocalVariables
      Capacity="32"
      Count="20">
      <Item>
        <Value
          AsString="null"
          Expression="nullMyClass"
          IsNull="True"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="MyClass">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="MyClass"
              Kind="Class"
              Module="DebugType.exe"
              Name="MyClass">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          AsString="null"
          Expression="nullObject"
          IsNull="True"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              FullName="System.Object"
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
          AsString="null"
          Expression="nullString"
          IsNull="True"
          IsReference="True"
          Type="System.String">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.String"
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
          AsString="{System.Object}"
          Expression="obj"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              FullName="System.Object"
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
          AsString="{MyClass}"
          Expression="myClass"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="MyClass">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="MyClass"
              Kind="Class"
              Module="DebugType.exe"
              Name="MyClass">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          AsString="42"
          Expression="loc"
          PrimitiveValue="42"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Int32"
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
          AsString="43"
          Expression="locByRef"
          PrimitiveValue="43"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Int32"
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
          AsString="{System.Int32*}"
          Expression="locPtr"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              ElementType="System.Int32"
              FullName="System.Int32*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32*">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  FullName="System.Int32"
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
          AsString="{System.Int32*}"
          Expression="locPtrByRef"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              ElementType="System.Int32"
              FullName="System.Int32*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32*">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  FullName="System.Int32"
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
          AsString="{System.Int32**}"
          Expression="locPtrPtr"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32**">
          <Type>
            <DebugType
              ElementType="System.Int32*"
              FullName="System.Int32**"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32**">
              <ElementType>
                <DebugType
                  ElementType="System.Int32"
                  FullName="System.Int32*"
                  Kind="Pointer"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Int32*">
                  <ElementType>
                    <DebugType
                      BaseType="System.Object"
                      FullName="System.Int32"
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
          AsString="{System.Void*}"
          Expression="locVoidPtr"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Void*">
          <Type>
            <DebugType
              ElementType="System.Void"
              FullName="System.Void*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Void*">
              <ElementType>
                <DebugType
                  FullName="System.Void"
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
          AsString="{System.Object}"
          Expression="locObj"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              FullName="System.Object"
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
          AsString="{System.Object}"
          Expression="locObjByRef"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              FullName="System.Object"
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
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[]"
              Kind="Array"
              Module="{Exception: The type is not a class or value type.}"
              Name="Char[]">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  FullName="System.Char"
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
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[,]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[,]"
              Kind="Array"
              Module="{Exception: The type is not a class or value type.}"
              Name="Char[,]">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  FullName="System.Char"
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
          AsString="{Point}"
          Expression="locStruct"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              FullName="Point"
              Kind="ValueType"
              Module="DebugType.exe"
              Name="Point">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          AsString="{Point*}"
          Expression="locStructPtr"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point*">
          <Type>
            <DebugType
              ElementType="Point"
              FullName="Point*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Point*">
              <ElementType>
                <DebugType
                  BaseType="System.ValueType"
                  FullName="Point"
                  Kind="ValueType"
                  Module="DebugType.exe"
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
          AsString="{System.Int32}"
          Expression="box"
          IsReference="True"
          PrimitiveValue="40"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Int32"
              Interfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable&lt;System.Int32&gt;, System.IEquatable&lt;System.Int32&gt;}"
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
          AsString="{MyInterfaceImpl&lt;System.Int32&gt;}"
          Expression="myInterfaceImpl"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="MyInterfaceImpl&lt;System.Int32&gt;">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="MyInterfaceImpl&lt;System.Int32&gt;"
              GenericArguments="{System.Int32}"
              Interfaces="{MyInterface&lt;System.String,System.Collections.Generic.IEnumerable&lt;System.Int32&gt;,MyClass&gt;}"
              Kind="Class"
              Module="DebugType.exe"
              Name="MyInterfaceImpl&lt;Int32&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          AsString="{MyInterfaceImpl&lt;System.Int32&gt;}"
          Expression="myInterface"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="MyInterfaceImpl&lt;System.Int32&gt;">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="MyInterfaceImpl&lt;System.Int32&gt;"
              GenericArguments="{System.Int32}"
              Interfaces="{MyInterface&lt;System.String,System.Collections.Generic.IEnumerable&lt;System.Int32&gt;,MyClass&gt;}"
              Kind="Class"
              Module="DebugType.exe"
              Name="MyInterfaceImpl&lt;Int32&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
    </LocalVariables>
    <DebuggingPaused>Break DebugType.cs:97,4-97,40</DebuggingPaused>
    <Arguments
      Capacity="16"
      Count="15">
      <Item>
        <Value
          AsString="42"
          Expression="arg"
          PrimitiveValue="42"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Int32"
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
          AsString="43"
          Expression="argByRef"
          PrimitiveValue="43"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Int32"
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
          AsString="{System.Int32*}"
          Expression="argPtr"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              ElementType="System.Int32"
              FullName="System.Int32*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32*">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  FullName="System.Int32"
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
          AsString="{System.Int32*}"
          Expression="argPtrByRef"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32*">
          <Type>
            <DebugType
              ElementType="System.Int32"
              FullName="System.Int32*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32*">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  FullName="System.Int32"
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
          AsString="{System.Int32**}"
          Expression="argPtrPtr"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32**">
          <Type>
            <DebugType
              ElementType="System.Int32*"
              FullName="System.Int32**"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32**">
              <ElementType>
                <DebugType
                  ElementType="System.Int32"
                  FullName="System.Int32*"
                  Kind="Pointer"
                  Module="{Exception: The type is not a class or value type.}"
                  Name="Int32*">
                  <ElementType>
                    <DebugType
                      BaseType="System.Object"
                      FullName="System.Int32"
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
          AsString="{System.Void*}"
          Expression="argVoidPtr"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Void*">
          <Type>
            <DebugType
              ElementType="System.Void"
              FullName="System.Void*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Void*">
              <ElementType>
                <DebugType
                  FullName="System.Void"
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
          AsString="{System.Object}"
          Expression="argObj"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              FullName="System.Object"
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
          AsString="{System.Object}"
          Expression="argObjByRef"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object">
          <Type>
            <DebugType
              FullName="System.Object"
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
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[]"
              Kind="Array"
              Module="{Exception: The type is not a class or value type.}"
              Name="Char[]">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  FullName="System.Char"
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
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Char[,]">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="System.Char"
              FullName="System.Char[,]"
              Kind="Array"
              Module="{Exception: The type is not a class or value type.}"
              Name="Char[,]">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  FullName="System.Char"
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
          AsString="{Point}"
          Expression="argStruct"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              FullName="Point"
              Kind="ValueType"
              Module="DebugType.exe"
              Name="Point">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          AsString="{Point}"
          Expression="argStructByRef"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              FullName="Point"
              Kind="ValueType"
              Module="DebugType.exe"
              Name="Point">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </Value>
      </Item>
      <Item>
        <Value
          AsString="{Point*}"
          Expression="argStructPtr"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Point*">
          <Type>
            <DebugType
              ElementType="Point"
              FullName="Point*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Point*">
              <ElementType>
                <DebugType
                  BaseType="System.ValueType"
                  FullName="Point"
                  Kind="ValueType"
                  Module="DebugType.exe"
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
          AsString="{System.Int32}"
          Expression="argBox"
          IsReference="True"
          PrimitiveValue="40"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Int32"
              Interfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable&lt;System.Int32&gt;, System.IEquatable&lt;System.Int32&gt;}"
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
          AsString="{System.Int32}"
          Expression="argBoxByRef"
          IsReference="True"
          PrimitiveValue="40"
          Type="System.Int32">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Int32"
              Interfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable&lt;System.Int32&gt;, System.IEquatable&lt;System.Int32&gt;}"
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