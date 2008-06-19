// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class DefinedTypes_Class
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
		}
	}
	
	public struct DefinedTypes_Struct
	{
		
	}
	
	public class DefinedTypes_GenericClass<K, V>
	{
		
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void DefinedTypes()
		{
			StartTest("DefinedTypes.cs");
			
			ObjectDump("TypesAsString", process.GetModule("DefinedTypes.exe").GetNamesOfDefinedTypes());
			ObjectDump("Types", process.GetModule("DefinedTypes.exe").GetDefinedTypes());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="DefinedTypes.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">DefinedTypes.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <TypesAsString Type="List`1" ToString="System.Collections.Generic.List`1[System.String]">
      <Capacity>4</Capacity>
      <Count>3</Count>
      <Item>Debugger.Tests.TestPrograms.DefinedTypes_Class</Item>
      <Item>Debugger.Tests.TestPrograms.DefinedTypes_Struct</Item>
      <Item>Debugger.Tests.TestPrograms.DefinedTypes_GenericClass`2</Item>
    </TypesAsString>
    <Types Type="List`1" ToString="System.Collections.Generic.List`1[Debugger.MetaData.DebugType]">
      <Capacity>4</Capacity>
      <Count>2</Count>
      <Item Type="DebugType" ToString="Debugger.Tests.TestPrograms.DefinedTypes_Class">
        <BaseType>System.Object</BaseType>
        <FullName>Debugger.Tests.TestPrograms.DefinedTypes_Class</FullName>
        <HasElementType>False</HasElementType>
        <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
        <IsArray>False</IsArray>
        <IsClass>True</IsClass>
        <IsGenericType>False</IsGenericType>
        <IsInteger>False</IsInteger>
        <IsInterface>False</IsInterface>
        <IsPrimitive>False</IsPrimitive>
        <IsValueType>False</IsValueType>
        <ManagedType>null</ManagedType>
        <Module>DefinedTypes.exe</Module>
      </Item>
      <Item Type="DebugType" ToString="Debugger.Tests.TestPrograms.DefinedTypes_Struct">
        <BaseType>System.ValueType</BaseType>
        <FullName>Debugger.Tests.TestPrograms.DefinedTypes_Struct</FullName>
        <HasElementType>False</HasElementType>
        <Interfaces>System.Collections.Generic.List`1[Debugger.MetaData.DebugType]</Interfaces>
        <IsArray>False</IsArray>
        <IsClass>False</IsClass>
        <IsGenericType>False</IsGenericType>
        <IsInteger>False</IsInteger>
        <IsInterface>False</IsInterface>
        <IsPrimitive>False</IsPrimitive>
        <IsValueType>True</IsValueType>
        <ManagedType>null</ManagedType>
        <Module>DefinedTypes.exe</Module>
      </Item>
    </Types>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT