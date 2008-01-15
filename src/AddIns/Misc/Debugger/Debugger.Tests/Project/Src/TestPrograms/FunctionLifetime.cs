// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class FunctionLifetime
	{
		public static void Main()
		{
			Function(1);
			System.Diagnostics.Debugger.Break(); // 4
		}
		
		static void Function(int i)
		{
			System.Diagnostics.Debugger.Break(); // 1
			SubFunction();
			System.Diagnostics.Debugger.Break(); // 3
		}
		
		static void SubFunction()
		{
			System.Diagnostics.Debugger.Break(); // 2
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void FunctionLifetime()
		{
			StartTest("FunctionLifetime");
			WaitForPause();
			StackFrame stackFrame = process.SelectedStackFrame;
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue(); // Go to the SubFunction
			WaitForPause();
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue(); // Go back to Function
			WaitForPause();
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue(); // Setp out of function
			WaitForPause();
			ObjectDump("Main", process.SelectedStackFrame);
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
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
  <Test name="FunctionLifetime">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">FunctionLifetime.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Function</Name>
        <FullName>Debugger.Tests.TestPrograms.FunctionLifetime.Function</FullName>
        <IsPrivate>True</IsPrivate>
        <IsPublic>False</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>FunctionLifetime.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.FunctionLifetime</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=22,4 End=22,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>1</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>1</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>1</PrimitiveValue>
          <Expression>i</Expression>
          <Name>i</Name>
          <IsNull>False</IsNull>
          <AsString>1</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="Old StackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Function</Name>
        <FullName>Debugger.Tests.TestPrograms.FunctionLifetime.Function</FullName>
        <IsPrivate>True</IsPrivate>
        <IsPublic>False</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>FunctionLifetime.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.FunctionLifetime</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>True</HasExpired>
      <NextStatement exception="StackFrame has expired" />
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount exception="StackFrame has expired" />
      <Arguments exception="StackFrame has expired" />
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>SubFunction</Name>
        <FullName>Debugger.Tests.TestPrograms.FunctionLifetime.SubFunction</FullName>
        <IsPrivate>True</IsPrivate>
        <IsPublic>False</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>FunctionLifetime.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.FunctionLifetime</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=29,4 End=29,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>0</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>0</Count>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="Old StackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Function</Name>
        <FullName>Debugger.Tests.TestPrograms.FunctionLifetime.Function</FullName>
        <IsPrivate>True</IsPrivate>
        <IsPublic>False</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>FunctionLifetime.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.FunctionLifetime</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>True</HasExpired>
      <NextStatement exception="StackFrame has expired" />
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount exception="StackFrame has expired" />
      <Arguments exception="StackFrame has expired" />
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Function</Name>
        <FullName>Debugger.Tests.TestPrograms.FunctionLifetime.Function</FullName>
        <IsPrivate>True</IsPrivate>
        <IsPublic>False</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>FunctionLifetime.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.FunctionLifetime</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=24,4 End=24,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>1</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>1</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>1</PrimitiveValue>
          <Expression>i</Expression>
          <Name>i</Name>
          <IsNull>False</IsNull>
          <AsString>1</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="Main" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Main</Name>
        <FullName>Debugger.Tests.TestPrograms.FunctionLifetime.Main</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>FunctionLifetime.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.FunctionLifetime</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=17,4 End=17,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>0</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>0</Count>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <ObjectDump name="Old StackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Function</Name>
        <FullName>Debugger.Tests.TestPrograms.FunctionLifetime.Function</FullName>
        <IsPrivate>True</IsPrivate>
        <IsPublic>False</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>FunctionLifetime.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.FunctionLifetime</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>True</HasExpired>
      <NextStatement exception="StackFrame has expired" />
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount exception="StackFrame has expired" />
      <Arguments exception="StackFrame has expired" />
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Main</Name>
        <FullName>Debugger.Tests.TestPrograms.FunctionLifetime.Main</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>FunctionLifetime.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.FunctionLifetime</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=17,4 End=17,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>0</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>0</Count>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>0</Count>
      </LocalVariables>
    </ObjectDump>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT