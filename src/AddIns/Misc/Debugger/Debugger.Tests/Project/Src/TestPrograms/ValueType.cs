// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests
{
	public struct ValueType
	{
		public static void Main()
		{
			new ValueType().Fun();
		}
		
		public void Fun()
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
		public void ValueType()
		{
			ExpandProperties(
				"Value.Type",
				"DebugType.BaseType"
			);
			StartTest("ValueType.cs");
			WaitForPause();
			
			ObjectDump("this", process.SelectedStackFrame.GetThisValue());
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
  <Test name="ValueType.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">ValueType.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <this Type="Value" ToString="this = {Debugger.Tests.ValueType}">
      <IsArray>False</IsArray>
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <ArrayDimensions exception="Value is not an array" />
      <Expression>this</Expression>
      <IsNull>False</IsNull>
      <AsString>{Debugger.Tests.ValueType}</AsString>
      <HasExpired>False</HasExpired>
      <Type Type="DebugType" ToString="Debugger.Tests.ValueType">
        <ManagedType>null</ManagedType>
        <Module>ValueType.exe</Module>
        <FullName>Debugger.Tests.ValueType</FullName>
        <HasElementType>False</HasElementType>
        <IsArray>False</IsArray>
        <IsGenericType>False</IsGenericType>
        <IsClass>False</IsClass>
        <IsValueType>True</IsValueType>
        <IsPrimitive>False</IsPrimitive>
        <IsInteger>False</IsInteger>
        <BaseType Type="DebugType" ToString="System.ValueType">
          <ManagedType>null</ManagedType>
          <Module>mscorlib.dll</Module>
          <FullName>System.ValueType</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsGenericType>False</IsGenericType>
          <IsClass>True</IsClass>
          <IsValueType>False</IsValueType>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <BaseType Type="DebugType" ToString="System.Object">
            <ManagedType>null</ManagedType>
            <Module>mscorlib.dll</Module>
            <FullName>System.Object</FullName>
            <HasElementType>False</HasElementType>
            <IsArray>False</IsArray>
            <IsGenericType>False</IsGenericType>
            <IsClass>True</IsClass>
            <IsValueType>False</IsValueType>
            <IsPrimitive>False</IsPrimitive>
            <IsInteger>False</IsInteger>
            <BaseType>null</BaseType>
          </BaseType>
        </BaseType>
      </Type>
      <IsObject>True</IsObject>
      <IsPrimitive>False</IsPrimitive>
      <IsInteger>False</IsInteger>
      <PrimitiveValue exception="Value is not a primitive type" />
    </this>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT