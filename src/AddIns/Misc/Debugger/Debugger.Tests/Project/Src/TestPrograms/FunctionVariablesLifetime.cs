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
			
			process.AsyncContinue(); // 2 - Go to the SubFunction
			WaitForPause();
			localInSubFunction = process.SelectedStackFrame.GetLocalVariableValue("localInSubFunction");
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.AsyncContinue(); // 3 - Go back to Function
			WaitForPause();
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.AsyncContinue(); // 4 - Go to the SubFunction
			WaitForPause();
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			localInSubFunction = process.SelectedStackFrame.GetLocalVariableValue("localInSubFunction");
			ObjectDump("localInSubFunction(new)", @localInSubFunction);
			
			process.AsyncContinue(); // 5 - Setp out of both functions
			WaitForPause();
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
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
  <Test name="FunctionVariablesLifetime.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">FunctionVariablesLifetime.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument Type="Value" ToString="argument = 1">
      <ArrayDimensions exception="Value is not an array" />
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <AsString>1</AsString>
      <Expression>argument</Expression>
      <HasExpired>False</HasExpired>
      <IsArray>False</IsArray>
      <IsInteger>True</IsInteger>
      <IsNull>False</IsNull>
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <PrimitiveValue>1</PrimitiveValue>
      <Type>System.Int32</Type>
    </argument>
    <local Type="Value" ToString="local = 2">
      <ArrayDimensions exception="Value is not an array" />
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <AsString>2</AsString>
      <Expression>local</Expression>
      <HasExpired>False</HasExpired>
      <IsArray>False</IsArray>
      <IsInteger>True</IsInteger>
      <IsNull>False</IsNull>
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <PrimitiveValue>2</PrimitiveValue>
      <Type>System.Int32</Type>
    </local>
    <_x0040_class Type="Value" ToString="this.class = 3">
      <ArrayDimensions exception="Value is not an array" />
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <AsString>3</AsString>
      <Expression>this.class</Expression>
      <HasExpired>False</HasExpired>
      <IsArray>False</IsArray>
      <IsInteger>True</IsInteger>
      <IsNull>False</IsNull>
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <PrimitiveValue>3</PrimitiveValue>
      <Type>System.Int32</Type>
    </_x0040_class>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>argument</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </argument>
    <local Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>local</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </local>
    <_x0040_class Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>this.class</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </_x0040_class>
    <localInSubFunction Type="Value" ToString="localInSubFunction = 4">
      <ArrayDimensions exception="Value is not an array" />
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <AsString>4</AsString>
      <Expression>localInSubFunction</Expression>
      <HasExpired>False</HasExpired>
      <IsArray>False</IsArray>
      <IsInteger>True</IsInteger>
      <IsNull>False</IsNull>
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <PrimitiveValue>4</PrimitiveValue>
      <Type>System.Int32</Type>
    </localInSubFunction>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>argument</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </argument>
    <local Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>local</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </local>
    <_x0040_class Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>this.class</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </_x0040_class>
    <localInSubFunction Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>localInSubFunction</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </localInSubFunction>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>argument</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </argument>
    <local Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>local</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </local>
    <_x0040_class Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>this.class</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </_x0040_class>
    <localInSubFunction Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>localInSubFunction</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </localInSubFunction>
    <localInSubFunction_x0028_new_x0029_ Type="Value" ToString="localInSubFunction = 4">
      <ArrayDimensions exception="Value is not an array" />
      <ArrayLenght exception="Value is not an array" />
      <ArrayRank exception="Value is not an array" />
      <AsString>4</AsString>
      <Expression>localInSubFunction</Expression>
      <HasExpired>False</HasExpired>
      <IsArray>False</IsArray>
      <IsInteger>True</IsInteger>
      <IsNull>False</IsNull>
      <IsObject>False</IsObject>
      <IsPrimitive>True</IsPrimitive>
      <PrimitiveValue>4</PrimitiveValue>
      <Type>System.Int32</Type>
    </localInSubFunction_x0028_new_x0029_>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>argument</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </argument>
    <local Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>local</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </local>
    <_x0040_class Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>this.class</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </_x0040_class>
    <localInSubFunction Type="Value" ToString_exception="Value has expired">
      <ArrayDimensions exception="Value has expired" />
      <ArrayLenght exception="Value has expired" />
      <ArrayRank exception="Value has expired" />
      <AsString exception="Value has expired" />
      <Expression>localInSubFunction</Expression>
      <HasExpired>True</HasExpired>
      <IsArray exception="Value has expired" />
      <IsInteger exception="Value has expired" />
      <IsNull exception="Value has expired" />
      <IsObject exception="Value has expired" />
      <IsPrimitive exception="Value has expired" />
      <PrimitiveValue exception="Value has expired" />
      <Type>System.Int32</Type>
    </localInSubFunction>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT