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
		public void DebugType_DefinedTypes()
		{
			StartTest("DebugType_DefinedTypes.cs");
			
			ObjectDump("TypesAsString", process.Modules["DebugType_DefinedTypes.exe"].GetNamesOfDefinedTypes());
			ObjectDump("Types", process.Modules["DebugType_DefinedTypes.exe"].GetDefinedTypes());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="DebugType_DefinedTypes.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>DebugType_DefinedTypes.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break DebugType_DefinedTypes.cs:16,4-16,40</DebuggingPaused>
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
          FullName="Debugger.Tests.TestPrograms.DefinedTypes_Class"
          Kind="Class"
          Module="DebugType_DefinedTypes.exe"
          Name="DefinedTypes_Class" />
      </Item>
      <Item>
        <DebugType
          BaseType="System.ValueType"
          FullName="Debugger.Tests.TestPrograms.DefinedTypes_Struct"
          Kind="ValueType"
          Module="DebugType_DefinedTypes.exe"
          Name="DefinedTypes_Struct" />
      </Item>
    </Types>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT