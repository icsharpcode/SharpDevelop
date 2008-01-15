// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Callstack
	{
		public static void Main()
		{
			Sub1();
		}
		
		static void Sub1()
		{
			Sub2();
		}
		
		static void Sub2()
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
		public void Callstack()
		{
			StartTest("Callstack.cs");
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.StepOut();
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
			process.StepOut();
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.Callstack);
			
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
  <Test name="Callstack.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">Callstack.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="Callstack" Type="ReadOnlyCollection`1">
      <Count>3</Count>
      <Item Type="StackFrame">
        <MethodInfo Type="MethodInfo">
          <Name>Sub2</Name>
          <FullName>Debugger.Tests.TestPrograms.Callstack.Sub2</FullName>
          <IsPrivate>True</IsPrivate>
          <IsPublic>False</IsPublic>
          <IsSpecialName>False</IsSpecialName>
          <IsStatic>True</IsStatic>
          <Module>Callstack.exe</Module>
          <DeclaringType>Debugger.Tests.TestPrograms.Callstack</DeclaringType>
        </MethodInfo>
        <HasSymbols>True</HasSymbols>
        <HasExpired>False</HasExpired>
        <NextStatement>Start=26,4 End=26,40</NextStatement>
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
      </Item>
      <Item Type="StackFrame">
        <MethodInfo Type="MethodInfo">
          <Name>Sub1</Name>
          <FullName>Debugger.Tests.TestPrograms.Callstack.Sub1</FullName>
          <IsPrivate>True</IsPrivate>
          <IsPublic>False</IsPublic>
          <IsSpecialName>False</IsSpecialName>
          <IsStatic>True</IsStatic>
          <Module>Callstack.exe</Module>
          <DeclaringType>Debugger.Tests.TestPrograms.Callstack</DeclaringType>
        </MethodInfo>
        <HasSymbols>True</HasSymbols>
        <HasExpired>False</HasExpired>
        <NextStatement>Start=21,4 End=21,11</NextStatement>
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
      </Item>
      <Item Type="StackFrame">
        <MethodInfo Type="MethodInfo">
          <Name>Main</Name>
          <FullName>Debugger.Tests.TestPrograms.Callstack.Main</FullName>
          <IsPrivate>False</IsPrivate>
          <IsPublic>True</IsPublic>
          <IsSpecialName>False</IsSpecialName>
          <IsStatic>True</IsStatic>
          <Module>Callstack.exe</Module>
          <DeclaringType>Debugger.Tests.TestPrograms.Callstack</DeclaringType>
        </MethodInfo>
        <HasSymbols>True</HasSymbols>
        <HasExpired>False</HasExpired>
        <NextStatement>Start=16,4 End=16,11</NextStatement>
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
      </Item>
    </ObjectDump>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ObjectDump name="Callstack" Type="ReadOnlyCollection`1">
      <Count>2</Count>
      <Item Type="StackFrame">
        <MethodInfo Type="MethodInfo">
          <Name>Sub1</Name>
          <FullName>Debugger.Tests.TestPrograms.Callstack.Sub1</FullName>
          <IsPrivate>True</IsPrivate>
          <IsPublic>False</IsPublic>
          <IsSpecialName>False</IsSpecialName>
          <IsStatic>True</IsStatic>
          <Module>Callstack.exe</Module>
          <DeclaringType>Debugger.Tests.TestPrograms.Callstack</DeclaringType>
        </MethodInfo>
        <HasSymbols>True</HasSymbols>
        <HasExpired>False</HasExpired>
        <NextStatement>Start=21,4 End=21,11</NextStatement>
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
      </Item>
      <Item Type="StackFrame">
        <MethodInfo Type="MethodInfo">
          <Name>Main</Name>
          <FullName>Debugger.Tests.TestPrograms.Callstack.Main</FullName>
          <IsPrivate>False</IsPrivate>
          <IsPublic>True</IsPublic>
          <IsSpecialName>False</IsSpecialName>
          <IsStatic>True</IsStatic>
          <Module>Callstack.exe</Module>
          <DeclaringType>Debugger.Tests.TestPrograms.Callstack</DeclaringType>
        </MethodInfo>
        <HasSymbols>True</HasSymbols>
        <HasExpired>False</HasExpired>
        <NextStatement>Start=16,4 End=16,11</NextStatement>
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
      </Item>
    </ObjectDump>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ObjectDump name="Callstack" Type="ReadOnlyCollection`1">
      <Count>1</Count>
      <Item Type="StackFrame">
        <MethodInfo Type="MethodInfo">
          <Name>Main</Name>
          <FullName>Debugger.Tests.TestPrograms.Callstack.Main</FullName>
          <IsPrivate>False</IsPrivate>
          <IsPublic>True</IsPublic>
          <IsSpecialName>False</IsSpecialName>
          <IsStatic>True</IsStatic>
          <Module>Callstack.exe</Module>
          <DeclaringType>Debugger.Tests.TestPrograms.Callstack</DeclaringType>
        </MethodInfo>
        <HasSymbols>True</HasSymbols>
        <HasExpired>False</HasExpired>
        <NextStatement>Start=16,4 End=16,11</NextStatement>
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
      </Item>
    </ObjectDump>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT