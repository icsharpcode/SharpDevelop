// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class FunctionLocalVariables
	{
		public static void Main()
		{
			int i = 0;
			string s = "S";
			string[] args = new string[] {"p1"};
			object n = null;
			object o = new object();
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void FunctionLocalVariables()
		{
			StartTest("FunctionLocalVariables");
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
  <Test name="FunctionLocalVariables">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">FunctionLocalVariables.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo Type="MethodInfo">
        <Name>Main</Name>
        <FullName>Debugger.Tests.TestPrograms.FunctionLocalVariables.Main</FullName>
        <IsPrivate>False</IsPrivate>
        <IsPublic>True</IsPublic>
        <IsSpecialName>False</IsSpecialName>
        <IsStatic>True</IsStatic>
        <Module>FunctionLocalVariables.exe</Module>
        <DeclaringType>Debugger.Tests.TestPrograms.FunctionLocalVariables</DeclaringType>
      </MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=21,4 End=21,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables Type="ValueCollection">
        <Count>0</Count>
      </ContaingClassVariables>
      <ArgumentCount>0</ArgumentCount>
      <Arguments Type="ValueCollection">
        <Count>0</Count>
      </Arguments>
      <LocalVariables Type="ValueCollection">
        <Count>5</Count>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>True</IsInteger>
          <PrimitiveValue>0</PrimitiveValue>
          <Expression>i</Expression>
          <Name>i</Name>
          <IsNull>False</IsNull>
          <AsString>0</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Int32</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>True</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue>S</PrimitiveValue>
          <Expression>s</Expression>
          <Name>s</Name>
          <IsNull>False</IsNull>
          <AsString>S</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.String</Type>
        </Item>
        <Item Type="Value">
          <IsArray>True</IsArray>
          <ArrayLenght>1</ArrayLenght>
          <ArrayRank>1</ArrayRank>
          <ArrayDimensions>[1]</ArrayDimensions>
          <IsObject>False</IsObject>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue exception="Value is not a primitive type" />
          <Expression>args</Expression>
          <Name>args</Name>
          <IsNull>False</IsNull>
          <AsString>{System.String[]}</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.String[]</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>False</IsObject>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue exception="Value is not a primitive type" />
          <Expression>n</Expression>
          <Name>n</Name>
          <IsNull>True</IsNull>
          <AsString>&lt;null&gt;</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Object</Type>
        </Item>
        <Item Type="Value">
          <IsArray>False</IsArray>
          <ArrayLenght exception="Value is not an array" />
          <ArrayRank exception="Value is not an array" />
          <ArrayDimensions exception="Value is not an array" />
          <IsObject>True</IsObject>
          <IsPrimitive>False</IsPrimitive>
          <IsInteger>False</IsInteger>
          <PrimitiveValue exception="Value is not a primitive type" />
          <Expression>o</Expression>
          <Name>o</Name>
          <IsNull>False</IsNull>
          <AsString>{System.Object}</AsString>
          <HasExpired>False</HasExpired>
          <Type>System.Object</Type>
        </Item>
      </LocalVariables>
    </ObjectDump>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT