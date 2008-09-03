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
  <Test
    name="DefinedTypes.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>DefinedTypes.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break DefinedTypes.cs:16,4-16,40</DebuggingPaused>
    <TypesAsString
      Capacity="4"
      Count="3">
      <Item>Debugger.Tests.TestPrograms.DefinedTypes_Class</Item>
      <Item>Debugger.Tests.TestPrograms.DefinedTypes_Struct</Item>
      <Item>Debugger.Tests.TestPrograms.DefinedTypes_GenericClass`2</Item>
    </TypesAsString>
    <Types
      Capacity="4"
      Count="2">
      <Item>
        <DebugType
          BaseType="System.Object"
          ElementType="null"
          FullName="Debugger.Tests.TestPrograms.DefinedTypes_Class"
          GenericArguments="{}"
          Interfaces="{}"
          Kind="Class"
          Module="DefinedTypes.exe"
          Name="DefinedTypes_Class" />
      </Item>
      <Item>
        <DebugType
          BaseType="System.ValueType"
          ElementType="null"
          FullName="Debugger.Tests.TestPrograms.DefinedTypes_Struct"
          GenericArguments="{}"
          Interfaces="{}"
          Kind="ValueType"
          Module="DefinedTypes.exe"
          Name="DefinedTypes_Struct" />
      </Item>
    </Types>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT