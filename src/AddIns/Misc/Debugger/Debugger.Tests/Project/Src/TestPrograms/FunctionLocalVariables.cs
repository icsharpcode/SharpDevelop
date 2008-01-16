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
			StartTest("FunctionLocalVariables.cs");
			WaitForPause();
			ObjectDump("LocalVariables", process.SelectedStackFrame.LocalVariables);
			
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
  <Test name="FunctionLocalVariables.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">FunctionLocalVariables.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <LocalVariables Type="ValueCollection" ToString="[ValueCollection Count=5]">
      <Count>5</Count>
      <Item Type="Value" ToString="i = 0">
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
      <Item Type="Value" ToString="s = S">
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
      <Item Type="Value" ToString="args = {System.String[]}">
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
      <Item Type="Value" ToString="n = &lt;null&gt;">
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
      <Item Type="Value" ToString="o = {System.Object}">
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
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT