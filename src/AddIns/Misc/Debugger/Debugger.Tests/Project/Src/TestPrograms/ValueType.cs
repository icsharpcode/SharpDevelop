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
			process.AsyncContinue();
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
      <ArrayDimensions exception="Value is not an array" />
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <AsString>{Debugger.Tests.ValueType}</AsString>
      <Expression>this</Expression>
      <HasExpired>False</HasExpired>
      <IsArray>False</IsArray>
      <IsInteger>False</IsInteger>
      <IsNull>False</IsNull>
      <IsObject>True</IsObject>
      <IsPrimitive>False</IsPrimitive>
      <PrimitiveValue exception="Value is not a primitive type" />
      <Type Type="DebugType" ToString="Debugger.Tests.ValueType">
        <BaseType Type="DebugType" ToString="System.ValueType">
          <BaseType Type="DebugType" ToString="System.Object">
            <BaseType>null</BaseType>
            <FullName>System.Object</FullName>
            <HasElementType>False</HasElementType>
            <IsArray>False</IsArray>
            <IsClass>True</IsClass>
            <IsGenericType>False</IsGenericType>
            <IsInteger>False</IsInteger>
            <IsPrimitive>False</IsPrimitive>
            <IsValueType>False</IsValueType>
            <ManagedType>null</ManagedType>
            <Module>mscorlib.dll</Module>
          </BaseType>
          <FullName>System.ValueType</FullName>
          <HasElementType>False</HasElementType>
          <IsArray>False</IsArray>
          <IsClass>True</IsClass>
          <IsGenericType>False</IsGenericType>
          <IsInteger>False</IsInteger>
          <IsPrimitive>False</IsPrimitive>
          <IsValueType>False</IsValueType>
          <ManagedType>null</ManagedType>
          <Module>mscorlib.dll</Module>
        </BaseType>
        <FullName>Debugger.Tests.ValueType</FullName>
        <HasElementType>False</HasElementType>
        <IsArray>False</IsArray>
        <IsClass>False</IsClass>
        <IsGenericType>False</IsGenericType>
        <IsInteger>False</IsInteger>
        <IsPrimitive>False</IsPrimitive>
        <IsValueType>True</IsValueType>
        <ManagedType>null</ManagedType>
        <Module>ValueType.exe</Module>
      </Type>
    </this>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT