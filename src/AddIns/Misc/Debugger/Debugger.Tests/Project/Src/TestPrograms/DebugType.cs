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
		public delegate int AddDelegate(byte b1, byte b2);
		
		public static int Add(byte b1, byte b2)
		{
			return b1 + b2;
		}
		
		// The classes are intentionally nested for testing
		
		public class MyClass
		{
		}
		
		public class MyGenClass<T>
		{
			public class MyGenNestedClass<U>
			{
			}
		}
		
		public struct MyStruct
		{
		}
		
		public struct MyGenStruct<T>
		{
			public struct MyGenNestedStruct<U>
			{
			}
		}
		
		public interface MyInterface<R, A, B>
		{
			R Fun<M>(A a, B b, M m);
		}
		
		public unsafe class MyInterfaceImpl<R> : MyInterface<R, MyClass, MyStruct>
		{
			public void* voidPtr;
			
			public List<R> Prop { get { return new List<R>(); } }
			
			R MyInterface<R, MyClass, MyStruct>.Fun<M>(MyClass a, MyStruct b, M m)
			{
				throw new NotImplementedException();
			}
			
			public M[] Fun2<M>(ref int** iPtrPtr, ref M[,] mdArray, ref List<M>.Enumerator listEnum)
			{
				throw new NotImplementedException();
			}
		}
		
		public static unsafe void Main()
		{
			// The nulls should be first to test for "Value does not fall within the expected range." exception of .BaseType
			object nullObject = null;
			string nullString = null;
			MyClass nullMyClass = null;
			
			// Simple types
			int i = 42;
			bool b = true;
			char c = 'a';
			object obj = new object();
			MyClass myClass = new MyClass();
			MyStruct point = new MyStruct();
			object box = 40;
			
			// Pointers
			int* iPtr = &i;
			int** iPtrPtr = &iPtr;
			bool* bPtr = &b;
			void* voidPtr = &i;
			MyStruct* pointPtr = &point;
			IntPtr ptr = IntPtr.Zero;
			
			// Arrays
			char[] szArray = "Test".ToCharArray();
			char[,] mdArray = new char[2,3];
			
			// Generics - nullables
			int? nullable_value = 5;
			int? nullable_null = null;
			
			// Generic class
			MyGenClass<int> myGenClass_int = new MyGenClass<int>();
			MyGenClass<int>[] array_MyGenClass_int = new MyGenClass<int>[] {};
			MyGenClass<MyGenStruct<int>> myGenClass_MyGenStruct_int = new MyGenClass<MyGenStruct<int>>();
			MyGenClass<int>.MyGenNestedClass<char> myGenNestedClass = new MyGenClass<int>.MyGenNestedClass<char>();
			
			// Generic struct
			MyGenStruct<int> myGenStruct_int = new MyGenStruct<int>();
			MyGenStruct<int>[] array_MyGenStruct_int = new MyGenStruct<int>[] {};
			MyGenStruct<MyGenClass<int>> myGenStruct_MyGenClass_int = new MyGenStruct<MyGenClass<int>>();
			MyGenStruct<int>.MyGenNestedStruct<char> myGenNestedStruct = new MyGenStruct<int>.MyGenNestedStruct<char>();
			
			// Generic interface
			MyInterfaceImpl<int> myInterfaceImpl = new MyInterfaceImpl<int>();
			MyInterface<int, MyClass, MyStruct> myInterface = myInterfaceImpl;
			
			// TypeRef generics
			List<int> list = new List<int>();
			// TODO List<int>.Enumerator listEnum = list.GetEnumerator();
			
			// Other
			AddDelegate fnPtr = Add;
			
			Access access = new Access();
			
			System.Diagnostics.Debugger.Break();
		}
		
		public class Access
		{
			private   int privateField = 0;
			public    int publicField = 0;
			protected int protectedField = 0;
			internal  int internalField = 0;
			static    int staticField = 0;
			
			private   int privateProperty   { get { return 1 + privateField; } }
			public    int publicProperty    { get { return 1 + publicField; } }
			protected int protectedProperty { get { return 1 + protectedField; } }
			internal  int internalProperty  { get { return 1 + internalField; } }
			static    int staticProperty    { get { return 1 + staticField; } }
			
			private   void privateMethod() {}
			public    void publicMethod() {}
			protected void protectedMethod() {}
			internal  void internalMethod() {}
			static    void staticMethod() {}
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using Debugger.MetaData;
	using System.Linq;
	
	public partial class DebuggerTests
	{
		class LocalVariable
		{
			public string Name { get; set; }
			public DebugType Type { get; set; }
			public Value Value { get; set; }
		}
		
		void PrintLocalVariables()
		{
			ObjectDump(
				"LocalVariables",
				process.SelectedStackFrame.MethodInfo.LocalVariables.Select(v => new LocalVariable() { Name = v.Name, Type = v.Type, Value = v.GetValue(process.SelectedStackFrame) })
			);
		}
		
		[NUnit.Framework.Test]
		public void DebugType()
		{
			ExpandProperties(
				"LocalVariable.Type",
				"DebugType.ElementType"
			);
			StartTest("DebugType.cs");
			
			process.Options.StepOverSingleLineProperties = false;
			process.Options.StepOverFieldAccessProperties = true;
			
			ObjectDump("DefinedTypes", process.Modules["DebugType.exe"].GetNamesOfDefinedTypes());
			ObjectDump("DefinedTypes", process.Modules["DebugType.exe"].GetDefinedTypes());
			
			ObjectDump("Access-Members", process.SelectedStackFrame.GetLocalVariableValue("access").Type.GetMembers());
			ObjectDump("MyInterfaceImpl-Members", process.SelectedStackFrame.GetLocalVariableValue("myInterfaceImpl").Type.GetMembers());
			PrintLocalVariables();
			
			// TODO: Identity
			
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
    <DebuggingPaused>Break DebugType.cs:125,4-125,40</DebuggingPaused>
    <DefinedTypes
      Capacity="16"
      Count="11">
      <Item>Debugger.Tests.TestPrograms.DebugType</Item>
      <Item>AddDelegate</Item>
      <Item>MyClass</Item>
      <Item>MyGenClass`1</Item>
      <Item>MyGenNestedClass`1</Item>
      <Item>MyStruct</Item>
      <Item>MyGenStruct`1</Item>
      <Item>MyGenNestedStruct`1</Item>
      <Item>MyInterface`3</Item>
      <Item>MyInterfaceImpl`1</Item>
      <Item>Access</Item>
    </DefinedTypes>
    <DefinedTypes
      Capacity="8"
      Count="5">
      <Item>
        <DebugType
          BaseType="System.Object"
          FullName="Debugger.Tests.TestPrograms.DebugType"
          Kind="Class"
          Module="DebugType.exe"
          Name="DebugType">
          <ElementType>null</ElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          BaseType="System.MulticastDelegate"
          FullName="AddDelegate"
          Kind="Class"
          Module="DebugType.exe"
          Name="AddDelegate">
          <ElementType>null</ElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          BaseType="System.Object"
          FullName="MyClass"
          Kind="Class"
          Module="DebugType.exe"
          Name="MyClass">
          <ElementType>null</ElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          BaseType="System.ValueType"
          FullName="MyStruct"
          Kind="ValueType"
          Module="DebugType.exe"
          Name="MyStruct">
          <ElementType>null</ElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          BaseType="System.Object"
          FullName="Access"
          Kind="Class"
          Module="DebugType.exe"
          Name="Access">
          <ElementType>null</ElementType>
        </DebugType>
      </Item>
    </DefinedTypes>
    <Access-Members
      Capacity="8"
      Count="5">
      <Item>
        <FieldInfo
          DeclaringType="Access"
          FullName="Access.publicField"
          IsPublic="True"
          Module="DebugType.exe"
          Name="publicField"
          Type="System.Int32" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Access"
          FullName="Access.get_publicProperty"
          IsPublic="True"
          IsSpecialName="True"
          Module="DebugType.exe"
          Name="get_publicProperty"
          ReturnType="System.Int32" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Access"
          FullName="Access.publicMethod"
          IsPublic="True"
          Module="DebugType.exe"
          Name="publicMethod" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Access"
          FullName="Access..ctor"
          IsPublic="True"
          IsSpecialName="True"
          Module="DebugType.exe"
          Name=".ctor" />
      </Item>
      <Item>
        <PropertyInfo
          DeclaringType="Access"
          FullName="Access.publicProperty"
          GetMethod="get_publicProperty"
          IsPublic="True"
          Module="DebugType.exe"
          Name="publicProperty"
          Type="System.Int32" />
      </Item>
    </Access-Members>
    <MyInterfaceImpl-Members
      Capacity="8"
      Count="5">
      <Item>
        <FieldInfo
          DeclaringType="MyInterfaceImpl&lt;System.Int32&gt;"
          FullName="MyInterfaceImpl&lt;System.Int32&gt;.voidPtr"
          IsPublic="True"
          Module="DebugType.exe"
          Name="voidPtr"
          Type="System.Void*" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="MyInterfaceImpl&lt;System.Int32&gt;"
          FullName="MyInterfaceImpl&lt;System.Int32&gt;.get_Prop"
          IsPublic="True"
          IsSpecialName="True"
          Module="DebugType.exe"
          Name="get_Prop"
          ReturnType="System.Collections.Generic.List&lt;System.Int32&gt;" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="MyInterfaceImpl&lt;System.Int32&gt;"
          FullName="MyInterfaceImpl&lt;System.Int32&gt;.Fun2"
          IsPublic="True"
          Module="DebugType.exe"
          Name="Fun2"
          ParameterCount="3"
          ParameterTypes="{Exception: Can not find type Enumerator}"
          ReturnType="System.Object[]" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="MyInterfaceImpl&lt;System.Int32&gt;"
          FullName="MyInterfaceImpl&lt;System.Int32&gt;..ctor"
          IsPublic="True"
          IsSpecialName="True"
          Module="DebugType.exe"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <PropertyInfo
          DeclaringType="MyInterfaceImpl&lt;System.Int32&gt;"
          FullName="MyInterfaceImpl&lt;System.Int32&gt;.Prop"
          GetMethod="get_Prop"
          IsPublic="True"
          Module="DebugType.exe"
          Name="Prop"
          Type="System.Collections.Generic.List&lt;System.Int32&gt;" />
      </Item>
    </MyInterfaceImpl-Members>
    <LocalVariables>
      <Item>
        <LocalVariable
          Name="nullObject"
          Type="System.Object"
          Value="nullObject = null">
          <Type>
            <DebugType
              FullName="System.Object"
              Kind="Class"
              Module="mscorlib.dll"
              Name="Object">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="nullString"
          Type="System.String"
          Value="nullString = null">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.String"
              Interfaces="{System.IComparable, System.ICloneable, System.IConvertible, System.IComparable&lt;System.String&gt;, System.Collections.Generic.IEnumerable&lt;System.Char&gt;, System.Collections.IEnumerable, System.IEquatable&lt;System.String&gt;}"
              Kind="Class"
              Module="mscorlib.dll"
              Name="String">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="nullMyClass"
          Type="MyClass"
          Value="nullMyClass = null">
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
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="i"
          Type="System.Int32"
          Value="i = 42">
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
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="b"
          Type="System.Boolean"
          Value="b = True">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Boolean"
              Interfaces="{System.IComparable, System.IConvertible, System.IComparable&lt;System.Boolean&gt;, System.IEquatable&lt;System.Boolean&gt;}"
              Kind="ValueType"
              Module="mscorlib.dll"
              Name="Boolean">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="c"
          Type="System.Char"
          Value="c = a">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Char"
              Interfaces="{System.IComparable, System.IConvertible, System.IComparable&lt;System.Char&gt;, System.IEquatable&lt;System.Char&gt;}"
              Kind="ValueType"
              Module="mscorlib.dll"
              Name="Char">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="obj"
          Type="System.Object"
          Value="obj = {System.Object}">
          <Type>
            <DebugType
              FullName="System.Object"
              Kind="Class"
              Module="mscorlib.dll"
              Name="Object">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myClass"
          Type="MyClass"
          Value="myClass = {MyClass}">
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
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="point"
          Type="MyStruct"
          Value="point = {MyStruct}">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              FullName="MyStruct"
              Kind="ValueType"
              Module="DebugType.exe"
              Name="MyStruct">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="box"
          Type="System.Object"
          Value="box = {System.Int32}">
          <Type>
            <DebugType
              FullName="System.Object"
              Kind="Class"
              Module="mscorlib.dll"
              Name="Object">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="iPtr"
          Type="System.Int32*"
          Value="iPtr = {System.Int32*}">
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
                  Interfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable&lt;System.Int32&gt;, System.IEquatable&lt;System.Int32&gt;}"
                  Kind="ValueType"
                  Module="mscorlib.dll"
                  Name="Int32">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="iPtrPtr"
          Type="System.Int32**"
          Value="iPtrPtr = {System.Int32**}">
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
                      Interfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable&lt;System.Int32&gt;, System.IEquatable&lt;System.Int32&gt;}"
                      Kind="ValueType"
                      Module="mscorlib.dll"
                      Name="Int32">
                      <ElementType>null</ElementType>
                    </DebugType>
                  </ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="bPtr"
          Type="System.Boolean*"
          Value="bPtr = {System.Boolean*}">
          <Type>
            <DebugType
              ElementType="System.Boolean"
              FullName="System.Boolean*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Boolean*">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  FullName="System.Boolean"
                  Interfaces="{System.IComparable, System.IConvertible, System.IComparable&lt;System.Boolean&gt;, System.IEquatable&lt;System.Boolean&gt;}"
                  Kind="ValueType"
                  Module="mscorlib.dll"
                  Name="Boolean">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="voidPtr"
          Type="System.Void*"
          Value="voidPtr = {System.Void*}">
          <Type>
            <DebugType
              ElementType="System.Void"
              FullName="System.Void*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="Void*">
              <ElementType>
                <DebugType
                  BaseType="System.ValueType"
                  FullName="System.Void"
                  Kind="ValueType"
                  Module="mscorlib.dll"
                  Name="Void">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="pointPtr"
          Type="MyStruct*"
          Value="pointPtr = {MyStruct*}">
          <Type>
            <DebugType
              ElementType="MyStruct"
              FullName="MyStruct*"
              Kind="Pointer"
              Module="{Exception: The type is not a class or value type.}"
              Name="MyStruct*">
              <ElementType>
                <DebugType
                  BaseType="System.ValueType"
                  FullName="MyStruct"
                  Kind="ValueType"
                  Module="DebugType.exe"
                  Name="MyStruct">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="ptr"
          Type="System.IntPtr"
          Value="ptr = 0">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.IntPtr"
              Interfaces="{System.Runtime.Serialization.ISerializable}"
              Kind="ValueType"
              Module="mscorlib.dll"
              Name="IntPtr">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="szArray"
          Type="System.Char[]"
          Value="szArray = {System.Char[]}">
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
                  Interfaces="{System.IComparable, System.IConvertible, System.IComparable&lt;System.Char&gt;, System.IEquatable&lt;System.Char&gt;}"
                  Kind="ValueType"
                  Module="mscorlib.dll"
                  Name="Char">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="mdArray"
          Type="System.Char[,]"
          Value="mdArray = {System.Char[,]}">
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
                  Interfaces="{System.IComparable, System.IConvertible, System.IComparable&lt;System.Char&gt;, System.IEquatable&lt;System.Char&gt;}"
                  Kind="ValueType"
                  Module="mscorlib.dll"
                  Name="Char">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="nullable_value"
          Type="System.Nullable&lt;System.Int32&gt;"
          Value="nullable_value = {System.Nullable&lt;System.Int32&gt;}">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              FullName="System.Nullable&lt;System.Int32&gt;"
              GenericArguments="{System.Int32}"
              Kind="ValueType"
              Module="mscorlib.dll"
              Name="Nullable&lt;Int32&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="nullable_null"
          Type="System.Nullable&lt;System.Int32&gt;"
          Value="nullable_null = {System.Nullable&lt;System.Int32&gt;}">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              FullName="System.Nullable&lt;System.Int32&gt;"
              GenericArguments="{System.Int32}"
              Kind="ValueType"
              Module="mscorlib.dll"
              Name="Nullable&lt;Int32&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myGenClass_int"
          Type="MyGenClass&lt;System.Int32&gt;"
          Value="myGenClass_int = {MyGenClass&lt;System.Int32&gt;}">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="MyGenClass&lt;System.Int32&gt;"
              GenericArguments="{System.Int32}"
              Kind="Class"
              Module="DebugType.exe"
              Name="MyGenClass&lt;Int32&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="array_MyGenClass_int"
          Type="MyGenClass&lt;System.Int32&gt;[]"
          Value="array_MyGenClass_int = {MyGenClass&lt;System.Int32&gt;[]}">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="MyGenClass&lt;System.Int32&gt;"
              FullName="MyGenClass&lt;System.Int32&gt;[]"
              Kind="Array"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32&gt;[]">
              <ElementType>
                <DebugType
                  BaseType="System.Object"
                  FullName="MyGenClass&lt;System.Int32&gt;"
                  GenericArguments="{System.Int32}"
                  Kind="Class"
                  Module="DebugType.exe"
                  Name="MyGenClass&lt;Int32&gt;">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myGenClass_MyGenStruct_int"
          Type="MyGenClass&lt;MyGenStruct&lt;System.Int32&gt;&gt;"
          Value="myGenClass_MyGenStruct_int = {MyGenClass&lt;MyGenStruct&lt;System.Int32&gt;&gt;}">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="MyGenClass&lt;MyGenStruct&lt;System.Int32&gt;&gt;"
              GenericArguments="{MyGenStruct&lt;System.Int32&gt;}"
              Kind="Class"
              Module="DebugType.exe"
              Name="MyGenClass&lt;MyGenStruct&lt;Int32&gt;&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myGenNestedClass"
          Type="MyGenNestedClass&lt;System.Int32,System.Char&gt;"
          Value="myGenNestedClass = {MyGenNestedClass&lt;System.Int32,System.Char&gt;}">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="MyGenNestedClass&lt;System.Int32,System.Char&gt;"
              GenericArguments="{System.Int32, System.Char}"
              Kind="Class"
              Module="DebugType.exe"
              Name="MyGenNestedClass&lt;Int32,Char&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myGenStruct_int"
          Type="MyGenStruct&lt;System.Int32&gt;"
          Value="myGenStruct_int = {MyGenStruct&lt;System.Int32&gt;}">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              FullName="MyGenStruct&lt;System.Int32&gt;"
              GenericArguments="{System.Int32}"
              Kind="ValueType"
              Module="DebugType.exe"
              Name="MyGenStruct&lt;Int32&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="array_MyGenStruct_int"
          Type="MyGenStruct&lt;System.Int32&gt;[]"
          Value="array_MyGenStruct_int = {MyGenStruct&lt;System.Int32&gt;[]}">
          <Type>
            <DebugType
              BaseType="System.Array"
              ElementType="MyGenStruct&lt;System.Int32&gt;"
              FullName="MyGenStruct&lt;System.Int32&gt;[]"
              Kind="Array"
              Module="{Exception: The type is not a class or value type.}"
              Name="Int32&gt;[]">
              <ElementType>
                <DebugType
                  BaseType="System.ValueType"
                  FullName="MyGenStruct&lt;System.Int32&gt;"
                  GenericArguments="{System.Int32}"
                  Kind="ValueType"
                  Module="DebugType.exe"
                  Name="MyGenStruct&lt;Int32&gt;">
                  <ElementType>null</ElementType>
                </DebugType>
              </ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myGenStruct_MyGenClass_int"
          Type="MyGenStruct&lt;MyGenClass&lt;System.Int32&gt;&gt;"
          Value="myGenStruct_MyGenClass_int = {MyGenStruct&lt;MyGenClass&lt;System.Int32&gt;&gt;}">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              FullName="MyGenStruct&lt;MyGenClass&lt;System.Int32&gt;&gt;"
              GenericArguments="{MyGenClass&lt;System.Int32&gt;}"
              Kind="ValueType"
              Module="DebugType.exe"
              Name="MyGenStruct&lt;MyGenClass&lt;Int32&gt;&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myGenNestedStruct"
          Type="MyGenNestedStruct&lt;System.Int32,System.Char&gt;"
          Value="myGenNestedStruct = {MyGenNestedStruct&lt;System.Int32,System.Char&gt;}">
          <Type>
            <DebugType
              BaseType="System.ValueType"
              FullName="MyGenNestedStruct&lt;System.Int32,System.Char&gt;"
              GenericArguments="{System.Int32, System.Char}"
              Kind="ValueType"
              Module="DebugType.exe"
              Name="MyGenNestedStruct&lt;Int32,Char&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myInterfaceImpl"
          Type="MyInterfaceImpl&lt;System.Int32&gt;"
          Value="myInterfaceImpl = {MyInterfaceImpl&lt;System.Int32&gt;}">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="MyInterfaceImpl&lt;System.Int32&gt;"
              GenericArguments="{System.Int32}"
              Interfaces="{MyInterface&lt;System.Int32,MyClass,MyStruct&gt;}"
              Kind="Class"
              Module="DebugType.exe"
              Name="MyInterfaceImpl&lt;Int32&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myInterface"
          Type="MyInterface&lt;System.Int32,MyClass,MyStruct&gt;"
          Value="myInterface = {MyInterfaceImpl&lt;System.Int32&gt;}">
          <Type>
            <DebugType
              FullName="MyInterface&lt;System.Int32,MyClass,MyStruct&gt;"
              GenericArguments="{System.Int32, MyClass, MyStruct}"
              Kind="Class"
              Module="DebugType.exe"
              Name="MyInterface&lt;Int32,MyClass,MyStruct&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="list"
          Type="System.Collections.Generic.List&lt;System.Int32&gt;"
          Value="list = {System.Collections.Generic.List&lt;System.Int32&gt;}">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="System.Collections.Generic.List&lt;System.Int32&gt;"
              GenericArguments="{System.Int32}"
              Interfaces="{System.Collections.Generic.IList&lt;System.Int32&gt;, System.Collections.Generic.ICollection&lt;System.Int32&gt;, System.Collections.Generic.IEnumerable&lt;System.Int32&gt;, System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable}"
              Kind="Class"
              Module="mscorlib.dll"
              Name="List&lt;Int32&gt;">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="fnPtr"
          Type="AddDelegate"
          Value="fnPtr = {AddDelegate}">
          <Type>
            <DebugType
              BaseType="System.MulticastDelegate"
              FullName="AddDelegate"
              Kind="Class"
              Module="DebugType.exe"
              Name="AddDelegate">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="access"
          Type="Access"
          Value="access = {Access}">
          <Type>
            <DebugType
              BaseType="System.Object"
              FullName="Access"
              Kind="Class"
              Module="DebugType.exe"
              Name="Access">
              <ElementType>null</ElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
    </LocalVariables>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT