// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public interface MyInterfaceBase
		{
			void MyInterfaceBaseMethod();
		}
		
		public interface MyInterface<R, A, B>: MyInterfaceBase
		{
			R Fun<M>(A a, B b, M m);
		}
		
		public interface ExtraInterface
		{
			
		}
		
		public unsafe class MyInterfaceImpl<R> : MyInterface<R, MyClass, MyStruct>, ExtraInterface
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
			
			void DebugType_Tests.MyInterfaceBase.MyInterfaceBaseMethod()
			{
				throw new NotImplementedException();
			}
		}
		
		public class MyInterfaceImplDerived: MyInterfaceImpl<int>, ExtraInterface
		{
			
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
		
		void DumpLocalVariables()
		{
			DumpLocalVariables("LocalVariables");
		}
		
		void DumpLocalVariables(string msg)
		{
			ObjectDump(
				msg,
				SelectedStackFrame.MethodInfo.GetLocalVariables(SelectedStackFrame.IP).Select(v => new LocalVariable() { Name = v.Name, Type = v.LocalType, Value = v.GetValue(process.SelectedStackFrame)})
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
			DumpLocalVariables();
			
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
    <DebuggingPaused>Break DebugType_Tests.cs:167,4-167,40</DebuggingPaused>
    <DefinedTypes
      Capacity="16"
      Count="15">
      <Item>Debugger.Tests.DebugType_Tests</Item>
      <Item>AddDelegate</Item>
      <Item>MyEnum</Item>
      <Item>MyClass</Item>
      <Item>MyStruct</Item>
      <Item>MyGenClass`1</Item>
      <Item>MyNestedStruct</Item>
      <Item>MyGenNestedStruct`1</Item>
      <Item>MyInterfaceBase</Item>
      <Item>MyInterface`3</Item>
      <Item>ExtraInterface</Item>
      <Item>MyInterfaceImpl`1</Item>
      <Item>MyInterfaceImplDerived</Item>
      <Item>Members</Item>
      <Item>Access</Item>
    </DefinedTypes>
    <DefinedTypes
      Capacity="16"
      Count="10">
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, Public, BeforeFieldInit"
          BaseType="System.Object"
          FullName="Debugger.Tests.DebugType_Tests"
          GetMembers="{static Int32 Debugger.Tests.DebugType_Tests.Add(Byte b1, Byte b2), static void Debugger.Tests.DebugType_Tests.Main(), void Debugger.Tests.DebugType_Tests..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetMethods="{static Int32 Debugger.Tests.DebugType_Tests.Add(Byte b1, Byte b2), static void Debugger.Tests.DebugType_Tests.Main(), void Debugger.Tests.DebugType_Tests..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          IsClass="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, Sealed"
          BaseType="System.MulticastDelegate"
          FullName="Debugger.Tests.DebugType_Tests+AddDelegate"
          GetInterfaces="{System.ICloneable, System.Runtime.Serialization.ISerializable}"
          GetMembers="{void Debugger.Tests.DebugType_Tests+AddDelegate..ctor(Object object, IntPtr method), Int32 Debugger.Tests.DebugType_Tests+AddDelegate.Invoke(Byte b1, Byte b2), IAsyncResult Debugger.Tests.DebugType_Tests+AddDelegate.BeginInvoke(Byte b1, Byte b2, AsyncCallback callback, Object object), Int32 Debugger.Tests.DebugType_Tests+AddDelegate.EndInvoke(IAsyncResult result), void System.MulticastDelegate.GetObjectData(SerializationInfo info, StreamingContext context), Boolean System.MulticastDelegate.Equals(Object obj), Delegate[] System.MulticastDelegate.GetInvocationList(), Int32 System.MulticastDelegate.GetHashCode(), Object System.Delegate.DynamicInvoke(Object[] args), Boolean System.Delegate.Equals(Object obj), Int32 System.Delegate.GetHashCode(), Delegate[] System.Delegate.GetInvocationList(), MethodInfo System.Delegate.get_Method(), Object System.Delegate.get_Target(), Object System.Delegate.Clone(), void System.Delegate.GetObjectData(SerializationInfo info, StreamingContext context), MethodInfo System.Delegate.Method, Object System.Delegate.Target, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetMethods="{void Debugger.Tests.DebugType_Tests+AddDelegate..ctor(Object object, IntPtr method), Int32 Debugger.Tests.DebugType_Tests+AddDelegate.Invoke(Byte b1, Byte b2), IAsyncResult Debugger.Tests.DebugType_Tests+AddDelegate.BeginInvoke(Byte b1, Byte b2, AsyncCallback callback, Object object), Int32 Debugger.Tests.DebugType_Tests+AddDelegate.EndInvoke(IAsyncResult result), void System.MulticastDelegate.GetObjectData(SerializationInfo info, StreamingContext context), Boolean System.MulticastDelegate.Equals(Object obj), Delegate[] System.MulticastDelegate.GetInvocationList(), Int32 System.MulticastDelegate.GetHashCode(), Object System.Delegate.DynamicInvoke(Object[] args), Boolean System.Delegate.Equals(Object obj), Int32 System.Delegate.GetHashCode(), Delegate[] System.Delegate.GetInvocationList(), MethodInfo System.Delegate.get_Method(), Object System.Delegate.get_Target(), Object System.Delegate.Clone(), void System.Delegate.GetObjectData(SerializationInfo info, StreamingContext context), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetProperties="{MethodInfo System.Delegate.Method, Object System.Delegate.Target}"
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
          GetInterfaces="{System.IComparable, System.IFormattable, System.IConvertible}"
          GetMembers="{System.Byte value__, Debugger.Tests.DebugType_Tests+MyEnum A, Debugger.Tests.DebugType_Tests+MyEnum B, Boolean System.Enum.Equals(Object obj), Int32 System.Enum.GetHashCode(), String System.Enum.ToString(), String System.Enum.ToString(String format, IFormatProvider provider), String System.Enum.ToString(String format), String System.Enum.ToString(IFormatProvider provider), Int32 System.Enum.CompareTo(Object target), Boolean System.Enum.HasFlag(Enum flag), TypeCode System.Enum.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetMethods="{Boolean System.Enum.Equals(Object obj), Int32 System.Enum.GetHashCode(), String System.Enum.ToString(), String System.Enum.ToString(String format, IFormatProvider provider), String System.Enum.ToString(String format), String System.Enum.ToString(IFormatProvider provider), Int32 System.Enum.CompareTo(Object target), Boolean System.Enum.HasFlag(Enum flag), TypeCode System.Enum.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
          GetMembers="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetMethods="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
          GetMembers="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetMethods="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          IsNested="True"
          IsValueType="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, ClassSemanticsMask, Abstract"
          FullName="Debugger.Tests.DebugType_Tests+MyInterfaceBase"
          GetMembers="{void Debugger.Tests.DebugType_Tests+MyInterfaceBase.MyInterfaceBaseMethod()}"
          GetMethods="{void Debugger.Tests.DebugType_Tests+MyInterfaceBase.MyInterfaceBaseMethod()}"
          IsInterface="True"
          IsNested="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, ClassSemanticsMask, Abstract"
          FullName="Debugger.Tests.DebugType_Tests+ExtraInterface"
          IsInterface="True"
          IsNested="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
          BaseType="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          FullName="Debugger.Tests.DebugType_Tests+MyInterfaceImplDerived"
          GetInterfaces="{Debugger.Tests.DebugType_Tests+ExtraInterface, Debugger.Tests.DebugType_Tests+MyInterface`3[System.Int32,Debugger.Tests.DebugType_Tests+MyClass,Debugger.Tests.DebugType_Tests+MyStruct], Debugger.Tests.DebugType_Tests+MyInterfaceBase}"
          GetMembers="{void Debugger.Tests.DebugType_Tests+MyInterfaceImplDerived..ctor(), List`1 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].get_Prop(), Int32 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun(MyClass a, MyStruct b, Object m), Object[] Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun2(Int32** iPtrPtr, Object[,] mdArray, Enumerator listEnum), void Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]..ctor(), List`1 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Prop, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetMethods="{void Debugger.Tests.DebugType_Tests+MyInterfaceImplDerived..ctor(), List`1 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].get_Prop(), Int32 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun(MyClass a, MyStruct b, Object m), Object[] Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun2(Int32** iPtrPtr, Object[,] mdArray, Enumerator listEnum), void Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetProperties="{List`1 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Prop}"
          IsClass="True"
          IsNested="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
          BaseType="System.Object"
          FullName="Debugger.Tests.DebugType_Tests+Members"
          GetFields="{System.Int32 IntLiteral, System.Int32 instanceInt, System.Int32 staticInt, System.Void* voidPtr}"
          GetMembers="{System.Int32 IntLiteral, System.Int32 instanceInt, System.Int32 staticInt, System.Void* voidPtr, void Debugger.Tests.DebugType_Tests+Members.set_SetterOnlyProp(Char value), Int32 Debugger.Tests.DebugType_Tests+Members.get_InstanceInt(), static Int32 Debugger.Tests.DebugType_Tests+Members.get_StaticInt(), Int32 Debugger.Tests.DebugType_Tests+Members.get_AutoProperty(), void Debugger.Tests.DebugType_Tests+Members.set_AutoProperty(Int32 value), Char Debugger.Tests.DebugType_Tests+Members.get_Item(Int32 i), Char Debugger.Tests.DebugType_Tests+Members.get_Item(String s), void Debugger.Tests.DebugType_Tests+Members.set_Item(Int32 i, Char value), void Debugger.Tests.DebugType_Tests+Members..ctor(), Char Debugger.Tests.DebugType_Tests+Members.SetterOnlyProp, Int32 Debugger.Tests.DebugType_Tests+Members.InstanceInt, static Int32 Debugger.Tests.DebugType_Tests+Members.StaticInt, Int32 Debugger.Tests.DebugType_Tests+Members.AutoProperty, Char Debugger.Tests.DebugType_Tests+Members.Item[Int32 i], Char Debugger.Tests.DebugType_Tests+Members.Item[String s], void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetMethods="{void Debugger.Tests.DebugType_Tests+Members.set_SetterOnlyProp(Char value), Int32 Debugger.Tests.DebugType_Tests+Members.get_InstanceInt(), static Int32 Debugger.Tests.DebugType_Tests+Members.get_StaticInt(), Int32 Debugger.Tests.DebugType_Tests+Members.get_AutoProperty(), void Debugger.Tests.DebugType_Tests+Members.set_AutoProperty(Int32 value), Char Debugger.Tests.DebugType_Tests+Members.get_Item(Int32 i), Char Debugger.Tests.DebugType_Tests+Members.get_Item(String s), void Debugger.Tests.DebugType_Tests+Members.set_Item(Int32 i, Char value), void Debugger.Tests.DebugType_Tests+Members..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetProperties="{Char Debugger.Tests.DebugType_Tests+Members.SetterOnlyProp, Int32 Debugger.Tests.DebugType_Tests+Members.InstanceInt, static Int32 Debugger.Tests.DebugType_Tests+Members.StaticInt, Int32 Debugger.Tests.DebugType_Tests+Members.AutoProperty, Char Debugger.Tests.DebugType_Tests+Members.Item[Int32 i], Char Debugger.Tests.DebugType_Tests+Members.Item[String s]}"
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
          GetMembers="{System.Int32 publicField, Int32 Debugger.Tests.DebugType_Tests+Access.get_publicProperty(), void Debugger.Tests.DebugType_Tests+Access.publicMethod(), void Debugger.Tests.DebugType_Tests+Access..ctor(), Int32 Debugger.Tests.DebugType_Tests+Access.publicProperty, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetMethods="{Int32 Debugger.Tests.DebugType_Tests+Access.get_publicProperty(), void Debugger.Tests.DebugType_Tests+Access.publicMethod(), void Debugger.Tests.DebugType_Tests+Access..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
          GetProperties="{Int32 Debugger.Tests.DebugType_Tests+Access.publicProperty}"
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
          FullName="void Debugger.Tests.DebugType_Tests+Members.set_SetterOnlyProp(Char value)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Members this}"
          Name="set_SetterOnlyProp" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          BackingField="System.Int32 instanceInt"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Int32 Debugger.Tests.DebugType_Tests+Members.get_InstanceInt()"
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
          FullName="static Int32 Debugger.Tests.DebugType_Tests+Members.get_StaticInt()"
          Name="get_StaticInt"
          ReturnType="System.Int32"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          BackingField="System.Int32 &lt;AutoProperty&gt;k__BackingField"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Int32 Debugger.Tests.DebugType_Tests+Members.get_AutoProperty()"
          Name="get_AutoProperty"
          ReturnType="System.Int32"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="void Debugger.Tests.DebugType_Tests+Members.set_AutoProperty(Int32 value)"
          Name="set_AutoProperty"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Char Debugger.Tests.DebugType_Tests+Members.get_Item(Int32 i)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Members this}"
          Name="get_Item"
          ReturnType="System.Char" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Char Debugger.Tests.DebugType_Tests+Members.get_Item(String s)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Members this}"
          Name="get_Item"
          ReturnType="System.Char" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="void Debugger.Tests.DebugType_Tests+Members.set_Item(Int32 i, Char value)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Members this}"
          Name="set_Item" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="void Debugger.Tests.DebugType_Tests+Members..ctor()"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Char Debugger.Tests.DebugType_Tests+Members.SetterOnlyProp"
          Name="SetterOnlyProp"
          PropertyType="System.Char" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Int32 Debugger.Tests.DebugType_Tests+Members.InstanceInt"
          Name="InstanceInt"
          PropertyType="System.Int32" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="static Int32 Debugger.Tests.DebugType_Tests+Members.StaticInt"
          Name="StaticInt"
          PropertyType="System.Int32" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Int32 Debugger.Tests.DebugType_Tests+Members.AutoProperty"
          Name="AutoProperty"
          PropertyType="System.Int32" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Char Debugger.Tests.DebugType_Tests+Members.Item[Int32 i]"
          GetIndexParameters="{System.Int32 i}"
          Name="Item"
          PropertyType="System.Char" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Members"
          FullName="Char Debugger.Tests.DebugType_Tests+Members.Item[String s]"
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
          FullName="Int32 Debugger.Tests.DebugType_Tests+Access.get_publicProperty()"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Access this}"
          Name="get_publicProperty"
          ReturnType="System.Int32" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig"
          DeclaringType="Debugger.Tests.DebugType_Tests+Access"
          FullName="void Debugger.Tests.DebugType_Tests+Access.publicMethod()"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Access this}"
          Name="publicMethod" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+Access"
          FullName="void Debugger.Tests.DebugType_Tests+Access..ctor()"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+Access this}"
          Name=".ctor" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+Access"
          FullName="Int32 Debugger.Tests.DebugType_Tests+Access.publicProperty"
          Name="publicProperty"
          PropertyType="System.Int32" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="System.Object"
          FullName="void System.Object..ctor()"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="String System.Object.ToString()"
          Name="ToString"
          ReturnType="System.String"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="Boolean System.Object.Equals(Object obj)"
          Name="Equals"
          ReturnType="System.Boolean"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="Int32 System.Object.GetHashCode()"
          Name="GetHashCode"
          ReturnType="System.Int32"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig"
          DeclaringType="System.Object"
          FullName="Type System.Object.GetType()"
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
          FullName="List`1 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].get_Prop()"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32] this}"
          Name="get_Prop"
          ReturnType="System.Collections.Generic.List`1[System.Int32]" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Final, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          FullName="Int32 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun(MyClass a, MyStruct b, Object m)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32] this}"
          Name="Fun"
          ReturnType="System.Int32" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig"
          DeclaringType="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          FullName="Object[] Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun2(Int32** iPtrPtr, Object[,] mdArray, Enumerator listEnum)"
          GetLocalVariables="{Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32] this}"
          Name="Fun2"
          ReturnType="System.Object[]" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          FullName="void Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]..ctor()"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]"
          FullName="List`1 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Prop"
          Name="Prop"
          PropertyType="System.Collections.Generic.List`1[System.Int32]" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="System.Object"
          FullName="void System.Object..ctor()"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="String System.Object.ToString()"
          Name="ToString"
          ReturnType="System.String"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="Boolean System.Object.Equals(Object obj)"
          Name="Equals"
          ReturnType="System.Boolean"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="System.Object"
          FullName="Int32 System.Object.GetHashCode()"
          Name="GetHashCode"
          ReturnType="System.Int32"
          StepOver="True" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig"
          DeclaringType="System.Object"
          FullName="Type System.Object.GetType()"
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
              GetMembers="{void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), static Boolean System.Object.Equals(Object objA, Object objB), static Boolean System.Object.ReferenceEquals(Object objA, Object objB), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), static Boolean System.Object.Equals(Object objA, Object objB), static Boolean System.Object.ReferenceEquals(Object objA, Object objB), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{System.String Empty, static String System.String.Join(String separator, String[] value), static String System.String.Join(String separator, Object[] values), static String System.String.Join(String separator, IEnumerable`1 values), static String System.String.Join(String separator, IEnumerable`1 values), static String System.String.Join(String separator, String[] value, Int32 startIndex, Int32 count), Boolean System.String.Equals(Object obj), Boolean System.String.Equals(String value), Boolean System.String.Equals(String value, StringComparison comparisonType), static Boolean System.String.Equals(String a, String b), static Boolean System.String.Equals(String a, String b, StringComparison comparisonType), static Boolean System.String.op_Equality(String a, String b), static Boolean System.String.op_Inequality(String a, String b), Char System.String.get_Chars(Int32 index), void System.String.CopyTo(Int32 sourceIndex, Char[] destination, Int32 destinationIndex, Int32 count), Char[] System.String.ToCharArray(), Char[] System.String.ToCharArray(Int32 startIndex, Int32 length), static Boolean System.String.IsNullOrEmpty(String value), static Boolean System.String.IsNullOrWhiteSpace(String value), Int32 System.String.GetHashCode(), Int32 System.String.get_Length(), String[] System.String.Split(Char[] separator), String[] System.String.Split(Char[] separator, Int32 count), String[] System.String.Split(Char[] separator, StringSplitOptions options), String[] System.String.Split(Char[] separator, Int32 count, StringSplitOptions options), String[] System.String.Split(String[] separator, StringSplitOptions options), String[] System.String.Split(String[] separator, Int32 count, StringSplitOptions options), String System.String.Substring(Int32 startIndex), String System.String.Substring(Int32 startIndex, Int32 length), String System.String.Trim(Char[] trimChars), String System.String.Trim(), String System.String.TrimStart(Char[] trimChars), String System.String.TrimEnd(Char[] trimChars), void System.String..ctor(Char* value), void System.String..ctor(Char* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length, Encoding enc), void System.String..ctor(Char[] value, Int32 startIndex, Int32 length), void System.String..ctor(Char[] value), void System.String..ctor(Char c, Int32 count), Boolean System.String.IsNormalized(), Boolean System.String.IsNormalized(NormalizationForm normalizationForm), String System.String.Normalize(), String System.String.Normalize(NormalizationForm normalizationForm), static Int32 System.String.Compare(String strA, String strB), static Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase), static Int32 System.String.Compare(String strA, String strB, StringComparison comparisonType), static Int32 System.String.Compare(String strA, String strB, CultureInfo culture, CompareOptions options), static Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase, CultureInfo culture), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase, CultureInfo culture), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, CultureInfo culture, CompareOptions options), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, StringComparison comparisonType), Int32 System.String.CompareTo(Object value), Int32 System.String.CompareTo(String strB), static Int32 System.String.CompareOrdinal(String strA, String strB), static Int32 System.String.CompareOrdinal(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), Boolean System.String.Contains(String value), Boolean System.String.EndsWith(String value), Boolean System.String.EndsWith(String value, StringComparison comparisonType), Boolean System.String.EndsWith(String value, Boolean ignoreCase, CultureInfo culture), Int32 System.String.IndexOf(Char value), Int32 System.String.IndexOf(Char value, Int32 startIndex), Int32 System.String.IndexOf(Char value, Int32 startIndex, Int32 count), Int32 System.String.IndexOf(String value), Int32 System.String.IndexOf(String value, Int32 startIndex), Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count), Int32 System.String.IndexOf(String value, StringComparison comparisonType), Int32 System.String.IndexOf(String value, Int32 startIndex, StringComparison comparisonType), Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), Int32 System.String.IndexOfAny(Char[] anyOf), Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex), Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(Char value), Int32 System.String.LastIndexOf(Char value, Int32 startIndex), Int32 System.String.LastIndexOf(Char value, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(String value), Int32 System.String.LastIndexOf(String value, Int32 startIndex), Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(String value, StringComparison comparisonType), Int32 System.String.LastIndexOf(String value, Int32 startIndex, StringComparison comparisonType), Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), Int32 System.String.LastIndexOfAny(Char[] anyOf), Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex), Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), String System.String.PadLeft(Int32 totalWidth), String System.String.PadLeft(Int32 totalWidth, Char paddingChar), String System.String.PadRight(Int32 totalWidth), String System.String.PadRight(Int32 totalWidth, Char paddingChar), Boolean System.String.StartsWith(String value), Boolean System.String.StartsWith(String value, StringComparison comparisonType), Boolean System.String.StartsWith(String value, Boolean ignoreCase, CultureInfo culture), String System.String.ToLower(), String System.String.ToLower(CultureInfo culture), String System.String.ToLowerInvariant(), String System.String.ToUpper(), String System.String.ToUpper(CultureInfo culture), String System.String.ToUpperInvariant(), String System.String.ToString(), String System.String.ToString(IFormatProvider provider), Object System.String.Clone(), String System.String.Insert(Int32 startIndex, String value), String System.String.Replace(Char oldChar, Char newChar), String System.String.Replace(String oldValue, String newValue), String System.String.Remove(Int32 startIndex, Int32 count), String System.String.Remove(Int32 startIndex), static String System.String.Format(String format, Object arg0), static String System.String.Format(String format, Object arg0, Object arg1), static String System.String.Format(String format, Object arg0, Object arg1, Object arg2), static String System.String.Format(String format, Object[] args), static String System.String.Format(IFormatProvider provider, String format, Object[] args), static String System.String.Copy(String str), static String System.String.Concat(Object arg0), static String System.String.Concat(Object arg0, Object arg1), static String System.String.Concat(Object arg0, Object arg1, Object arg2), static String System.String.Concat(Object arg0, Object arg1, Object arg2, Object arg3), static String System.String.Concat(Object[] args), static String System.String.Concat(IEnumerable`1 values), static String System.String.Concat(IEnumerable`1 values), static String System.String.Concat(String str0, String str1), static String System.String.Concat(String str0, String str1, String str2), static String System.String.Concat(String str0, String str1, String str2, String str3), static String System.String.Concat(String[] values), static String System.String.Intern(String str), static String System.String.IsInterned(String str), TypeCode System.String.GetTypeCode(), CharEnumerator System.String.GetEnumerator(), Char System.String.Chars[Int32 index], Int32 System.String.Length, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{static String System.String.Join(String separator, String[] value), static String System.String.Join(String separator, Object[] values), static String System.String.Join(String separator, IEnumerable`1 values), static String System.String.Join(String separator, IEnumerable`1 values), static String System.String.Join(String separator, String[] value, Int32 startIndex, Int32 count), Boolean System.String.Equals(Object obj), Boolean System.String.Equals(String value), Boolean System.String.Equals(String value, StringComparison comparisonType), static Boolean System.String.Equals(String a, String b), static Boolean System.String.Equals(String a, String b, StringComparison comparisonType), static Boolean System.String.op_Equality(String a, String b), static Boolean System.String.op_Inequality(String a, String b), Char System.String.get_Chars(Int32 index), void System.String.CopyTo(Int32 sourceIndex, Char[] destination, Int32 destinationIndex, Int32 count), Char[] System.String.ToCharArray(), Char[] System.String.ToCharArray(Int32 startIndex, Int32 length), static Boolean System.String.IsNullOrEmpty(String value), static Boolean System.String.IsNullOrWhiteSpace(String value), Int32 System.String.GetHashCode(), Int32 System.String.get_Length(), String[] System.String.Split(Char[] separator), String[] System.String.Split(Char[] separator, Int32 count), String[] System.String.Split(Char[] separator, StringSplitOptions options), String[] System.String.Split(Char[] separator, Int32 count, StringSplitOptions options), String[] System.String.Split(String[] separator, StringSplitOptions options), String[] System.String.Split(String[] separator, Int32 count, StringSplitOptions options), String System.String.Substring(Int32 startIndex), String System.String.Substring(Int32 startIndex, Int32 length), String System.String.Trim(Char[] trimChars), String System.String.Trim(), String System.String.TrimStart(Char[] trimChars), String System.String.TrimEnd(Char[] trimChars), void System.String..ctor(Char* value), void System.String..ctor(Char* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length, Encoding enc), void System.String..ctor(Char[] value, Int32 startIndex, Int32 length), void System.String..ctor(Char[] value), void System.String..ctor(Char c, Int32 count), Boolean System.String.IsNormalized(), Boolean System.String.IsNormalized(NormalizationForm normalizationForm), String System.String.Normalize(), String System.String.Normalize(NormalizationForm normalizationForm), static Int32 System.String.Compare(String strA, String strB), static Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase), static Int32 System.String.Compare(String strA, String strB, StringComparison comparisonType), static Int32 System.String.Compare(String strA, String strB, CultureInfo culture, CompareOptions options), static Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase, CultureInfo culture), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase, CultureInfo culture), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, CultureInfo culture, CompareOptions options), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, StringComparison comparisonType), Int32 System.String.CompareTo(Object value), Int32 System.String.CompareTo(String strB), static Int32 System.String.CompareOrdinal(String strA, String strB), static Int32 System.String.CompareOrdinal(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), Boolean System.String.Contains(String value), Boolean System.String.EndsWith(String value), Boolean System.String.EndsWith(String value, StringComparison comparisonType), Boolean System.String.EndsWith(String value, Boolean ignoreCase, CultureInfo culture), Int32 System.String.IndexOf(Char value), Int32 System.String.IndexOf(Char value, Int32 startIndex), Int32 System.String.IndexOf(Char value, Int32 startIndex, Int32 count), Int32 System.String.IndexOf(String value), Int32 System.String.IndexOf(String value, Int32 startIndex), Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count), Int32 System.String.IndexOf(String value, StringComparison comparisonType), Int32 System.String.IndexOf(String value, Int32 startIndex, StringComparison comparisonType), Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), Int32 System.String.IndexOfAny(Char[] anyOf), Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex), Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(Char value), Int32 System.String.LastIndexOf(Char value, Int32 startIndex), Int32 System.String.LastIndexOf(Char value, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(String value), Int32 System.String.LastIndexOf(String value, Int32 startIndex), Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(String value, StringComparison comparisonType), Int32 System.String.LastIndexOf(String value, Int32 startIndex, StringComparison comparisonType), Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), Int32 System.String.LastIndexOfAny(Char[] anyOf), Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex), Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), String System.String.PadLeft(Int32 totalWidth), String System.String.PadLeft(Int32 totalWidth, Char paddingChar), String System.String.PadRight(Int32 totalWidth), String System.String.PadRight(Int32 totalWidth, Char paddingChar), Boolean System.String.StartsWith(String value), Boolean System.String.StartsWith(String value, StringComparison comparisonType), Boolean System.String.StartsWith(String value, Boolean ignoreCase, CultureInfo culture), String System.String.ToLower(), String System.String.ToLower(CultureInfo culture), String System.String.ToLowerInvariant(), String System.String.ToUpper(), String System.String.ToUpper(CultureInfo culture), String System.String.ToUpperInvariant(), String System.String.ToString(), String System.String.ToString(IFormatProvider provider), Object System.String.Clone(), String System.String.Insert(Int32 startIndex, String value), String System.String.Replace(Char oldChar, Char newChar), String System.String.Replace(String oldValue, String newValue), String System.String.Remove(Int32 startIndex, Int32 count), String System.String.Remove(Int32 startIndex), static String System.String.Format(String format, Object arg0), static String System.String.Format(String format, Object arg0, Object arg1), static String System.String.Format(String format, Object arg0, Object arg1, Object arg2), static String System.String.Format(String format, Object[] args), static String System.String.Format(IFormatProvider provider, String format, Object[] args), static String System.String.Copy(String str), static String System.String.Concat(Object arg0), static String System.String.Concat(Object arg0, Object arg1), static String System.String.Concat(Object arg0, Object arg1, Object arg2), static String System.String.Concat(Object arg0, Object arg1, Object arg2, Object arg3), static String System.String.Concat(Object[] args), static String System.String.Concat(IEnumerable`1 values), static String System.String.Concat(IEnumerable`1 values), static String System.String.Concat(String str0, String str1), static String System.String.Concat(String str0, String str1, String str2), static String System.String.Concat(String str0, String str1, String str2, String str3), static String System.String.Concat(String[] values), static String System.String.Intern(String str), static String System.String.IsInterned(String str), TypeCode System.String.GetTypeCode(), CharEnumerator System.String.GetEnumerator(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Char System.String.Chars[Int32 index], Int32 System.String.Length}"
              IsClass="True">
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
              GetMembers="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{System.Int32 MaxValue, System.Int32 MinValue, Int32 System.Int32.CompareTo(Object value), Int32 System.Int32.CompareTo(Int32 value), Boolean System.Int32.Equals(Object obj), Boolean System.Int32.Equals(Int32 obj), Int32 System.Int32.GetHashCode(), String System.Int32.ToString(), String System.Int32.ToString(String format), String System.Int32.ToString(IFormatProvider provider), String System.Int32.ToString(String format, IFormatProvider provider), static Int32 System.Int32.Parse(String s), static Int32 System.Int32.Parse(String s, NumberStyles style), static Int32 System.Int32.Parse(String s, IFormatProvider provider), static Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static Boolean System.Int32.TryParse(String s, Int32 result), static Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), TypeCode System.Int32.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Int32 System.Int32.CompareTo(Object value), Int32 System.Int32.CompareTo(Int32 value), Boolean System.Int32.Equals(Object obj), Boolean System.Int32.Equals(Int32 obj), Int32 System.Int32.GetHashCode(), String System.Int32.ToString(), String System.Int32.ToString(String format), String System.Int32.ToString(IFormatProvider provider), String System.Int32.ToString(String format, IFormatProvider provider), static Int32 System.Int32.Parse(String s), static Int32 System.Int32.Parse(String s, NumberStyles style), static Int32 System.Int32.Parse(String s, IFormatProvider provider), static Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static Boolean System.Int32.TryParse(String s, Int32 result), static Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), TypeCode System.Int32.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{System.String TrueString, System.String FalseString, Int32 System.Boolean.GetHashCode(), String System.Boolean.ToString(), String System.Boolean.ToString(IFormatProvider provider), Boolean System.Boolean.Equals(Object obj), Boolean System.Boolean.Equals(Boolean obj), Int32 System.Boolean.CompareTo(Object obj), Int32 System.Boolean.CompareTo(Boolean value), static Boolean System.Boolean.Parse(String value), static Boolean System.Boolean.TryParse(String value, Boolean result), TypeCode System.Boolean.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Int32 System.Boolean.GetHashCode(), String System.Boolean.ToString(), String System.Boolean.ToString(IFormatProvider provider), Boolean System.Boolean.Equals(Object obj), Boolean System.Boolean.Equals(Boolean obj), Int32 System.Boolean.CompareTo(Object obj), Int32 System.Boolean.CompareTo(Boolean value), static Boolean System.Boolean.Parse(String value), static Boolean System.Boolean.TryParse(String value, Boolean result), TypeCode System.Boolean.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{System.Char MaxValue, System.Char MinValue, Int32 System.Char.GetHashCode(), Boolean System.Char.Equals(Object obj), Boolean System.Char.Equals(Char obj), Int32 System.Char.CompareTo(Object value), Int32 System.Char.CompareTo(Char value), String System.Char.ToString(), String System.Char.ToString(IFormatProvider provider), static String System.Char.ToString(Char c), static Char System.Char.Parse(String s), static Boolean System.Char.TryParse(String s, Char result), static Boolean System.Char.IsDigit(Char c), static Boolean System.Char.IsDigit(String s, Int32 index), static Boolean System.Char.IsLetter(Char c), static Boolean System.Char.IsLetter(String s, Int32 index), static Boolean System.Char.IsWhiteSpace(Char c), static Boolean System.Char.IsWhiteSpace(String s, Int32 index), static Boolean System.Char.IsUpper(Char c), static Boolean System.Char.IsUpper(String s, Int32 index), static Boolean System.Char.IsLower(Char c), static Boolean System.Char.IsLower(String s, Int32 index), static Boolean System.Char.IsPunctuation(Char c), static Boolean System.Char.IsPunctuation(String s, Int32 index), static Boolean System.Char.IsLetterOrDigit(Char c), static Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static Char System.Char.ToUpper(Char c, CultureInfo culture), static Char System.Char.ToUpper(Char c), static Char System.Char.ToUpperInvariant(Char c), static Char System.Char.ToLower(Char c, CultureInfo culture), static Char System.Char.ToLower(Char c), static Char System.Char.ToLowerInvariant(Char c), TypeCode System.Char.GetTypeCode(), static Boolean System.Char.IsControl(Char c), static Boolean System.Char.IsControl(String s, Int32 index), static Boolean System.Char.IsNumber(Char c), static Boolean System.Char.IsNumber(String s, Int32 index), static Boolean System.Char.IsSeparator(Char c), static Boolean System.Char.IsSeparator(String s, Int32 index), static Boolean System.Char.IsSurrogate(Char c), static Boolean System.Char.IsSurrogate(String s, Int32 index), static Boolean System.Char.IsSymbol(Char c), static Boolean System.Char.IsSymbol(String s, Int32 index), static UnicodeCategory System.Char.GetUnicodeCategory(Char c), static UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static Double System.Char.GetNumericValue(Char c), static Double System.Char.GetNumericValue(String s, Int32 index), static Boolean System.Char.IsHighSurrogate(Char c), static Boolean System.Char.IsHighSurrogate(String s, Int32 index), static Boolean System.Char.IsLowSurrogate(Char c), static Boolean System.Char.IsLowSurrogate(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static String System.Char.ConvertFromUtf32(Int32 utf32), static Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static Int32 System.Char.ConvertToUtf32(String s, Int32 index), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Int32 System.Char.GetHashCode(), Boolean System.Char.Equals(Object obj), Boolean System.Char.Equals(Char obj), Int32 System.Char.CompareTo(Object value), Int32 System.Char.CompareTo(Char value), String System.Char.ToString(), String System.Char.ToString(IFormatProvider provider), static String System.Char.ToString(Char c), static Char System.Char.Parse(String s), static Boolean System.Char.TryParse(String s, Char result), static Boolean System.Char.IsDigit(Char c), static Boolean System.Char.IsDigit(String s, Int32 index), static Boolean System.Char.IsLetter(Char c), static Boolean System.Char.IsLetter(String s, Int32 index), static Boolean System.Char.IsWhiteSpace(Char c), static Boolean System.Char.IsWhiteSpace(String s, Int32 index), static Boolean System.Char.IsUpper(Char c), static Boolean System.Char.IsUpper(String s, Int32 index), static Boolean System.Char.IsLower(Char c), static Boolean System.Char.IsLower(String s, Int32 index), static Boolean System.Char.IsPunctuation(Char c), static Boolean System.Char.IsPunctuation(String s, Int32 index), static Boolean System.Char.IsLetterOrDigit(Char c), static Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static Char System.Char.ToUpper(Char c, CultureInfo culture), static Char System.Char.ToUpper(Char c), static Char System.Char.ToUpperInvariant(Char c), static Char System.Char.ToLower(Char c, CultureInfo culture), static Char System.Char.ToLower(Char c), static Char System.Char.ToLowerInvariant(Char c), TypeCode System.Char.GetTypeCode(), static Boolean System.Char.IsControl(Char c), static Boolean System.Char.IsControl(String s, Int32 index), static Boolean System.Char.IsNumber(Char c), static Boolean System.Char.IsNumber(String s, Int32 index), static Boolean System.Char.IsSeparator(Char c), static Boolean System.Char.IsSeparator(String s, Int32 index), static Boolean System.Char.IsSurrogate(Char c), static Boolean System.Char.IsSurrogate(String s, Int32 index), static Boolean System.Char.IsSymbol(Char c), static Boolean System.Char.IsSymbol(String s, Int32 index), static UnicodeCategory System.Char.GetUnicodeCategory(Char c), static UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static Double System.Char.GetNumericValue(Char c), static Double System.Char.GetNumericValue(String s, Int32 index), static Boolean System.Char.IsHighSurrogate(Char c), static Boolean System.Char.IsHighSurrogate(String s, Int32 index), static Boolean System.Char.IsLowSurrogate(Char c), static Boolean System.Char.IsLowSurrogate(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static String System.Char.ConvertFromUtf32(Int32 utf32), static Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static Int32 System.Char.ConvertToUtf32(String s, Int32 index), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{System.String Empty, static String System.String.Join(String separator, String[] value), static String System.String.Join(String separator, Object[] values), static String System.String.Join(String separator, IEnumerable`1 values), static String System.String.Join(String separator, IEnumerable`1 values), static String System.String.Join(String separator, String[] value, Int32 startIndex, Int32 count), Boolean System.String.Equals(Object obj), Boolean System.String.Equals(String value), Boolean System.String.Equals(String value, StringComparison comparisonType), static Boolean System.String.Equals(String a, String b), static Boolean System.String.Equals(String a, String b, StringComparison comparisonType), static Boolean System.String.op_Equality(String a, String b), static Boolean System.String.op_Inequality(String a, String b), Char System.String.get_Chars(Int32 index), void System.String.CopyTo(Int32 sourceIndex, Char[] destination, Int32 destinationIndex, Int32 count), Char[] System.String.ToCharArray(), Char[] System.String.ToCharArray(Int32 startIndex, Int32 length), static Boolean System.String.IsNullOrEmpty(String value), static Boolean System.String.IsNullOrWhiteSpace(String value), Int32 System.String.GetHashCode(), Int32 System.String.get_Length(), String[] System.String.Split(Char[] separator), String[] System.String.Split(Char[] separator, Int32 count), String[] System.String.Split(Char[] separator, StringSplitOptions options), String[] System.String.Split(Char[] separator, Int32 count, StringSplitOptions options), String[] System.String.Split(String[] separator, StringSplitOptions options), String[] System.String.Split(String[] separator, Int32 count, StringSplitOptions options), String System.String.Substring(Int32 startIndex), String System.String.Substring(Int32 startIndex, Int32 length), String System.String.Trim(Char[] trimChars), String System.String.Trim(), String System.String.TrimStart(Char[] trimChars), String System.String.TrimEnd(Char[] trimChars), void System.String..ctor(Char* value), void System.String..ctor(Char* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length, Encoding enc), void System.String..ctor(Char[] value, Int32 startIndex, Int32 length), void System.String..ctor(Char[] value), void System.String..ctor(Char c, Int32 count), Boolean System.String.IsNormalized(), Boolean System.String.IsNormalized(NormalizationForm normalizationForm), String System.String.Normalize(), String System.String.Normalize(NormalizationForm normalizationForm), static Int32 System.String.Compare(String strA, String strB), static Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase), static Int32 System.String.Compare(String strA, String strB, StringComparison comparisonType), static Int32 System.String.Compare(String strA, String strB, CultureInfo culture, CompareOptions options), static Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase, CultureInfo culture), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase, CultureInfo culture), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, CultureInfo culture, CompareOptions options), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, StringComparison comparisonType), Int32 System.String.CompareTo(Object value), Int32 System.String.CompareTo(String strB), static Int32 System.String.CompareOrdinal(String strA, String strB), static Int32 System.String.CompareOrdinal(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), Boolean System.String.Contains(String value), Boolean System.String.EndsWith(String value), Boolean System.String.EndsWith(String value, StringComparison comparisonType), Boolean System.String.EndsWith(String value, Boolean ignoreCase, CultureInfo culture), Int32 System.String.IndexOf(Char value), Int32 System.String.IndexOf(Char value, Int32 startIndex), Int32 System.String.IndexOf(Char value, Int32 startIndex, Int32 count), Int32 System.String.IndexOf(String value), Int32 System.String.IndexOf(String value, Int32 startIndex), Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count), Int32 System.String.IndexOf(String value, StringComparison comparisonType), Int32 System.String.IndexOf(String value, Int32 startIndex, StringComparison comparisonType), Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), Int32 System.String.IndexOfAny(Char[] anyOf), Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex), Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(Char value), Int32 System.String.LastIndexOf(Char value, Int32 startIndex), Int32 System.String.LastIndexOf(Char value, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(String value), Int32 System.String.LastIndexOf(String value, Int32 startIndex), Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(String value, StringComparison comparisonType), Int32 System.String.LastIndexOf(String value, Int32 startIndex, StringComparison comparisonType), Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), Int32 System.String.LastIndexOfAny(Char[] anyOf), Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex), Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), String System.String.PadLeft(Int32 totalWidth), String System.String.PadLeft(Int32 totalWidth, Char paddingChar), String System.String.PadRight(Int32 totalWidth), String System.String.PadRight(Int32 totalWidth, Char paddingChar), Boolean System.String.StartsWith(String value), Boolean System.String.StartsWith(String value, StringComparison comparisonType), Boolean System.String.StartsWith(String value, Boolean ignoreCase, CultureInfo culture), String System.String.ToLower(), String System.String.ToLower(CultureInfo culture), String System.String.ToLowerInvariant(), String System.String.ToUpper(), String System.String.ToUpper(CultureInfo culture), String System.String.ToUpperInvariant(), String System.String.ToString(), String System.String.ToString(IFormatProvider provider), Object System.String.Clone(), String System.String.Insert(Int32 startIndex, String value), String System.String.Replace(Char oldChar, Char newChar), String System.String.Replace(String oldValue, String newValue), String System.String.Remove(Int32 startIndex, Int32 count), String System.String.Remove(Int32 startIndex), static String System.String.Format(String format, Object arg0), static String System.String.Format(String format, Object arg0, Object arg1), static String System.String.Format(String format, Object arg0, Object arg1, Object arg2), static String System.String.Format(String format, Object[] args), static String System.String.Format(IFormatProvider provider, String format, Object[] args), static String System.String.Copy(String str), static String System.String.Concat(Object arg0), static String System.String.Concat(Object arg0, Object arg1), static String System.String.Concat(Object arg0, Object arg1, Object arg2), static String System.String.Concat(Object arg0, Object arg1, Object arg2, Object arg3), static String System.String.Concat(Object[] args), static String System.String.Concat(IEnumerable`1 values), static String System.String.Concat(IEnumerable`1 values), static String System.String.Concat(String str0, String str1), static String System.String.Concat(String str0, String str1, String str2), static String System.String.Concat(String str0, String str1, String str2, String str3), static String System.String.Concat(String[] values), static String System.String.Intern(String str), static String System.String.IsInterned(String str), TypeCode System.String.GetTypeCode(), CharEnumerator System.String.GetEnumerator(), Char System.String.Chars[Int32 index], Int32 System.String.Length, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{static String System.String.Join(String separator, String[] value), static String System.String.Join(String separator, Object[] values), static String System.String.Join(String separator, IEnumerable`1 values), static String System.String.Join(String separator, IEnumerable`1 values), static String System.String.Join(String separator, String[] value, Int32 startIndex, Int32 count), Boolean System.String.Equals(Object obj), Boolean System.String.Equals(String value), Boolean System.String.Equals(String value, StringComparison comparisonType), static Boolean System.String.Equals(String a, String b), static Boolean System.String.Equals(String a, String b, StringComparison comparisonType), static Boolean System.String.op_Equality(String a, String b), static Boolean System.String.op_Inequality(String a, String b), Char System.String.get_Chars(Int32 index), void System.String.CopyTo(Int32 sourceIndex, Char[] destination, Int32 destinationIndex, Int32 count), Char[] System.String.ToCharArray(), Char[] System.String.ToCharArray(Int32 startIndex, Int32 length), static Boolean System.String.IsNullOrEmpty(String value), static Boolean System.String.IsNullOrWhiteSpace(String value), Int32 System.String.GetHashCode(), Int32 System.String.get_Length(), String[] System.String.Split(Char[] separator), String[] System.String.Split(Char[] separator, Int32 count), String[] System.String.Split(Char[] separator, StringSplitOptions options), String[] System.String.Split(Char[] separator, Int32 count, StringSplitOptions options), String[] System.String.Split(String[] separator, StringSplitOptions options), String[] System.String.Split(String[] separator, Int32 count, StringSplitOptions options), String System.String.Substring(Int32 startIndex), String System.String.Substring(Int32 startIndex, Int32 length), String System.String.Trim(Char[] trimChars), String System.String.Trim(), String System.String.TrimStart(Char[] trimChars), String System.String.TrimEnd(Char[] trimChars), void System.String..ctor(Char* value), void System.String..ctor(Char* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length), void System.String..ctor(SByte* value, Int32 startIndex, Int32 length, Encoding enc), void System.String..ctor(Char[] value, Int32 startIndex, Int32 length), void System.String..ctor(Char[] value), void System.String..ctor(Char c, Int32 count), Boolean System.String.IsNormalized(), Boolean System.String.IsNormalized(NormalizationForm normalizationForm), String System.String.Normalize(), String System.String.Normalize(NormalizationForm normalizationForm), static Int32 System.String.Compare(String strA, String strB), static Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase), static Int32 System.String.Compare(String strA, String strB, StringComparison comparisonType), static Int32 System.String.Compare(String strA, String strB, CultureInfo culture, CompareOptions options), static Int32 System.String.Compare(String strA, String strB, Boolean ignoreCase, CultureInfo culture), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, Boolean ignoreCase, CultureInfo culture), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, CultureInfo culture, CompareOptions options), static Int32 System.String.Compare(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length, StringComparison comparisonType), Int32 System.String.CompareTo(Object value), Int32 System.String.CompareTo(String strB), static Int32 System.String.CompareOrdinal(String strA, String strB), static Int32 System.String.CompareOrdinal(String strA, Int32 indexA, String strB, Int32 indexB, Int32 length), Boolean System.String.Contains(String value), Boolean System.String.EndsWith(String value), Boolean System.String.EndsWith(String value, StringComparison comparisonType), Boolean System.String.EndsWith(String value, Boolean ignoreCase, CultureInfo culture), Int32 System.String.IndexOf(Char value), Int32 System.String.IndexOf(Char value, Int32 startIndex), Int32 System.String.IndexOf(Char value, Int32 startIndex, Int32 count), Int32 System.String.IndexOf(String value), Int32 System.String.IndexOf(String value, Int32 startIndex), Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count), Int32 System.String.IndexOf(String value, StringComparison comparisonType), Int32 System.String.IndexOf(String value, Int32 startIndex, StringComparison comparisonType), Int32 System.String.IndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), Int32 System.String.IndexOfAny(Char[] anyOf), Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex), Int32 System.String.IndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(Char value), Int32 System.String.LastIndexOf(Char value, Int32 startIndex), Int32 System.String.LastIndexOf(Char value, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(String value), Int32 System.String.LastIndexOf(String value, Int32 startIndex), Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count), Int32 System.String.LastIndexOf(String value, StringComparison comparisonType), Int32 System.String.LastIndexOf(String value, Int32 startIndex, StringComparison comparisonType), Int32 System.String.LastIndexOf(String value, Int32 startIndex, Int32 count, StringComparison comparisonType), Int32 System.String.LastIndexOfAny(Char[] anyOf), Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex), Int32 System.String.LastIndexOfAny(Char[] anyOf, Int32 startIndex, Int32 count), String System.String.PadLeft(Int32 totalWidth), String System.String.PadLeft(Int32 totalWidth, Char paddingChar), String System.String.PadRight(Int32 totalWidth), String System.String.PadRight(Int32 totalWidth, Char paddingChar), Boolean System.String.StartsWith(String value), Boolean System.String.StartsWith(String value, StringComparison comparisonType), Boolean System.String.StartsWith(String value, Boolean ignoreCase, CultureInfo culture), String System.String.ToLower(), String System.String.ToLower(CultureInfo culture), String System.String.ToLowerInvariant(), String System.String.ToUpper(), String System.String.ToUpper(CultureInfo culture), String System.String.ToUpperInvariant(), String System.String.ToString(), String System.String.ToString(IFormatProvider provider), Object System.String.Clone(), String System.String.Insert(Int32 startIndex, String value), String System.String.Replace(Char oldChar, Char newChar), String System.String.Replace(String oldValue, String newValue), String System.String.Remove(Int32 startIndex, Int32 count), String System.String.Remove(Int32 startIndex), static String System.String.Format(String format, Object arg0), static String System.String.Format(String format, Object arg0, Object arg1), static String System.String.Format(String format, Object arg0, Object arg1, Object arg2), static String System.String.Format(String format, Object[] args), static String System.String.Format(IFormatProvider provider, String format, Object[] args), static String System.String.Copy(String str), static String System.String.Concat(Object arg0), static String System.String.Concat(Object arg0, Object arg1), static String System.String.Concat(Object arg0, Object arg1, Object arg2), static String System.String.Concat(Object arg0, Object arg1, Object arg2, Object arg3), static String System.String.Concat(Object[] args), static String System.String.Concat(IEnumerable`1 values), static String System.String.Concat(IEnumerable`1 values), static String System.String.Concat(String str0, String str1), static String System.String.Concat(String str0, String str1, String str2), static String System.String.Concat(String str0, String str1, String str2, String str3), static String System.String.Concat(String[] values), static String System.String.Intern(String str), static String System.String.IsInterned(String str), TypeCode System.String.GetTypeCode(), CharEnumerator System.String.GetEnumerator(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Char System.String.Chars[Int32 index], Int32 System.String.Length}"
              IsClass="True">
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
              GetMembers="{void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), static Boolean System.Object.Equals(Object objA, Object objB), static Boolean System.Object.ReferenceEquals(Object objA, Object objB), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), static Boolean System.Object.Equals(Object objA, Object objB), static Boolean System.Object.ReferenceEquals(Object objA, Object objB), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+MyClass..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), static Boolean System.Object.Equals(Object objA, Object objB), static Boolean System.Object.ReferenceEquals(Object objA, Object objB), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), static Boolean System.Object.Equals(Object objA, Object objB), static Boolean System.Object.ReferenceEquals(Object objA, Object objB), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
                  GetMembers="{System.Int32 MaxValue, System.Int32 MinValue, Int32 System.Int32.CompareTo(Object value), Int32 System.Int32.CompareTo(Int32 value), Boolean System.Int32.Equals(Object obj), Boolean System.Int32.Equals(Int32 obj), Int32 System.Int32.GetHashCode(), String System.Int32.ToString(), String System.Int32.ToString(String format), String System.Int32.ToString(IFormatProvider provider), String System.Int32.ToString(String format, IFormatProvider provider), static Int32 System.Int32.Parse(String s), static Int32 System.Int32.Parse(String s, NumberStyles style), static Int32 System.Int32.Parse(String s, IFormatProvider provider), static Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static Boolean System.Int32.TryParse(String s, Int32 result), static Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), TypeCode System.Int32.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                  GetMethods="{Int32 System.Int32.CompareTo(Object value), Int32 System.Int32.CompareTo(Int32 value), Boolean System.Int32.Equals(Object obj), Boolean System.Int32.Equals(Int32 obj), Int32 System.Int32.GetHashCode(), String System.Int32.ToString(), String System.Int32.ToString(String format), String System.Int32.ToString(IFormatProvider provider), String System.Int32.ToString(String format, IFormatProvider provider), static Int32 System.Int32.Parse(String s), static Int32 System.Int32.Parse(String s, NumberStyles style), static Int32 System.Int32.Parse(String s, IFormatProvider provider), static Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static Boolean System.Int32.TryParse(String s, Int32 result), static Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), TypeCode System.Int32.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
                      GetMembers="{System.Int32 MaxValue, System.Int32 MinValue, Int32 System.Int32.CompareTo(Object value), Int32 System.Int32.CompareTo(Int32 value), Boolean System.Int32.Equals(Object obj), Boolean System.Int32.Equals(Int32 obj), Int32 System.Int32.GetHashCode(), String System.Int32.ToString(), String System.Int32.ToString(String format), String System.Int32.ToString(IFormatProvider provider), String System.Int32.ToString(String format, IFormatProvider provider), static Int32 System.Int32.Parse(String s), static Int32 System.Int32.Parse(String s, NumberStyles style), static Int32 System.Int32.Parse(String s, IFormatProvider provider), static Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static Boolean System.Int32.TryParse(String s, Int32 result), static Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), TypeCode System.Int32.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                      GetMethods="{Int32 System.Int32.CompareTo(Object value), Int32 System.Int32.CompareTo(Int32 value), Boolean System.Int32.Equals(Object obj), Boolean System.Int32.Equals(Int32 obj), Int32 System.Int32.GetHashCode(), String System.Int32.ToString(), String System.Int32.ToString(String format), String System.Int32.ToString(IFormatProvider provider), String System.Int32.ToString(String format, IFormatProvider provider), static Int32 System.Int32.Parse(String s), static Int32 System.Int32.Parse(String s, NumberStyles style), static Int32 System.Int32.Parse(String s, IFormatProvider provider), static Int32 System.Int32.Parse(String s, NumberStyles style, IFormatProvider provider), static Boolean System.Int32.TryParse(String s, Int32 result), static Boolean System.Int32.TryParse(String s, NumberStyles style, IFormatProvider provider, Int32 result), TypeCode System.Int32.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
                  GetMembers="{System.String TrueString, System.String FalseString, Int32 System.Boolean.GetHashCode(), String System.Boolean.ToString(), String System.Boolean.ToString(IFormatProvider provider), Boolean System.Boolean.Equals(Object obj), Boolean System.Boolean.Equals(Boolean obj), Int32 System.Boolean.CompareTo(Object obj), Int32 System.Boolean.CompareTo(Boolean value), static Boolean System.Boolean.Parse(String value), static Boolean System.Boolean.TryParse(String value, Boolean result), TypeCode System.Boolean.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                  GetMethods="{Int32 System.Boolean.GetHashCode(), String System.Boolean.ToString(), String System.Boolean.ToString(IFormatProvider provider), Boolean System.Boolean.Equals(Object obj), Boolean System.Boolean.Equals(Boolean obj), Int32 System.Boolean.CompareTo(Object obj), Int32 System.Boolean.CompareTo(Boolean value), static Boolean System.Boolean.Parse(String value), static Boolean System.Boolean.TryParse(String value, Boolean result), TypeCode System.Boolean.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
                  GetMembers="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                  GetMethods="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
                  GetMembers="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                  GetMethods="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{System.IntPtr Zero, void System.IntPtr..ctor(Int32 value), void System.IntPtr..ctor(Int64 value), void System.IntPtr..ctor(Void* value), Boolean System.IntPtr.Equals(Object obj), Int32 System.IntPtr.GetHashCode(), Int32 System.IntPtr.ToInt32(), Int64 System.IntPtr.ToInt64(), String System.IntPtr.ToString(), String System.IntPtr.ToString(String format), static IntPtr System.IntPtr.op_Explicit(Int32 value), static IntPtr System.IntPtr.op_Explicit(Int64 value), static IntPtr System.IntPtr.op_Explicit(Void* value), static Void* System.IntPtr.op_Explicit(IntPtr value), static Int32 System.IntPtr.op_Explicit(IntPtr value), static Int64 System.IntPtr.op_Explicit(IntPtr value), static Boolean System.IntPtr.op_Equality(IntPtr value1, IntPtr value2), static Boolean System.IntPtr.op_Inequality(IntPtr value1, IntPtr value2), static IntPtr System.IntPtr.Add(IntPtr pointer, Int32 offset), static IntPtr System.IntPtr.op_Addition(IntPtr pointer, Int32 offset), static IntPtr System.IntPtr.Subtract(IntPtr pointer, Int32 offset), static IntPtr System.IntPtr.op_Subtraction(IntPtr pointer, Int32 offset), static Int32 System.IntPtr.get_Size(), Void* System.IntPtr.ToPointer(), static Int32 System.IntPtr.Size, Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void System.IntPtr..ctor(Int32 value), void System.IntPtr..ctor(Int64 value), void System.IntPtr..ctor(Void* value), Boolean System.IntPtr.Equals(Object obj), Int32 System.IntPtr.GetHashCode(), Int32 System.IntPtr.ToInt32(), Int64 System.IntPtr.ToInt64(), String System.IntPtr.ToString(), String System.IntPtr.ToString(String format), static IntPtr System.IntPtr.op_Explicit(Int32 value), static IntPtr System.IntPtr.op_Explicit(Int64 value), static IntPtr System.IntPtr.op_Explicit(Void* value), static Void* System.IntPtr.op_Explicit(IntPtr value), static Int32 System.IntPtr.op_Explicit(IntPtr value), static Int64 System.IntPtr.op_Explicit(IntPtr value), static Boolean System.IntPtr.op_Equality(IntPtr value1, IntPtr value2), static Boolean System.IntPtr.op_Inequality(IntPtr value1, IntPtr value2), static IntPtr System.IntPtr.Add(IntPtr pointer, Int32 offset), static IntPtr System.IntPtr.op_Addition(IntPtr pointer, Int32 offset), static IntPtr System.IntPtr.Subtract(IntPtr pointer, Int32 offset), static IntPtr System.IntPtr.op_Subtraction(IntPtr pointer, Int32 offset), static Int32 System.IntPtr.get_Size(), Void* System.IntPtr.ToPointer(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{static Int32 System.IntPtr.Size}"
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
              GetInterfaces="{System.Collections.Generic.IList`1[System.Char], System.Collections.Generic.ICollection`1[System.Char], System.Collections.Generic.IEnumerable`1[System.Char], System.Collections.IEnumerable, System.ICloneable, System.Collections.IList, System.Collections.ICollection, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable}"
              GetMembers="{Object System.Array.GetValue(Int32[] indices), Object System.Array.GetValue(Int32 index), Object System.Array.GetValue(Int32 index1, Int32 index2), Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), Object System.Array.GetValue(Int64 index), Object System.Array.GetValue(Int64 index1, Int64 index2), Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), Int32 System.Array.get_Length(), Int64 System.Array.get_LongLength(), Int32 System.Array.GetLength(Int32 dimension), Int64 System.Array.GetLongLength(Int32 dimension), Int32 System.Array.get_Rank(), Int32 System.Array.GetUpperBound(Int32 dimension), Int32 System.Array.GetLowerBound(Int32 dimension), Object System.Array.get_SyncRoot(), Boolean System.Array.get_IsReadOnly(), Boolean System.Array.get_IsFixedSize(), Boolean System.Array.get_IsSynchronized(), Object System.Array.Clone(), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), Int32 System.Array.Length, Int64 System.Array.LongLength, Int32 System.Array.Rank, Object System.Array.SyncRoot, Boolean System.Array.IsReadOnly, Boolean System.Array.IsFixedSize, Boolean System.Array.IsSynchronized, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Object System.Array.GetValue(Int32[] indices), Object System.Array.GetValue(Int32 index), Object System.Array.GetValue(Int32 index1, Int32 index2), Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), Object System.Array.GetValue(Int64 index), Object System.Array.GetValue(Int64 index1, Int64 index2), Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), Int32 System.Array.get_Length(), Int64 System.Array.get_LongLength(), Int32 System.Array.GetLength(Int32 dimension), Int64 System.Array.GetLongLength(Int32 dimension), Int32 System.Array.get_Rank(), Int32 System.Array.GetUpperBound(Int32 dimension), Int32 System.Array.GetLowerBound(Int32 dimension), Object System.Array.get_SyncRoot(), Boolean System.Array.get_IsReadOnly(), Boolean System.Array.get_IsFixedSize(), Boolean System.Array.get_IsSynchronized(), Object System.Array.Clone(), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Int32 System.Array.Length, Int64 System.Array.LongLength, Int32 System.Array.Rank, Object System.Array.SyncRoot, Boolean System.Array.IsReadOnly, Boolean System.Array.IsFixedSize, Boolean System.Array.IsSynchronized}"
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
                  GetMembers="{System.Char MaxValue, System.Char MinValue, Int32 System.Char.GetHashCode(), Boolean System.Char.Equals(Object obj), Boolean System.Char.Equals(Char obj), Int32 System.Char.CompareTo(Object value), Int32 System.Char.CompareTo(Char value), String System.Char.ToString(), String System.Char.ToString(IFormatProvider provider), static String System.Char.ToString(Char c), static Char System.Char.Parse(String s), static Boolean System.Char.TryParse(String s, Char result), static Boolean System.Char.IsDigit(Char c), static Boolean System.Char.IsDigit(String s, Int32 index), static Boolean System.Char.IsLetter(Char c), static Boolean System.Char.IsLetter(String s, Int32 index), static Boolean System.Char.IsWhiteSpace(Char c), static Boolean System.Char.IsWhiteSpace(String s, Int32 index), static Boolean System.Char.IsUpper(Char c), static Boolean System.Char.IsUpper(String s, Int32 index), static Boolean System.Char.IsLower(Char c), static Boolean System.Char.IsLower(String s, Int32 index), static Boolean System.Char.IsPunctuation(Char c), static Boolean System.Char.IsPunctuation(String s, Int32 index), static Boolean System.Char.IsLetterOrDigit(Char c), static Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static Char System.Char.ToUpper(Char c, CultureInfo culture), static Char System.Char.ToUpper(Char c), static Char System.Char.ToUpperInvariant(Char c), static Char System.Char.ToLower(Char c, CultureInfo culture), static Char System.Char.ToLower(Char c), static Char System.Char.ToLowerInvariant(Char c), TypeCode System.Char.GetTypeCode(), static Boolean System.Char.IsControl(Char c), static Boolean System.Char.IsControl(String s, Int32 index), static Boolean System.Char.IsNumber(Char c), static Boolean System.Char.IsNumber(String s, Int32 index), static Boolean System.Char.IsSeparator(Char c), static Boolean System.Char.IsSeparator(String s, Int32 index), static Boolean System.Char.IsSurrogate(Char c), static Boolean System.Char.IsSurrogate(String s, Int32 index), static Boolean System.Char.IsSymbol(Char c), static Boolean System.Char.IsSymbol(String s, Int32 index), static UnicodeCategory System.Char.GetUnicodeCategory(Char c), static UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static Double System.Char.GetNumericValue(Char c), static Double System.Char.GetNumericValue(String s, Int32 index), static Boolean System.Char.IsHighSurrogate(Char c), static Boolean System.Char.IsHighSurrogate(String s, Int32 index), static Boolean System.Char.IsLowSurrogate(Char c), static Boolean System.Char.IsLowSurrogate(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static String System.Char.ConvertFromUtf32(Int32 utf32), static Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static Int32 System.Char.ConvertToUtf32(String s, Int32 index), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                  GetMethods="{Int32 System.Char.GetHashCode(), Boolean System.Char.Equals(Object obj), Boolean System.Char.Equals(Char obj), Int32 System.Char.CompareTo(Object value), Int32 System.Char.CompareTo(Char value), String System.Char.ToString(), String System.Char.ToString(IFormatProvider provider), static String System.Char.ToString(Char c), static Char System.Char.Parse(String s), static Boolean System.Char.TryParse(String s, Char result), static Boolean System.Char.IsDigit(Char c), static Boolean System.Char.IsDigit(String s, Int32 index), static Boolean System.Char.IsLetter(Char c), static Boolean System.Char.IsLetter(String s, Int32 index), static Boolean System.Char.IsWhiteSpace(Char c), static Boolean System.Char.IsWhiteSpace(String s, Int32 index), static Boolean System.Char.IsUpper(Char c), static Boolean System.Char.IsUpper(String s, Int32 index), static Boolean System.Char.IsLower(Char c), static Boolean System.Char.IsLower(String s, Int32 index), static Boolean System.Char.IsPunctuation(Char c), static Boolean System.Char.IsPunctuation(String s, Int32 index), static Boolean System.Char.IsLetterOrDigit(Char c), static Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static Char System.Char.ToUpper(Char c, CultureInfo culture), static Char System.Char.ToUpper(Char c), static Char System.Char.ToUpperInvariant(Char c), static Char System.Char.ToLower(Char c, CultureInfo culture), static Char System.Char.ToLower(Char c), static Char System.Char.ToLowerInvariant(Char c), TypeCode System.Char.GetTypeCode(), static Boolean System.Char.IsControl(Char c), static Boolean System.Char.IsControl(String s, Int32 index), static Boolean System.Char.IsNumber(Char c), static Boolean System.Char.IsNumber(String s, Int32 index), static Boolean System.Char.IsSeparator(Char c), static Boolean System.Char.IsSeparator(String s, Int32 index), static Boolean System.Char.IsSurrogate(Char c), static Boolean System.Char.IsSurrogate(String s, Int32 index), static Boolean System.Char.IsSymbol(Char c), static Boolean System.Char.IsSymbol(String s, Int32 index), static UnicodeCategory System.Char.GetUnicodeCategory(Char c), static UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static Double System.Char.GetNumericValue(Char c), static Double System.Char.GetNumericValue(String s, Int32 index), static Boolean System.Char.IsHighSurrogate(Char c), static Boolean System.Char.IsHighSurrogate(String s, Int32 index), static Boolean System.Char.IsLowSurrogate(Char c), static Boolean System.Char.IsLowSurrogate(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static String System.Char.ConvertFromUtf32(Int32 utf32), static Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static Int32 System.Char.ConvertToUtf32(String s, Int32 index), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetInterfaces="{System.Collections.Generic.IList`1[System.Char], System.Collections.Generic.ICollection`1[System.Char], System.Collections.Generic.IEnumerable`1[System.Char], System.Collections.IEnumerable, System.ICloneable, System.Collections.IList, System.Collections.ICollection, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable}"
              GetMembers="{Object System.Array.GetValue(Int32[] indices), Object System.Array.GetValue(Int32 index), Object System.Array.GetValue(Int32 index1, Int32 index2), Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), Object System.Array.GetValue(Int64 index), Object System.Array.GetValue(Int64 index1, Int64 index2), Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), Int32 System.Array.get_Length(), Int64 System.Array.get_LongLength(), Int32 System.Array.GetLength(Int32 dimension), Int64 System.Array.GetLongLength(Int32 dimension), Int32 System.Array.get_Rank(), Int32 System.Array.GetUpperBound(Int32 dimension), Int32 System.Array.GetLowerBound(Int32 dimension), Object System.Array.get_SyncRoot(), Boolean System.Array.get_IsReadOnly(), Boolean System.Array.get_IsFixedSize(), Boolean System.Array.get_IsSynchronized(), Object System.Array.Clone(), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), Int32 System.Array.Length, Int64 System.Array.LongLength, Int32 System.Array.Rank, Object System.Array.SyncRoot, Boolean System.Array.IsReadOnly, Boolean System.Array.IsFixedSize, Boolean System.Array.IsSynchronized, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Object System.Array.GetValue(Int32[] indices), Object System.Array.GetValue(Int32 index), Object System.Array.GetValue(Int32 index1, Int32 index2), Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), Object System.Array.GetValue(Int64 index), Object System.Array.GetValue(Int64 index1, Int64 index2), Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), Int32 System.Array.get_Length(), Int64 System.Array.get_LongLength(), Int32 System.Array.GetLength(Int32 dimension), Int64 System.Array.GetLongLength(Int32 dimension), Int32 System.Array.get_Rank(), Int32 System.Array.GetUpperBound(Int32 dimension), Int32 System.Array.GetLowerBound(Int32 dimension), Object System.Array.get_SyncRoot(), Boolean System.Array.get_IsReadOnly(), Boolean System.Array.get_IsFixedSize(), Boolean System.Array.get_IsSynchronized(), Object System.Array.Clone(), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Int32 System.Array.Length, Int64 System.Array.LongLength, Int32 System.Array.Rank, Object System.Array.SyncRoot, Boolean System.Array.IsReadOnly, Boolean System.Array.IsFixedSize, Boolean System.Array.IsSynchronized}"
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
                  GetMembers="{System.Char MaxValue, System.Char MinValue, Int32 System.Char.GetHashCode(), Boolean System.Char.Equals(Object obj), Boolean System.Char.Equals(Char obj), Int32 System.Char.CompareTo(Object value), Int32 System.Char.CompareTo(Char value), String System.Char.ToString(), String System.Char.ToString(IFormatProvider provider), static String System.Char.ToString(Char c), static Char System.Char.Parse(String s), static Boolean System.Char.TryParse(String s, Char result), static Boolean System.Char.IsDigit(Char c), static Boolean System.Char.IsDigit(String s, Int32 index), static Boolean System.Char.IsLetter(Char c), static Boolean System.Char.IsLetter(String s, Int32 index), static Boolean System.Char.IsWhiteSpace(Char c), static Boolean System.Char.IsWhiteSpace(String s, Int32 index), static Boolean System.Char.IsUpper(Char c), static Boolean System.Char.IsUpper(String s, Int32 index), static Boolean System.Char.IsLower(Char c), static Boolean System.Char.IsLower(String s, Int32 index), static Boolean System.Char.IsPunctuation(Char c), static Boolean System.Char.IsPunctuation(String s, Int32 index), static Boolean System.Char.IsLetterOrDigit(Char c), static Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static Char System.Char.ToUpper(Char c, CultureInfo culture), static Char System.Char.ToUpper(Char c), static Char System.Char.ToUpperInvariant(Char c), static Char System.Char.ToLower(Char c, CultureInfo culture), static Char System.Char.ToLower(Char c), static Char System.Char.ToLowerInvariant(Char c), TypeCode System.Char.GetTypeCode(), static Boolean System.Char.IsControl(Char c), static Boolean System.Char.IsControl(String s, Int32 index), static Boolean System.Char.IsNumber(Char c), static Boolean System.Char.IsNumber(String s, Int32 index), static Boolean System.Char.IsSeparator(Char c), static Boolean System.Char.IsSeparator(String s, Int32 index), static Boolean System.Char.IsSurrogate(Char c), static Boolean System.Char.IsSurrogate(String s, Int32 index), static Boolean System.Char.IsSymbol(Char c), static Boolean System.Char.IsSymbol(String s, Int32 index), static UnicodeCategory System.Char.GetUnicodeCategory(Char c), static UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static Double System.Char.GetNumericValue(Char c), static Double System.Char.GetNumericValue(String s, Int32 index), static Boolean System.Char.IsHighSurrogate(Char c), static Boolean System.Char.IsHighSurrogate(String s, Int32 index), static Boolean System.Char.IsLowSurrogate(Char c), static Boolean System.Char.IsLowSurrogate(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static String System.Char.ConvertFromUtf32(Int32 utf32), static Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static Int32 System.Char.ConvertToUtf32(String s, Int32 index), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                  GetMethods="{Int32 System.Char.GetHashCode(), Boolean System.Char.Equals(Object obj), Boolean System.Char.Equals(Char obj), Int32 System.Char.CompareTo(Object value), Int32 System.Char.CompareTo(Char value), String System.Char.ToString(), String System.Char.ToString(IFormatProvider provider), static String System.Char.ToString(Char c), static Char System.Char.Parse(String s), static Boolean System.Char.TryParse(String s, Char result), static Boolean System.Char.IsDigit(Char c), static Boolean System.Char.IsDigit(String s, Int32 index), static Boolean System.Char.IsLetter(Char c), static Boolean System.Char.IsLetter(String s, Int32 index), static Boolean System.Char.IsWhiteSpace(Char c), static Boolean System.Char.IsWhiteSpace(String s, Int32 index), static Boolean System.Char.IsUpper(Char c), static Boolean System.Char.IsUpper(String s, Int32 index), static Boolean System.Char.IsLower(Char c), static Boolean System.Char.IsLower(String s, Int32 index), static Boolean System.Char.IsPunctuation(Char c), static Boolean System.Char.IsPunctuation(String s, Int32 index), static Boolean System.Char.IsLetterOrDigit(Char c), static Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static Char System.Char.ToUpper(Char c, CultureInfo culture), static Char System.Char.ToUpper(Char c), static Char System.Char.ToUpperInvariant(Char c), static Char System.Char.ToLower(Char c, CultureInfo culture), static Char System.Char.ToLower(Char c), static Char System.Char.ToLowerInvariant(Char c), TypeCode System.Char.GetTypeCode(), static Boolean System.Char.IsControl(Char c), static Boolean System.Char.IsControl(String s, Int32 index), static Boolean System.Char.IsNumber(Char c), static Boolean System.Char.IsNumber(String s, Int32 index), static Boolean System.Char.IsSeparator(Char c), static Boolean System.Char.IsSeparator(String s, Int32 index), static Boolean System.Char.IsSurrogate(Char c), static Boolean System.Char.IsSurrogate(String s, Int32 index), static Boolean System.Char.IsSymbol(Char c), static Boolean System.Char.IsSymbol(String s, Int32 index), static UnicodeCategory System.Char.GetUnicodeCategory(Char c), static UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static Double System.Char.GetNumericValue(Char c), static Double System.Char.GetNumericValue(String s, Int32 index), static Boolean System.Char.IsHighSurrogate(Char c), static Boolean System.Char.IsHighSurrogate(String s, Int32 index), static Boolean System.Char.IsLowSurrogate(Char c), static Boolean System.Char.IsLowSurrogate(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static String System.Char.ConvertFromUtf32(Int32 utf32), static Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static Int32 System.Char.ConvertToUtf32(String s, Int32 index), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetInterfaces="{System.Collections.Generic.IList`1[System.Char[,]], System.Collections.Generic.ICollection`1[System.Char[,]], System.Collections.Generic.IEnumerable`1[System.Char[,]], System.Collections.IEnumerable, System.ICloneable, System.Collections.IList, System.Collections.ICollection, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable}"
              GetMembers="{Object System.Array.GetValue(Int32[] indices), Object System.Array.GetValue(Int32 index), Object System.Array.GetValue(Int32 index1, Int32 index2), Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), Object System.Array.GetValue(Int64 index), Object System.Array.GetValue(Int64 index1, Int64 index2), Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), Int32 System.Array.get_Length(), Int64 System.Array.get_LongLength(), Int32 System.Array.GetLength(Int32 dimension), Int64 System.Array.GetLongLength(Int32 dimension), Int32 System.Array.get_Rank(), Int32 System.Array.GetUpperBound(Int32 dimension), Int32 System.Array.GetLowerBound(Int32 dimension), Object System.Array.get_SyncRoot(), Boolean System.Array.get_IsReadOnly(), Boolean System.Array.get_IsFixedSize(), Boolean System.Array.get_IsSynchronized(), Object System.Array.Clone(), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), Int32 System.Array.Length, Int64 System.Array.LongLength, Int32 System.Array.Rank, Object System.Array.SyncRoot, Boolean System.Array.IsReadOnly, Boolean System.Array.IsFixedSize, Boolean System.Array.IsSynchronized, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Object System.Array.GetValue(Int32[] indices), Object System.Array.GetValue(Int32 index), Object System.Array.GetValue(Int32 index1, Int32 index2), Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), Object System.Array.GetValue(Int64 index), Object System.Array.GetValue(Int64 index1, Int64 index2), Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), Int32 System.Array.get_Length(), Int64 System.Array.get_LongLength(), Int32 System.Array.GetLength(Int32 dimension), Int64 System.Array.GetLongLength(Int32 dimension), Int32 System.Array.get_Rank(), Int32 System.Array.GetUpperBound(Int32 dimension), Int32 System.Array.GetLowerBound(Int32 dimension), Object System.Array.get_SyncRoot(), Boolean System.Array.get_IsReadOnly(), Boolean System.Array.get_IsFixedSize(), Boolean System.Array.get_IsSynchronized(), Object System.Array.Clone(), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Int32 System.Array.Length, Int64 System.Array.LongLength, Int32 System.Array.Rank, Object System.Array.SyncRoot, Boolean System.Array.IsReadOnly, Boolean System.Array.IsFixedSize, Boolean System.Array.IsSynchronized}"
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
                  GetInterfaces="{System.Collections.Generic.IList`1[System.Char], System.Collections.Generic.ICollection`1[System.Char], System.Collections.Generic.IEnumerable`1[System.Char], System.Collections.IEnumerable, System.ICloneable, System.Collections.IList, System.Collections.ICollection, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable}"
                  GetMembers="{Object System.Array.GetValue(Int32[] indices), Object System.Array.GetValue(Int32 index), Object System.Array.GetValue(Int32 index1, Int32 index2), Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), Object System.Array.GetValue(Int64 index), Object System.Array.GetValue(Int64 index1, Int64 index2), Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), Int32 System.Array.get_Length(), Int64 System.Array.get_LongLength(), Int32 System.Array.GetLength(Int32 dimension), Int64 System.Array.GetLongLength(Int32 dimension), Int32 System.Array.get_Rank(), Int32 System.Array.GetUpperBound(Int32 dimension), Int32 System.Array.GetLowerBound(Int32 dimension), Object System.Array.get_SyncRoot(), Boolean System.Array.get_IsReadOnly(), Boolean System.Array.get_IsFixedSize(), Boolean System.Array.get_IsSynchronized(), Object System.Array.Clone(), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), Int32 System.Array.Length, Int64 System.Array.LongLength, Int32 System.Array.Rank, Object System.Array.SyncRoot, Boolean System.Array.IsReadOnly, Boolean System.Array.IsFixedSize, Boolean System.Array.IsSynchronized, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                  GetMethods="{Object System.Array.GetValue(Int32[] indices), Object System.Array.GetValue(Int32 index), Object System.Array.GetValue(Int32 index1, Int32 index2), Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), Object System.Array.GetValue(Int64 index), Object System.Array.GetValue(Int64 index1, Int64 index2), Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), Int32 System.Array.get_Length(), Int64 System.Array.get_LongLength(), Int32 System.Array.GetLength(Int32 dimension), Int64 System.Array.GetLongLength(Int32 dimension), Int32 System.Array.get_Rank(), Int32 System.Array.GetUpperBound(Int32 dimension), Int32 System.Array.GetLowerBound(Int32 dimension), Object System.Array.get_SyncRoot(), Boolean System.Array.get_IsReadOnly(), Boolean System.Array.get_IsFixedSize(), Boolean System.Array.get_IsSynchronized(), Object System.Array.Clone(), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                  GetProperties="{Int32 System.Array.Length, Int64 System.Array.LongLength, Int32 System.Array.Rank, Object System.Array.SyncRoot, Boolean System.Array.IsReadOnly, Boolean System.Array.IsFixedSize, Boolean System.Array.IsSynchronized}"
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
                      GetMembers="{System.Char MaxValue, System.Char MinValue, Int32 System.Char.GetHashCode(), Boolean System.Char.Equals(Object obj), Boolean System.Char.Equals(Char obj), Int32 System.Char.CompareTo(Object value), Int32 System.Char.CompareTo(Char value), String System.Char.ToString(), String System.Char.ToString(IFormatProvider provider), static String System.Char.ToString(Char c), static Char System.Char.Parse(String s), static Boolean System.Char.TryParse(String s, Char result), static Boolean System.Char.IsDigit(Char c), static Boolean System.Char.IsDigit(String s, Int32 index), static Boolean System.Char.IsLetter(Char c), static Boolean System.Char.IsLetter(String s, Int32 index), static Boolean System.Char.IsWhiteSpace(Char c), static Boolean System.Char.IsWhiteSpace(String s, Int32 index), static Boolean System.Char.IsUpper(Char c), static Boolean System.Char.IsUpper(String s, Int32 index), static Boolean System.Char.IsLower(Char c), static Boolean System.Char.IsLower(String s, Int32 index), static Boolean System.Char.IsPunctuation(Char c), static Boolean System.Char.IsPunctuation(String s, Int32 index), static Boolean System.Char.IsLetterOrDigit(Char c), static Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static Char System.Char.ToUpper(Char c, CultureInfo culture), static Char System.Char.ToUpper(Char c), static Char System.Char.ToUpperInvariant(Char c), static Char System.Char.ToLower(Char c, CultureInfo culture), static Char System.Char.ToLower(Char c), static Char System.Char.ToLowerInvariant(Char c), TypeCode System.Char.GetTypeCode(), static Boolean System.Char.IsControl(Char c), static Boolean System.Char.IsControl(String s, Int32 index), static Boolean System.Char.IsNumber(Char c), static Boolean System.Char.IsNumber(String s, Int32 index), static Boolean System.Char.IsSeparator(Char c), static Boolean System.Char.IsSeparator(String s, Int32 index), static Boolean System.Char.IsSurrogate(Char c), static Boolean System.Char.IsSurrogate(String s, Int32 index), static Boolean System.Char.IsSymbol(Char c), static Boolean System.Char.IsSymbol(String s, Int32 index), static UnicodeCategory System.Char.GetUnicodeCategory(Char c), static UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static Double System.Char.GetNumericValue(Char c), static Double System.Char.GetNumericValue(String s, Int32 index), static Boolean System.Char.IsHighSurrogate(Char c), static Boolean System.Char.IsHighSurrogate(String s, Int32 index), static Boolean System.Char.IsLowSurrogate(Char c), static Boolean System.Char.IsLowSurrogate(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static String System.Char.ConvertFromUtf32(Int32 utf32), static Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static Int32 System.Char.ConvertToUtf32(String s, Int32 index), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                      GetMethods="{Int32 System.Char.GetHashCode(), Boolean System.Char.Equals(Object obj), Boolean System.Char.Equals(Char obj), Int32 System.Char.CompareTo(Object value), Int32 System.Char.CompareTo(Char value), String System.Char.ToString(), String System.Char.ToString(IFormatProvider provider), static String System.Char.ToString(Char c), static Char System.Char.Parse(String s), static Boolean System.Char.TryParse(String s, Char result), static Boolean System.Char.IsDigit(Char c), static Boolean System.Char.IsDigit(String s, Int32 index), static Boolean System.Char.IsLetter(Char c), static Boolean System.Char.IsLetter(String s, Int32 index), static Boolean System.Char.IsWhiteSpace(Char c), static Boolean System.Char.IsWhiteSpace(String s, Int32 index), static Boolean System.Char.IsUpper(Char c), static Boolean System.Char.IsUpper(String s, Int32 index), static Boolean System.Char.IsLower(Char c), static Boolean System.Char.IsLower(String s, Int32 index), static Boolean System.Char.IsPunctuation(Char c), static Boolean System.Char.IsPunctuation(String s, Int32 index), static Boolean System.Char.IsLetterOrDigit(Char c), static Boolean System.Char.IsLetterOrDigit(String s, Int32 index), static Char System.Char.ToUpper(Char c, CultureInfo culture), static Char System.Char.ToUpper(Char c), static Char System.Char.ToUpperInvariant(Char c), static Char System.Char.ToLower(Char c, CultureInfo culture), static Char System.Char.ToLower(Char c), static Char System.Char.ToLowerInvariant(Char c), TypeCode System.Char.GetTypeCode(), static Boolean System.Char.IsControl(Char c), static Boolean System.Char.IsControl(String s, Int32 index), static Boolean System.Char.IsNumber(Char c), static Boolean System.Char.IsNumber(String s, Int32 index), static Boolean System.Char.IsSeparator(Char c), static Boolean System.Char.IsSeparator(String s, Int32 index), static Boolean System.Char.IsSurrogate(Char c), static Boolean System.Char.IsSurrogate(String s, Int32 index), static Boolean System.Char.IsSymbol(Char c), static Boolean System.Char.IsSymbol(String s, Int32 index), static UnicodeCategory System.Char.GetUnicodeCategory(Char c), static UnicodeCategory System.Char.GetUnicodeCategory(String s, Int32 index), static Double System.Char.GetNumericValue(Char c), static Double System.Char.GetNumericValue(String s, Int32 index), static Boolean System.Char.IsHighSurrogate(Char c), static Boolean System.Char.IsHighSurrogate(String s, Int32 index), static Boolean System.Char.IsLowSurrogate(Char c), static Boolean System.Char.IsLowSurrogate(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(String s, Int32 index), static Boolean System.Char.IsSurrogatePair(Char highSurrogate, Char lowSurrogate), static String System.Char.ConvertFromUtf32(Int32 utf32), static Int32 System.Char.ConvertToUtf32(Char highSurrogate, Char lowSurrogate), static Int32 System.Char.ConvertToUtf32(String s, Int32 index), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{void System.Nullable`1[System.Int32]..ctor(Int32 value), Boolean System.Nullable`1[System.Int32].get_HasValue(), Int32 System.Nullable`1[System.Int32].get_Value(), Int32 System.Nullable`1[System.Int32].GetValueOrDefault(), Int32 System.Nullable`1[System.Int32].GetValueOrDefault(Int32 defaultValue), Boolean System.Nullable`1[System.Int32].Equals(Object other), Int32 System.Nullable`1[System.Int32].GetHashCode(), String System.Nullable`1[System.Int32].ToString(), static Nullable`1 System.Nullable`1[System.Int32].op_Implicit(Int32 value), static Int32 System.Nullable`1[System.Int32].op_Explicit(Nullable`1 value), Boolean System.Nullable`1[System.Int32].HasValue, Int32 System.Nullable`1[System.Int32].Value, Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void System.Nullable`1[System.Int32]..ctor(Int32 value), Boolean System.Nullable`1[System.Int32].get_HasValue(), Int32 System.Nullable`1[System.Int32].get_Value(), Int32 System.Nullable`1[System.Int32].GetValueOrDefault(), Int32 System.Nullable`1[System.Int32].GetValueOrDefault(Int32 defaultValue), Boolean System.Nullable`1[System.Int32].Equals(Object other), Int32 System.Nullable`1[System.Int32].GetHashCode(), String System.Nullable`1[System.Int32].ToString(), static Nullable`1 System.Nullable`1[System.Int32].op_Implicit(Int32 value), static Int32 System.Nullable`1[System.Int32].op_Explicit(Nullable`1 value), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Boolean System.Nullable`1[System.Int32].HasValue, Int32 System.Nullable`1[System.Int32].Value}"
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
              GetMembers="{void System.Nullable`1[System.Int32]..ctor(Int32 value), Boolean System.Nullable`1[System.Int32].get_HasValue(), Int32 System.Nullable`1[System.Int32].get_Value(), Int32 System.Nullable`1[System.Int32].GetValueOrDefault(), Int32 System.Nullable`1[System.Int32].GetValueOrDefault(Int32 defaultValue), Boolean System.Nullable`1[System.Int32].Equals(Object other), Int32 System.Nullable`1[System.Int32].GetHashCode(), String System.Nullable`1[System.Int32].ToString(), static Nullable`1 System.Nullable`1[System.Int32].op_Implicit(Int32 value), static Int32 System.Nullable`1[System.Int32].op_Explicit(Nullable`1 value), Boolean System.Nullable`1[System.Int32].HasValue, Int32 System.Nullable`1[System.Int32].Value, Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void System.Nullable`1[System.Int32]..ctor(Int32 value), Boolean System.Nullable`1[System.Int32].get_HasValue(), Int32 System.Nullable`1[System.Int32].get_Value(), Int32 System.Nullable`1[System.Int32].GetValueOrDefault(), Int32 System.Nullable`1[System.Int32].GetValueOrDefault(Int32 defaultValue), Boolean System.Nullable`1[System.Int32].Equals(Object other), Int32 System.Nullable`1[System.Int32].GetHashCode(), String System.Nullable`1[System.Int32].ToString(), static Nullable`1 System.Nullable`1[System.Int32].op_Implicit(Int32 value), static Int32 System.Nullable`1[System.Int32].op_Explicit(Nullable`1 value), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Boolean System.Nullable`1[System.Int32].HasValue, Int32 System.Nullable`1[System.Int32].Value}"
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
              GetMembers="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetInterfaces="{System.Collections.Generic.IList`1[Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]], System.Collections.Generic.ICollection`1[Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]], System.Collections.Generic.IEnumerable`1[Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]], System.Collections.IEnumerable, System.ICloneable, System.Collections.IList, System.Collections.ICollection, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable}"
              GetMembers="{Object System.Array.GetValue(Int32[] indices), Object System.Array.GetValue(Int32 index), Object System.Array.GetValue(Int32 index1, Int32 index2), Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), Object System.Array.GetValue(Int64 index), Object System.Array.GetValue(Int64 index1, Int64 index2), Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), Int32 System.Array.get_Length(), Int64 System.Array.get_LongLength(), Int32 System.Array.GetLength(Int32 dimension), Int64 System.Array.GetLongLength(Int32 dimension), Int32 System.Array.get_Rank(), Int32 System.Array.GetUpperBound(Int32 dimension), Int32 System.Array.GetLowerBound(Int32 dimension), Object System.Array.get_SyncRoot(), Boolean System.Array.get_IsReadOnly(), Boolean System.Array.get_IsFixedSize(), Boolean System.Array.get_IsSynchronized(), Object System.Array.Clone(), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), Int32 System.Array.Length, Int64 System.Array.LongLength, Int32 System.Array.Rank, Object System.Array.SyncRoot, Boolean System.Array.IsReadOnly, Boolean System.Array.IsFixedSize, Boolean System.Array.IsSynchronized, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Object System.Array.GetValue(Int32[] indices), Object System.Array.GetValue(Int32 index), Object System.Array.GetValue(Int32 index1, Int32 index2), Object System.Array.GetValue(Int32 index1, Int32 index2, Int32 index3), Object System.Array.GetValue(Int64 index), Object System.Array.GetValue(Int64 index1, Int64 index2), Object System.Array.GetValue(Int64 index1, Int64 index2, Int64 index3), Object System.Array.GetValue(Int64[] indices), void System.Array.SetValue(Object value, Int32 index), void System.Array.SetValue(Object value, Int32 index1, Int32 index2), void System.Array.SetValue(Object value, Int32 index1, Int32 index2, Int32 index3), void System.Array.SetValue(Object value, Int32[] indices), void System.Array.SetValue(Object value, Int64 index), void System.Array.SetValue(Object value, Int64 index1, Int64 index2), void System.Array.SetValue(Object value, Int64 index1, Int64 index2, Int64 index3), void System.Array.SetValue(Object value, Int64[] indices), Int32 System.Array.get_Length(), Int64 System.Array.get_LongLength(), Int32 System.Array.GetLength(Int32 dimension), Int64 System.Array.GetLongLength(Int32 dimension), Int32 System.Array.get_Rank(), Int32 System.Array.GetUpperBound(Int32 dimension), Int32 System.Array.GetLowerBound(Int32 dimension), Object System.Array.get_SyncRoot(), Boolean System.Array.get_IsReadOnly(), Boolean System.Array.get_IsFixedSize(), Boolean System.Array.get_IsSynchronized(), Object System.Array.Clone(), void System.Array.CopyTo(Array array, Int32 index), void System.Array.CopyTo(Array array, Int64 index), IEnumerator System.Array.GetEnumerator(), void System.Array.Initialize(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Int32 System.Array.Length, Int64 System.Array.LongLength, Int32 System.Array.Rank, Object System.Array.SyncRoot, Boolean System.Array.IsReadOnly, Boolean System.Array.IsFixedSize, Boolean System.Array.IsSynchronized}"
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
                  GetMembers="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
                  GetMethods="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Int32]..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Nullable`1[System.Int32]]..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+MyGenClass`1[System.Nullable`1[System.Int32]]..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetInterfaces="{Debugger.Tests.DebugType_Tests+MyInterface`3[System.Int32,Debugger.Tests.DebugType_Tests+MyClass,Debugger.Tests.DebugType_Tests+MyStruct], Debugger.Tests.DebugType_Tests+MyInterfaceBase, Debugger.Tests.DebugType_Tests+ExtraInterface}"
              GetMembers="{List`1 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].get_Prop(), Int32 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun(MyClass a, MyStruct b, Object m), Object[] Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun2(Int32** iPtrPtr, Object[,] mdArray, Enumerator listEnum), void Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]..ctor(), List`1 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Prop, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{List`1 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].get_Prop(), Int32 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun(MyClass a, MyStruct b, Object m), Object[] Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Fun2(Int32** iPtrPtr, Object[,] mdArray, Enumerator listEnum), void Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32]..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{List`1 Debugger.Tests.DebugType_Tests+MyInterfaceImpl`1[System.Int32].Prop}"
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
              GetInterfaces="{Debugger.Tests.DebugType_Tests+MyInterfaceBase}"
              GetMembers="{Int32 Debugger.Tests.DebugType_Tests+MyInterface`3[System.Int32,Debugger.Tests.DebugType_Tests+MyClass,Debugger.Tests.DebugType_Tests+MyStruct].Fun(MyClass a, MyStruct b, Object m), void Debugger.Tests.DebugType_Tests+MyInterfaceBase.MyInterfaceBaseMethod()}"
              GetMethods="{Int32 Debugger.Tests.DebugType_Tests+MyInterface`3[System.Int32,Debugger.Tests.DebugType_Tests+MyClass,Debugger.Tests.DebugType_Tests+MyStruct].Fun(MyClass a, MyStruct b, Object m), void Debugger.Tests.DebugType_Tests+MyInterfaceBase.MyInterfaceBaseMethod()}"
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
              GetMembers="{void System.Collections.Generic.List`1[System.Int32]..ctor(), void System.Collections.Generic.List`1[System.Int32]..ctor(Int32 capacity), void System.Collections.Generic.List`1[System.Int32]..ctor(IEnumerable`1 collection), Int32 System.Collections.Generic.List`1[System.Int32].get_Capacity(), void System.Collections.Generic.List`1[System.Int32].set_Capacity(Int32 value), Int32 System.Collections.Generic.List`1[System.Int32].get_Count(), Int32 System.Collections.Generic.List`1[System.Int32].get_Item(Int32 index), void System.Collections.Generic.List`1[System.Int32].set_Item(Int32 index, Int32 value), void System.Collections.Generic.List`1[System.Int32].Add(Int32 item), void System.Collections.Generic.List`1[System.Int32].AddRange(IEnumerable`1 collection), ReadOnlyCollection`1 System.Collections.Generic.List`1[System.Int32].AsReadOnly(), Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 index, Int32 count, Int32 item, IComparer`1 comparer), Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 item), Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 item, IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Clear(), Boolean System.Collections.Generic.List`1[System.Int32].Contains(Int32 item), List`1 System.Collections.Generic.List`1[System.Int32].ConvertAll(Converter`2 converter), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32[] array), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32 index, Int32[] array, Int32 arrayIndex, Int32 count), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32[] array, Int32 arrayIndex), Boolean System.Collections.Generic.List`1[System.Int32].Exists(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].Find(Predicate`1 match), List`1 System.Collections.Generic.List`1[System.Int32].FindAll(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Int32 startIndex, Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Int32 startIndex, Int32 count, Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindLast(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Int32 startIndex, Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Int32 startIndex, Int32 count, Predicate`1 match), void System.Collections.Generic.List`1[System.Int32].ForEach(Action`1 action), Enumerator System.Collections.Generic.List`1[System.Int32].GetEnumerator(), List`1 System.Collections.Generic.List`1[System.Int32].GetRange(Int32 index, Int32 count), Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item), Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item, Int32 index), Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item, Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Insert(Int32 index, Int32 item), void System.Collections.Generic.List`1[System.Int32].InsertRange(Int32 index, IEnumerable`1 collection), Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item), Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item, Int32 index), Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item, Int32 index, Int32 count), Boolean System.Collections.Generic.List`1[System.Int32].Remove(Int32 item), Int32 System.Collections.Generic.List`1[System.Int32].RemoveAll(Predicate`1 match), void System.Collections.Generic.List`1[System.Int32].RemoveAt(Int32 index), void System.Collections.Generic.List`1[System.Int32].RemoveRange(Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Reverse(), void System.Collections.Generic.List`1[System.Int32].Reverse(Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Sort(), void System.Collections.Generic.List`1[System.Int32].Sort(IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Sort(Int32 index, Int32 count, IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Sort(Comparison`1 comparison), Int32[] System.Collections.Generic.List`1[System.Int32].ToArray(), void System.Collections.Generic.List`1[System.Int32].TrimExcess(), Boolean System.Collections.Generic.List`1[System.Int32].TrueForAll(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].Capacity, Int32 System.Collections.Generic.List`1[System.Int32].Count, Int32 System.Collections.Generic.List`1[System.Int32].Item[Int32 index], void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void System.Collections.Generic.List`1[System.Int32]..ctor(), void System.Collections.Generic.List`1[System.Int32]..ctor(Int32 capacity), void System.Collections.Generic.List`1[System.Int32]..ctor(IEnumerable`1 collection), Int32 System.Collections.Generic.List`1[System.Int32].get_Capacity(), void System.Collections.Generic.List`1[System.Int32].set_Capacity(Int32 value), Int32 System.Collections.Generic.List`1[System.Int32].get_Count(), Int32 System.Collections.Generic.List`1[System.Int32].get_Item(Int32 index), void System.Collections.Generic.List`1[System.Int32].set_Item(Int32 index, Int32 value), void System.Collections.Generic.List`1[System.Int32].Add(Int32 item), void System.Collections.Generic.List`1[System.Int32].AddRange(IEnumerable`1 collection), ReadOnlyCollection`1 System.Collections.Generic.List`1[System.Int32].AsReadOnly(), Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 index, Int32 count, Int32 item, IComparer`1 comparer), Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 item), Int32 System.Collections.Generic.List`1[System.Int32].BinarySearch(Int32 item, IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Clear(), Boolean System.Collections.Generic.List`1[System.Int32].Contains(Int32 item), List`1 System.Collections.Generic.List`1[System.Int32].ConvertAll(Converter`2 converter), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32[] array), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32 index, Int32[] array, Int32 arrayIndex, Int32 count), void System.Collections.Generic.List`1[System.Int32].CopyTo(Int32[] array, Int32 arrayIndex), Boolean System.Collections.Generic.List`1[System.Int32].Exists(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].Find(Predicate`1 match), List`1 System.Collections.Generic.List`1[System.Int32].FindAll(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Int32 startIndex, Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindIndex(Int32 startIndex, Int32 count, Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindLast(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Int32 startIndex, Predicate`1 match), Int32 System.Collections.Generic.List`1[System.Int32].FindLastIndex(Int32 startIndex, Int32 count, Predicate`1 match), void System.Collections.Generic.List`1[System.Int32].ForEach(Action`1 action), Enumerator System.Collections.Generic.List`1[System.Int32].GetEnumerator(), List`1 System.Collections.Generic.List`1[System.Int32].GetRange(Int32 index, Int32 count), Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item), Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item, Int32 index), Int32 System.Collections.Generic.List`1[System.Int32].IndexOf(Int32 item, Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Insert(Int32 index, Int32 item), void System.Collections.Generic.List`1[System.Int32].InsertRange(Int32 index, IEnumerable`1 collection), Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item), Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item, Int32 index), Int32 System.Collections.Generic.List`1[System.Int32].LastIndexOf(Int32 item, Int32 index, Int32 count), Boolean System.Collections.Generic.List`1[System.Int32].Remove(Int32 item), Int32 System.Collections.Generic.List`1[System.Int32].RemoveAll(Predicate`1 match), void System.Collections.Generic.List`1[System.Int32].RemoveAt(Int32 index), void System.Collections.Generic.List`1[System.Int32].RemoveRange(Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Reverse(), void System.Collections.Generic.List`1[System.Int32].Reverse(Int32 index, Int32 count), void System.Collections.Generic.List`1[System.Int32].Sort(), void System.Collections.Generic.List`1[System.Int32].Sort(IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Sort(Int32 index, Int32 count, IComparer`1 comparer), void System.Collections.Generic.List`1[System.Int32].Sort(Comparison`1 comparison), Int32[] System.Collections.Generic.List`1[System.Int32].ToArray(), void System.Collections.Generic.List`1[System.Int32].TrimExcess(), Boolean System.Collections.Generic.List`1[System.Int32].TrueForAll(Predicate`1 match), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Int32 System.Collections.Generic.List`1[System.Int32].Capacity, Int32 System.Collections.Generic.List`1[System.Int32].Count, Int32 System.Collections.Generic.List`1[System.Int32].Item[Int32 index]}"
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
              GetMembers="{void System.Collections.Generic.List`1+Enumerator[System.Int32].Dispose(), Boolean System.Collections.Generic.List`1+Enumerator[System.Int32].MoveNext(), Int32 System.Collections.Generic.List`1+Enumerator[System.Int32].get_Current(), Int32 System.Collections.Generic.List`1+Enumerator[System.Int32].Current, Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void System.Collections.Generic.List`1+Enumerator[System.Int32].Dispose(), Boolean System.Collections.Generic.List`1+Enumerator[System.Int32].MoveNext(), Int32 System.Collections.Generic.List`1+Enumerator[System.Int32].get_Current(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Int32 System.Collections.Generic.List`1+Enumerator[System.Int32].Current}"
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
              GetInterfaces="{System.ICloneable, System.Runtime.Serialization.ISerializable}"
              GetMembers="{void Debugger.Tests.DebugType_Tests+AddDelegate..ctor(Object object, IntPtr method), Int32 Debugger.Tests.DebugType_Tests+AddDelegate.Invoke(Byte b1, Byte b2), IAsyncResult Debugger.Tests.DebugType_Tests+AddDelegate.BeginInvoke(Byte b1, Byte b2, AsyncCallback callback, Object object), Int32 Debugger.Tests.DebugType_Tests+AddDelegate.EndInvoke(IAsyncResult result), void System.MulticastDelegate.GetObjectData(SerializationInfo info, StreamingContext context), Boolean System.MulticastDelegate.Equals(Object obj), Delegate[] System.MulticastDelegate.GetInvocationList(), Int32 System.MulticastDelegate.GetHashCode(), Object System.Delegate.DynamicInvoke(Object[] args), Boolean System.Delegate.Equals(Object obj), Int32 System.Delegate.GetHashCode(), Delegate[] System.Delegate.GetInvocationList(), MethodInfo System.Delegate.get_Method(), Object System.Delegate.get_Target(), Object System.Delegate.Clone(), void System.Delegate.GetObjectData(SerializationInfo info, StreamingContext context), MethodInfo System.Delegate.Method, Object System.Delegate.Target, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+AddDelegate..ctor(Object object, IntPtr method), Int32 Debugger.Tests.DebugType_Tests+AddDelegate.Invoke(Byte b1, Byte b2), IAsyncResult Debugger.Tests.DebugType_Tests+AddDelegate.BeginInvoke(Byte b1, Byte b2, AsyncCallback callback, Object object), Int32 Debugger.Tests.DebugType_Tests+AddDelegate.EndInvoke(IAsyncResult result), void System.MulticastDelegate.GetObjectData(SerializationInfo info, StreamingContext context), Boolean System.MulticastDelegate.Equals(Object obj), Delegate[] System.MulticastDelegate.GetInvocationList(), Int32 System.MulticastDelegate.GetHashCode(), Object System.Delegate.DynamicInvoke(Object[] args), Boolean System.Delegate.Equals(Object obj), Int32 System.Delegate.GetHashCode(), Delegate[] System.Delegate.GetInvocationList(), MethodInfo System.Delegate.get_Method(), Object System.Delegate.get_Target(), Object System.Delegate.Clone(), void System.Delegate.GetObjectData(SerializationInfo info, StreamingContext context), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{MethodInfo System.Delegate.Method, Object System.Delegate.Target}"
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
              GetMembers="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{static Boolean System.Enum.TryParse(String value, Object result), static Boolean System.Enum.TryParse(String value, Boolean ignoreCase, Object result), static Object System.Enum.Parse(Type enumType, String value), static Object System.Enum.Parse(Type enumType, String value, Boolean ignoreCase), static Type System.Enum.GetUnderlyingType(Type enumType), static Array System.Enum.GetValues(Type enumType), static String System.Enum.GetName(Type enumType, Object value), static String[] System.Enum.GetNames(Type enumType), static Object System.Enum.ToObject(Type enumType, Object value), static Object System.Enum.ToObject(Type enumType, SByte value), static Object System.Enum.ToObject(Type enumType, Int16 value), static Object System.Enum.ToObject(Type enumType, Int32 value), static Object System.Enum.ToObject(Type enumType, Byte value), static Object System.Enum.ToObject(Type enumType, UInt16 value), static Object System.Enum.ToObject(Type enumType, UInt32 value), static Object System.Enum.ToObject(Type enumType, Int64 value), static Object System.Enum.ToObject(Type enumType, UInt64 value), static Boolean System.Enum.IsDefined(Type enumType, Object value), static String System.Enum.Format(Type enumType, Object value, String format), Boolean System.Enum.Equals(Object obj), Int32 System.Enum.GetHashCode(), String System.Enum.ToString(), String System.Enum.ToString(String format, IFormatProvider provider), String System.Enum.ToString(String format), String System.Enum.ToString(IFormatProvider provider), Int32 System.Enum.CompareTo(Object target), Boolean System.Enum.HasFlag(Enum flag), TypeCode System.Enum.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{static Boolean System.Enum.TryParse(String value, Object result), static Boolean System.Enum.TryParse(String value, Boolean ignoreCase, Object result), static Object System.Enum.Parse(Type enumType, String value), static Object System.Enum.Parse(Type enumType, String value, Boolean ignoreCase), static Type System.Enum.GetUnderlyingType(Type enumType), static Array System.Enum.GetValues(Type enumType), static String System.Enum.GetName(Type enumType, Object value), static String[] System.Enum.GetNames(Type enumType), static Object System.Enum.ToObject(Type enumType, Object value), static Object System.Enum.ToObject(Type enumType, SByte value), static Object System.Enum.ToObject(Type enumType, Int16 value), static Object System.Enum.ToObject(Type enumType, Int32 value), static Object System.Enum.ToObject(Type enumType, Byte value), static Object System.Enum.ToObject(Type enumType, UInt16 value), static Object System.Enum.ToObject(Type enumType, UInt32 value), static Object System.Enum.ToObject(Type enumType, Int64 value), static Object System.Enum.ToObject(Type enumType, UInt64 value), static Boolean System.Enum.IsDefined(Type enumType, Object value), static String System.Enum.Format(Type enumType, Object value, String format), Boolean System.Enum.Equals(Object obj), Int32 System.Enum.GetHashCode(), String System.Enum.ToString(), String System.Enum.ToString(String format, IFormatProvider provider), String System.Enum.ToString(String format), String System.Enum.ToString(IFormatProvider provider), Int32 System.Enum.CompareTo(Object target), Boolean System.Enum.HasFlag(Enum flag), TypeCode System.Enum.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetInterfaces="{System.IComparable, System.IFormattable, System.IConvertible}"
              GetMembers="{System.Byte value__, Debugger.Tests.DebugType_Tests+MyEnum A, Debugger.Tests.DebugType_Tests+MyEnum B, Boolean System.Enum.Equals(Object obj), Int32 System.Enum.GetHashCode(), String System.Enum.ToString(), String System.Enum.ToString(String format, IFormatProvider provider), String System.Enum.ToString(String format), String System.Enum.ToString(IFormatProvider provider), Int32 System.Enum.CompareTo(Object target), Boolean System.Enum.HasFlag(Enum flag), TypeCode System.Enum.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Boolean System.Enum.Equals(Object obj), Int32 System.Enum.GetHashCode(), String System.Enum.ToString(), String System.Enum.ToString(String format, IFormatProvider provider), String System.Enum.ToString(String format), String System.Enum.ToString(IFormatProvider provider), Int32 System.Enum.CompareTo(Object target), Boolean System.Enum.HasFlag(Enum flag), TypeCode System.Enum.GetTypeCode(), Boolean System.ValueType.Equals(Object obj), Int32 System.ValueType.GetHashCode(), String System.ValueType.ToString(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
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
              GetMembers="{System.Int32 IntLiteral, System.Int32 instanceInt, System.Int32 staticInt, System.Void* voidPtr, void Debugger.Tests.DebugType_Tests+Members.set_SetterOnlyProp(Char value), Int32 Debugger.Tests.DebugType_Tests+Members.get_InstanceInt(), static Int32 Debugger.Tests.DebugType_Tests+Members.get_StaticInt(), Int32 Debugger.Tests.DebugType_Tests+Members.get_AutoProperty(), void Debugger.Tests.DebugType_Tests+Members.set_AutoProperty(Int32 value), Char Debugger.Tests.DebugType_Tests+Members.get_Item(Int32 i), Char Debugger.Tests.DebugType_Tests+Members.get_Item(String s), void Debugger.Tests.DebugType_Tests+Members.set_Item(Int32 i, Char value), void Debugger.Tests.DebugType_Tests+Members..ctor(), Char Debugger.Tests.DebugType_Tests+Members.SetterOnlyProp, Int32 Debugger.Tests.DebugType_Tests+Members.InstanceInt, static Int32 Debugger.Tests.DebugType_Tests+Members.StaticInt, Int32 Debugger.Tests.DebugType_Tests+Members.AutoProperty, Char Debugger.Tests.DebugType_Tests+Members.Item[Int32 i], Char Debugger.Tests.DebugType_Tests+Members.Item[String s], void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{void Debugger.Tests.DebugType_Tests+Members.set_SetterOnlyProp(Char value), Int32 Debugger.Tests.DebugType_Tests+Members.get_InstanceInt(), static Int32 Debugger.Tests.DebugType_Tests+Members.get_StaticInt(), Int32 Debugger.Tests.DebugType_Tests+Members.get_AutoProperty(), void Debugger.Tests.DebugType_Tests+Members.set_AutoProperty(Int32 value), Char Debugger.Tests.DebugType_Tests+Members.get_Item(Int32 i), Char Debugger.Tests.DebugType_Tests+Members.get_Item(String s), void Debugger.Tests.DebugType_Tests+Members.set_Item(Int32 i, Char value), void Debugger.Tests.DebugType_Tests+Members..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Char Debugger.Tests.DebugType_Tests+Members.SetterOnlyProp, Int32 Debugger.Tests.DebugType_Tests+Members.InstanceInt, static Int32 Debugger.Tests.DebugType_Tests+Members.StaticInt, Int32 Debugger.Tests.DebugType_Tests+Members.AutoProperty, Char Debugger.Tests.DebugType_Tests+Members.Item[Int32 i], Char Debugger.Tests.DebugType_Tests+Members.Item[String s]}"
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
              GetMembers="{System.Int32 publicField, Int32 Debugger.Tests.DebugType_Tests+Access.get_publicProperty(), void Debugger.Tests.DebugType_Tests+Access.publicMethod(), void Debugger.Tests.DebugType_Tests+Access..ctor(), Int32 Debugger.Tests.DebugType_Tests+Access.publicProperty, void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetMethods="{Int32 Debugger.Tests.DebugType_Tests+Access.get_publicProperty(), void Debugger.Tests.DebugType_Tests+Access.publicMethod(), void Debugger.Tests.DebugType_Tests+Access..ctor(), void System.Object..ctor(), String System.Object.ToString(), Boolean System.Object.Equals(Object obj), Int32 System.Object.GetHashCode(), Type System.Object.GetType()}"
              GetProperties="{Int32 Debugger.Tests.DebugType_Tests+Access.publicProperty}"
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
