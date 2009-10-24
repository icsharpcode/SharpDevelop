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
			public void* voidPtr;
			
			public List<R> Prop { get { return new List<R>(); } }
			
			public char SetterOnlyProp { set { ; } }
			
			public R Fun<M>(MyClass a, MyStruct b, M m)
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
		public void DebugType()
		{
			ExpandProperties(
				"LocalVariable.Type",
				"DebugType.GetElementType"
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
    <DebuggingPaused>Break DebugType.cs:124,4-124,40</DebuggingPaused>
    <DefinedTypes
      Capacity="16"
      Count="11">
      <Item>Debugger.Tests.TestPrograms.DebugType</Item>
      <Item>AddDelegate</Item>
      <Item>MyEnum</Item>
      <Item>MyClass</Item>
      <Item>MyStruct</Item>
      <Item>MyGenClass`1</Item>
      <Item>MyNestedStruct</Item>
      <Item>MyGenNestedStruct`1</Item>
      <Item>MyInterface`3</Item>
      <Item>MyInterfaceImpl`1</Item>
      <Item>Access</Item>
    </DefinedTypes>
    <DefinedTypes
      Capacity="8"
      Count="6">
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, Public, BeforeFieldInit"
          BaseType="System.Object"
          FullName="Debugger.Tests.TestPrograms.DebugType"
          GetMembers="{Add, Main, .ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
          GetMethods="{Add, Main, .ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
          IsClass="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, Sealed"
          BaseType="System.MulticastDelegate"
          FullName="Debugger.Tests.TestPrograms.DebugType.AddDelegate"
          GetMembers="{.ctor, Invoke, BeginInvoke, EndInvoke, GetObjectData, Equals, GetInvocationList, GetHashCode, DynamicInvoke, Equals, GetHashCode, GetInvocationList, get_Method, get_Target, Clone, GetObjectData, System.Reflection.MethodInfo Method, System.Object Target, .ctor, ToString, Equals, GetHashCode, GetType}"
          GetMethods="{.ctor, Invoke, BeginInvoke, EndInvoke, GetObjectData, Equals, GetInvocationList, GetHashCode, DynamicInvoke, Equals, GetHashCode, GetInvocationList, get_Method, get_Target, Clone, GetObjectData, .ctor, ToString, Equals, GetHashCode, GetType}"
          GetProperties="{System.Reflection.MethodInfo Method, System.Object Target}"
          IsClass="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, Sealed"
          BaseType="System.Enum"
          FullName="Debugger.Tests.TestPrograms.DebugType.MyEnum"
          GetEnumUnderlyingType="System.Byte"
          GetFields="{System.Byte value__}"
          GetMembers="{System.Byte value__, Equals, GetHashCode, ToString, ToString, ToString, ToString, CompareTo, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
          GetMethods="{Equals, GetHashCode, ToString, ToString, ToString, ToString, CompareTo, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
          IsEnum="True"
          IsValueType="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
          BaseType="System.Object"
          FullName="Debugger.Tests.TestPrograms.DebugType.MyClass"
          GetMembers="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
          GetMethods="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
          IsClass="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, BeforeFieldInit"
          BaseType="System.ValueType"
          FullName="Debugger.Tests.TestPrograms.DebugType.MyStruct"
          GetMembers="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
          GetMethods="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
          IsValueType="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
      <Item>
        <DebugType
          Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
          BaseType="System.Object"
          FullName="Debugger.Tests.TestPrograms.DebugType.Access"
          GetFields="{System.Int32 publicField}"
          GetMembers="{System.Int32 publicField, get_publicProperty, publicMethod, .ctor, System.Int32 publicProperty, .ctor, ToString, Equals, GetHashCode, GetType}"
          GetMethods="{get_publicProperty, publicMethod, .ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
          GetProperties="{System.Int32 publicProperty}"
          IsClass="True">
          <GetElementType>null</GetElementType>
        </DebugType>
      </Item>
    </DefinedTypes>
    <Access-Members>
      <Item>
        <DebugFieldInfo
          Attributes="Public"
          DeclaringType="Debugger.Tests.TestPrograms.DebugType.Access"
          FieldType="System.Int32"
          Name="publicField" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.TestPrograms.DebugType.Access"
          FullName="Debugger.Tests.TestPrograms.DebugType.Access.get_publicProperty()"
          GetLocalVariables="{Debugger.Tests.TestPrograms.DebugType.Access this}"
          Name="get_publicProperty"
          ReturnType="System.Int32" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig"
          DeclaringType="Debugger.Tests.TestPrograms.DebugType.Access"
          FullName="Debugger.Tests.TestPrograms.DebugType.Access.publicMethod()"
          GetLocalVariables="{Debugger.Tests.TestPrograms.DebugType.Access this}"
          Name="publicMethod" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="Debugger.Tests.TestPrograms.DebugType.Access"
          FullName="Debugger.Tests.TestPrograms.DebugType.Access..ctor()"
          GetLocalVariables="{Debugger.Tests.TestPrograms.DebugType.Access this}"
          Name=".ctor" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.TestPrograms.DebugType.Access"
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
        <DebugFieldInfo
          Attributes="Public"
          DeclaringType="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]"
          FieldType="System.Void*"
          Name="voidPtr" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]"
          FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]].get_Prop()"
          GetLocalVariables="{Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]] this}"
          Name="get_Prop"
          ReturnType="System.Collections.Generic.List`1[[System.Int32]]" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName"
          DeclaringType="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]"
          FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]].set_SetterOnlyProp(Char value)"
          GetLocalVariables="{Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]] this}"
          Name="set_SetterOnlyProp" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, Final, Virtual, HideBySig, VtableLayoutMask"
          DeclaringType="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]"
          FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]].Fun(MyClass a, MyStruct b, Object m)"
          GetLocalVariables="{Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]] this}"
          Name="Fun"
          ReturnType="System.Int32" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig"
          DeclaringType="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]"
          FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]].Fun2(Int32** iPtrPtr, Object[,] mdArray, Enumerator listEnum)"
          GetLocalVariables="{Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]] this}"
          Name="Fun2"
          ReturnType="System.Object[]" />
      </Item>
      <Item>
        <DebugMethodInfo
          Attributes="PrivateScope, Public, HideBySig, SpecialName, RTSpecialName"
          DeclaringType="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]"
          FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]..ctor()"
          Name=".ctor"
          StepOver="True" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]"
          Name="Prop"
          PropertyType="System.Collections.Generic.List`1[[System.Int32]]" />
      </Item>
      <Item>
        <DebugPropertyInfo
          DeclaringType="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]"
          Name="SetterOnlyProp"
          PropertyType="System.Char" />
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
              GetMembers="{.ctor, ToString, Equals, Equals, ReferenceEquals, GetHashCode, GetType}"
              GetMethods="{.ctor, ToString, Equals, Equals, ReferenceEquals, GetHashCode, GetType}"
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
              GetInterfaces="{System.IComparable, System.ICloneable, System.IConvertible, System.IComparable`1[[System.String]], System.Collections.Generic.IEnumerable`1[[System.Char]], System.Collections.IEnumerable, System.IEquatable`1[[System.String]]}"
              GetMembers="{System.String Empty, Join, Join, Equals, Equals, Equals, Equals, Equals, op_Equality, op_Inequality, get_Chars, CopyTo, ToCharArray, ToCharArray, IsNullOrEmpty, GetHashCode, get_Length, Split, Split, Split, Split, Split, Split, Substring, Substring, Trim, Trim, TrimStart, TrimEnd, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, IsNormalized, IsNormalized, Normalize, Normalize, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, CompareTo, CompareTo, CompareOrdinal, CompareOrdinal, Contains, EndsWith, EndsWith, EndsWith, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOfAny, IndexOfAny, IndexOfAny, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOfAny, LastIndexOfAny, LastIndexOfAny, PadLeft, PadLeft, PadRight, PadRight, StartsWith, StartsWith, StartsWith, ToLower, ToLower, ToLowerInvariant, ToUpper, ToUpper, ToUpperInvariant, ToString, ToString, Clone, Insert, Replace, Replace, Remove, Remove, Format, Format, Format, Format, Format, Copy, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Intern, IsInterned, GetTypeCode, GetEnumerator, System.Char Chars, System.Int32 Length, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{Join, Join, Equals, Equals, Equals, Equals, Equals, op_Equality, op_Inequality, get_Chars, CopyTo, ToCharArray, ToCharArray, IsNullOrEmpty, GetHashCode, get_Length, Split, Split, Split, Split, Split, Split, Substring, Substring, Trim, Trim, TrimStart, TrimEnd, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, IsNormalized, IsNormalized, Normalize, Normalize, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, CompareTo, CompareTo, CompareOrdinal, CompareOrdinal, Contains, EndsWith, EndsWith, EndsWith, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOfAny, IndexOfAny, IndexOfAny, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOfAny, LastIndexOfAny, LastIndexOfAny, PadLeft, PadLeft, PadRight, PadRight, StartsWith, StartsWith, StartsWith, ToLower, ToLower, ToLowerInvariant, ToUpper, ToUpper, ToUpperInvariant, ToString, ToString, Clone, Insert, Replace, Replace, Remove, Remove, Format, Format, Format, Format, Format, Copy, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Intern, IsInterned, GetTypeCode, GetEnumerator, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetProperties="{System.Char Chars, System.Int32 Length}"
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
          Type="Debugger.Tests.TestPrograms.DebugType.MyClass"
          Value="null">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.TestPrograms.DebugType.MyClass"
              GetMembers="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
              IsClass="True">
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
              GetInterfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable`1[[System.Int32]], System.IEquatable`1[[System.Int32]]}"
              GetMembers="{CompareTo, CompareTo, Equals, Equals, GetHashCode, ToString, ToString, ToString, ToString, Parse, Parse, Parse, Parse, TryParse, TryParse, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{CompareTo, CompareTo, Equals, Equals, GetHashCode, ToString, ToString, ToString, ToString, Parse, Parse, Parse, Parse, TryParse, TryParse, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
              GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[[System.Boolean]], System.IEquatable`1[[System.Boolean]]}"
              GetMembers="{System.String TrueString, System.String FalseString, GetHashCode, ToString, ToString, Equals, Equals, CompareTo, CompareTo, Parse, TryParse, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{GetHashCode, ToString, ToString, Equals, Equals, CompareTo, CompareTo, Parse, TryParse, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
              GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[[System.Char]], System.IEquatable`1[[System.Char]]}"
              GetMembers="{GetHashCode, Equals, Equals, CompareTo, CompareTo, ToString, ToString, ToString, Parse, TryParse, IsDigit, IsDigit, IsLetter, IsLetter, IsWhiteSpace, IsWhiteSpace, IsUpper, IsUpper, IsLower, IsLower, IsPunctuation, IsPunctuation, IsLetterOrDigit, IsLetterOrDigit, ToUpper, ToUpper, ToUpperInvariant, ToLower, ToLower, ToLowerInvariant, GetTypeCode, IsControl, IsControl, IsNumber, IsNumber, IsSeparator, IsSeparator, IsSurrogate, IsSurrogate, IsSymbol, IsSymbol, GetUnicodeCategory, GetUnicodeCategory, GetNumericValue, GetNumericValue, IsHighSurrogate, IsHighSurrogate, IsLowSurrogate, IsLowSurrogate, IsSurrogatePair, IsSurrogatePair, ConvertFromUtf32, ConvertToUtf32, ConvertToUtf32, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{GetHashCode, Equals, Equals, CompareTo, CompareTo, ToString, ToString, ToString, Parse, TryParse, IsDigit, IsDigit, IsLetter, IsLetter, IsWhiteSpace, IsWhiteSpace, IsUpper, IsUpper, IsLower, IsLower, IsPunctuation, IsPunctuation, IsLetterOrDigit, IsLetterOrDigit, ToUpper, ToUpper, ToUpperInvariant, ToLower, ToLower, ToLowerInvariant, GetTypeCode, IsControl, IsControl, IsNumber, IsNumber, IsSeparator, IsSeparator, IsSurrogate, IsSurrogate, IsSymbol, IsSymbol, GetUnicodeCategory, GetUnicodeCategory, GetNumericValue, GetNumericValue, IsHighSurrogate, IsHighSurrogate, IsLowSurrogate, IsLowSurrogate, IsSurrogatePair, IsSurrogatePair, ConvertFromUtf32, ConvertToUtf32, ConvertToUtf32, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
              GetInterfaces="{System.IComparable, System.ICloneable, System.IConvertible, System.IComparable`1[[System.String]], System.Collections.Generic.IEnumerable`1[[System.Char]], System.Collections.IEnumerable, System.IEquatable`1[[System.String]]}"
              GetMembers="{System.String Empty, Join, Join, Equals, Equals, Equals, Equals, Equals, op_Equality, op_Inequality, get_Chars, CopyTo, ToCharArray, ToCharArray, IsNullOrEmpty, GetHashCode, get_Length, Split, Split, Split, Split, Split, Split, Substring, Substring, Trim, Trim, TrimStart, TrimEnd, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, IsNormalized, IsNormalized, Normalize, Normalize, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, CompareTo, CompareTo, CompareOrdinal, CompareOrdinal, Contains, EndsWith, EndsWith, EndsWith, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOfAny, IndexOfAny, IndexOfAny, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOfAny, LastIndexOfAny, LastIndexOfAny, PadLeft, PadLeft, PadRight, PadRight, StartsWith, StartsWith, StartsWith, ToLower, ToLower, ToLowerInvariant, ToUpper, ToUpper, ToUpperInvariant, ToString, ToString, Clone, Insert, Replace, Replace, Remove, Remove, Format, Format, Format, Format, Format, Copy, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Intern, IsInterned, GetTypeCode, GetEnumerator, System.Char Chars, System.Int32 Length, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{Join, Join, Equals, Equals, Equals, Equals, Equals, op_Equality, op_Inequality, get_Chars, CopyTo, ToCharArray, ToCharArray, IsNullOrEmpty, GetHashCode, get_Length, Split, Split, Split, Split, Split, Split, Substring, Substring, Trim, Trim, TrimStart, TrimEnd, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, .ctor, IsNormalized, IsNormalized, Normalize, Normalize, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, Compare, CompareTo, CompareTo, CompareOrdinal, CompareOrdinal, Contains, EndsWith, EndsWith, EndsWith, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOf, IndexOfAny, IndexOfAny, IndexOfAny, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOf, LastIndexOfAny, LastIndexOfAny, LastIndexOfAny, PadLeft, PadLeft, PadRight, PadRight, StartsWith, StartsWith, StartsWith, ToLower, ToLower, ToLowerInvariant, ToUpper, ToUpper, ToUpperInvariant, ToString, ToString, Clone, Insert, Replace, Replace, Remove, Remove, Format, Format, Format, Format, Format, Copy, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Concat, Intern, IsInterned, GetTypeCode, GetEnumerator, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetProperties="{System.Char Chars, System.Int32 Length}"
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
              GetMembers="{.ctor, ToString, Equals, Equals, ReferenceEquals, GetHashCode, GetType}"
              GetMethods="{.ctor, ToString, Equals, Equals, ReferenceEquals, GetHashCode, GetType}"
              IsClass="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myClass"
          Type="Debugger.Tests.TestPrograms.DebugType.MyClass"
          Value="{Debugger.Tests.TestPrograms.DebugType.MyClass}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.TestPrograms.DebugType.MyClass"
              GetMembers="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
              IsClass="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myStruct"
          Type="Debugger.Tests.TestPrograms.DebugType.MyStruct"
          Value="{Debugger.Tests.TestPrograms.DebugType.MyStruct}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="Debugger.Tests.TestPrograms.DebugType.MyStruct"
              GetMembers="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
              GetMembers="{.ctor, ToString, Equals, Equals, ReferenceEquals, GetHashCode, GetType}"
              GetMethods="{.ctor, ToString, Equals, Equals, ReferenceEquals, GetHashCode, GetType}"
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
                  GetInterfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable`1[[System.Int32]], System.IEquatable`1[[System.Int32]]}"
                  GetMembers="{CompareTo, CompareTo, Equals, Equals, GetHashCode, ToString, ToString, ToString, ToString, Parse, Parse, Parse, Parse, TryParse, TryParse, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
                  GetMethods="{CompareTo, CompareTo, Equals, Equals, GetHashCode, ToString, ToString, ToString, ToString, Parse, Parse, Parse, Parse, TryParse, TryParse, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
                      GetInterfaces="{System.IComparable, System.IFormattable, System.IConvertible, System.IComparable`1[[System.Int32]], System.IEquatable`1[[System.Int32]]}"
                      GetMembers="{CompareTo, CompareTo, Equals, Equals, GetHashCode, ToString, ToString, ToString, ToString, Parse, Parse, Parse, Parse, TryParse, TryParse, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
                      GetMethods="{CompareTo, CompareTo, Equals, Equals, GetHashCode, ToString, ToString, ToString, ToString, Parse, Parse, Parse, Parse, TryParse, TryParse, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
                  GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[[System.Boolean]], System.IEquatable`1[[System.Boolean]]}"
                  GetMembers="{System.String TrueString, System.String FalseString, GetHashCode, ToString, ToString, Equals, Equals, CompareTo, CompareTo, Parse, TryParse, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
                  GetMethods="{GetHashCode, ToString, ToString, Equals, Equals, CompareTo, CompareTo, Parse, TryParse, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
                  GetMembers="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
                  GetMethods="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
          Type="Debugger.Tests.TestPrograms.DebugType.MyStruct*"
          Value="{Debugger.Tests.TestPrograms.DebugType.MyStruct*}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              FullName="Debugger.Tests.TestPrograms.DebugType.MyStruct*"
              GetElementType="Debugger.Tests.TestPrograms.DebugType.MyStruct"
              HasElementType="True"
              IsClass="True"
              IsCompilerGenerated="True"
              IsPointer="True">
              <GetElementType>
                <DebugType
                  Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, BeforeFieldInit"
                  BaseType="System.ValueType"
                  FullName="Debugger.Tests.TestPrograms.DebugType.MyStruct"
                  GetMembers="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
                  GetMethods="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
              GetMembers="{System.IntPtr Zero, .ctor, .ctor, .ctor, Equals, GetHashCode, ToInt32, ToInt64, ToString, ToString, op_Explicit, op_Explicit, op_Explicit, op_Explicit, op_Explicit, op_Explicit, op_Equality, op_Inequality, get_Size, ToPointer, System.Int32 Size, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{.ctor, .ctor, .ctor, Equals, GetHashCode, ToInt32, ToInt64, ToString, ToString, op_Explicit, op_Explicit, op_Explicit, op_Explicit, op_Explicit, op_Explicit, op_Equality, op_Inequality, get_Size, ToPointer, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
              GetMembers="{GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, get_Length, get_LongLength, GetLength, GetLongLength, get_Rank, GetUpperBound, GetLowerBound, get_SyncRoot, get_IsReadOnly, get_IsFixedSize, get_IsSynchronized, Clone, CompareTo, Equals, GetHashCode, CopyTo, CopyTo, GetEnumerator, Initialize, System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, get_Length, get_LongLength, GetLength, GetLongLength, get_Rank, GetUpperBound, GetLowerBound, get_SyncRoot, get_IsReadOnly, get_IsFixedSize, get_IsSynchronized, Clone, CompareTo, Equals, GetHashCode, CopyTo, CopyTo, GetEnumerator, Initialize, .ctor, ToString, Equals, GetHashCode, GetType}"
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
                  GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[[System.Char]], System.IEquatable`1[[System.Char]]}"
                  GetMembers="{GetHashCode, Equals, Equals, CompareTo, CompareTo, ToString, ToString, ToString, Parse, TryParse, IsDigit, IsDigit, IsLetter, IsLetter, IsWhiteSpace, IsWhiteSpace, IsUpper, IsUpper, IsLower, IsLower, IsPunctuation, IsPunctuation, IsLetterOrDigit, IsLetterOrDigit, ToUpper, ToUpper, ToUpperInvariant, ToLower, ToLower, ToLowerInvariant, GetTypeCode, IsControl, IsControl, IsNumber, IsNumber, IsSeparator, IsSeparator, IsSurrogate, IsSurrogate, IsSymbol, IsSymbol, GetUnicodeCategory, GetUnicodeCategory, GetNumericValue, GetNumericValue, IsHighSurrogate, IsHighSurrogate, IsLowSurrogate, IsLowSurrogate, IsSurrogatePair, IsSurrogatePair, ConvertFromUtf32, ConvertToUtf32, ConvertToUtf32, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
                  GetMethods="{GetHashCode, Equals, Equals, CompareTo, CompareTo, ToString, ToString, ToString, Parse, TryParse, IsDigit, IsDigit, IsLetter, IsLetter, IsWhiteSpace, IsWhiteSpace, IsUpper, IsUpper, IsLower, IsLower, IsPunctuation, IsPunctuation, IsLetterOrDigit, IsLetterOrDigit, ToUpper, ToUpper, ToUpperInvariant, ToLower, ToLower, ToLowerInvariant, GetTypeCode, IsControl, IsControl, IsNumber, IsNumber, IsSeparator, IsSeparator, IsSurrogate, IsSurrogate, IsSymbol, IsSymbol, GetUnicodeCategory, GetUnicodeCategory, GetNumericValue, GetNumericValue, IsHighSurrogate, IsHighSurrogate, IsLowSurrogate, IsLowSurrogate, IsSurrogatePair, IsSurrogatePair, ConvertFromUtf32, ConvertToUtf32, ConvertToUtf32, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
              GetMembers="{GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, get_Length, get_LongLength, GetLength, GetLongLength, get_Rank, GetUpperBound, GetLowerBound, get_SyncRoot, get_IsReadOnly, get_IsFixedSize, get_IsSynchronized, Clone, CompareTo, Equals, GetHashCode, CopyTo, CopyTo, GetEnumerator, Initialize, System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, get_Length, get_LongLength, GetLength, GetLongLength, get_Rank, GetUpperBound, GetLowerBound, get_SyncRoot, get_IsReadOnly, get_IsFixedSize, get_IsSynchronized, Clone, CompareTo, Equals, GetHashCode, CopyTo, CopyTo, GetEnumerator, Initialize, .ctor, ToString, Equals, GetHashCode, GetType}"
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
                  GetInterfaces="{System.IComparable, System.IConvertible, System.IComparable`1[[System.Char]], System.IEquatable`1[[System.Char]]}"
                  GetMembers="{GetHashCode, Equals, Equals, CompareTo, CompareTo, ToString, ToString, ToString, Parse, TryParse, IsDigit, IsDigit, IsLetter, IsLetter, IsWhiteSpace, IsWhiteSpace, IsUpper, IsUpper, IsLower, IsLower, IsPunctuation, IsPunctuation, IsLetterOrDigit, IsLetterOrDigit, ToUpper, ToUpper, ToUpperInvariant, ToLower, ToLower, ToLowerInvariant, GetTypeCode, IsControl, IsControl, IsNumber, IsNumber, IsSeparator, IsSeparator, IsSurrogate, IsSurrogate, IsSymbol, IsSymbol, GetUnicodeCategory, GetUnicodeCategory, GetNumericValue, GetNumericValue, IsHighSurrogate, IsHighSurrogate, IsLowSurrogate, IsLowSurrogate, IsSurrogatePair, IsSurrogatePair, ConvertFromUtf32, ConvertToUtf32, ConvertToUtf32, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
                  GetMethods="{GetHashCode, Equals, Equals, CompareTo, CompareTo, ToString, ToString, ToString, Parse, TryParse, IsDigit, IsDigit, IsLetter, IsLetter, IsWhiteSpace, IsWhiteSpace, IsUpper, IsUpper, IsLower, IsLower, IsPunctuation, IsPunctuation, IsLetterOrDigit, IsLetterOrDigit, ToUpper, ToUpper, ToUpperInvariant, ToLower, ToLower, ToLowerInvariant, GetTypeCode, IsControl, IsControl, IsNumber, IsNumber, IsSeparator, IsSeparator, IsSurrogate, IsSurrogate, IsSymbol, IsSymbol, GetUnicodeCategory, GetUnicodeCategory, GetNumericValue, GetNumericValue, IsHighSurrogate, IsHighSurrogate, IsLowSurrogate, IsLowSurrogate, IsSurrogatePair, IsSurrogatePair, ConvertFromUtf32, ConvertToUtf32, ConvertToUtf32, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
          Name="nullable_value"
          Type="System.Nullable`1[[System.Int32]]"
          Value="{System.Nullable`1[[System.Int32]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.Nullable`1[[System.Int32]]"
              GetGenericArguments="{System.Int32}"
              GetMembers="{.ctor, get_HasValue, get_Value, GetValueOrDefault, GetValueOrDefault, Equals, GetHashCode, ToString, op_Implicit, op_Explicit, System.Boolean HasValue, System.Int32 Value, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{.ctor, get_HasValue, get_Value, GetValueOrDefault, GetValueOrDefault, Equals, GetHashCode, ToString, op_Implicit, op_Explicit, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
          Type="System.Nullable`1[[System.Int32]]"
          Value="{System.Nullable`1[[System.Int32]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.Nullable`1[[System.Int32]]"
              GetGenericArguments="{System.Int32}"
              GetMembers="{.ctor, get_HasValue, get_Value, GetValueOrDefault, GetValueOrDefault, Equals, GetHashCode, ToString, op_Implicit, op_Explicit, System.Boolean HasValue, System.Int32 Value, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{.ctor, get_HasValue, get_Value, GetValueOrDefault, GetValueOrDefault, Equals, GetHashCode, ToString, op_Implicit, op_Explicit, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
          Type="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]]"
          Value="{Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]]"
              GetGenericArguments="{System.Int32}"
              GetMembers="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
              IsClass="True"
              IsGenericType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="array_MyGenClass_int"
          Type="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]][]"
          Value="{Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]][]}">
          <Type>
            <DebugType
              Attributes="NotPublic"
              BaseType="System.Array"
              FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]][]"
              GetArrayRank="1"
              GetElementType="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]]"
              GetMembers="{GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, get_Length, get_LongLength, GetLength, GetLongLength, get_Rank, GetUpperBound, GetLowerBound, get_SyncRoot, get_IsReadOnly, get_IsFixedSize, get_IsSynchronized, Clone, CompareTo, Equals, GetHashCode, CopyTo, CopyTo, GetEnumerator, Initialize, System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, GetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, SetValue, get_Length, get_LongLength, GetLength, GetLongLength, get_Rank, GetUpperBound, GetLowerBound, get_SyncRoot, get_IsReadOnly, get_IsFixedSize, get_IsSynchronized, Clone, CompareTo, Equals, GetHashCode, CopyTo, CopyTo, GetEnumerator, Initialize, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetProperties="{System.Int32 Length, System.Int64 LongLength, System.Int32 Rank, System.Object SyncRoot, System.Boolean IsReadOnly, System.Boolean IsFixedSize, System.Boolean IsSynchronized}"
              HasElementType="True"
              IsArray="True"
              IsClass="True"
              IsCompilerGenerated="True">
              <GetElementType>
                <DebugType
                  Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
                  BaseType="System.Object"
                  FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]]"
                  GetGenericArguments="{System.Int32}"
                  GetMembers="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
                  GetMethods="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
                  IsClass="True"
                  IsGenericType="True">
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
          Type="Debugger.Tests.TestPrograms.DebugType[[System.Nullable`1[[System.Int32]]]].MyGenClass`1[[System.Nullable`1[[System.Int32]]]]"
          Value="{Debugger.Tests.TestPrograms.DebugType[[System.Nullable`1[[System.Int32]]]].MyGenClass`1[[System.Nullable`1[[System.Int32]]]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.TestPrograms.DebugType[[System.Nullable`1[[System.Int32]]]].MyGenClass`1[[System.Nullable`1[[System.Int32]]]]"
              GetGenericArguments="{System.Nullable`1[[System.Int32]]}"
              GetMembers="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{.ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
              IsClass="True"
              IsGenericType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myNestedStruct"
          Type="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]].MyNestedStruct[[System.Int32]]"
          Value="{Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]].MyNestedStruct[[System.Int32]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyGenClass`1[[System.Int32]].MyNestedStruct[[System.Int32]]"
              GetGenericArguments="{System.Int32}"
              GetMembers="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              IsGenericType="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myGenNestedStruct"
          Type="Debugger.Tests.TestPrograms.DebugType[[System.Int32, System.Char]].MyGenClass`1[[System.Int32, System.Char]].MyGenNestedStruct`1[[System.Int32, System.Char]]"
          Value="{Debugger.Tests.TestPrograms.DebugType[[System.Int32, System.Char]].MyGenClass`1[[System.Int32, System.Char]].MyGenNestedStruct`1[[System.Int32, System.Char]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32, System.Char]].MyGenClass`1[[System.Int32, System.Char]].MyGenNestedStruct`1[[System.Int32, System.Char]]"
              GetGenericArguments="{System.Int32, System.Char}"
              GetMembers="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              IsGenericType="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myInterfaceImpl"
          Type="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]"
          Value="{Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]"
              GetFields="{System.Void* voidPtr}"
              GetGenericArguments="{System.Int32}"
              GetInterfaces="{Debugger.Tests.TestPrograms.DebugType[[System.Int32, Debugger.Tests.TestPrograms.DebugType.MyClass, Debugger.Tests.TestPrograms.DebugType.MyStruct]].MyInterface`3[[System.Int32, Debugger.Tests.TestPrograms.DebugType.MyClass, Debugger.Tests.TestPrograms.DebugType.MyStruct]]}"
              GetMembers="{System.Void* voidPtr, get_Prop, set_SetterOnlyProp, Fun, Fun2, .ctor, System.Collections.Generic.List`1[[System.Int32]] Prop, System.Char SetterOnlyProp, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{get_Prop, set_SetterOnlyProp, Fun, Fun2, .ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetProperties="{System.Collections.Generic.List`1[[System.Int32]] Prop, System.Char SetterOnlyProp}"
              IsClass="True"
              IsGenericType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myInterface"
          Type="Debugger.Tests.TestPrograms.DebugType[[System.Int32, Debugger.Tests.TestPrograms.DebugType.MyClass, Debugger.Tests.TestPrograms.DebugType.MyStruct]].MyInterface`3[[System.Int32, Debugger.Tests.TestPrograms.DebugType.MyClass, Debugger.Tests.TestPrograms.DebugType.MyStruct]]"
          Value="{Debugger.Tests.TestPrograms.DebugType[[System.Int32]].MyInterfaceImpl`1[[System.Int32]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, ClassSemanticsMask, Abstract"
              FullName="Debugger.Tests.TestPrograms.DebugType[[System.Int32, Debugger.Tests.TestPrograms.DebugType.MyClass, Debugger.Tests.TestPrograms.DebugType.MyStruct]].MyInterface`3[[System.Int32, Debugger.Tests.TestPrograms.DebugType.MyClass, Debugger.Tests.TestPrograms.DebugType.MyStruct]]"
              GetGenericArguments="{System.Int32, Debugger.Tests.TestPrograms.DebugType.MyClass, Debugger.Tests.TestPrograms.DebugType.MyStruct}"
              GetMembers="{Fun}"
              GetMethods="{Fun}"
              IsGenericType="True"
              IsInterface="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="list"
          Type="System.Collections.Generic.List`1[[System.Int32]]"
          Value="{System.Collections.Generic.List`1[[System.Int32]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, Public, Serializable, BeforeFieldInit"
              BaseType="System.Object"
              FullName="System.Collections.Generic.List`1[[System.Int32]]"
              GetGenericArguments="{System.Int32}"
              GetInterfaces="{System.Collections.Generic.IList`1[[System.Int32]], System.Collections.Generic.ICollection`1[[System.Int32]], System.Collections.Generic.IEnumerable`1[[System.Int32]], System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable}"
              GetMembers="{.ctor, .ctor, .ctor, get_Capacity, set_Capacity, get_Count, get_Item, set_Item, Add, AddRange, AsReadOnly, BinarySearch, BinarySearch, BinarySearch, Clear, Contains, ConvertAll, CopyTo, CopyTo, CopyTo, Exists, Find, FindAll, FindIndex, FindIndex, FindIndex, FindLast, FindLastIndex, FindLastIndex, FindLastIndex, ForEach, GetEnumerator, GetRange, IndexOf, IndexOf, IndexOf, Insert, InsertRange, LastIndexOf, LastIndexOf, LastIndexOf, Remove, RemoveAll, RemoveAt, RemoveRange, Reverse, Reverse, Sort, Sort, Sort, Sort, ToArray, TrimExcess, TrueForAll, System.Int32 Capacity, System.Int32 Count, System.Int32 Item, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{.ctor, .ctor, .ctor, get_Capacity, set_Capacity, get_Count, get_Item, set_Item, Add, AddRange, AsReadOnly, BinarySearch, BinarySearch, BinarySearch, Clear, Contains, ConvertAll, CopyTo, CopyTo, CopyTo, Exists, Find, FindAll, FindIndex, FindIndex, FindIndex, FindLast, FindLastIndex, FindLastIndex, FindLastIndex, ForEach, GetEnumerator, GetRange, IndexOf, IndexOf, IndexOf, Insert, InsertRange, LastIndexOf, LastIndexOf, LastIndexOf, Remove, RemoveAll, RemoveAt, RemoveRange, Reverse, Reverse, Sort, Sort, Sort, Sort, ToArray, TrimExcess, TrueForAll, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetProperties="{System.Int32 Capacity, System.Int32 Count, System.Int32 Item}"
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
          Type="System.Collections.Generic.List`1[[System.Int32]].Enumerator[[System.Int32]]"
          Value="{System.Collections.Generic.List`1[[System.Int32]].Enumerator[[System.Int32]]}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, SequentialLayout, Sealed, Serializable, BeforeFieldInit"
              BaseType="System.ValueType"
              FullName="System.Collections.Generic.List`1[[System.Int32]].Enumerator[[System.Int32]]"
              GetGenericArguments="{System.Int32}"
              GetInterfaces="{System.Collections.Generic.IEnumerator`1[[System.Int32]], System.IDisposable, System.Collections.IEnumerator}"
              GetMembers="{Dispose, MoveNext, get_Current, System.Int32 Current, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{Dispose, MoveNext, get_Current, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetProperties="{System.Int32 Current}"
              IsGenericType="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="fnPtr"
          Type="Debugger.Tests.TestPrograms.DebugType.AddDelegate"
          Value="{Debugger.Tests.TestPrograms.DebugType.AddDelegate}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, Sealed"
              BaseType="System.MulticastDelegate"
              FullName="Debugger.Tests.TestPrograms.DebugType.AddDelegate"
              GetMembers="{.ctor, Invoke, BeginInvoke, EndInvoke, GetObjectData, Equals, GetInvocationList, GetHashCode, DynamicInvoke, Equals, GetHashCode, GetInvocationList, get_Method, get_Target, Clone, GetObjectData, System.Reflection.MethodInfo Method, System.Object Target, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{.ctor, Invoke, BeginInvoke, EndInvoke, GetObjectData, Equals, GetInvocationList, GetHashCode, DynamicInvoke, Equals, GetHashCode, GetInvocationList, get_Method, get_Target, Clone, GetObjectData, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetProperties="{System.Reflection.MethodInfo Method, System.Object Target}"
              IsClass="True">
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
              GetMembers="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
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
              GetMembers="{Parse, Parse, GetUnderlyingType, GetValues, GetName, GetNames, ToObject, ToObject, ToObject, ToObject, ToObject, ToObject, ToObject, ToObject, ToObject, IsDefined, Format, Equals, GetHashCode, ToString, ToString, ToString, ToString, CompareTo, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{Parse, Parse, GetUnderlyingType, GetValues, GetName, GetNames, ToObject, ToObject, ToObject, ToObject, ToObject, ToObject, ToObject, ToObject, ToObject, IsDefined, Format, Equals, GetHashCode, ToString, ToString, ToString, ToString, CompareTo, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              IsClass="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="myEnum"
          Type="Debugger.Tests.TestPrograms.DebugType.MyEnum"
          Value="{Debugger.Tests.TestPrograms.DebugType.MyEnum}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, Sealed"
              BaseType="System.Enum"
              FullName="Debugger.Tests.TestPrograms.DebugType.MyEnum"
              GetEnumUnderlyingType="System.Byte"
              GetFields="{System.Byte value__}"
              GetMembers="{System.Byte value__, Equals, GetHashCode, ToString, ToString, ToString, ToString, CompareTo, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{Equals, GetHashCode, ToString, ToString, ToString, ToString, CompareTo, GetTypeCode, Equals, GetHashCode, ToString, .ctor, ToString, Equals, GetHashCode, GetType}"
              IsEnum="True"
              IsValueType="True">
              <GetElementType>null</GetElementType>
            </DebugType>
          </Type>
        </LocalVariable>
      </Item>
      <Item>
        <LocalVariable
          Name="access"
          Type="Debugger.Tests.TestPrograms.DebugType.Access"
          Value="{Debugger.Tests.TestPrograms.DebugType.Access}">
          <Type>
            <DebugType
              Attributes="AutoLayout, AnsiClass, Class, NestedPublic, BeforeFieldInit"
              BaseType="System.Object"
              FullName="Debugger.Tests.TestPrograms.DebugType.Access"
              GetFields="{System.Int32 publicField}"
              GetMembers="{System.Int32 publicField, get_publicProperty, publicMethod, .ctor, System.Int32 publicProperty, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetMethods="{get_publicProperty, publicMethod, .ctor, .ctor, ToString, Equals, GetHashCode, GetType}"
              GetProperties="{System.Int32 publicProperty}"
              IsClass="True">
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