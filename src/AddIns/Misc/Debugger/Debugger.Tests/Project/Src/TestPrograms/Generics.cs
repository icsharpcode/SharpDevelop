// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class MainClass
	{
		public static void Main()
		{
			GenericClass<int, string> gClass = new GenericClass<int, string>();
			gClass.Metod(1, "1!");
			gClass.GenericMethod<bool>(2, "2!");
			GenericClass<int, string>.StaticMetod(3, "3!");
			GenericClass<int, string>.StaticGenericMethod<bool>(4, "4!");
			
			GenericStruct<int, string> gStruct = new GenericStruct<int, string>();
			gStruct.Metod(5, "5!");
			gStruct.GenericMethod<bool>(6, "6!");
			GenericStruct<int, string>.StaticMetod(7, "7!");
			GenericStruct<int, string>.StaticGenericMethod<bool>(8, "8!");
			
			System.Diagnostics.Debugger.Break();
		}
	}
	
	public class GenericClass<V, K>
	{
		public V Prop {
			get {
				return default(V);
			}
		}
		
		public static V StaticProp {
			get {
				return default(V);
			}
		}
		
		public K Metod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public T GenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
		
		public static K StaticMetod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public static T StaticGenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
	}
	
	public struct GenericStruct<V, K>
	{
		public K Metod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public T GenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
		
		public static K StaticMetod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public static T StaticGenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Generics()
		{
			ExpandProperties(
				"StackFrame.MethodInfo",
				"MemberInfo.DeclaringType"
			);
			StartTest("Generics.cs");
			
			for(int i = 0; i < 8; i++) {
				ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
				ObjectDump("SelectedStackFrame-GetArguments", process.SelectedStackFrame.GetArgumentValues());
				process.Continue();
			}
			ObjectDump("Prop", process.SelectedStackFrame.GetLocalVariableValue("gClass").GetMemberValue("Prop"));
			ObjectDump("StaticProp", process.SelectedStackFrame.GetLocalVariableValue("gClass").GetMemberValue("StaticProp"));
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="Generics.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">Generics.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.Metod">
      <ArgumentCount>2</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo Type="MethodInfo" ToString="Metod">
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;">
          <BaseType>System.Object</BaseType>
          <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>True</IsClass>
          <IsGenericType>True</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>False</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
        </DeclaringType>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.Metod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <Module>Generics.exe</Module>
        <Name>Metod</Name>
      </MethodInfo>
      <NextStatement>Generics.cs:48,4-48,40</NextStatement>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="v = 1">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>1</AsString>
        <Expression>v</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>1</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="k = 1!">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>1!</AsString>
        <Expression>k</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>1!</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.GenericMethod">
      <ArgumentCount>2</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo Type="MethodInfo" ToString="GenericMethod">
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;">
          <BaseType>System.Object</BaseType>
          <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>True</IsClass>
          <IsGenericType>True</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>False</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
        </DeclaringType>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.GenericMethod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <Module>Generics.exe</Module>
        <Name>GenericMethod</Name>
      </MethodInfo>
      <NextStatement>Generics.cs:54,4-54,40</NextStatement>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="v = 2">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>2</AsString>
        <Expression>v</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>2</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="k = 2!">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>2!</AsString>
        <Expression>k</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>2!</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticMetod">
      <ArgumentCount>2</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo Type="MethodInfo" ToString="StaticMetod">
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;">
          <BaseType>System.Object</BaseType>
          <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>True</IsClass>
          <IsGenericType>True</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>False</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
        </DeclaringType>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticMetod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Generics.exe</Module>
        <Name>StaticMetod</Name>
      </MethodInfo>
      <NextStatement>Generics.cs:60,4-60,40</NextStatement>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="v = 3">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>3</AsString>
        <Expression>v</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>3</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="k = 3!">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>3!</AsString>
        <Expression>k</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>3!</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticGenericMethod">
      <ArgumentCount>2</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo Type="MethodInfo" ToString="StaticGenericMethod">
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;">
          <BaseType>System.Object</BaseType>
          <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>True</IsClass>
          <IsGenericType>True</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>False</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
        </DeclaringType>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticGenericMethod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Generics.exe</Module>
        <Name>StaticGenericMethod</Name>
      </MethodInfo>
      <NextStatement>Generics.cs:66,4-66,40</NextStatement>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="v = 4">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>4</AsString>
        <Expression>v</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>4</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="k = 4!">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>4!</AsString>
        <Expression>k</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>4!</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.Metod">
      <ArgumentCount>2</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo Type="MethodInfo" ToString="Metod">
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;">
          <BaseType>System.ValueType</BaseType>
          <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>True</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>False</IsPrimitive>
          <IsValueType>True</IsValueType>
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
        </DeclaringType>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.Metod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <Module>Generics.exe</Module>
        <Name>Metod</Name>
      </MethodInfo>
      <NextStatement>Generics.cs:75,4-75,40</NextStatement>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="v = 5">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>5</AsString>
        <Expression>v</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>5</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="k = 5!">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>5!</AsString>
        <Expression>k</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>5!</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.GenericMethod">
      <ArgumentCount>2</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo Type="MethodInfo" ToString="GenericMethod">
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;">
          <BaseType>System.ValueType</BaseType>
          <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>True</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>False</IsPrimitive>
          <IsValueType>True</IsValueType>
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
        </DeclaringType>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.GenericMethod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <Module>Generics.exe</Module>
        <Name>GenericMethod</Name>
      </MethodInfo>
      <NextStatement>Generics.cs:81,4-81,40</NextStatement>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="v = 6">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>6</AsString>
        <Expression>v</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>6</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="k = 6!">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>6!</AsString>
        <Expression>k</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>6!</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticMetod">
      <ArgumentCount>2</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo Type="MethodInfo" ToString="StaticMetod">
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;">
          <BaseType>System.ValueType</BaseType>
          <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>True</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>False</IsPrimitive>
          <IsValueType>True</IsValueType>
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
        </DeclaringType>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticMetod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Generics.exe</Module>
        <Name>StaticMetod</Name>
      </MethodInfo>
      <NextStatement>Generics.cs:87,4-87,40</NextStatement>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="v = 7">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>7</AsString>
        <Expression>v</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>7</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="k = 7!">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>7!</AsString>
        <Expression>k</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>7!</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticGenericMethod">
      <ArgumentCount>2</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo Type="MethodInfo" ToString="StaticGenericMethod">
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;">
          <BaseType>System.ValueType</BaseType>
          <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
          <IsArray>False</IsArray>
          <IsClass>False</IsClass>
          <IsGenericType>True</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsInterface>False</IsInterface>
          <IsPrimitive>False</IsPrimitive>
          <IsValueType>True</IsValueType>
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
        </DeclaringType>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticGenericMethod</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Generics.exe</Module>
        <Name>StaticGenericMethod</Name>
      </MethodInfo>
      <NextStatement>Generics.cs:93,4-93,40</NextStatement>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="v = 8">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>8</AsString>
        <Expression>v</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>8</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="k = 8!">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>8!</AsString>
        <Expression>k</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>8!</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break</DebuggingPaused>
    <Prop Type="Value" ToString="gClass.Prop = 0">
      <ArrayDimensions exception="Value is not an array" />
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <AsString>0</AsString>
      <Expression>gClass.Prop</Expression>
      <HasExpired>False</HasExpired>
      <IsArray>False</IsArray>
      <IsInteger>True</IsInteger>
      <IsNull>False</IsNull>
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <PrimitiveValue>0</PrimitiveValue>
      <Type>System.Int32</Type>
    </Prop>
    <StaticProp Type="Value" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticProp = 0">
      <ArrayDimensions exception="Value is not an array" />
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <AsString>0</AsString>
      <Expression>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticProp</Expression>
      <HasExpired>False</HasExpired>
      <IsArray>False</IsArray>
      <IsInteger>True</IsInteger>
      <IsNull>False</IsNull>
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <PrimitiveValue>0</PrimitiveValue>
      <Type>System.Int32</Type>
    </StaticProp>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT