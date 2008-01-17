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
			gClass.GenericMethod<bool>(1, "1!");
			GenericClass<int, string>.StaticMetod(1, "1!");
			GenericClass<int, string>.StaticGenericMethod<bool>(1, "1!");
			
			GenericStruct<int, string> gStruct = new GenericStruct<int, string>();
			gStruct.Metod(1, "1!");
			gStruct.GenericMethod<bool>(1, "1!");
			GenericStruct<int, string>.StaticMetod(1, "1!");
			GenericStruct<int, string>.StaticGenericMethod<bool>(1, "1!");
			
			System.Diagnostics.Debugger.Break();
		}
	}
	
	public class GenericClass<V, K>
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
				"MemberInfo.DeclaringType",
				"StackFrame.Arguments"
			);
			StartTest("Generics.cs");
			
			for(int i = 0; i < 8; i++) {
				WaitForPause();
				ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
				process.Continue();
			}
			
			WaitForPause();
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
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
      <MethodInfo Type="MethodInfo" ToString="Metod">
        <Name>Metod</Name>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.Metod</FullName>
        <Module>Generics.exe</Module>
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;">
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
          <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsGenericType>True</IsGenericType>
          <IsClass>True</IsClass>
          <IsValueType>False</IsValueType>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <BaseType>System.Object</BaseType>
        </DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=36,4 End=36,40</NextStatement>
      <ArgumentCount>2</ArgumentCount>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.GenericMethod">
      <MethodInfo Type="MethodInfo" ToString="GenericMethod">
        <Name>GenericMethod</Name>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.GenericMethod</FullName>
        <Module>Generics.exe</Module>
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;">
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
          <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsGenericType>True</IsGenericType>
          <IsClass>True</IsClass>
          <IsValueType>False</IsValueType>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <BaseType>System.Object</BaseType>
        </DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=42,4 End=42,40</NextStatement>
      <ArgumentCount>2</ArgumentCount>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticMetod">
      <MethodInfo Type="MethodInfo" ToString="StaticMetod">
        <Name>StaticMetod</Name>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticMetod</FullName>
        <Module>Generics.exe</Module>
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;">
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
          <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsGenericType>True</IsGenericType>
          <IsClass>True</IsClass>
          <IsValueType>False</IsValueType>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <BaseType>System.Object</BaseType>
        </DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=48,4 End=48,40</NextStatement>
      <ArgumentCount>2</ArgumentCount>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticGenericMethod">
      <MethodInfo Type="MethodInfo" ToString="StaticGenericMethod">
        <Name>StaticGenericMethod</Name>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticGenericMethod</FullName>
        <Module>Generics.exe</Module>
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;">
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
          <FullName>Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsGenericType>True</IsGenericType>
          <IsClass>True</IsClass>
          <IsValueType>False</IsValueType>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <BaseType>System.Object</BaseType>
        </DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=54,4 End=54,40</NextStatement>
      <ArgumentCount>2</ArgumentCount>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.Metod">
      <MethodInfo Type="MethodInfo" ToString="Metod">
        <Name>Metod</Name>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.Metod</FullName>
        <Module>Generics.exe</Module>
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;">
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
          <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsGenericType>True</IsGenericType>
          <IsClass>False</IsClass>
          <IsValueType>True</IsValueType>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <BaseType>System.ValueType</BaseType>
        </DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=63,4 End=63,40</NextStatement>
      <ArgumentCount>2</ArgumentCount>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.GenericMethod">
      <MethodInfo Type="MethodInfo" ToString="GenericMethod">
        <Name>GenericMethod</Name>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>False</IsStatic>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.GenericMethod</FullName>
        <Module>Generics.exe</Module>
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;">
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
          <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsGenericType>True</IsGenericType>
          <IsClass>False</IsClass>
          <IsValueType>True</IsValueType>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <BaseType>System.ValueType</BaseType>
        </DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=69,4 End=69,40</NextStatement>
      <ArgumentCount>2</ArgumentCount>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticMetod">
      <MethodInfo Type="MethodInfo" ToString="StaticMetod">
        <Name>StaticMetod</Name>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticMetod</FullName>
        <Module>Generics.exe</Module>
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;">
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
          <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsGenericType>True</IsGenericType>
          <IsClass>False</IsClass>
          <IsValueType>True</IsValueType>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <BaseType>System.ValueType</BaseType>
        </DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=75,4 End=75,40</NextStatement>
      <ArgumentCount>2</ArgumentCount>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticGenericMethod">
      <MethodInfo Type="MethodInfo" ToString="StaticGenericMethod">
        <Name>StaticGenericMethod</Name>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticGenericMethod</FullName>
        <Module>Generics.exe</Module>
        <DeclaringType Type="DebugType" ToString="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;">
          <ManagedType>null</ManagedType>
          <Module>Generics.exe</Module>
          <FullName>Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsGenericType>True</IsGenericType>
          <IsClass>False</IsClass>
          <IsValueType>True</IsValueType>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <BaseType>System.ValueType</BaseType>
        </DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=81,4 End=81,40</NextStatement>
      <ArgumentCount>2</ArgumentCount>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT