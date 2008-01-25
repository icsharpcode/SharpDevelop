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
			
			ObjectDump("LocalVariables", process.SelectedStackFrame.GetLocalVariableValues());
			
			EndTest();
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
    <LocalVariables Type="Value[]" ToString="Debugger.Value[]">
      <Item Type="Value" ToString="i = 0">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>0</AsString>
        <Expression>i</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>True</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>0</PrimitiveValue>
        <Type>System.Int32</Type>
      </Item>
      <Item Type="Value" ToString="s = S">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>S</AsString>
        <Expression>s</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>True</IsPrimitive>
        <PrimitiveValue>S</PrimitiveValue>
        <Type>System.String</Type>
      </Item>
      <Item Type="Value" ToString="args = {System.String[]}">
        <ArrayDimensions>[1]</ArrayDimensions>
        <ArrayLenght>1</ArrayLenght>
        <ArrayRank>1</ArrayRank>
        <AsString>{System.String[]}</AsString>
        <Expression>args</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>True</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Type>System.String[]</Type>
      </Item>
      <Item Type="Value" ToString="n = null">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>null</AsString>
        <Expression>n</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>True</IsNull>
        <IsObject>False</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Type>System.Object</Type>
      </Item>
      <Item Type="Value" ToString="o = {System.Object}">
        <ArrayDimensions exception="Value is not an array" />
        <ArrayLenght exception="Value is not an array" />
        <ArrayRank exception="Value is not an array" />
        <AsString>{System.Object}</AsString>
        <Expression>o</Expression>
        <HasExpired>False</HasExpired>
        <IsArray>False</IsArray>
        <IsInteger>False</IsInteger>
        <IsNull>False</IsNull>
        <IsObject>True</IsObject>
        <IsPrimitive>False</IsPrimitive>
        <PrimitiveValue exception="Value is not a primitive type" />
        <Type>System.Object</Type>
      </Item>
    </LocalVariables>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT