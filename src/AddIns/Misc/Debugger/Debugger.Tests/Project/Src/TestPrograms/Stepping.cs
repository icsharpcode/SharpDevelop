// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Stepping
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			System.Diagnostics.Debug.WriteLine("1"); // Step over external code
			Sub(); // Step in internal code
			Sub2(); // Step over internal code
		}
		
		public static void Sub()
		{ // Step in noop
			System.Diagnostics.Debug.WriteLine("2"); // Step in external code
			System.Diagnostics.Debug.WriteLine("3"); // Step out
			System.Diagnostics.Debug.WriteLine("4");
		}
		
		public static void Sub2()
		{
			System.Diagnostics.Debug.WriteLine("5");
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Stepping()
		{
			StartTest("Stepping.cs");
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepOver(); // Debugger.Break
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepOver(); // Debug.WriteLine 1
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepInto(); // Method Sub
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepInto(); // '{'
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepInto(); // Debug.WriteLine 2
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepOut(); // Method Sub
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepOver(); // Method Sub
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepOver(); // Method Sub2
			WaitForPause();
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
  <Test name="Stepping.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">Stepping.exe</ModuleLoaded>
    <ModuleLoaded symbols="False">System.dll</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Main</Name>
        <FullName>Debugger.Tests.TestPrograms.Stepping.Main</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Stepping.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.Stepping</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=16,4 End=16,40</NextStatement>
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
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Main</Name>
        <FullName>Debugger.Tests.TestPrograms.Stepping.Main</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Stepping.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.Stepping</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=17,4 End=17,44</NextStatement>
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
    <ModuleLoaded symbols="False">System.Configuration.dll</ModuleLoaded>
    <ModuleLoaded symbols="False">System.Xml.dll</ModuleLoaded>
    <LogMessage>1\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Main</Name>
        <FullName>Debugger.Tests.TestPrograms.Stepping.Main</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Stepping.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.Stepping</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=18,4 End=18,10</NextStatement>
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
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Sub</Name>
        <FullName>Debugger.Tests.TestPrograms.Stepping.Sub</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Stepping.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.Stepping</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=23,3 End=23,4</NextStatement>
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
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Sub</Name>
        <FullName>Debugger.Tests.TestPrograms.Stepping.Sub</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Stepping.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.Stepping</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=24,4 End=24,44</NextStatement>
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
    <LogMessage>2\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Sub</Name>
        <FullName>Debugger.Tests.TestPrograms.Stepping.Sub</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Stepping.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.Stepping</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=25,4 End=25,44</NextStatement>
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
    <LogMessage>3\r\n</LogMessage>
    <LogMessage>4\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Main</Name>
        <FullName>Debugger.Tests.TestPrograms.Stepping.Main</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Stepping.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.Stepping</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=18,4 End=18,10</NextStatement>
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
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Main</Name>
        <FullName>Debugger.Tests.TestPrograms.Stepping.Main</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Stepping.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.Stepping</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=19,4 End=19,11</NextStatement>
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
    <LogMessage>5\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Main</Name>
        <FullName>Debugger.Tests.TestPrograms.Stepping.Main</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>Stepping.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.Stepping</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=20,3 End=20,4</NextStatement>
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