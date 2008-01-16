// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class FunctionArgumentVariables
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			int ref_i = 2;
			int out_i;
			int out_i2;
			string ref_s = "B";
			int? iNull = 5;
			int? iNull_null = null;
			StaticFunction(
				1,
				"A",
				null,
				ref ref_i,
				out out_i,
				out out_i2,
				ref ref_s,
				iNull,
				iNull_null
			);
			VarArgs();
			VarArgs("A");
			VarArgs("A", "B");
			new FunctionArgumentVariables().Function(1, "A");
		}
		
		static void StaticFunction(int i,
		                           string s,
		                           string s_null,
		                           ref int ref_i,
		                           out int out_i,
		                           out int out_i2,
		                           ref string ref_s,
		                           int? iNull,
		                           int? iNull_null)
		{
			out_i = 3;
			System.Diagnostics.Debugger.Break();
			out_i2 = 4;
		}
		
		static void VarArgs(params string[] args)
		{
			System.Diagnostics.Debugger.Break();
		}
		
		void Function(int i, string s)
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
		public void FunctionArgumentVariables()
		{
			StartTest("FunctionArgumentVariables.cs");
			WaitForPause();
			
			for(int i = 0; i < 5; i++) {
				process.Continue();
				WaitForPause();
				ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			}
			
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
  <Test name="FunctionArgumentVariables.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">FunctionArgumentVariables.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo>StaticFunction</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=51,4 End=51,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>9</ArgumentCount>
      <Arguments>[ValueCollection Count=9]</Arguments>
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo>VarArgs</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=57,4 End=57,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>1</ArgumentCount>
      <Arguments>[ValueCollection Count=1]</Arguments>
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo>VarArgs</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=57,4 End=57,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>1</ArgumentCount>
      <Arguments>[ValueCollection Count=1]</Arguments>
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo>VarArgs</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=57,4 End=57,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>1</ArgumentCount>
      <Arguments>[ValueCollection Count=1]</Arguments>
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </ObjectDump>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo>Function</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=62,4 End=62,40</NextStatement>
      <ThisValue>Debugger.Value</ThisValue>
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>2</ArgumentCount>
      <Arguments>[ValueCollection Count=2]</Arguments>
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </ObjectDump>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT