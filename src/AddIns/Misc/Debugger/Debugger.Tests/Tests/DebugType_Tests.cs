// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger.Tests
{
	public class DebugType_Tests
	{
		public delegate int AddDelegate(byte b1, byte b2);
		
		public static int Add(byte b1, byte b2)
		{
			return b1 + b2;
		}
		
		// The classes are intentionally nested for testing
		
		public enum MyEnum: byte { A, B }
		
		public class MyClass
		{
		}
		
		public struct MyStruct
		{
		}
		
		public class MyGenClass<T>
		{
			public struct MyNestedStruct
			{
			}
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
			public List<R> Prop { get { return new List<R>(); } }
			
			public R Fun<M>(MyClass a, MyStruct b, M m)
			{
				throw new NotImplementedException();
			}
			
			public M[] Fun2<M>(ref int** iPtrPtr, ref M[,] mdArray, ref List<M>.Enumerator listEnum)
			{
				throw new NotImplementedException();
			}
		}
		
		public unsafe class Members
		{
			public int instanceInt;
			public static int staticInt;
			
			public void* voidPtr;
			public char SetterOnlyProp { set { ; } }
			public const int IntLiteral = 42;
			
			public int InstanceInt {
				get { return instanceInt; }
			}
			
			public static int StaticInt {
				get { return staticInt; }
			}
			
			public int AutoProperty { get; set; }
			
			public char this[int i] {
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}
			
			public char this[string s] {
				get { throw new NotImplementedException(); }
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
			string s = "abc";
			object obj = new object();
			MyClass myClass = new MyClass();
			MyStruct myStruct = new MyStruct();
			object box = 40;
			
			// Pointers
			int* iPtr = &i;
			int** iPtrPtr = &iPtr;
			bool* bPtr = &b;
			void* voidPtr = &i;
			MyStruct* myStructPtr = &myStruct;
			IntPtr ptr = IntPtr.Zero;
			
			// Arrays
			char[] szArray = "Test".ToCharArray();
			char[,] mdArray = new char[2,3];
			char[][,] jagArray = new char[][,] { mdArray };
			
			// Generics - nullables
			int? nullable_value = 5;
			int? nullable_null = null;
			
			// Generic class
			MyGenClass<int> myGenClass_int = new MyGenClass<int>();
			MyGenClass<int>[] array_MyGenClass_int = new MyGenClass<int>[] {};
			MyGenClass<int?> myGenClass_Nullable_int = new MyGenClass<int?>();
			MyGenClass<int>.MyNestedStruct myNestedStruct = new MyGenClass<int>.MyNestedStruct();
			MyGenClass<int>.MyGenNestedStruct<char> myGenNestedStruct = new MyGenClass<int>.MyGenNestedStruct<char>();
			
			// Generic interface
			MyInterfaceImpl<int> myInterfaceImpl = new MyInterfaceImpl<int>();
			MyInterface<int, MyClass, MyStruct> myInterface = myInterfaceImpl;
			
			// TypeRef generics
			List<int> list = new List<int>();
			List<int>.Enumerator listEnumerator = list.GetEnumerator();
			
			// Other
			AddDelegate fnPtr = Add;
			ValueType valueType = null;
			Enum enumType = null;
			MyEnum myEnum = MyEnum.B;
			
			Members members = new Members();
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
			public Type Type { get; set; }
			public Value Value { get; set; }
		}
		
		void PrintLocalVariables()
		{
			PrintLocalVariables("LocalVariables");
		}
		
		void PrintLocalVariables(string msg)
		{
			ObjectDump(
				msg,
				process.SelectedStackFrame.MethodInfo.GetLocalVariables().Select(v => new LocalVariable() { Name = v.Name, Type = v.LocalType, Value = v.GetValue(process.SelectedStackFrame) })
			);
		}
		
		[NUnit.Framework.Test]
		public void DebugType_Tests()
		{
			ExpandProperties(
				"LocalVariable.Type",
				"DebugType.GetElementType"
			);
			StartTest();
			
			process.Options.StepOverSingleLineProperties = false;
			process.Options.StepOverFieldAccessProperties = true;
			
			ObjectDump("DefinedTypes", process.Modules["DebugType_Tests.exe"].GetNamesOfDefinedTypes());
			ObjectDump("DefinedTypes", process.Modules["DebugType_Tests.exe"].GetDefinedTypes());
			
			ObjectDump("Members", process.SelectedStackFrame.GetLocalVariableValue("members").Type.GetMembers(DebugType.BindingFlagsAllDeclared));
			ObjectDump("Access-Members", process.SelectedStackFrame.GetLocalVariableValue("access").Type.GetMembers());
			ObjectDump("MyInterfaceImpl-Members", process.SelectedStackFrame.GetLocalVariableValue("myInterfaceImpl").Type.GetMembers());
			PrintLocalVariables();
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="DebugType_Tests.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>DebugType_Tests.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break DebugType_Tests.cs:151,4-151,40</DebuggingPaused>
    <DefinedTypes
      Capacity="16"
      Count="12">
      <Item>Debugger.Tests.DebugType_Tests</Item>
      <Item>AddDelegate</Item>
      <Item>MyEnum</Item>
      <Item>MyClass</Item>
      <Item>MyStruct</Item>
      <Item>MyGenClass`1</Item>
      <Item>MyNestedStruct</Item>
      <Item>MyGenNestedStruct`1</Item>
      <Item>MyInterface`3</Item>
      <Item>MyInterfaceImpl`1</Item>
      <Item>Members</Item>
      <Item>Access</Item>
    </DefinedTypes>
    <DefinedTypes
      Capacity="8"
      Count="7">
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, Public, BeforeFieldInit"
          BaseType="System.Object"
          FullName="Debugger.Tests.DebugType_Tests"
          GetMembers="{static System.Int32 Debugger.Tests.DebugType_Tests.Add(Byte b1, Byte b2), static void Debugger.Tests.DebugType_Tests.Main(), void Debugger.Tests.DebugType_Tests..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          GetMethods="{static System.Int32 Debugger.Tests.DebugType_Tests.Add(Byte b1, Byte b2), static void Debugger.Tests.DebugType_Tests.Main(), void Debugger.Tests.DebugType_Tests..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          IsClass="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, Sealed"
          BaseType="System.MulticastDelegate"
          FullName="Debugger.Tests.DebugType_Tests+AddDelegate"
          GetMembers="{void Debugger.Tests.DebugType_Tests+AddDelegate..ctor(Object object, IntPtr method), System.Int32 Debugger.Tests.DebugType_Tests+AddDelegate.Invoke(Byte b1, Byte b2), System.IAsyncResult Debugger.Tests.DebugType_Tests+AddDelegate.BeginInvoke(Byte b1, Byte b2, AsyncCallback callback, Object object), System.Int32 Debugger.Tests.DebugType_Tests+AddDelegate.EndInvoke(IAsyncResult result), void System.MulticastDelegate.GetObjectData(SerializationInfo info, StreamingContext context), System.Boolean System.MulticastDelegate.Equals(Object obj), System.Delegate[] System.MulticastDelegate.GetInvocationList(), System.Int32 System.MulticastDelegate.GetHashCode(), System.Object System.Delegate.DynamicInvoke(Object[] args), System.Boolean System.Delegate.Equals(Object obj), System.Int32 System.Delegate.GetHashCode(), System.Delegate[] System.Delegate.GetInvocationList(), System.Reflection.MethodInfo System.Delegate.get_Method(), System.Object System.Delegate.get_Target(), System.Object System.Delegate.Clone(), void System.Delegate.GetObjectData(SerializationInfo info, StreamingContext context), System.Reflection.MethodInfo Method, System.Object Target, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          GetMethods="{void Debugger.Tests.DebugType_Tests+AddDelegate..ctor(Object object, IntPtr method), System.Int32 Debugger.Tests.DebugType_Tests+AddDelegate.Invoke(Byte b1, Byte b2), System.IAsyncResult Debugger.Tests.DebugType_Tests+AddDelegate.BeginInvoke(Byte b1, Byte b2, AsyncCallback callback, Object object), System.Int32 Debugger.Tests.DebugType_Tests+AddDelegate.EndInvoke(IAsyncResult result), void System.MulticastDelegate.GetObjectData(SerializationInfo info, StreamingContext context), System.Boolean System.MulticastDelegate.Equals(Object obj), System.Delegate[] System.MulticastDelegate.GetInvocationList(), System.Int32 System.MulticastDelegate.GetHashCode(), System.Object System.Delegate.DynamicInvoke(Object[] args), System.Boolean System.Delegate.Equals(Object obj), System.Int32 System.Delegate.GetHashCode(), System.Delegate[] System.Delegate.GetInvocationList(), System.Reflection.MethodInfo System.Delegate.get_Method(), System.Object System.Delegate.get_Target(), System.Object System.Delegate.Clone(), void System.Delegate.GetObjectData(SerializationInfo info, StreamingContext context), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          GetProperties="{System.Reflection.MethodInfo Method, System.Object Target}"
          IsClass="True"
          IsNested="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, Sealed"
          BaseType="System.Enum"
          FullName="Debugger.Tests.DebugType_Tests+MyEnum"
          GetEnumUnderlyingType="System.Byte"
          GetFields="{System.Byte value__, Debugger.Tests.DebugType_Tests+MyEnum A, Debugger.Tests.DebugType_Tests+MyEnum B}"
          GetMembers="{System.Byte value__, Debugger.Tests.DebugType_Tests+MyEnum A, Debugger.Tests.DebugType_Tests+MyEnum B, System.Boolean System.Enum.Equals(Object obj), System.Int32 System.Enum.GetHashCode(), System.String System.Enum.ToString(), System.String System.Enum.ToString(String format, IFormatProvider provider), System.String System.Enum.ToString(String format), System.String System.Enum.ToString(IFormatProvider provider), System.Int32 System.Enum.CompareTo(Object target), System.TypeCode System.Enum.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          GetMethods="{System.Boolean System.Enum.Equals(Object obj), System.Int32 System.Enum.GetHashCode(), System.String System.Enum.ToString(), System.String System.Enum.ToString(String format, IFormatProvider provider), System.String System.Enum.ToString(String format), System.String System.Enum.ToString(IFormatProvider provider), System.Int32 System.Enum.CompareTo(Object target), System.TypeCode System.Enum.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          IsEnum="True"
          IsNested="True"
          IsValueType="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
          BaseType="System.Object"
          FullName="Debugger.Tests.DebugType_Tests+MyClass"
          GetMembers="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          GetMethods="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          IsClass="True"
          IsNested="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, BeforeFieldInit"
          BaseType="System.ValueType"
          FullName="Debugger.Tests.DebugType_Tests+MyStruct"
          GetMembers="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          GetMethods="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          IsNested="True"
          IsValueType="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
          BaseType="System.Object"
          FullName="Debugger.Tests.DebugType_Tests+Members"
          GetFields="{System.Int32 IntLiteral, System.Int32 instanceInt, System.Int32 staticInt, System.Void* voidPtr}"
          GetMembers="{System.Int32 IntLiteral, System.Int32 instanceInt, System.Int32 staticInt, System.Void* voidPtr, void Debugger.Tests.DebugType_Tests+Members.set_SetterOnlyProp(Char value), System.Int32 Debugger.Tests.DebugType_Tests+Members.get_InstanceInt(), static System.Int32 Debugger.Tests.DebugType_Tests+Members.get_StaticInt(), System.Int32 Debugger.Tests.DebugType_Tests+Members.get_AutoProperty(), void Debugger.Tests.DebugType_Tests+Members.set_AutoProperty(Int32 value), System.Char Debugger.Tests.DebugType_Tests+Members.get_Item(Int32 i), System.Char Debugger.Tests.DebugType_Tests+Members.get_Item(String s), void Debugger.Tests.DebugType_Tests+Members.set_Item(Int32 i, Char value), void Debugger.Tests.DebugType_Tests+Members..ctor(), System.Char SetterOnlyProp, System.Int32 InstanceInt, System.Int32 StaticInt, System.Int32 AutoProperty, System.Char Item[Int32 i], System.Char Item[String s], void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          GetMethods="{void Debugger.Tests.DebugType_Tests+Members.set_SetterOnlyProp(Char value), System.Int32 Debugger.Tests.DebugType_Tests+Members.get_InstanceInt(), static System.Int32 Debugger.Tests.DebugType_Tests+Members.get_StaticInt(), System.Int32 Debugger.Tests.DebugType_Tests+Members.get_AutoProperty(), void Debugger.Tests.DebugType_Tests+Members.set_AutoProperty(Int32 value), System.Char Debugger.Tests.DebugType_Tests+Members.get_Item(Int32 i), System.Char Debugger.Tests.DebugType_Tests+Members.get_Item(String s), void Debugger.Tests.DebugType_Tests+Members.set_Item(Int32 i, Char value), void Debugger.Tests.DebugType_Tests+Members..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          GetProperties="{System.Char SetterOnlyProp, System.Int32 InstanceInt, System.Int32 StaticInt, System.Int32 AutoProperty, System.Char Item[Int32 i], System.Char Item[String s]}"
          IsClass="True"
          IsNested="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
          BaseType="System.Object"
          FullName="Debugger.Tests.DebugType_Tests+Access"
          GetFields="{System.Int32 publicField}"
          GetMembers="{System.Int32 publicField, System.Int32 Debugger.Tests.DebugType_Tests+Access.get_publicProperty(), void Debugger.Tests.DebugType_Tests+Access.publicMethod(), void Debugger.Tests.DebugType_Tests+Access..ctor(), System.Int32 publicProperty, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          GetMethods="{System.Int32 Debugger.Tests.DebugType_Tests+Access.get_publicProperty(), void Debugger.Tests.DebugType_Tests+Access.publicMethod(), void Debugger.Tests.DebugType_Tests+Access..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
          GetProperties="{System.Int32 publicProperty}"
          IsClass="True"
          IsNested="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
    </DefinedTypes>
    <Members>
      <Item>
        <DebugFieldInfo
          Attributes="Public, Static, Literal, HasDefault"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FieldType="System.Int32"
          IsLiteral="True"
          Name="IntLiteral" />
      </Item>
      <Item>
        <DebugFieldInfo
          Attributes="Public"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FieldType="System.Int32"
          Name="instanceInt" />
      </Item>
      <Item>
        <DebugFieldInfo
          Attributes="Public, Static"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FieldType="System.Int32"
          Name="staticInt" />
      </Item>
      <Item>
        <DebugFieldInfo
          Attributes="Public"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FieldType="System.Void*"
          Name="voidPtr" />
      </Item>
      <Item>
        <DebugFieldInfo
          Attributes="Private"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FieldType="System.Int32"
          Name="&lt;AutoProperty&gt;k__BackingField" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Debugger.Tests.DebugType_Tests+Members.set_SetterOnlyProp(Char value)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Members this}"
          Name="set_SetterOnlyProp" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          BackingField="System.Int32 instanceInt"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Debugger.Tests.DebugType_Tests+Members.get_InstanceInt()"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Members this}"
          Name="get_InstanceInt"
          ReturnType="System.Int32"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Static, HideBySig, SpecialName"
          BackingField="System.Int32 staticInt"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Debugger.Tests.DebugType_Tests+Members.get_StaticInt()"
          Name="get_StaticInt"
          ReturnType="System.Int32"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          BackingField="System.Int32 &lt;AutoProperty&gt;k__BackingField"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Debugger.Tests.DebugType_Tests+Members.get_AutoProperty()"
          Name="get_AutoProperty"
          ReturnType="System.Int32"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Debugger.Tests.DebugType_Tests+Members.set_AutoProperty(Int32 value)"
          Name="set_AutoProperty"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Debugger.Tests.DebugType_Tests+Members.get_Item(Int32 i)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Members this}"
          Name="get_Item"
          ReturnType="System.Char" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Debugger.Tests.DebugType_Tests+Members.get_Item(String s)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Members this}"
          Name="get_Item"
          ReturnType="System.Char" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Debugger.Tests.DebugType_Tests+Members.set_Item(Int32 i, Char value)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Members this}"
          Name="set_Item" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Debugger.Tests.DebugType_Tests+Members..ctor()"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          Name="SetterOnlyProp"
          PropertyType="System.Char" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          Name="InstanceInt"
          PropertyType="System.Int32" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          Name="StaticInt"
          PropertyType="System.Int32" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          Name="AutoProperty"
          PropertyType="System.Int32" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          GetIndexParameters="{System.Int32 i}"
          Name="Item"
          PropertyType="System.Char" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          GetIndexParameters="{System.String s}"
          Name="Item"
          PropertyType="System.Char" />
      </Item>
    </Members>
    <Access-Members>
      <Item>
        <DebugFieldInfo
          Attributes="Public"
          DeclaringType="Debugger.Tests.DebugType_Tests+Access"
          FieldType="System.Int32"
          Name="publicField" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Access"
          FullName="Debugger.Tests.DebugType_Tests+Access.get_publicProperty()"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Access this}"
          Name="get_publicProperty"
          ReturnType="System.Int32" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig"
          DeclaringType="Debugger.Tests.DebugType_Tests+Access"
          FullName="Debugger.Tests.DebugType_Tests+Access.publicMethod()"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Access this}"
          Name="publicMethod" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Access"
          FullName="Debugger.Tests.DebugType_Tests+Access..ctor()"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Access this}"
          Name=".ctor" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Access"
          Name="publicProperty"
          PropertyType="System.Int32" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="System.Object"
          FullName="System.Object..ctor()"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="System.Object.ToString()"
          Name="ToString"
          ReturnType="System.String"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="System.Object.Equals(Object obj)"
          Name="Equals"
          ReturnType="System.Boolean"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="System.Object.GetHashCode()"
          Name="GetHashCode"
          ReturnType="System.Int32"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig"
          DeclaringType="System.Object"
          FullName="System.Object.GetType()"
          Name="GetType"
          ReturnType="System.Type"
          StepOver="True" />
      </Item>
    </Access-Members>
    <MyInterfaceImpl-Members>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          FullName="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].get_Prop()"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32] this}"
          Name="get_Prop"
          ReturnType="System.Collections.Generic.List`1[System.Int32]" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Final, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          FullName="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun(MyClass a, MyStruct b, Object m)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32] this}"
          Name="Fun"
          ReturnType="System.Int32" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig"
          DeclaringType="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          FullName="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun2(Int32** iPtrPtr, Object[,] mdArray, Enumerator listEnum)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32] this}"
          Name="Fun2"
          ReturnType="System.Object[]" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          FullName="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]..ctor()"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          Name="Prop"
          PropertyType="System.Collections.Generic.List`1[System.Int32]" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="System.Object"
          FullName="System.Object..ctor()"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="System.Object.ToString()"
          Name="ToString"
          ReturnType="System.String"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="System.Object.Equals(Object obj)"
          Name="Equals"
          ReturnType="System.Boolean"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="System.Object.GetHashCode()"
          Name="GetHashCode"
          ReturnType="System.Int32"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig"
          DeclaringType="System.Object"
          FullName="System.Object.GetType()"
          Name="GetType"
          ReturnType="System.Type"
          StepOver="True" />
      </Item>
    </MyInterfaceImpl-Members>
    <LocalVariables>
      <Item>
        <LocalVariable
          Name="nullObject"
          Type="System.Object"
          Value="null">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, Serializable, BeforeFieldInit"
              FullName="System.Object"
              GetMembers="{void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), static System.Boolean System.Object.Equals(Object objA, Object objB), static System.Boolean System.Object.ReferenceEquals(Object objA, Object objB), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), static System.Boolean System.Object.Equals(Object objA, Object objB), static System.Boolean System.Object.ReferenceEquals(Object objA, Object objB), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsClass="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="nullString"
          Type="System.String"
          Value="null">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.Object"
              FullName="System.String"
              GetFields="{System.String Empty}"
              GetInterfaces="{System.IComparable, System.ICloneable, System.IConvertible, System.IComparable`1[System.String], System.Collections.Generic.IEnumerable`1[System.Char], System.Collections.IEnumerable, System.IEquatable`1[System.String]}"
              GetMembers="{System.String Empty, static System.String System.String.Join(String separator, String[] value), static System.String System.String.Join(String separator, String[] value, Int32 startIndex, Int32 count), System.Boolean System.String.Equals(Object obj), System.Boolean System.String.Equals(String value), System.Boolean System.String.Equals(String value, StringComparison comparisonType), static System.Boolean System.String.Equals(String a, String b), static System.Boolean System.String.Equals(String a, String b, StringComparison comparisonType), static System.Boolean System.String.op_Equality(String a, String b), static System.Boolean System.String.op_Inequality(String a, String b), System.Char System.String.get_Chars(Int32 index), void System.String.CopyTo(Int32 sourceIndex, Char[] destination, Int32 destinationIndex, Int32 count), System.Char[] System.String.ToCharArray(), System.Char[] System.String.ToCharArray(Int32 startIndex, Int32 length), static System.Boolean System.String.IsNullOrEmpty(String value), System.Int32 System.String.GetHashCode(), System.Int32 System.String.get_Length(), System.String[] System.String.Split(Char[] separator), System.String[] System.String.Split(Char[] separator, Int32 count), System.String[] System.String.Split(Char[] separator, StringSplitOptions options), System.String[] System.String.Split(Char[] separator, Int32 count, StringSplitOptions options), System.String[] System.String.Split(String[] separator, StringSplitOptions options), System.String[] System.String.Split(String[] separator, Int32 count, StringSplitOptions options), System.String System.String.Substring(Int32 startIndex), System.String System.String.Substring(Int32 startIndex, Int32 length), System.String System.String.Trim(Char[] trimChars), System.String System.String.Trim(), System.String System.String.TrimStart(Char[] trimChars), System.String System.String.TrimEnd(Char[] trimChars), void System.String..ctor(Char* value), void System.String..ctor(Char* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length, Encoding enc), void System.String..ctor(Char[] value, Int32 startIndex, Int32 length), void System.String..ctor(Char[] value), void System.String..ctor(Char c, Int32 count), System.Boolean System.String.IsNormalized(), System.Boolean System.String.IsNormalized(NormalizationForm normalizationForm), System.String System.String.Normalize(), System.String System.String.Normalize(NormalizationForm normalizationForm), static System.Int32 System.String.Compare(String strA, String strB), static System.Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase), static System.Int32 System.String.Compare(String strA, String strB, StringComparison comparisonType), static System.Int32 System.String.Compare(String strA, String strB, CultureInfo culture, CompareOptions options), static System.Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase, CultureInfo culture), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase, CultureInfo culture), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, CultureInfo culture, CompareOptions options), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, StringComparison comparisonType), System.Int32 System.String.CompareTo(Object value), System.Int32 System.String.CompareTo(String strB), static System.Int32 System.String.CompareOrdinal(String strA, String strB), static System.Int32 System.String.CompareOrdinal(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), System.Boolean System.String.Contains(String value), System.Boolean System.String.EndsWith(String value), System.Boolean System.String.EndsWith(String value, StringComparison comparisonType), System.Boolean System.String.EndsWith(String value, Boolean ignoreCase, CultureInfo culture), System.Int32 System.String.IndexOf(Char value), System.Int32 System.String.IndexOf(Char value, Int32 startIndex), System.Int32 System.String.IndexOf(Char value, Int32 startIndex, Int32 count), System.Int32 System.String.IndexOf(String value), System.Int32 System.String.IndexOf(String value, Int32 startIndex), System.Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count), System.Int32 System.String.IndexOf(String value, StringComparison comparisonType), System.Int32 System.String.IndexOf(String value, Int32 startIndex, StringComparison comparisonType), System.Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), System.Int32 System.String.IndexOfAny(Char[] anyOf), System.Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex), System.Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(Char value), System.Int32 System.String.LastIndexOf(Char value, Int32 startIndex), System.Int32 System.String.LastIndexOf(Char value, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(String value), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(String value, StringComparison comparisonType), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, StringComparison comparisonType), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), System.Int32 System.String.LastIndexOfAny(Char[] anyOf), System.Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex), System.Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), System.String System.String.PadLeft(Int32 totalWidth), System.String System.String.PadLeft(Int32 totalWidth, Char paddingChar), System.String System.String.PadRight(Int32 totalWidth), System.String System.String.PadRight(Int32 totalWidth, Char paddingChar), System.Boolean System.String.StartsWith(String value), System.Boolean System.String.StartsWith(String value, StringComparison comparisonType), System.Boolean System.String.StartsWith(String value, Boolean ignoreCase, CultureInfo culture), System.String System.String.ToLower(), System.String System.String.ToLower(CultureInfo culture), System.String System.String.ToLowerInvariant(), System.String System.String.ToUpper(), System.String System.String.ToUpper(CultureInfo culture), System.String System.String.ToUpperInvariant(), System.String System.String.ToString(), System.String System.String.ToString(IFormatProvider provider), System.Object System.String.Clone(), System.String System.String.Insert(Int32 startIndex, String value), System.String System.String.Replace(Char oldChar, Char newChar), System.String System.String.Replace(String oldValue, String newValue), System.String System.String.Remove(Int32 startIndex, Int32 count), System.String System.String.Remove(Int32 startIndex), static System.String System.String.Format(String format, Object arg0), static System.String System.String.Format(String format, Object arg0, Object arg1), static System.String System.String.Format(String format, Object arg0, Object arg1, Object arg2), static System.String System.String.Format(String format, Object[] args), static System.String System.String.Format(IFormatProvider provider, String format, Object[] args), static System.String System.String.Copy(String str), static System.String System.String.Concat(Object arg0), static System.String System.String.Concat(Object arg0, Object arg1), static System.String System.String.Concat(Object arg0, Object arg1, Object arg2), static System.String System.String.Concat(Object arg0, Object arg1, Object arg2, Object arg3), static System.String System.String.Concat(Object[] args), static System.String System.String.Concat(String str0, String str1), static System.String System.String.Concat(String str0, String str1, String str2), static System.String System.String.Concat(String str0, String str1, String str2, String str3), static System.String System.String.Concat(String[] values), static System.String System.String.Intern(String str), static System.String System.String.IsInterned(String str), System.TypeCode System.String.GetTypeCode(), System.CharEnumerator System.String.GetEnumerator(), System.Char Chars[Int32 index], System.Int32 Length, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{static System.String System.String.Join(String separator, String[] value), static System.String System.String.Join(String separator, String[] value, Int32 startIndex, Int32 count), System.Boolean System.String.Equals(Object obj), System.Boolean System.String.Equals(String value), System.Boolean System.String.Equals(String value, StringComparison comparisonType), static System.Boolean System.String.Equals(String a, String b), static System.Boolean System.String.Equals(String a, String b, StringComparison comparisonType), static System.Boolean System.String.op_Equality(String a, String b), static System.Boolean System.String.op_Inequality(String a, String b), System.Char System.String.get_Chars(Int32 index), void System.String.CopyTo(Int32 sourceIndex, Char[] destination, Int32 destinationIndex, Int32 count), System.Char[] System.String.ToCharArray(), System.Char[] System.String.ToCharArray(Int32 startIndex, Int32 length), static System.Boolean System.String.IsNullOrEmpty(String value), System.Int32 System.String.GetHashCode(), System.Int32 System.String.get_Length(), System.String[] System.String.Split(Char[] separator), System.String[] System.String.Split(Char[] separator, Int32 count), System.String[] System.String.Split(Char[] separator, StringSplitOptions options), System.String[] System.String.Split(Char[] separator, Int32 count, StringSplitOptions options), System.String[] System.String.Split(String[] separator, StringSplitOptions options), System.String[] System.String.Split(String[] separator, Int32 count, StringSplitOptions options), System.String System.String.Substring(Int32 startIndex), System.String System.String.Substring(Int32 startIndex, Int32 length), System.String System.String.Trim(Char[] trimChars), System.String System.String.Trim(), System.String System.String.TrimStart(Char[] trimChars), System.String System.String.TrimEnd(Char[] trimChars), void System.String..ctor(Char* value), void System.String..ctor(Char* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length, Encoding enc), void System.String..ctor(Char[] value, Int32 startIndex, Int32 length), void System.String..ctor(Char[] value), void System.String..ctor(Char c, Int32 count), System.Boolean System.String.IsNormalized(), System.Boolean System.String.IsNormalized(NormalizationForm normalizationForm), System.String System.String.Normalize(), System.String System.String.Normalize(NormalizationForm normalizationForm), static System.Int32 System.String.Compare(String strA, String strB), static System.Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase), static System.Int32 System.String.Compare(String strA, String strB, StringComparison comparisonType), static System.Int32 System.String.Compare(String strA, String strB, CultureInfo culture, CompareOptions options), static System.Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase, CultureInfo culture), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase, CultureInfo culture), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, CultureInfo culture, CompareOptions options), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, StringComparison comparisonType), System.Int32 System.String.CompareTo(Object value), System.Int32 System.String.CompareTo(String strB), static System.Int32 System.String.CompareOrdinal(String strA, String strB), static System.Int32 System.String.CompareOrdinal(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), System.Boolean System.String.Contains(String value), System.Boolean System.String.EndsWith(String value), System.Boolean System.String.EndsWith(String value, StringComparison comparisonType), System.Boolean System.String.EndsWith(String value, Boolean ignoreCase, CultureInfo culture), System.Int32 System.String.IndexOf(Char value), System.Int32 System.String.IndexOf(Char value, Int32 startIndex), System.Int32 System.String.IndexOf(Char value, Int32 startIndex, Int32 count), System.Int32 System.String.IndexOf(String value), System.Int32 System.String.IndexOf(String value, Int32 startIndex), System.Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count), System.Int32 System.String.IndexOf(String value, StringComparison comparisonType), System.Int32 System.String.IndexOf(String value, Int32 startIndex, StringComparison comparisonType), System.Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), System.Int32 System.String.IndexOfAny(Char[] anyOf), System.Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex), System.Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(Char value), System.Int32 System.String.LastIndexOf(Char value, Int32 startIndex), System.Int32 System.String.LastIndexOf(Char value, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(String value), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(String value, StringComparison comparisonType), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, StringComparison comparisonType), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), System.Int32 System.String.LastIndexOfAny(Char[] anyOf), System.Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex), System.Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), System.String System.String.PadLeft(Int32 totalWidth), System.String System.String.PadLeft(Int32 totalWidth, Char paddingChar), System.String System.String.PadRight(Int32 totalWidth), System.String System.String.PadRight(Int32 totalWidth, Char paddingChar), System.Boolean System.String.StartsWith(String value), System.Boolean System.String.StartsWith(String value, StringComparison comparisonType), System.Boolean System.String.StartsWith(String value, Boolean ignoreCase, CultureInfo culture), System.String System.String.ToLower(), System.String System.String.ToLower(CultureInfo culture), System.String System.String.ToLowerInvariant(), System.String System.String.ToUpper(), System.String System.String.ToUpper(CultureInfo culture), System.String System.String.ToUpperInvariant(), System.String System.String.ToString(), System.String System.String.ToString(IFormatProvider provider), System.Object System.String.Clone(), System.String System.String.Insert(Int32 startIndex, String value), System.String System.String.Replace(Char oldChar, Char newChar), System.String System.String.Replace(String oldValue, String newValue), System.String System.String.Remove(Int32 startIndex, Int32 count), System.String System.String.Remove(Int32 startIndex), static System.String System.String.Format(String format, Object arg0), static System.String System.String.Format(String format, Object arg0, Object arg1), static System.String System.String.Format(String format, Object arg0, Object arg1, Object arg2), static System.String System.String.Format(String format, Object[] args), static System.String System.String.Format(IFormatProvider provider, String format, Object[] args), static System.String System.String.Copy(String str), static System.String System.String.Concat(Object arg0), static System.String System.String.Concat(Object arg0, Object arg1), static System.String System.String.Concat(Object arg0, Object arg1, Object arg2), static System.String System.String.Concat(Object arg0, Object arg1, Object arg2, Object arg3), static System.String System.String.Concat(Object[] args), static System.String System.String.Concat(String str0, String str1), static System.String System.String.Concat(String str0, String str1, String str2), static System.String System.String.Concat(String str0, String str1, String str2, String str3), static System.String System.String.Concat(String[] values), static System.String System.String.Intern(String str), static System.String System.String.IsInterned(String str), System.TypeCode System.String.GetTypeCode(), System.CharEnumerator System.String.GetEnumerator(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Char Chars[Int32 index], System.Int32 Length}"
              IsClass="True"
              IsPrimitive="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="nullMyClass"
          Type="Debugger.Tests.DebugType_Tests+MyClass"
          Value="null">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.DebugType_Tests+MyClass"
              GetMembers="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsClass="True"
              IsNested="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="i"
          Type="System.Int32"
          Value="42">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.Int32"
              GetFields="{System.Int32 MaxValue, System.Int32 MinValue}"
              GetInterfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable`1[System.Int32], System.IEquatable`1[System.Int32]}"
              GetMembers="{System.Int32 MaxValue, System.Int32 MinValue, System.Int32 System.Int32.CompareTo(Object value), System.Int32 System.Int32.CompareTo(Int32 value), System.Boolean System.Int32.Equals(Object obj), System.Boolean System.Int32.Equals(Int32 obj), System.Int32 System.Int32.GetHashCode(), System.String System.Int32.ToString(), System.String System.Int32.ToString(String format), System.String System.Int32.ToString(IFormatProvider provider), System.String System.Int32.ToString(String format, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s), static System.Int32 System.Int32.Parse(String s, NumberStyles style), static System.Int32 System.Int32.Parse(String s, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static System.Boolean System.Int32.TryParse(String s, Int32 result), static System.Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), System.TypeCode System.Int32.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Int32 System.Int32.CompareTo(Object value), System.Int32 System.Int32.CompareTo(Int32 value), System.Boolean System.Int32.Equals(Object obj), System.Boolean System.Int32.Equals(Int32 obj), System.Int32 System.Int32.GetHashCode(), System.String System.Int32.ToString(), System.String System.Int32.ToString(String format), System.String System.Int32.ToString(IFormatProvider provider), System.String System.Int32.ToString(String format, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s), static System.Int32 System.Int32.Parse(String s, NumberStyles style), static System.Int32 System.Int32.Parse(String s, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static System.Boolean System.Int32.TryParse(String s, Int32 result), static System.Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), System.TypeCode System.Int32.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsInteger="True"
              IsPrimitive="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="b"
          Type="System.Boolean"
          Value="True">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.Boolean"
              GetFields="{System.String TrueString, System.String FalseString}"
              GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[System.Boolean], System.IEquatable`1[System.Boolean]}"
              GetMembers="{System.String TrueString, System.String FalseString, System.Int32 System.Boolean.GetHashCode(), System.String System.Boolean.ToString(), System.String System.Boolean.ToString(IFormatProvider provider), System.Boolean System.Boolean.Equals(Object obj), System.Boolean System.Boolean.Equals(Boolean obj), System.Int32 System.Boolean.CompareTo(Object obj), System.Int32 System.Boolean.CompareTo(Boolean value), static System.Boolean System.Boolean.Parse(String value), static System.Boolean System.Boolean.TryParse(String value, Boolean result), System.TypeCode System.Boolean.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Int32 System.Boolean.GetHashCode(), System.String System.Boolean.ToString(), System.String System.Boolean.ToString(IFormatProvider provider), System.Boolean System.Boolean.Equals(Object obj), System.Boolean System.Boolean.Equals(Boolean obj), System.Int32 System.Boolean.CompareTo(Object obj), System.Int32 System.Boolean.CompareTo(Boolean value), static System.Boolean System.Boolean.Parse(String value), static System.Boolean System.Boolean.TryParse(String value, Boolean result), System.TypeCode System.Boolean.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsPrimitive="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="c"
          Type="System.Char"
          Value="a">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.Char"
              GetFields="{System.Char MaxValue, System.Char MinValue}"
              GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[System.Char], System.IEquatable`1[System.Char]}"
              GetMembers="{System.Char MaxValue, System.Char MinValue, System.Int32 System.Char.GetHashCode(), System.Boolean System.Char.Equals(Object obj), System.Boolean System.Char.Equals(Char obj), System.Int32 System.Char.CompareTo(Object value), System.Int32 System.Char.CompareTo(Char value), System.String System.Char.ToString(), System.String System.Char.ToString(IFormatProvider provider), static System.String System.Char.ToString(Char c), static System.Char System.Char.Parse(String s), static System.Boolean System.Char.TryParse(String s, Char result), static System.Boolean System.Char.IsDigit(Char c), static System.Boolean System.Char.IsDigit(String s, Int32 index), static System.Boolean System.Char.IsLetter(Char c), static System.Boolean System.Char.IsLetter(String s, Int32 index), static System.Boolean System.Char.IsWhiteSpace(Char c), static System.Boolean System.Char.IsWhiteSpace(String s, Int32 index), static System.Boolean System.Char.IsUpper(Char c), static System.Boolean System.Char.IsUpper(String s, Int32 index), static System.Boolean System.Char.IsLower(Char c), static System.Boolean System.Char.IsLower(String s, Int32 index), static System.Boolean System.Char.IsPunctuation(Char c), static System.Boolean System.Char.IsPunctuation(String s, Int32 index), static System.Boolean System.Char.IsLetterOrDigit(Char c), static System.Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static System.Char System.Char.ToUpper(Char c, CultureInfo culture), static System.Char System.Char.ToUpper(Char c), static System.Char System.Char.ToUpperInvariant(Char c), static System.Char System.Char.ToLower(Char c, CultureInfo culture), static System.Char System.Char.ToLower(Char c), static System.Char System.Char.ToLowerInvariant(Char c), System.TypeCode System.Char.GetTypeCode(), static System.Boolean System.Char.IsControl(Char c), static System.Boolean System.Char.IsControl(String s, Int32 index), static System.Boolean System.Char.IsNumber(Char c), static System.Boolean System.Char.IsNumber(String s, Int32 index), static System.Boolean System.Char.IsSeparator(Char c), static System.Boolean System.Char.IsSeparator(String s, Int32 index), static System.Boolean System.Char.IsSurrogate(Char c), static System.Boolean System.Char.IsSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSymbol(Char c), static System.Boolean System.Char.IsSymbol(String s, Int32 index), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(Char c), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static System.Double System.Char.GetNumericValue(Char c), static System.Double System.Char.GetNumericValue(String s, Int32 index), static System.Boolean System.Char.IsHighSurrogate(Char c), static System.Boolean System.Char.IsHighSurrogate(String s, Int32 index), static System.Boolean System.Char.IsLowSurrogate(Char c), static System.Boolean System.Char.IsLowSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static System.String System.Char.ConvertFromUtf32(Int32 utf32), static System.Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static System.Int32 System.Char.ConvertToUtf32(String s, Int32 index), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Int32 System.Char.GetHashCode(), System.Boolean System.Char.Equals(Object obj), System.Boolean System.Char.Equals(Char obj), System.Int32 System.Char.CompareTo(Object value), System.Int32 System.Char.CompareTo(Char value), System.String System.Char.ToString(), System.String System.Char.ToString(IFormatProvider provider), static System.String System.Char.ToString(Char c), static System.Char System.Char.Parse(String s), static System.Boolean System.Char.TryParse(String s, Char result), static System.Boolean System.Char.IsDigit(Char c), static System.Boolean System.Char.IsDigit(String s, Int32 index), static System.Boolean System.Char.IsLetter(Char c), static System.Boolean System.Char.IsLetter(String s, Int32 index), static System.Boolean System.Char.IsWhiteSpace(Char c), static System.Boolean System.Char.IsWhiteSpace(String s, Int32 index), static System.Boolean System.Char.IsUpper(Char c), static System.Boolean System.Char.IsUpper(String s, Int32 index), static System.Boolean System.Char.IsLower(Char c), static System.Boolean System.Char.IsLower(String s, Int32 index), static System.Boolean System.Char.IsPunctuation(Char c), static System.Boolean System.Char.IsPunctuation(String s, Int32 index), static System.Boolean System.Char.IsLetterOrDigit(Char c), static System.Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static System.Char System.Char.ToUpper(Char c, CultureInfo culture), static System.Char System.Char.ToUpper(Char c), static System.Char System.Char.ToUpperInvariant(Char c), static System.Char System.Char.ToLower(Char c, CultureInfo culture), static System.Char System.Char.ToLower(Char c), static System.Char System.Char.ToLowerInvariant(Char c), System.TypeCode System.Char.GetTypeCode(), static System.Boolean System.Char.IsControl(Char c), static System.Boolean System.Char.IsControl(String s, Int32 index), static System.Boolean System.Char.IsNumber(Char c), static System.Boolean System.Char.IsNumber(String s, Int32 index), static System.Boolean System.Char.IsSeparator(Char c), static System.Boolean System.Char.IsSeparator(String s, Int32 index), static System.Boolean System.Char.IsSurrogate(Char c), static System.Boolean System.Char.IsSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSymbol(Char c), static System.Boolean System.Char.IsSymbol(String s, Int32 index), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(Char c), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static System.Double System.Char.GetNumericValue(Char c), static System.Double System.Char.GetNumericValue(String s, Int32 index), static System.Boolean System.Char.IsHighSurrogate(Char c), static System.Boolean System.Char.IsHighSurrogate(String s, Int32 index), static System.Boolean System.Char.IsLowSurrogate(Char c), static System.Boolean System.Char.IsLowSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static System.String System.Char.ConvertFromUtf32(Int32 utf32), static System.Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static System.Int32 System.Char.ConvertToUtf32(String s, Int32 index), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsPrimitive="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="s"
          Type="System.String"
          Value="abc">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.Object"
              FullName="System.String"
              GetFields="{System.String Empty}"
              GetInterfaces="{System.IComparable, System.ICloneable, System.IConvertible, System.IComparable`1[System.String], System.Collections.Generic.IEnumerable`1[System.Char], System.Collections.IEnumerable, System.IEquatable`1[System.String]}"
              GetMembers="{System.String Empty, static System.String System.String.Join(String separator, String[] value), static System.String System.String.Join(String separator, String[] value, Int32 startIndex, Int32 count), System.Boolean System.String.Equals(Object obj), System.Boolean System.String.Equals(String value), System.Boolean System.String.Equals(String value, StringComparison comparisonType), static System.Boolean System.String.Equals(String a, String b), static System.Boolean System.String.Equals(String a, String b, StringComparison comparisonType), static System.Boolean System.String.op_Equality(String a, String b), static System.Boolean System.String.op_Inequality(String a, String b), System.Char System.String.get_Chars(Int32 index), void System.String.CopyTo(Int32 sourceIndex, Char[] destination, Int32 destinationIndex, Int32 count), System.Char[] System.String.ToCharArray(), System.Char[] System.String.ToCharArray(Int32 startIndex, Int32 length), static System.Boolean System.String.IsNullOrEmpty(String value), System.Int32 System.String.GetHashCode(), System.Int32 System.String.get_Length(), System.String[] System.String.Split(Char[] separator), System.String[] System.String.Split(Char[] separator, Int32 count), System.String[] System.String.Split(Char[] separator, StringSplitOptions options), System.String[] System.String.Split(Char[] separator, Int32 count, StringSplitOptions options), System.String[] System.String.Split(String[] separator, StringSplitOptions options), System.String[] System.String.Split(String[] separator, Int32 count, StringSplitOptions options), System.String System.String.Substring(Int32 startIndex), System.String System.String.Substring(Int32 startIndex, Int32 length), System.String System.String.Trim(Char[] trimChars), System.String System.String.Trim(), System.String System.String.TrimStart(Char[] trimChars), System.String System.String.TrimEnd(Char[] trimChars), void System.String..ctor(Char* value), void System.String..ctor(Char* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length, Encoding enc), void System.String..ctor(Char[] value, Int32 startIndex, Int32 length), void System.String..ctor(Char[] value), void System.String..ctor(Char c, Int32 count), System.Boolean System.String.IsNormalized(), System.Boolean System.String.IsNormalized(NormalizationForm normalizationForm), System.String System.String.Normalize(), System.String System.String.Normalize(NormalizationForm normalizationForm), static System.Int32 System.String.Compare(String strA, String strB), static System.Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase), static System.Int32 System.String.Compare(String strA, String strB, StringComparison comparisonType), static System.Int32 System.String.Compare(String strA, String strB, CultureInfo culture, CompareOptions options), static System.Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase, CultureInfo culture), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase, CultureInfo culture), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, CultureInfo culture, CompareOptions options), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, StringComparison comparisonType), System.Int32 System.String.CompareTo(Object value), System.Int32 System.String.CompareTo(String strB), static System.Int32 System.String.CompareOrdinal(String strA, String strB), static System.Int32 System.String.CompareOrdinal(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), System.Boolean System.String.Contains(String value), System.Boolean System.String.EndsWith(String value), System.Boolean System.String.EndsWith(String value, StringComparison comparisonType), System.Boolean System.String.EndsWith(String value, Boolean ignoreCase, CultureInfo culture), System.Int32 System.String.IndexOf(Char value), System.Int32 System.String.IndexOf(Char value, Int32 startIndex), System.Int32 System.String.IndexOf(Char value, Int32 startIndex, Int32 count), System.Int32 System.String.IndexOf(String value), System.Int32 System.String.IndexOf(String value, Int32 startIndex), System.Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count), System.Int32 System.String.IndexOf(String value, StringComparison comparisonType), System.Int32 System.String.IndexOf(String value, Int32 startIndex, StringComparison comparisonType), System.Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), System.Int32 System.String.IndexOfAny(Char[] anyOf), System.Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex), System.Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(Char value), System.Int32 System.String.LastIndexOf(Char value, Int32 startIndex), System.Int32 System.String.LastIndexOf(Char value, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(String value), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(String value, StringComparison comparisonType), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, StringComparison comparisonType), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), System.Int32 System.String.LastIndexOfAny(Char[] anyOf), System.Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex), System.Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), System.String System.String.PadLeft(Int32 totalWidth), System.String System.String.PadLeft(Int32 totalWidth, Char paddingChar), System.String System.String.PadRight(Int32 totalWidth), System.String System.String.PadRight(Int32 totalWidth, Char paddingChar), System.Boolean System.String.StartsWith(String value), System.Boolean System.String.StartsWith(String value, StringComparison comparisonType), System.Boolean System.String.StartsWith(String value, Boolean ignoreCase, CultureInfo culture), System.String System.String.ToLower(), System.String System.String.ToLower(CultureInfo culture), System.String System.String.ToLowerInvariant(), System.String System.String.ToUpper(), System.String System.String.ToUpper(CultureInfo culture), System.String System.String.ToUpperInvariant(), System.String System.String.ToString(), System.String System.String.ToString(IFormatProvider provider), System.Object System.String.Clone(), System.String System.String.Insert(Int32 startIndex, String value), System.String System.String.Replace(Char oldChar, Char newChar), System.String System.String.Replace(String oldValue, String newValue), System.String System.String.Remove(Int32 startIndex, Int32 count), System.String System.String.Remove(Int32 startIndex), static System.String System.String.Format(String format, Object arg0), static System.String System.String.Format(String format, Object arg0, Object arg1), static System.String System.String.Format(String format, Object arg0, Object arg1, Object arg2), static System.String System.String.Format(String format, Object[] args), static System.String System.String.Format(IFormatProvider provider, String format, Object[] args), static System.String System.String.Copy(String str), static System.String System.String.Concat(Object arg0), static System.String System.String.Concat(Object arg0, Object arg1), static System.String System.String.Concat(Object arg0, Object arg1, Object arg2), static System.String System.String.Concat(Object arg0, Object arg1, Object arg2, Object arg3), static System.String System.String.Concat(Object[] args), static System.String System.String.Concat(String str0, String str1), static System.String System.String.Concat(String str0, String str1, String str2), static System.String System.String.Concat(String str0, String str1, String str2, String str3), static System.String System.String.Concat(String[] values), static System.String System.String.Intern(String str), static System.String System.String.IsInterned(String str), System.TypeCode System.String.GetTypeCode(), System.CharEnumerator System.String.GetEnumerator(), System.Char Chars[Int32 index], System.Int32 Length, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{static System.String System.String.Join(String separator, String[] value), static System.String System.String.Join(String separator, String[] value, Int32 startIndex, Int32 count), System.Boolean System.String.Equals(Object obj), System.Boolean System.String.Equals(String value), System.Boolean System.String.Equals(String value, StringComparison comparisonType), static System.Boolean System.String.Equals(String a, String b), static System.Boolean System.String.Equals(String a, String b, StringComparison comparisonType), static System.Boolean System.String.op_Equality(String a, String b), static System.Boolean System.String.op_Inequality(String a, String b), System.Char System.String.get_Chars(Int32 index), void System.String.CopyTo(Int32 sourceIndex, Char[] destination, Int32 destinationIndex, Int32 count), System.Char[] System.String.ToCharArray(), System.Char[] System.String.ToCharArray(Int32 startIndex, Int32 length), static System.Boolean System.String.IsNullOrEmpty(String value), System.Int32 System.String.GetHashCode(), System.Int32 System.String.get_Length(), System.String[] System.String.Split(Char[] separator), System.String[] System.String.Split(Char[] separator, Int32 count), System.String[] System.String.Split(Char[] separator, StringSplitOptions options), System.String[] System.String.Split(Char[] separator, Int32 count, StringSplitOptions options), System.String[] System.String.Split(String[] separator, StringSplitOptions options), System.String[] System.String.Split(String[] separator, Int32 count, StringSplitOptions options), System.String System.String.Substring(Int32 startIndex), System.String System.String.Substring(Int32 startIndex, Int32 length), System.String System.String.Trim(Char[] trimChars), System.String System.String.Trim(), System.String System.String.TrimStart(Char[] trimChars), System.String System.String.TrimEnd(Char[] trimChars), void System.String..ctor(Char* value), void System.String..ctor(Char* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length, Encoding enc), void System.String..ctor(Char[] value, Int32 startIndex, Int32 length), void System.String..ctor(Char[] value), void System.String..ctor(Char c, Int32 count), System.Boolean System.String.IsNormalized(), System.Boolean System.String.IsNormalized(NormalizationForm normalizationForm), System.String System.String.Normalize(), System.String System.String.Normalize(NormalizationForm normalizationForm), static System.Int32 System.String.Compare(String strA, String strB), static System.Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase), static System.Int32 System.String.Compare(String strA, String strB, StringComparison comparisonType), static System.Int32 System.String.Compare(String strA, String strB, CultureInfo culture, CompareOptions options), static System.Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase, CultureInfo culture), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase, CultureInfo culture), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, CultureInfo culture, CompareOptions options), static System.Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, StringComparison comparisonType), System.Int32 System.String.CompareTo(Object value), System.Int32 System.String.CompareTo(String strB), static System.Int32 System.String.CompareOrdinal(String strA, String strB), static System.Int32 System.String.CompareOrdinal(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), System.Boolean System.String.Contains(String value), System.Boolean System.String.EndsWith(String value), System.Boolean System.String.EndsWith(String value, StringComparison comparisonType), System.Boolean System.String.EndsWith(String value, Boolean ignoreCase, CultureInfo culture), System.Int32 System.String.IndexOf(Char value), System.Int32 System.String.IndexOf(Char value, Int32 startIndex), System.Int32 System.String.IndexOf(Char value, Int32 startIndex, Int32 count), System.Int32 System.String.IndexOf(String value), System.Int32 System.String.IndexOf(String value, Int32 startIndex), System.Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count), System.Int32 System.String.IndexOf(String value, StringComparison comparisonType), System.Int32 System.String.IndexOf(String value, Int32 startIndex, StringComparison comparisonType), System.Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), System.Int32 System.String.IndexOfAny(Char[] anyOf), System.Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex), System.Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(Char value), System.Int32 System.String.LastIndexOf(Char value, Int32 startIndex), System.Int32 System.String.LastIndexOf(Char value, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(String value), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count), System.Int32 System.String.LastIndexOf(String value, StringComparison comparisonType), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, StringComparison comparisonType), System.Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), System.Int32 System.String.LastIndexOfAny(Char[] anyOf), System.Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex), System.Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), System.String System.String.PadLeft(Int32 totalWidth), System.String System.String.PadLeft(Int32 totalWidth, Char paddingChar), System.String System.String.PadRight(Int32 totalWidth), System.String System.String.PadRight(Int32 totalWidth, Char paddingChar), System.Boolean System.String.StartsWith(String value), System.Boolean System.String.StartsWith(String value, StringComparison comparisonType), System.Boolean System.String.StartsWith(String value, Boolean ignoreCase, CultureInfo culture), System.String System.String.ToLower(), System.String System.String.ToLower(CultureInfo culture), System.String System.String.ToLowerInvariant(), System.String System.String.ToUpper(), System.String System.String.ToUpper(CultureInfo culture), System.String System.String.ToUpperInvariant(), System.String System.String.ToString(), System.String System.String.ToString(IFormatProvider provider), System.Object System.String.Clone(), System.String System.String.Insert(Int32 startIndex, String value), System.String System.String.Replace(Char oldChar, Char newChar), System.String System.String.Replace(String oldValue, String newValue), System.String System.String.Remove(Int32 startIndex, Int32 count), System.String System.String.Remove(Int32 startIndex), static System.String System.String.Format(String format, Object arg0), static System.String System.String.Format(String format, Object arg0, Object arg1), static System.String System.String.Format(String format, Object arg0, Object arg1, Object arg2), static System.String System.String.Format(String format, Object[] args), static System.String System.String.Format(IFormatProvider provider, String format, Object[] args), static System.String System.String.Copy(String str), static System.String System.String.Concat(Object arg0), static System.String System.String.Concat(Object arg0, Object arg1), static System.String System.String.Concat(Object arg0, Object arg1, Object arg2), static System.String System.String.Concat(Object arg0, Object arg1, Object arg2, Object arg3), static System.String System.String.Concat(Object[] args), static System.String System.String.Concat(String str0, String str1), static System.String System.String.Concat(String str0, String str1, String str2), static System.String System.String.Concat(String str0, String str1, String str2, String str3), static System.String System.String.Concat(String[] values), static System.String System.String.Intern(String str), static System.String System.String.IsInterned(String str), System.TypeCode System.String.GetTypeCode(), System.CharEnumerator System.String.GetEnumerator(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Char Chars[Int32 index], System.Int32 Length}"
              IsClass="True"
              IsPrimitive="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="obj"
          Type="System.Object"
          Value="{System.Object}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, Serializable, BeforeFieldInit"
              FullName="System.Object"
              GetMembers="{void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), static System.Boolean System.Object.Equals(Object objA, Object objB), static System.Boolean System.Object.ReferenceEquals(Object objA, Object objB), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), static System.Boolean System.Object.Equals(Object objA, Object objB), static System.Boolean System.Object.ReferenceEquals(Object objA, Object objB), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsClass="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myClass"
          Type="Debugger.Tests.DebugType_Tests+MyClass"
          Value="{Debugger.Tests.DebugType_Tests+MyClass}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.DebugType_Tests+MyClass"
              GetMembers="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsClass="True"
              IsNested="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myStruct"
          Type="Debugger.Tests.DebugType_Tests+MyStruct"
          Value="{Debugger.Tests.DebugType_Tests+MyStruct}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="Debugger.Tests.DebugType_Tests+MyStruct"
              GetMembers="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsNested="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="box"
          Type="System.Object"
          Value="40">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, Serializable, BeforeFieldInit"
              FullName="System.Object"
              GetMembers="{void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), static System.Boolean System.Object.Equals(Object objA, Object objB), static System.Boolean System.Object.ReferenceEquals(Object objA, Object objB), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), static System.Boolean System.Object.Equals(Object objA, Object objB), static System.Boolean System.Object.ReferenceEquals(Object objA, Object objB), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsClass="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="iPtr"
          Type="System.Int32*"
          Value="{System.Int32*}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              FullName="System.Int32*"
              GetElementType="System.Int32"
              HasElementType="True"
              IsClass="True"
              IsCompilerGenerated="True"
              IsPointer="True">
              <GetElementType>
                <DebugType
                  Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
                  BaseType="System.ValueType"
                  FullName="System.Int32"
                  GetFields="{System.Int32 MaxValue, System.Int32 MinValue}"
                  GetInterfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable`1[System.Int32], System.IEquatable`1[System.Int32]}"
                  GetMembers="{System.Int32 MaxValue, System.Int32 MinValue, System.Int32 System.Int32.CompareTo(Object value), System.Int32 System.Int32.CompareTo(Int32 value), System.Boolean System.Int32.Equals(Object obj), System.Boolean System.Int32.Equals(Int32 obj), System.Int32 System.Int32.GetHashCode(), System.String System.Int32.ToString(), System.String System.Int32.ToString(String format), System.String System.Int32.ToString(IFormatProvider provider), System.String System.Int32.ToString(String format, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s), static System.Int32 System.Int32.Parse(String s, NumberStyles style), static System.Int32 System.Int32.Parse(String s, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static System.Boolean System.Int32.TryParse(String s, Int32 result), static System.Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), System.TypeCode System.Int32.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  GetMethods="{System.Int32 System.Int32.CompareTo(Object value), System.Int32 System.Int32.CompareTo(Int32 value), System.Boolean System.Int32.Equals(Object obj), System.Boolean System.Int32.Equals(Int32 obj), System.Int32 System.Int32.GetHashCode(), System.String System.Int32.ToString(), System.String System.Int32.ToString(String format), System.String System.Int32.ToString(IFormatProvider provider), System.String System.Int32.ToString(String format, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s), static System.Int32 System.Int32.Parse(String s, NumberStyles style), static System.Int32 System.Int32.Parse(String s, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static System.Boolean System.Int32.TryParse(String s, Int32 result), static System.Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), System.TypeCode System.Int32.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  IsInteger="True"
                  IsPrimitive="True"
                  IsValueType="True">
                  <GetElementType>null</GetElementType>
                </DebugType>
              </GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="iPtrPtr"
          Type="System.Int32**"
          Value="{System.Int32**}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              FullName="System.Int32**"
              GetElementType="System.Int32*"
              HasElementType="True"
              IsClass="True"
              IsCompilerGenerated="True"
              IsPointer="True">
              <GetElementType>
                <DebugType
                  Attributes="NotPublic"
                  FullName="System.Int32*"
                  GetElementType="System.Int32"
                  HasElementType="True"
                  IsClass="True"
                  IsCompilerGenerated="True"
                  IsPointer="True">
                  <GetElementType>
                    <DebugType
                      Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
                      BaseType="System.ValueType"
                      FullName="System.Int32"
                      GetFields="{System.Int32 MaxValue, System.Int32 MinValue}"
                      GetInterfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable`1[System.Int32], System.IEquatable`1[System.Int32]}"
                      GetMembers="{System.Int32 MaxValue, System.Int32 MinValue, System.Int32 System.Int32.CompareTo(Object value), System.Int32 System.Int32.CompareTo(Int32 value), System.Boolean System.Int32.Equals(Object obj), System.Boolean System.Int32.Equals(Int32 obj), System.Int32 System.Int32.GetHashCode(), System.String System.Int32.ToString(), System.String System.Int32.ToString(String format), System.String System.Int32.ToString(IFormatProvider provider), System.String System.Int32.ToString(String format, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s), static System.Int32 System.Int32.Parse(String s, NumberStyles style), static System.Int32 System.Int32.Parse(String s, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static System.Boolean System.Int32.TryParse(String s, Int32 result), static System.Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), System.TypeCode System.Int32.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                      GetMethods="{System.Int32 System.Int32.CompareTo(Object value), System.Int32 System.Int32.CompareTo(Int32 value), System.Boolean System.Int32.Equals(Object obj), System.Boolean System.Int32.Equals(Int32 obj), System.Int32 System.Int32.GetHashCode(), System.String System.Int32.ToString(), System.String System.Int32.ToString(String format), System.String System.Int32.ToString(IFormatProvider provider), System.String System.Int32.ToString(String format, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s), static System.Int32 System.Int32.Parse(String s, NumberStyles style), static System.Int32 System.Int32.Parse(String s, IFormatProvider provider), static System.Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static System.Boolean System.Int32.TryParse(String s, Int32 result), static System.Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), System.TypeCode System.Int32.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                      IsInteger="True"
                      IsPrimitive="True"
                      IsValueType="True">
                      <GetElementType>null</GetElementType>
                    </DebugType>
                  </GetElementType>
                </DebugType>
              </GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="bPtr"
          Type="System.Boolean*"
          Value="{System.Boolean*}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              FullName="System.Boolean*"
              GetElementType="System.Boolean"
              HasElementType="True"
              IsClass="True"
              IsCompilerGenerated="True"
              IsPointer="True">
              <GetElementType>
                <DebugType
                  Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
                  BaseType="System.ValueType"
                  FullName="System.Boolean"
                  GetFields="{System.String TrueString, System.String FalseString}"
                  GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[System.Boolean], System.IEquatable`1[System.Boolean]}"
                  GetMembers="{System.String TrueString, System.String FalseString, System.Int32 System.Boolean.GetHashCode(), System.String System.Boolean.ToString(), System.String System.Boolean.ToString(IFormatProvider provider), System.Boolean System.Boolean.Equals(Object obj), System.Boolean System.Boolean.Equals(Boolean obj), System.Int32 System.Boolean.CompareTo(Object obj), System.Int32 System.Boolean.CompareTo(Boolean value), static System.Boolean System.Boolean.Parse(String value), static System.Boolean System.Boolean.TryParse(String value, Boolean result), System.TypeCode System.Boolean.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  GetMethods="{System.Int32 System.Boolean.GetHashCode(), System.String System.Boolean.ToString(), System.String System.Boolean.ToString(IFormatProvider provider), System.Boolean System.Boolean.Equals(Object obj), System.Boolean System.Boolean.Equals(Boolean obj), System.Int32 System.Boolean.CompareTo(Object obj), System.Int32 System.Boolean.CompareTo(Boolean value), static System.Boolean System.Boolean.Parse(String value), static System.Boolean System.Boolean.TryParse(String value, Boolean result), System.TypeCode System.Boolean.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  IsPrimitive="True"
                  IsValueType="True">
                  <GetElementType>null</GetElementType>
                </DebugType>
              </GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="voidPtr"
          Type="System.Void*"
          Value="{System.Void*}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              FullName="System.Void*"
              GetElementType="System.Void"
              HasElementType="True"
              IsClass="True"
              IsCompilerGenerated="True"
              IsPointer="True">
              <GetElementType>
                <DebugType
                  Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
                  BaseType="System.ValueType"
                  FullName="System.Void"
                  GetMembers="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  GetMethods="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  IsValueType="True">
                  <GetElementType>null</GetElementType>
                </DebugType>
              </GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myStructPtr"
          Type="Debugger.Tests.DebugType_Tests+MyStruct*"
          Value="{Debugger.Tests.DebugType_Tests+MyStruct*}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              FullName="Debugger.Tests.DebugType_Tests+MyStruct*"
              GetElementType="Debugger.Tests.DebugType_Tests+MyStruct"
              HasElementType="True"
              IsClass="True"
              IsCompilerGenerated="True"
              IsPointer="True">
              <GetElementType>
                <DebugType
                  Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, BeforeFieldInit"
                  BaseType="System.ValueType"
                  FullName="Debugger.Tests.DebugType_Tests+MyStruct"
                  GetMembers="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  GetMethods="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  IsNested="True"
                  IsValueType="True">
                  <GetElementType>null</GetElementType>
                </DebugType>
              </GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="ptr"
          Type="System.IntPtr"
          Value="{System.IntPtr}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.IntPtr"
              GetFields="{System.IntPtr Zero}"
              GetInterfaces="{System.Runtime.Serialization.ISerializable}"
              GetMembers="{System.IntPtr Zero, void System.IntPtr..ctor(Int32 value), void System.IntPtr..ctor(Int64 value), void System.IntPtr..ctor(Void* value), System.Boolean System.IntPtr.Equals(Object obj), System.Int32 System.IntPtr.GetHashCode(), System.Int32 System.IntPtr.ToInt32(), System.Int64 System.IntPtr.ToInt64(), System.String System.IntPtr.ToString(), System.String System.IntPtr.ToString(String format), static System.IntPtr System.IntPtr.op_Explicit(Int32 value), static System.IntPtr System.IntPtr.op_Explicit(Int64 value), static System.IntPtr System.IntPtr.op_Explicit(Void* value), static System.Void* System.IntPtr.op_Explicit(IntPtr value), static System.Int32 System.IntPtr.op_Explicit(IntPtr value), static System.Int64 System.IntPtr.op_Explicit(IntPtr value), static System.Boolean System.IntPtr.op_Equality(IntPtr value1, IntPtr value2), static System.Boolean System.IntPtr.op_Inequality(IntPtr value1, IntPtr value2), static System.Int32 System.IntPtr.get_Size(), System.Void* System.IntPtr.ToPointer(), System.Int32 Size, System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void System.IntPtr..ctor(Int32 value), void System.IntPtr..ctor(Int64 value), void System.IntPtr..ctor(Void* value), System.Boolean System.IntPtr.Equals(Object obj), System.Int32 System.IntPtr.GetHashCode(), System.Int32 System.IntPtr.ToInt32(), System.Int64 System.IntPtr.ToInt64(), System.String System.IntPtr.ToString(), System.String System.IntPtr.ToString(String format), static System.IntPtr System.IntPtr.op_Explicit(Int32 value), static System.IntPtr System.IntPtr.op_Explicit(Int64 value), static System.IntPtr System.IntPtr.op_Explicit(Void* value), static System.Void* System.IntPtr.op_Explicit(IntPtr value), static System.Int32 System.IntPtr.op_Explicit(IntPtr value), static System.Int64 System.IntPtr.op_Explicit(IntPtr value), static System.Boolean System.IntPtr.op_Equality(IntPtr value1, IntPtr value2), static System.Boolean System.IntPtr.op_Inequality(IntPtr value1, IntPtr value2), static System.Int32 System.IntPtr.get_Size(), System.Void* System.IntPtr.ToPointer(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Int32 Size}"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="szArray"
          Type="System.Char[]"
          Value="{System.Char[]}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              BaseType="System.Array"
              FullName="System.Char[]"
              GetArrayRank="1"
              GetElementType="System.Char"
              GetInterfaces="{System.Collections.Generic.IList`1[System.Char], System.Collections.Generic.ICollection`1[System.Char], System.Collections.Generic.IEnumerable`1[System.Char], System.Collections.IEnumerable}"
              GetMembers="{System.Object System.Array.GetValue(Int32[] indices), System.Object System.Array.GetValue(Int32 index), System.Object System.Array.GetValue(Int32 index1, Int32 index2), System.Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), System.Object System.Array.GetValue(Int64 index), System.Object System.Array.GetValue(Int64 index1, Int64 index2), System.Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), System.Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), System.Int32 System.Array.get_Length(), System.Int64 System.Array.get_LongLength(), System.Int32 System.Array.GetLength(Int32 dimension), System.Int64 System.Array.GetLongLength(Int32 dimension), System.Int32 System.Array.get_Rank(), System.Int32 System.Array.GetUpperBound(Int32 dimension), System.Int32 System.Array.GetLowerBound(Int32 dimension), System.Object System.Array.get_SyncRoot(), System.Boolean System.Array.get_IsReadOnly(), System.Boolean System.Array.get_IsFixedSize(), System.Boolean System.Array.get_IsSynchronized(), System.Object System.Array.Clone(), System.Int32 System.Array.CompareTo(Object other, IComparer comparer), System.Boolean System.Array.Equals(Object other, IEqualityComparer comparer), System.Int32 System.Array.GetHashCode(IEqualityComparer comparer), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), System.Collections.IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Object System.Array.GetValue(Int32[] indices), System.Object System.Array.GetValue(Int32 index), System.Object System.Array.GetValue(Int32 index1, Int32 index2), System.Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), System.Object System.Array.GetValue(Int64 index), System.Object System.Array.GetValue(Int64 index1, Int64 index2), System.Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), System.Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), System.Int32 System.Array.get_Length(), System.Int64 System.Array.get_LongLength(), System.Int32 System.Array.GetLength(Int32 dimension), System.Int64 System.Array.GetLongLength(Int32 dimension), System.Int32 System.Array.get_Rank(), System.Int32 System.Array.GetUpperBound(Int32 dimension), System.Int32 System.Array.GetLowerBound(Int32 dimension), System.Object System.Array.get_SyncRoot(), System.Boolean System.Array.get_IsReadOnly(), System.Boolean System.Array.get_IsFixedSize(), System.Boolean System.Array.get_IsSynchronized(), System.Object System.Array.Clone(), System.Int32 System.Array.CompareTo(Object other, IComparer comparer), System.Boolean System.Array.Equals(Object other, IEqualityComparer comparer), System.Int32 System.Array.GetHashCode(IEqualityComparer comparer), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), System.Collections.IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized}"
              HasElementType="True"
              IsArray="True"
              IsClass="True"
              IsCompilerGenerated="True">
              <GetElementType>
                <DebugType
                  Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
                  BaseType="System.ValueType"
                  FullName="System.Char"
                  GetFields="{System.Char MaxValue, System.Char MinValue}"
                  GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[System.Char], System.IEquatable`1[System.Char]}"
                  GetMembers="{System.Char MaxValue, System.Char MinValue, System.Int32 System.Char.GetHashCode(), System.Boolean System.Char.Equals(Object obj), System.Boolean System.Char.Equals(Char obj), System.Int32 System.Char.CompareTo(Object value), System.Int32 System.Char.CompareTo(Char value), System.String System.Char.ToString(), System.String System.Char.ToString(IFormatProvider provider), static System.String System.Char.ToString(Char c), static System.Char System.Char.Parse(String s), static System.Boolean System.Char.TryParse(String s, Char result), static System.Boolean System.Char.IsDigit(Char c), static System.Boolean System.Char.IsDigit(String s, Int32 index), static System.Boolean System.Char.IsLetter(Char c), static System.Boolean System.Char.IsLetter(String s, Int32 index), static System.Boolean System.Char.IsWhiteSpace(Char c), static System.Boolean System.Char.IsWhiteSpace(String s, Int32 index), static System.Boolean System.Char.IsUpper(Char c), static System.Boolean System.Char.IsUpper(String s, Int32 index), static System.Boolean System.Char.IsLower(Char c), static System.Boolean System.Char.IsLower(String s, Int32 index), static System.Boolean System.Char.IsPunctuation(Char c), static System.Boolean System.Char.IsPunctuation(String s, Int32 index), static System.Boolean System.Char.IsLetterOrDigit(Char c), static System.Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static System.Char System.Char.ToUpper(Char c, CultureInfo culture), static System.Char System.Char.ToUpper(Char c), static System.Char System.Char.ToUpperInvariant(Char c), static System.Char System.Char.ToLower(Char c, CultureInfo culture), static System.Char System.Char.ToLower(Char c), static System.Char System.Char.ToLowerInvariant(Char c), System.TypeCode System.Char.GetTypeCode(), static System.Boolean System.Char.IsControl(Char c), static System.Boolean System.Char.IsControl(String s, Int32 index), static System.Boolean System.Char.IsNumber(Char c), static System.Boolean System.Char.IsNumber(String s, Int32 index), static System.Boolean System.Char.IsSeparator(Char c), static System.Boolean System.Char.IsSeparator(String s, Int32 index), static System.Boolean System.Char.IsSurrogate(Char c), static System.Boolean System.Char.IsSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSymbol(Char c), static System.Boolean System.Char.IsSymbol(String s, Int32 index), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(Char c), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static System.Double System.Char.GetNumericValue(Char c), static System.Double System.Char.GetNumericValue(String s, Int32 index), static System.Boolean System.Char.IsHighSurrogate(Char c), static System.Boolean System.Char.IsHighSurrogate(String s, Int32 index), static System.Boolean System.Char.IsLowSurrogate(Char c), static System.Boolean System.Char.IsLowSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static System.String System.Char.ConvertFromUtf32(Int32 utf32), static System.Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static System.Int32 System.Char.ConvertToUtf32(String s, Int32 index), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  GetMethods="{System.Int32 System.Char.GetHashCode(), System.Boolean System.Char.Equals(Object obj), System.Boolean System.Char.Equals(Char obj), System.Int32 System.Char.CompareTo(Object value), System.Int32 System.Char.CompareTo(Char value), System.String System.Char.ToString(), System.String System.Char.ToString(IFormatProvider provider), static System.String System.Char.ToString(Char c), static System.Char System.Char.Parse(String s), static System.Boolean System.Char.TryParse(String s, Char result), static System.Boolean System.Char.IsDigit(Char c), static System.Boolean System.Char.IsDigit(String s, Int32 index), static System.Boolean System.Char.IsLetter(Char c), static System.Boolean System.Char.IsLetter(String s, Int32 index), static System.Boolean System.Char.IsWhiteSpace(Char c), static System.Boolean System.Char.IsWhiteSpace(String s, Int32 index), static System.Boolean System.Char.IsUpper(Char c), static System.Boolean System.Char.IsUpper(String s, Int32 index), static System.Boolean System.Char.IsLower(Char c), static System.Boolean System.Char.IsLower(String s, Int32 index), static System.Boolean System.Char.IsPunctuation(Char c), static System.Boolean System.Char.IsPunctuation(String s, Int32 index), static System.Boolean System.Char.IsLetterOrDigit(Char c), static System.Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static System.Char System.Char.ToUpper(Char c, CultureInfo culture), static System.Char System.Char.ToUpper(Char c), static System.Char System.Char.ToUpperInvariant(Char c), static System.Char System.Char.ToLower(Char c, CultureInfo culture), static System.Char System.Char.ToLower(Char c), static System.Char System.Char.ToLowerInvariant(Char c), System.TypeCode System.Char.GetTypeCode(), static System.Boolean System.Char.IsControl(Char c), static System.Boolean System.Char.IsControl(String s, Int32 index), static System.Boolean System.Char.IsNumber(Char c), static System.Boolean System.Char.IsNumber(String s, Int32 index), static System.Boolean System.Char.IsSeparator(Char c), static System.Boolean System.Char.IsSeparator(String s, Int32 index), static System.Boolean System.Char.IsSurrogate(Char c), static System.Boolean System.Char.IsSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSymbol(Char c), static System.Boolean System.Char.IsSymbol(String s, Int32 index), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(Char c), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static System.Double System.Char.GetNumericValue(Char c), static System.Double System.Char.GetNumericValue(String s, Int32 index), static System.Boolean System.Char.IsHighSurrogate(Char c), static System.Boolean System.Char.IsHighSurrogate(String s, Int32 index), static System.Boolean System.Char.IsLowSurrogate(Char c), static System.Boolean System.Char.IsLowSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static System.String System.Char.ConvertFromUtf32(Int32 utf32), static System.Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static System.Int32 System.Char.ConvertToUtf32(String s, Int32 index), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  IsPrimitive="True"
                  IsValueType="True">
                  <GetElementType>null</GetElementType>
                </DebugType>
              </GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="mdArray"
          Type="System.Char[,]"
          Value="{System.Char[,]}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              BaseType="System.Array"
              FullName="System.Char[,]"
              GetArrayRank="2"
              GetElementType="System.Char"
              GetInterfaces="{System.Collections.Generic.IList`1[System.Char], System.Collections.Generic.ICollection`1[System.Char], System.Collections.Generic.IEnumerable`1[System.Char], System.Collections.IEnumerable}"
              GetMembers="{System.Object System.Array.GetValue(Int32[] indices), System.Object System.Array.GetValue(Int32 index), System.Object System.Array.GetValue(Int32 index1, Int32 index2), System.Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), System.Object System.Array.GetValue(Int64 index), System.Object System.Array.GetValue(Int64 index1, Int64 index2), System.Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), System.Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), System.Int32 System.Array.get_Length(), System.Int64 System.Array.get_LongLength(), System.Int32 System.Array.GetLength(Int32 dimension), System.Int64 System.Array.GetLongLength(Int32 dimension), System.Int32 System.Array.get_Rank(), System.Int32 System.Array.GetUpperBound(Int32 dimension), System.Int32 System.Array.GetLowerBound(Int32 dimension), System.Object System.Array.get_SyncRoot(), System.Boolean System.Array.get_IsReadOnly(), System.Boolean System.Array.get_IsFixedSize(), System.Boolean System.Array.get_IsSynchronized(), System.Object System.Array.Clone(), System.Int32 System.Array.CompareTo(Object other, IComparer comparer), System.Boolean System.Array.Equals(Object other, IEqualityComparer comparer), System.Int32 System.Array.GetHashCode(IEqualityComparer comparer), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), System.Collections.IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Object System.Array.GetValue(Int32[] indices), System.Object System.Array.GetValue(Int32 index), System.Object System.Array.GetValue(Int32 index1, Int32 index2), System.Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), System.Object System.Array.GetValue(Int64 index), System.Object System.Array.GetValue(Int64 index1, Int64 index2), System.Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), System.Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), System.Int32 System.Array.get_Length(), System.Int64 System.Array.get_LongLength(), System.Int32 System.Array.GetLength(Int32 dimension), System.Int64 System.Array.GetLongLength(Int32 dimension), System.Int32 System.Array.get_Rank(), System.Int32 System.Array.GetUpperBound(Int32 dimension), System.Int32 System.Array.GetLowerBound(Int32 dimension), System.Object System.Array.get_SyncRoot(), System.Boolean System.Array.get_IsReadOnly(), System.Boolean System.Array.get_IsFixedSize(), System.Boolean System.Array.get_IsSynchronized(), System.Object System.Array.Clone(), System.Int32 System.Array.CompareTo(Object other, IComparer comparer), System.Boolean System.Array.Equals(Object other, IEqualityComparer comparer), System.Int32 System.Array.GetHashCode(IEqualityComparer comparer), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), System.Collections.IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized}"
              HasElementType="True"
              IsArray="True"
              IsClass="True"
              IsCompilerGenerated="True">
              <GetElementType>
                <DebugType
                  Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
                  BaseType="System.ValueType"
                  FullName="System.Char"
                  GetFields="{System.Char MaxValue, System.Char MinValue}"
                  GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[System.Char], System.IEquatable`1[System.Char]}"
                  GetMembers="{System.Char MaxValue, System.Char MinValue, System.Int32 System.Char.GetHashCode(), System.Boolean System.Char.Equals(Object obj), System.Boolean System.Char.Equals(Char obj), System.Int32 System.Char.CompareTo(Object value), System.Int32 System.Char.CompareTo(Char value), System.String System.Char.ToString(), System.String System.Char.ToString(IFormatProvider provider), static System.String System.Char.ToString(Char c), static System.Char System.Char.Parse(String s), static System.Boolean System.Char.TryParse(String s, Char result), static System.Boolean System.Char.IsDigit(Char c), static System.Boolean System.Char.IsDigit(String s, Int32 index), static System.Boolean System.Char.IsLetter(Char c), static System.Boolean System.Char.IsLetter(String s, Int32 index), static System.Boolean System.Char.IsWhiteSpace(Char c), static System.Boolean System.Char.IsWhiteSpace(String s, Int32 index), static System.Boolean System.Char.IsUpper(Char c), static System.Boolean System.Char.IsUpper(String s, Int32 index), static System.Boolean System.Char.IsLower(Char c), static System.Boolean System.Char.IsLower(String s, Int32 index), static System.Boolean System.Char.IsPunctuation(Char c), static System.Boolean System.Char.IsPunctuation(String s, Int32 index), static System.Boolean System.Char.IsLetterOrDigit(Char c), static System.Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static System.Char System.Char.ToUpper(Char c, CultureInfo culture), static System.Char System.Char.ToUpper(Char c), static System.Char System.Char.ToUpperInvariant(Char c), static System.Char System.Char.ToLower(Char c, CultureInfo culture), static System.Char System.Char.ToLower(Char c), static System.Char System.Char.ToLowerInvariant(Char c), System.TypeCode System.Char.GetTypeCode(), static System.Boolean System.Char.IsControl(Char c), static System.Boolean System.Char.IsControl(String s, Int32 index), static System.Boolean System.Char.IsNumber(Char c), static System.Boolean System.Char.IsNumber(String s, Int32 index), static System.Boolean System.Char.IsSeparator(Char c), static System.Boolean System.Char.IsSeparator(String s, Int32 index), static System.Boolean System.Char.IsSurrogate(Char c), static System.Boolean System.Char.IsSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSymbol(Char c), static System.Boolean System.Char.IsSymbol(String s, Int32 index), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(Char c), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static System.Double System.Char.GetNumericValue(Char c), static System.Double System.Char.GetNumericValue(String s, Int32 index), static System.Boolean System.Char.IsHighSurrogate(Char c), static System.Boolean System.Char.IsHighSurrogate(String s, Int32 index), static System.Boolean System.Char.IsLowSurrogate(Char c), static System.Boolean System.Char.IsLowSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static System.String System.Char.ConvertFromUtf32(Int32 utf32), static System.Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static System.Int32 System.Char.ConvertToUtf32(String s, Int32 index), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  GetMethods="{System.Int32 System.Char.GetHashCode(), System.Boolean System.Char.Equals(Object obj), System.Boolean System.Char.Equals(Char obj), System.Int32 System.Char.CompareTo(Object value), System.Int32 System.Char.CompareTo(Char value), System.String System.Char.ToString(), System.String System.Char.ToString(IFormatProvider provider), static System.String System.Char.ToString(Char c), static System.Char System.Char.Parse(String s), static System.Boolean System.Char.TryParse(String s, Char result), static System.Boolean System.Char.IsDigit(Char c), static System.Boolean System.Char.IsDigit(String s, Int32 index), static System.Boolean System.Char.IsLetter(Char c), static System.Boolean System.Char.IsLetter(String s, Int32 index), static System.Boolean System.Char.IsWhiteSpace(Char c), static System.Boolean System.Char.IsWhiteSpace(String s, Int32 index), static System.Boolean System.Char.IsUpper(Char c), static System.Boolean System.Char.IsUpper(String s, Int32 index), static System.Boolean System.Char.IsLower(Char c), static System.Boolean System.Char.IsLower(String s, Int32 index), static System.Boolean System.Char.IsPunctuation(Char c), static System.Boolean System.Char.IsPunctuation(String s, Int32 index), static System.Boolean System.Char.IsLetterOrDigit(Char c), static System.Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static System.Char System.Char.ToUpper(Char c, CultureInfo culture), static System.Char System.Char.ToUpper(Char c), static System.Char System.Char.ToUpperInvariant(Char c), static System.Char System.Char.ToLower(Char c, CultureInfo culture), static System.Char System.Char.ToLower(Char c), static System.Char System.Char.ToLowerInvariant(Char c), System.TypeCode System.Char.GetTypeCode(), static System.Boolean System.Char.IsControl(Char c), static System.Boolean System.Char.IsControl(String s, Int32 index), static System.Boolean System.Char.IsNumber(Char c), static System.Boolean System.Char.IsNumber(String s, Int32 index), static System.Boolean System.Char.IsSeparator(Char c), static System.Boolean System.Char.IsSeparator(String s, Int32 index), static System.Boolean System.Char.IsSurrogate(Char c), static System.Boolean System.Char.IsSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSymbol(Char c), static System.Boolean System.Char.IsSymbol(String s, Int32 index), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(Char c), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static System.Double System.Char.GetNumericValue(Char c), static System.Double System.Char.GetNumericValue(String s, Int32 index), static System.Boolean System.Char.IsHighSurrogate(Char c), static System.Boolean System.Char.IsHighSurrogate(String s, Int32 index), static System.Boolean System.Char.IsLowSurrogate(Char c), static System.Boolean System.Char.IsLowSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static System.String System.Char.ConvertFromUtf32(Int32 utf32), static System.Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static System.Int32 System.Char.ConvertToUtf32(String s, Int32 index), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  IsPrimitive="True"
                  IsValueType="True">
                  <GetElementType>null</GetElementType>
                </DebugType>
              </GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="jagArray"
          Type="System.Char[,][]"
          Value="{System.Char[,][]}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              BaseType="System.Array"
              FullName="System.Char[,][]"
              GetArrayRank="1"
              GetElementType="System.Char[,]"
              GetInterfaces="{System.Collections.Generic.IList`1[System.Char[,]], System.Collections.Generic.ICollection`1[System.Char[,]], System.Collections.Generic.IEnumerable`1[System.Char[,]], System.Collections.IEnumerable}"
              GetMembers="{System.Object System.Array.GetValue(Int32[] indices), System.Object System.Array.GetValue(Int32 index), System.Object System.Array.GetValue(Int32 index1, Int32 index2), System.Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), System.Object System.Array.GetValue(Int64 index), System.Object System.Array.GetValue(Int64 index1, Int64 index2), System.Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), System.Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), System.Int32 System.Array.get_Length(), System.Int64 System.Array.get_LongLength(), System.Int32 System.Array.GetLength(Int32 dimension), System.Int64 System.Array.GetLongLength(Int32 dimension), System.Int32 System.Array.get_Rank(), System.Int32 System.Array.GetUpperBound(Int32 dimension), System.Int32 System.Array.GetLowerBound(Int32 dimension), System.Object System.Array.get_SyncRoot(), System.Boolean System.Array.get_IsReadOnly(), System.Boolean System.Array.get_IsFixedSize(), System.Boolean System.Array.get_IsSynchronized(), System.Object System.Array.Clone(), System.Int32 System.Array.CompareTo(Object other, IComparer comparer), System.Boolean System.Array.Equals(Object other, IEqualityComparer comparer), System.Int32 System.Array.GetHashCode(IEqualityComparer comparer), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), System.Collections.IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Object System.Array.GetValue(Int32[] indices), System.Object System.Array.GetValue(Int32 index), System.Object System.Array.GetValue(Int32 index1, Int32 index2), System.Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), System.Object System.Array.GetValue(Int64 index), System.Object System.Array.GetValue(Int64 index1, Int64 index2), System.Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), System.Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), System.Int32 System.Array.get_Length(), System.Int64 System.Array.get_LongLength(), System.Int32 System.Array.GetLength(Int32 dimension), System.Int64 System.Array.GetLongLength(Int32 dimension), System.Int32 System.Array.get_Rank(), System.Int32 System.Array.GetUpperBound(Int32 dimension), System.Int32 System.Array.GetLowerBound(Int32 dimension), System.Object System.Array.get_SyncRoot(), System.Boolean System.Array.get_IsReadOnly(), System.Boolean System.Array.get_IsFixedSize(), System.Boolean System.Array.get_IsSynchronized(), System.Object System.Array.Clone(), System.Int32 System.Array.CompareTo(Object other, IComparer comparer), System.Boolean System.Array.Equals(Object other, IEqualityComparer comparer), System.Int32 System.Array.GetHashCode(IEqualityComparer comparer), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), System.Collections.IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized}"
              HasElementType="True"
              IsArray="True"
              IsClass="True"
              IsCompilerGenerated="True">
              <GetElementType>
                <DebugType
                  Attributes="NotPublic"
                  BaseType="System.Array"
                  FullName="System.Char[,]"
                  GetArrayRank="2"
                  GetElementType="System.Char"
                  GetInterfaces="{System.Collections.Generic.IList`1[System.Char], System.Collections.Generic.ICollection`1[System.Char], System.Collections.Generic.IEnumerable`1[System.Char], System.Collections.IEnumerable}"
                  GetMembers="{System.Object System.Array.GetValue(Int32[] indices), System.Object System.Array.GetValue(Int32 index), System.Object System.Array.GetValue(Int32 index1, Int32 index2), System.Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), System.Object System.Array.GetValue(Int64 index), System.Object System.Array.GetValue(Int64 index1, Int64 index2), System.Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), System.Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), System.Int32 System.Array.get_Length(), System.Int64 System.Array.get_LongLength(), System.Int32 System.Array.GetLength(Int32 dimension), System.Int64 System.Array.GetLongLength(Int32 dimension), System.Int32 System.Array.get_Rank(), System.Int32 System.Array.GetUpperBound(Int32 dimension), System.Int32 System.Array.GetLowerBound(Int32 dimension), System.Object System.Array.get_SyncRoot(), System.Boolean System.Array.get_IsReadOnly(), System.Boolean System.Array.get_IsFixedSize(), System.Boolean System.Array.get_IsSynchronized(), System.Object System.Array.Clone(), System.Int32 System.Array.CompareTo(Object other, IComparer comparer), System.Boolean System.Array.Equals(Object other, IEqualityComparer comparer), System.Int32 System.Array.GetHashCode(IEqualityComparer comparer), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), System.Collections.IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  GetMethods="{System.Object System.Array.GetValue(Int32[] indices), System.Object System.Array.GetValue(Int32 index), System.Object System.Array.GetValue(Int32 index1, Int32 index2), System.Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), System.Object System.Array.GetValue(Int64 index), System.Object System.Array.GetValue(Int64 index1, Int64 index2), System.Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), System.Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), System.Int32 System.Array.get_Length(), System.Int64 System.Array.get_LongLength(), System.Int32 System.Array.GetLength(Int32 dimension), System.Int64 System.Array.GetLongLength(Int32 dimension), System.Int32 System.Array.get_Rank(), System.Int32 System.Array.GetUpperBound(Int32 dimension), System.Int32 System.Array.GetLowerBound(Int32 dimension), System.Object System.Array.get_SyncRoot(), System.Boolean System.Array.get_IsReadOnly(), System.Boolean System.Array.get_IsFixedSize(), System.Boolean System.Array.get_IsSynchronized(), System.Object System.Array.Clone(), System.Int32 System.Array.CompareTo(Object other, IComparer comparer), System.Boolean System.Array.Equals(Object other, IEqualityComparer comparer), System.Int32 System.Array.GetHashCode(IEqualityComparer comparer), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), System.Collections.IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  GetProperties="{System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized}"
                  HasElementType="True"
                  IsArray="True"
                  IsClass="True"
                  IsCompilerGenerated="True">
                  <GetElementType>
                    <DebugType
                      Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
                      BaseType="System.ValueType"
                      FullName="System.Char"
                      GetFields="{System.Char MaxValue, System.Char MinValue}"
                      GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[System.Char], System.IEquatable`1[System.Char]}"
                      GetMembers="{System.Char MaxValue, System.Char MinValue, System.Int32 System.Char.GetHashCode(), System.Boolean System.Char.Equals(Object obj), System.Boolean System.Char.Equals(Char obj), System.Int32 System.Char.CompareTo(Object value), System.Int32 System.Char.CompareTo(Char value), System.String System.Char.ToString(), System.String System.Char.ToString(IFormatProvider provider), static System.String System.Char.ToString(Char c), static System.Char System.Char.Parse(String s), static System.Boolean System.Char.TryParse(String s, Char result), static System.Boolean System.Char.IsDigit(Char c), static System.Boolean System.Char.IsDigit(String s, Int32 index), static System.Boolean System.Char.IsLetter(Char c), static System.Boolean System.Char.IsLetter(String s, Int32 index), static System.Boolean System.Char.IsWhiteSpace(Char c), static System.Boolean System.Char.IsWhiteSpace(String s, Int32 index), static System.Boolean System.Char.IsUpper(Char c), static System.Boolean System.Char.IsUpper(String s, Int32 index), static System.Boolean System.Char.IsLower(Char c), static System.Boolean System.Char.IsLower(String s, Int32 index), static System.Boolean System.Char.IsPunctuation(Char c), static System.Boolean System.Char.IsPunctuation(String s, Int32 index), static System.Boolean System.Char.IsLetterOrDigit(Char c), static System.Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static System.Char System.Char.ToUpper(Char c, CultureInfo culture), static System.Char System.Char.ToUpper(Char c), static System.Char System.Char.ToUpperInvariant(Char c), static System.Char System.Char.ToLower(Char c, CultureInfo culture), static System.Char System.Char.ToLower(Char c), static System.Char System.Char.ToLowerInvariant(Char c), System.TypeCode System.Char.GetTypeCode(), static System.Boolean System.Char.IsControl(Char c), static System.Boolean System.Char.IsControl(String s, Int32 index), static System.Boolean System.Char.IsNumber(Char c), static System.Boolean System.Char.IsNumber(String s, Int32 index), static System.Boolean System.Char.IsSeparator(Char c), static System.Boolean System.Char.IsSeparator(String s, Int32 index), static System.Boolean System.Char.IsSurrogate(Char c), static System.Boolean System.Char.IsSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSymbol(Char c), static System.Boolean System.Char.IsSymbol(String s, Int32 index), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(Char c), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static System.Double System.Char.GetNumericValue(Char c), static System.Double System.Char.GetNumericValue(String s, Int32 index), static System.Boolean System.Char.IsHighSurrogate(Char c), static System.Boolean System.Char.IsHighSurrogate(String s, Int32 index), static System.Boolean System.Char.IsLowSurrogate(Char c), static System.Boolean System.Char.IsLowSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static System.String System.Char.ConvertFromUtf32(Int32 utf32), static System.Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static System.Int32 System.Char.ConvertToUtf32(String s, Int32 index), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                      GetMethods="{System.Int32 System.Char.GetHashCode(), System.Boolean System.Char.Equals(Object obj), System.Boolean System.Char.Equals(Char obj), System.Int32 System.Char.CompareTo(Object value), System.Int32 System.Char.CompareTo(Char value), System.String System.Char.ToString(), System.String System.Char.ToString(IFormatProvider provider), static System.String System.Char.ToString(Char c), static System.Char System.Char.Parse(String s), static System.Boolean System.Char.TryParse(String s, Char result), static System.Boolean System.Char.IsDigit(Char c), static System.Boolean System.Char.IsDigit(String s, Int32 index), static System.Boolean System.Char.IsLetter(Char c), static System.Boolean System.Char.IsLetter(String s, Int32 index), static System.Boolean System.Char.IsWhiteSpace(Char c), static System.Boolean System.Char.IsWhiteSpace(String s, Int32 index), static System.Boolean System.Char.IsUpper(Char c), static System.Boolean System.Char.IsUpper(String s, Int32 index), static System.Boolean System.Char.IsLower(Char c), static System.Boolean System.Char.IsLower(String s, Int32 index), static System.Boolean System.Char.IsPunctuation(Char c), static System.Boolean System.Char.IsPunctuation(String s, Int32 index), static System.Boolean System.Char.IsLetterOrDigit(Char c), static System.Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static System.Char System.Char.ToUpper(Char c, CultureInfo culture), static System.Char System.Char.ToUpper(Char c), static System.Char System.Char.ToUpperInvariant(Char c), static System.Char System.Char.ToLower(Char c, CultureInfo culture), static System.Char System.Char.ToLower(Char c), static System.Char System.Char.ToLowerInvariant(Char c), System.TypeCode System.Char.GetTypeCode(), static System.Boolean System.Char.IsControl(Char c), static System.Boolean System.Char.IsControl(String s, Int32 index), static System.Boolean System.Char.IsNumber(Char c), static System.Boolean System.Char.IsNumber(String s, Int32 index), static System.Boolean System.Char.IsSeparator(Char c), static System.Boolean System.Char.IsSeparator(String s, Int32 index), static System.Boolean System.Char.IsSurrogate(Char c), static System.Boolean System.Char.IsSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSymbol(Char c), static System.Boolean System.Char.IsSymbol(String s, Int32 index), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(Char c), static System.Globalization.UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static System.Double System.Char.GetNumericValue(Char c), static System.Double System.Char.GetNumericValue(String s, Int32 index), static System.Boolean System.Char.IsHighSurrogate(Char c), static System.Boolean System.Char.IsHighSurrogate(String s, Int32 index), static System.Boolean System.Char.IsLowSurrogate(Char c), static System.Boolean System.Char.IsLowSurrogate(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(String s, Int32 index), static System.Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static System.String System.Char.ConvertFromUtf32(Int32 utf32), static System.Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static System.Int32 System.Char.ConvertToUtf32(String s, Int32 index), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                      IsPrimitive="True"
                      IsValueType="True">
                      <GetElementType>null</GetElementType>
                    </DebugType>
                  </GetElementType>
                </DebugType>
              </GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="nullable_value"
          Type="System.Nullable`1[System.Int32]"
          Value="{System.Nullable`1[System.Int32]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.Nullable`1[System.Int32]"
              GetGenericArguments="{System.Int32}"
              GetMembers="{void System.Nullable`1[System.Int32]..ctor(Int32 value), System.Boolean System.Nullable`1[System.Int32].get_HasValue(), System.Int32 System.Nullable`1[System.Int32].get_Value(), System.Int32 System.Nullable`1[System.Int32].GetValueOrDefault(), System.Int32 System.Nullable`1[System.Int32].GetValueOrDefault(Int32 defaultValue), System.Boolean System.Nullable`1[System.Int32].Equals(Object other), System.Int32 System.Nullable`1[System.Int32].GetHashCode(), System.String System.Nullable`1[System.Int32].ToString(), static System.Nullable`1[System.Int32] System.Nullable`1[System.Int32].op_Implicit(Int32 value), static System.Int32 System.Nullable`1[System.Int32].op_Explicit(Nullable`1 value), System.Boolean HasValue, System.Int32 Value, System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void System.Nullable`1[System.Int32]..ctor(Int32 value), System.Boolean System.Nullable`1[System.Int32].get_HasValue(), System.Int32 System.Nullable`1[System.Int32].get_Value(), System.Int32 System.Nullable`1[System.Int32].GetValueOrDefault(), System.Int32 System.Nullable`1[System.Int32].GetValueOrDefault(Int32 defaultValue), System.Boolean System.Nullable`1[System.Int32].Equals(Object other), System.Int32 System.Nullable`1[System.Int32].GetHashCode(), System.String System.Nullable`1[System.Int32].ToString(), static System.Nullable`1[System.Int32] System.Nullable`1[System.Int32].op_Implicit(Int32 value), static System.Int32 System.Nullable`1[System.Int32].op_Explicit(Nullable`1 value), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Boolean HasValue, System.Int32 Value}"
              IsGenericType="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="nullable_null"
          Type="System.Nullable`1[System.Int32]"
          Value="{System.Nullable`1[System.Int32]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.Nullable`1[System.Int32]"
              GetGenericArguments="{System.Int32}"
              GetMembers="{void System.Nullable`1[System.Int32]..ctor(Int32 value), System.Boolean System.Nullable`1[System.Int32].get_HasValue(), System.Int32 System.Nullable`1[System.Int32].get_Value(), System.Int32 System.Nullable`1[System.Int32].GetValueOrDefault(), System.Int32 System.Nullable`1[System.Int32].GetValueOrDefault(Int32 defaultValue), System.Boolean System.Nullable`1[System.Int32].Equals(Object other), System.Int32 System.Nullable`1[System.Int32].GetHashCode(), System.String System.Nullable`1[System.Int32].ToString(), static System.Nullable`1[System.Int32] System.Nullable`1[System.Int32].op_Implicit(Int32 value), static System.Int32 System.Nullable`1[System.Int32].op_Explicit(Nullable`1 value), System.Boolean HasValue, System.Int32 Value, System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void System.Nullable`1[System.Int32]..ctor(Int32 value), System.Boolean System.Nullable`1[System.Int32].get_HasValue(), System.Int32 System.Nullable`1[System.Int32].get_Value(), System.Int32 System.Nullable`1[System.Int32].GetValueOrDefault(), System.Int32 System.Nullable`1[System.Int32].GetValueOrDefault(Int32 defaultValue), System.Boolean System.Nullable`1[System.Int32].Equals(Object other), System.Int32 System.Nullable`1[System.Int32].GetHashCode(), System.String System.Nullable`1[System.Int32].ToString(), static System.Nullable`1[System.Int32] System.Nullable`1[System.Int32].op_Implicit(Int32 value), static System.Int32 System.Nullable`1[System.Int32].op_Explicit(Nullable`1 value), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Boolean HasValue, System.Int32 Value}"
              IsGenericType="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myGenClass_int"
          Type="Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]"
          Value="{Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]"
              GetGenericArguments="{System.Int32}"
              GetMembers="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsClass="True"
              IsGenericType="True"
              IsNested="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="array_MyGenClass_int"
          Type="Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32][]"
          Value="{Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32][]}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              BaseType="System.Array"
              FullName="Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32][]"
              GetArrayRank="1"
              GetElementType="Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]"
              GetInterfaces="{System.Collections.Generic.IList`1[Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]], System.Collections.Generic.ICollection`1[Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]], System.Collections.Generic.IEnumerable`1[Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]], System.Collections.IEnumerable}"
              GetMembers="{System.Object System.Array.GetValue(Int32[] indices), System.Object System.Array.GetValue(Int32 index), System.Object System.Array.GetValue(Int32 index1, Int32 index2), System.Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), System.Object System.Array.GetValue(Int64 index), System.Object System.Array.GetValue(Int64 index1, Int64 index2), System.Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), System.Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), System.Int32 System.Array.get_Length(), System.Int64 System.Array.get_LongLength(), System.Int32 System.Array.GetLength(Int32 dimension), System.Int64 System.Array.GetLongLength(Int32 dimension), System.Int32 System.Array.get_Rank(), System.Int32 System.Array.GetUpperBound(Int32 dimension), System.Int32 System.Array.GetLowerBound(Int32 dimension), System.Object System.Array.get_SyncRoot(), System.Boolean System.Array.get_IsReadOnly(), System.Boolean System.Array.get_IsFixedSize(), System.Boolean System.Array.get_IsSynchronized(), System.Object System.Array.Clone(), System.Int32 System.Array.CompareTo(Object other, IComparer comparer), System.Boolean System.Array.Equals(Object other, IEqualityComparer comparer), System.Int32 System.Array.GetHashCode(IEqualityComparer comparer), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), System.Collections.IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Object System.Array.GetValue(Int32[] indices), System.Object System.Array.GetValue(Int32 index), System.Object System.Array.GetValue(Int32 index1, Int32 index2), System.Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), System.Object System.Array.GetValue(Int64 index), System.Object System.Array.GetValue(Int64 index1, Int64 index2), System.Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), System.Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), System.Int32 System.Array.get_Length(), System.Int64 System.Array.get_LongLength(), System.Int32 System.Array.GetLength(Int32 dimension), System.Int64 System.Array.GetLongLength(Int32 dimension), System.Int32 System.Array.get_Rank(), System.Int32 System.Array.GetUpperBound(Int32 dimension), System.Int32 System.Array.GetLowerBound(Int32 dimension), System.Object System.Array.get_SyncRoot(), System.Boolean System.Array.get_IsReadOnly(), System.Boolean System.Array.get_IsFixedSize(), System.Boolean System.Array.get_IsSynchronized(), System.Object System.Array.Clone(), System.Int32 System.Array.CompareTo(Object other, IComparer comparer), System.Boolean System.Array.Equals(Object other, IEqualityComparer comparer), System.Int32 System.Array.GetHashCode(IEqualityComparer comparer), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), System.Collections.IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized}"
              HasElementType="True"
              IsArray="True"
              IsClass="True"
              IsCompilerGenerated="True">
              <GetElementType>
                <DebugType
                  Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
                  BaseType="System.Object"
                  FullName="Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]"
                  GetGenericArguments="{System.Int32}"
                  GetMembers="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  GetMethods="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
                  IsClass="True"
                  IsGenericType="True"
                  IsNested="True">
                  <GetElementType>null</GetElementType>
                </DebugType>
              </GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myGenClass_Nullable_int"
          Type="Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Nullable`1[System.Int32]]"
          Value="{Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Nullable`1[System.Int32]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Nullable`1[System.Int32]]"
              GetGenericArguments="{System.Nullable`1[System.Int32]}"
              GetMembers="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Nullable`1[System.Int32]]..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Nullable`1[System.Int32]]..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsClass="True"
              IsGenericType="True"
              IsNested="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myNestedStruct"
          Type="Debugger.Tests.DebugType_Tests+MyGenClass`1+MyNestedStruct[System.Int32]"
          Value="{Debugger.Tests.DebugType_Tests+MyGenClass`1+MyNestedStruct[System.Int32]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="Debugger.Tests.DebugType_Tests+MyGenClass`1+MyNestedStruct[System.Int32]"
              GetGenericArguments="{System.Int32}"
              GetMembers="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsGenericType="True"
              IsNested="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myGenNestedStruct"
          Type="Debugger.Tests.DebugType_Tests+MyGenClass`1+MyGenNestedStruct`1[System.Int32,System.Char]"
          Value="{Debugger.Tests.DebugType_Tests+MyGenClass`1+MyGenNestedStruct`1[System.Int32,System.Char]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="Debugger.Tests.DebugType_Tests+MyGenClass`1+MyGenNestedStruct`1[System.Int32,System.Char]"
              GetGenericArguments="{System.Int32, System.Char}"
              GetMembers="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsGenericType="True"
              IsNested="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myInterfaceImpl"
          Type="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          Value="{Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
              GetGenericArguments="{System.Int32}"
              GetInterfaces="{Debugger.Tests.DebugType_Tests+MyInterface`3[System.Int32,Debugger.Tests.DebugType_Tests+MyClass,Debugger.Tests.DebugType_Tests+MyStruct]}"
              GetMembers="{System.Collections.Generic.List`1[System.Int32] Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].get_Prop(), System.Int32 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun(MyClass a, MyStruct b, Object m), System.Object[] Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun2(Int32** iPtrPtr, Object[,] mdArray, Enumerator listEnum), void Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]..ctor(), System.Collections.Generic.List`1[System.Int32] Prop, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Collections.Generic.List`1[System.Int32] Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].get_Prop(), System.Int32 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun(MyClass a, MyStruct b, Object m), System.Object[] Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun2(Int32** iPtrPtr, Object[,] mdArray, Enumerator listEnum), void Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Collections.Generic.List`1[System.Int32] Prop}"
              IsClass="True"
              IsGenericType="True"
              IsNested="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myInterface"
          Type="Debugger.Tests.DebugType_Tests+MyInterface`3[System.Int32,Debugger.Tests.DebugType_Tests+MyClass,Debugger.Tests.DebugType_Tests+MyStruct]"
          Value="{Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, ClassSemanticsMask, Abstract"
              FullName="Debugger.Tests.DebugType_Tests+MyInterface`3[System.Int32,Debugger.Tests.DebugType_Tests+MyClass,Debugger.Tests.DebugType_Tests+MyStruct]"
              GetGenericArguments="{System.Int32, Debugger.Tests.DebugType_Tests+MyClass, Debugger.Tests.DebugType_Tests+MyStruct}"
              GetMembers="{System.Int32 Debugger.Tests.DebugType_Tests+MyInterface`3[System.Int32,Debugger.Tests.DebugType_Tests+MyClass,Debugger.Tests.DebugType_Tests+MyStruct].Fun(MyClass a, MyStruct b, Object m)}"
              GetMethods="{System.Int32 Debugger.Tests.DebugType_Tests+MyInterface`3[System.Int32,Debugger.Tests.DebugType_Tests+MyClass,Debugger.Tests.DebugType_Tests+MyStruct].Fun(MyClass a, MyStruct b, Object m)}"
              IsGenericType="True"
              IsInterface="True"
              IsNested="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="list"
          Type="System.Collections.Generic.List`1[System.Int32]"
          Value="{System.Collections.Generic.List`1[System.Int32]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, Serializable, BeforeFieldInit"
              BaseType="System.Object"
              FullName="System.Collections.Generic.List`1[System.Int32]"
              GetGenericArguments="{System.Int32}"
              GetInterfaces="{System.Collections.Generic.IList`1[System.Int32], System.Collections.Generic.ICollection`1[System.Int32], System.Collections.Generic.IEnumerable`1[System.Int32], System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable}"
              GetMembers="{void System.Collections.Generic.List`1[System.Int32]..ctor(), void System.Collections.Generic.List`1[System.Int32]..ctor(Int32 capacity), void System.Collections.Generic.List`1[System.Int32]..ctor(IEnumerable`1 collection), System.Int32 System.Collections.Generic.List`1[System.Int32].get_Capacity(), void System.Collections.Generic.List`1[System.Int32].set_Capacity(Int32 value), System.Int32 System.Collections.Generic.List`1[System.Int32].get_Count(), System.Int32 System.Collections.Generic.List`1[System.Int32].get_Item(Int32 index), void System.Collections.Generic.List`1[System.Int32].set_Item(Int32 index, Int32 value), void System.Collections.Generic.List`1[System.Int32].Add(Int32 item), void System.Collections.Generic.List`1[System.Int32].AddRange(IEnumerable`1 collection), System.Collections.ObjectModel.ReadOnlyCollection`1[System.Int32] System.Collections.Generic.List`1[System.Int32].AsReadOnly(), System.Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 index, Int32 count, Int32 item, IComparer`1 comparer), System.Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 item), System.Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 item, IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Clear(), System.Boolean System.Collections.Generic.List`1[System.Int32].Contains(Int32 item), System.Collections.Generic.List`1[System.Object] System.Collections.Generic.List`1[System.Int32].ConvertAll(Converter`2 converter), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32[] array), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32 index, Int32[] array, Int32 arrayIndex, Int32 count), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32[] array, Int32 arrayIndex), System.Boolean System.Collections.Generic.List`1[System.Int32].Exists(Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].Find(Predicate`1 match), System.Collections.Generic.List`1[System.Int32] System.Collections.Generic.List`1[System.Int32].FindAll(Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Int32 startIndex, Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Int32 startIndex, Int32 count, Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindLast(Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Int32 startIndex, Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Int32 startIndex, Int32 count, Predicate`1 match), void System.Collections.Generic.List`1[System.Int32].ForEach(Action`1 action), System.Collections.Generic.List`1+Enumerator[System.Int32] System.Collections.Generic.List`1[System.Int32].GetEnumerator(), System.Collections.Generic.List`1[System.Int32] System.Collections.Generic.List`1[System.Int32].GetRange(Int32 index, Int32 count), System.Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item), System.Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item, Int32 index), System.Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item, Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Insert(Int32 index, Int32 item), void System.Collections.Generic.List`1[System.Int32].InsertRange(Int32 index, IEnumerable`1 collection), System.Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item), System.Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item, Int32 index), System.Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item, Int32 index, Int32 count), System.Boolean System.Collections.Generic.List`1[System.Int32].Remove(Int32 item), System.Int32 System.Collections.Generic.List`1[System.Int32].RemoveAll(Predicate`1 match), void System.Collections.Generic.List`1[System.Int32].RemoveAt(Int32 index), void System.Collections.Generic.List`1[System.Int32].RemoveRange(Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Reverse(), void System.Collections.Generic.List`1[System.Int32].Reverse(Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Sort(), void System.Collections.Generic.List`1[System.Int32].Sort(IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Sort(Int32 index, Int32 count, IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Sort(Comparison`1 comparison), System.Int32[] System.Collections.Generic.List`1[System.Int32].ToArray(), void System.Collections.Generic.List`1[System.Int32].TrimExcess(), System.Boolean System.Collections.Generic.List`1[System.Int32].TrueForAll(Predicate`1 match), System.Int32 Capacity, System.Int32 Count, System.Int32 Item[Int32 index], void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void System.Collections.Generic.List`1[System.Int32]..ctor(), void System.Collections.Generic.List`1[System.Int32]..ctor(Int32 capacity), void System.Collections.Generic.List`1[System.Int32]..ctor(IEnumerable`1 collection), System.Int32 System.Collections.Generic.List`1[System.Int32].get_Capacity(), void System.Collections.Generic.List`1[System.Int32].set_Capacity(Int32 value), System.Int32 System.Collections.Generic.List`1[System.Int32].get_Count(), System.Int32 System.Collections.Generic.List`1[System.Int32].get_Item(Int32 index), void System.Collections.Generic.List`1[System.Int32].set_Item(Int32 index, Int32 value), void System.Collections.Generic.List`1[System.Int32].Add(Int32 item), void System.Collections.Generic.List`1[System.Int32].AddRange(IEnumerable`1 collection), System.Collections.ObjectModel.ReadOnlyCollection`1[System.Int32] System.Collections.Generic.List`1[System.Int32].AsReadOnly(), System.Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 index, Int32 count, Int32 item, IComparer`1 comparer), System.Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 item), System.Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 item, IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Clear(), System.Boolean System.Collections.Generic.List`1[System.Int32].Contains(Int32 item), System.Collections.Generic.List`1[System.Object] System.Collections.Generic.List`1[System.Int32].ConvertAll(Converter`2 converter), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32[] array), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32 index, Int32[] array, Int32 arrayIndex, Int32 count), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32[] array, Int32 arrayIndex), System.Boolean System.Collections.Generic.List`1[System.Int32].Exists(Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].Find(Predicate`1 match), System.Collections.Generic.List`1[System.Int32] System.Collections.Generic.List`1[System.Int32].FindAll(Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Int32 startIndex, Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Int32 startIndex, Int32 count, Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindLast(Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Int32 startIndex, Predicate`1 match), System.Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Int32 startIndex, Int32 count, Predicate`1 match), void System.Collections.Generic.List`1[System.Int32].ForEach(Action`1 action), System.Collections.Generic.List`1+Enumerator[System.Int32] System.Collections.Generic.List`1[System.Int32].GetEnumerator(), System.Collections.Generic.List`1[System.Int32] System.Collections.Generic.List`1[System.Int32].GetRange(Int32 index, Int32 count), System.Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item), System.Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item, Int32 index), System.Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item, Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Insert(Int32 index, Int32 item), void System.Collections.Generic.List`1[System.Int32].InsertRange(Int32 index, IEnumerable`1 collection), System.Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item), System.Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item, Int32 index), System.Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item, Int32 index, Int32 count), System.Boolean System.Collections.Generic.List`1[System.Int32].Remove(Int32 item), System.Int32 System.Collections.Generic.List`1[System.Int32].RemoveAll(Predicate`1 match), void System.Collections.Generic.List`1[System.Int32].RemoveAt(Int32 index), void System.Collections.Generic.List`1[System.Int32].RemoveRange(Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Reverse(), void System.Collections.Generic.List`1[System.Int32].Reverse(Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Sort(), void System.Collections.Generic.List`1[System.Int32].Sort(IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Sort(Int32 index, Int32 count, IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Sort(Comparison`1 comparison), System.Int32[] System.Collections.Generic.List`1[System.Int32].ToArray(), void System.Collections.Generic.List`1[System.Int32].TrimExcess(), System.Boolean System.Collections.Generic.List`1[System.Int32].TrueForAll(Predicate`1 match), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Int32 Capacity, System.Int32 Count, System.Int32 Item[Int32 index]}"
              IsClass="True"
              IsGenericType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="listEnumerator"
          Type="System.Collections.Generic.List`1+Enumerator[System.Int32]"
          Value="{System.Collections.Generic.List`1+Enumerator[System.Int32]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.Collections.Generic.List`1+Enumerator[System.Int32]"
              GetGenericArguments="{System.Int32}"
              GetInterfaces="{System.Collections.Generic.IEnumerator`1[System.Int32], System.IDisposable, System.Collections.IEnumerator}"
              GetMembers="{void System.Collections.Generic.List`1+Enumerator[System.Int32].Dispose(), System.Boolean System.Collections.Generic.List`1+Enumerator[System.Int32].MoveNext(), System.Int32 System.Collections.Generic.List`1+Enumerator[System.Int32].get_Current(), System.Int32 Current, System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void System.Collections.Generic.List`1+Enumerator[System.Int32].Dispose(), System.Boolean System.Collections.Generic.List`1+Enumerator[System.Int32].MoveNext(), System.Int32 System.Collections.Generic.List`1+Enumerator[System.Int32].get_Current(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Int32 Current}"
              IsGenericType="True"
              IsNested="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="fnPtr"
          Type="Debugger.Tests.DebugType_Tests+AddDelegate"
          Value="{Debugger.Tests.DebugType_Tests+AddDelegate}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, Sealed"
              BaseType="System.MulticastDelegate"
              FullName="Debugger.Tests.DebugType_Tests+AddDelegate"
              GetMembers="{void Debugger.Tests.DebugType_Tests+AddDelegate..ctor(Object object, IntPtr method), System.Int32 Debugger.Tests.DebugType_Tests+AddDelegate.Invoke(Byte b1, Byte b2), System.IAsyncResult Debugger.Tests.DebugType_Tests+AddDelegate.BeginInvoke(Byte b1, Byte b2, AsyncCallback callback, Object object), System.Int32 Debugger.Tests.DebugType_Tests+AddDelegate.EndInvoke(IAsyncResult result), void System.MulticastDelegate.GetObjectData(SerializationInfo info, StreamingContext context), System.Boolean System.MulticastDelegate.Equals(Object obj), System.Delegate[] System.MulticastDelegate.GetInvocationList(), System.Int32 System.MulticastDelegate.GetHashCode(), System.Object System.Delegate.DynamicInvoke(Object[] args), System.Boolean System.Delegate.Equals(Object obj), System.Int32 System.Delegate.GetHashCode(), System.Delegate[] System.Delegate.GetInvocationList(), System.Reflection.MethodInfo System.Delegate.get_Method(), System.Object System.Delegate.get_Target(), System.Object System.Delegate.Clone(), void System.Delegate.GetObjectData(SerializationInfo info, StreamingContext context), System.Reflection.MethodInfo Method, System.Object Target, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+AddDelegate..ctor(Object object, IntPtr method), System.Int32 Debugger.Tests.DebugType_Tests+AddDelegate.Invoke(Byte b1, Byte b2), System.IAsyncResult Debugger.Tests.DebugType_Tests+AddDelegate.BeginInvoke(Byte b1, Byte b2, AsyncCallback callback, Object object), System.Int32 Debugger.Tests.DebugType_Tests+AddDelegate.EndInvoke(IAsyncResult result), void System.MulticastDelegate.GetObjectData(SerializationInfo info, StreamingContext context), System.Boolean System.MulticastDelegate.Equals(Object obj), System.Delegate[] System.MulticastDelegate.GetInvocationList(), System.Int32 System.MulticastDelegate.GetHashCode(), System.Object System.Delegate.DynamicInvoke(Object[] args), System.Boolean System.Delegate.Equals(Object obj), System.Int32 System.Delegate.GetHashCode(), System.Delegate[] System.Delegate.GetInvocationList(), System.Reflection.MethodInfo System.Delegate.get_Method(), System.Object System.Delegate.get_Target(), System.Object System.Delegate.Clone(), void System.Delegate.GetObjectData(SerializationInfo info, StreamingContext context), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Reflection.MethodInfo Method, System.Object Target}"
              IsClass="True"
              IsNested="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="valueType"
          Type="System.ValueType"
          Value="null">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, Abstract, Serializable, BeforeFieldInit"
              BaseType="System.Object"
              FullName="System.ValueType"
              GetMembers="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsClass="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="enumType"
          Type="System.Enum"
          Value="null">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, Abstract, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.Enum"
              GetInterfaces="{System.IComparable, System.IFormattable, System.IConvertible}"
              GetMembers="{static System.Object System.Enum.Parse(Type enumType, String value), static System.Object System.Enum.Parse(Type enumType, String value, Boolean ignoreCase), static System.Type System.Enum.GetUnderlyingType(Type enumType), static System.Array System.Enum.GetValues(Type enumType), static System.String System.Enum.GetName(Type enumType, Object value), static System.String[] System.Enum.GetNames(Type enumType), static System.Object System.Enum.ToObject(Type enumType, Object value), static System.Object System.Enum.ToObject(Type enumType, SByte value), static System.Object System.Enum.ToObject(Type enumType, Int16 value), static System.Object System.Enum.ToObject(Type enumType, Int32 value), static System.Object System.Enum.ToObject(Type enumType, Byte value), static System.Object System.Enum.ToObject(Type enumType, UInt16 value), static System.Object System.Enum.ToObject(Type enumType, UInt32 value), static System.Object System.Enum.ToObject(Type enumType, Int64 value), static System.Object System.Enum.ToObject(Type enumType, UInt64 value), static System.Boolean System.Enum.IsDefined(Type enumType, Object value), static System.String System.Enum.Format(Type enumType, Object value, String format), System.Boolean System.Enum.Equals(Object obj), System.Int32 System.Enum.GetHashCode(), System.String System.Enum.ToString(), System.String System.Enum.ToString(String format, IFormatProvider provider), System.String System.Enum.ToString(String format), System.String System.Enum.ToString(IFormatProvider provider), System.Int32 System.Enum.CompareTo(Object target), System.TypeCode System.Enum.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{static System.Object System.Enum.Parse(Type enumType, String value), static System.Object System.Enum.Parse(Type enumType, String value, Boolean ignoreCase), static System.Type System.Enum.GetUnderlyingType(Type enumType), static System.Array System.Enum.GetValues(Type enumType), static System.String System.Enum.GetName(Type enumType, Object value), static System.String[] System.Enum.GetNames(Type enumType), static System.Object System.Enum.ToObject(Type enumType, Object value), static System.Object System.Enum.ToObject(Type enumType, SByte value), static System.Object System.Enum.ToObject(Type enumType, Int16 value), static System.Object System.Enum.ToObject(Type enumType, Int32 value), static System.Object System.Enum.ToObject(Type enumType, Byte value), static System.Object System.Enum.ToObject(Type enumType, UInt16 value), static System.Object System.Enum.ToObject(Type enumType, UInt32 value), static System.Object System.Enum.ToObject(Type enumType, Int64 value), static System.Object System.Enum.ToObject(Type enumType, UInt64 value), static System.Boolean System.Enum.IsDefined(Type enumType, Object value), static System.String System.Enum.Format(Type enumType, Object value, String format), System.Boolean System.Enum.Equals(Object obj), System.Int32 System.Enum.GetHashCode(), System.String System.Enum.ToString(), System.String System.Enum.ToString(String format, IFormatProvider provider), System.String System.Enum.ToString(String format), System.String System.Enum.ToString(IFormatProvider provider), System.Int32 System.Enum.CompareTo(Object target), System.TypeCode System.Enum.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsClass="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myEnum"
          Type="Debugger.Tests.DebugType_Tests+MyEnum"
          Value="{Debugger.Tests.DebugType_Tests+MyEnum}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, Sealed"
              BaseType="System.Enum"
              FullName="Debugger.Tests.DebugType_Tests+MyEnum"
              GetEnumUnderlyingType="System.Byte"
              GetFields="{System.Byte value__, Debugger.Tests.DebugType_Tests+MyEnum A, Debugger.Tests.DebugType_Tests+MyEnum B}"
              GetMembers="{System.Byte value__, Debugger.Tests.DebugType_Tests+MyEnum A, Debugger.Tests.DebugType_Tests+MyEnum B, System.Boolean System.Enum.Equals(Object obj), System.Int32 System.Enum.GetHashCode(), System.String System.Enum.ToString(), System.String System.Enum.ToString(String format, IFormatProvider provider), System.String System.Enum.ToString(String format), System.String System.Enum.ToString(IFormatProvider provider), System.Int32 System.Enum.CompareTo(Object target), System.TypeCode System.Enum.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Boolean System.Enum.Equals(Object obj), System.Int32 System.Enum.GetHashCode(), System.String System.Enum.ToString(), System.String System.Enum.ToString(String format, IFormatProvider provider), System.String System.Enum.ToString(String format), System.String System.Enum.ToString(IFormatProvider provider), System.Int32 System.Enum.CompareTo(Object target), System.TypeCode System.Enum.GetTypeCode(), System.Boolean System.ValueType.Equals(Object obj), System.Int32 System.ValueType.GetHashCode(), System.String System.ValueType.ToString(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              IsEnum="True"
              IsNested="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="members"
          Type="Debugger.Tests.DebugType_Tests+Members"
          Value="{Debugger.Tests.DebugType_Tests+Members}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.DebugType_Tests+Members"
              GetFields="{System.Int32 IntLiteral, System.Int32 instanceInt, System.Int32 staticInt, System.Void* voidPtr}"
              GetMembers="{System.Int32 IntLiteral, System.Int32 instanceInt, System.Int32 staticInt, System.Void* voidPtr, void Debugger.Tests.DebugType_Tests+Members.set_SetterOnlyProp(Char value), System.Int32 Debugger.Tests.DebugType_Tests+Members.get_InstanceInt(), static System.Int32 Debugger.Tests.DebugType_Tests+Members.get_StaticInt(), System.Int32 Debugger.Tests.DebugType_Tests+Members.get_AutoProperty(), void Debugger.Tests.DebugType_Tests+Members.set_AutoProperty(Int32 value), System.Char Debugger.Tests.DebugType_Tests+Members.get_Item(Int32 i), System.Char Debugger.Tests.DebugType_Tests+Members.get_Item(String s), void Debugger.Tests.DebugType_Tests+Members.set_Item(Int32 i, Char value), void Debugger.Tests.DebugType_Tests+Members..ctor(), System.Char SetterOnlyProp, System.Int32 InstanceInt, System.Int32 StaticInt, System.Int32 AutoProperty, System.Char Item[Int32 i], System.Char Item[String s], void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+Members.set_SetterOnlyProp(Char value), System.Int32 Debugger.Tests.DebugType_Tests+Members.get_InstanceInt(), static System.Int32 Debugger.Tests.DebugType_Tests+Members.get_StaticInt(), System.Int32 Debugger.Tests.DebugType_Tests+Members.get_AutoProperty(), void Debugger.Tests.DebugType_Tests+Members.set_AutoProperty(Int32 value), System.Char Debugger.Tests.DebugType_Tests+Members.get_Item(Int32 i), System.Char Debugger.Tests.DebugType_Tests+Members.get_Item(String s), void Debugger.Tests.DebugType_Tests+Members.set_Item(Int32 i, Char value), void Debugger.Tests.DebugType_Tests+Members..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Char SetterOnlyProp, System.Int32 InstanceInt, System.Int32 StaticInt, System.Int32 AutoProperty, System.Char Item[Int32 i], System.Char Item[String s]}"
              IsClass="True"
              IsNested="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="access"
          Type="Debugger.Tests.DebugType_Tests+Access"
          Value="{Debugger.Tests.DebugType_Tests+Access}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.DebugType_Tests+Access"
              GetFields="{System.Int32 publicField}"
              GetMembers="{System.Int32 publicField, System.Int32 Debugger.Tests.DebugType_Tests+Access.get_publicProperty(), void Debugger.Tests.DebugType_Tests+Access.publicMethod(), void Debugger.Tests.DebugType_Tests+Access..ctor(), System.Int32 publicProperty, void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetMethods="{System.Int32 Debugger.Tests.DebugType_Tests+Access.get_publicProperty(), void Debugger.Tests.DebugType_Tests+Access.publicMethod(), void Debugger.Tests.DebugType_Tests+Access..ctor(), void System.Object..ctor(), System.String System.Object.ToString(), System.Boolean System.Object.Equals(Object obj), System.Int32 System.Object.GetHashCode(), System.Type System.Object.GetType()}"
              GetProperties="{System.Int32 publicProperty}"
              IsClass="True"
              IsNested="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
    </LocalVariables>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT