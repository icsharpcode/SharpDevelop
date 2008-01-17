// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class FunctionVariablesLifetime
	{
		public int @class = 3;
		
		public static void Main()
		{
			new FunctionVariablesLifetime().Function(1);
			System.Diagnostics.Debugger.Break(); // 5
		}
		
		void Function(int argument)
		{
			int local = 2;
			System.Diagnostics.Debugger.Break(); // 1
			SubFunction();
			System.Diagnostics.Debugger.Break(); // 3
			SubFunction();
		}
		
		void SubFunction()
		{
			int localInSubFunction = 4;
			System.Diagnostics.Debugger.Break(); // 2, 4
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void FunctionVariablesLifetime()
		{
			Value argument = null;
			Value local    = null;
			Value localInSubFunction = null;
			Value @class   = null;
			
			StartTest("FunctionVariablesLifetime.cs"); // 1 - Enter program
			WaitForPause();
			argument = process.SelectedStackFrame.GetArgumentValue(0);
			local = process.SelectedStackFrame.GetLocalVariableValue("local");
			@class = process.SelectedStackFrame.GetThisValue().GetMemberValue("class");
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			
			process.Continue(); // 2 - Go to the SubFunction
			WaitForPause();
			localInSubFunction = process.SelectedStackFrame.GetLocalVariableValue("localInSubFunction");
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.Continue(); // 3 - Go back to Function
			WaitForPause();
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.Continue(); // 4 - Go to the SubFunction
			WaitForPause();
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			localInSubFunction = process.SelectedStackFrame.GetLocalVariableValue("localInSubFunction");
			ObjectDump("localInSubFunction(new)", @localInSubFunction);
			
			process.Continue(); // 5 - Setp out of both functions
			WaitForPause();
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
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
  <Test name="FunctionVariablesLifetime.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">FunctionVariablesLifetime.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument Type="Value" ToString="argument = 1">
      <IsArray>False</IsArray>
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <ArrayDimensions exception="Value is not an array" />
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <IsInteger>True</IsInteger>
      <PrimitiveValue>1</PrimitiveValue>
      <Expression>argument</Expression>
      <IsNull>False</IsNull>
      <AsString>1</AsString>
      <HasExpired>False</HasExpired>
      <Type>System.Int32</Type>
    </argument>
    <local Type="Value" ToString="local = 2">
      <IsArray>False</IsArray>
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <ArrayDimensions exception="Value is not an array" />
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <IsInteger>True</IsInteger>
      <PrimitiveValue>2</PrimitiveValue>
      <Expression>local</Expression>
      <IsNull>False</IsNull>
      <AsString>2</AsString>
      <HasExpired>False</HasExpired>
      <Type>System.Int32</Type>
    </local>
    <_x0040_class Type="Value" ToString="this.class = 3">
      <IsArray>False</IsArray>
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <ArrayDimensions exception="Value is not an array" />
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <IsInteger>True</IsInteger>
      <PrimitiveValue>3</PrimitiveValue>
      <Expression>this.class</Expression>
      <IsNull>False</IsNull>
      <AsString>3</AsString>
      <HasExpired>False</HasExpired>
      <Type>System.Int32</Type>
    </_x0040_class>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>argument</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </argument>
    <local Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>local</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </local>
    <_x0040_class Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>this.class</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </_x0040_class>
    <localInSubFunction Type="Value" ToString="localInSubFunction = 4">
      <IsArray>False</IsArray>
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <ArrayDimensions exception="Value is not an array" />
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <IsInteger>True</IsInteger>
      <PrimitiveValue>4</PrimitiveValue>
      <Expression>localInSubFunction</Expression>
      <IsNull>False</IsNull>
      <AsString>4</AsString>
      <HasExpired>False</HasExpired>
      <Type>System.Int32</Type>
    </localInSubFunction>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>argument</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </argument>
    <local Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>local</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </local>
    <_x0040_class Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>this.class</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </_x0040_class>
    <localInSubFunction Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>localInSubFunction</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </localInSubFunction>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>argument</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </argument>
    <local Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>local</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </local>
    <_x0040_class Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>this.class</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </_x0040_class>
    <localInSubFunction Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>localInSubFunction</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </localInSubFunction>
    <localInSubFunction_x0028_new_x0029_ Type="Value" ToString="localInSubFunction = 4">
      <IsArray>False</IsArray>
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <ArrayDimensions exception="Value is not an array" />
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <IsInteger>True</IsInteger>
      <PrimitiveValue>4</PrimitiveValue>
      <Expression>localInSubFunction</Expression>
      <IsNull>False</IsNull>
      <AsString>4</AsString>
      <HasExpired>False</HasExpired>
      <Type>System.Int32</Type>
    </localInSubFunction_x0028_new_x0029_>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>argument</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </argument>
    <local Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>local</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </local>
    <_x0040_class Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>this.class</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </_x0040_class>
    <localInSubFunction Type="Value" ToString_exception="Value has expired">
      <IsArray exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <ArrayDimensions exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Expression>localInSubFunction</Expression>
      <IsNull exception="Value has expired" />
      <AsString exception="Value has expired" />
      <HasExpired>True</HasExpired>
      <Type>System.Int32</Type>
    </localInSubFunction>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT