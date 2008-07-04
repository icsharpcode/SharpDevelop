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
			
			argument = process.SelectedStackFrame.GetArgumentValue(0);
			local = process.SelectedStackFrame.GetLocalVariableValue("local");
			@class = process.SelectedStackFrame.GetThisValue().GetMemberValue("class");
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			
			process.Continue(); // 2 - Go to the SubFunction
			localInSubFunction = process.SelectedStackFrame.GetLocalVariableValue("localInSubFunction");
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.Continue(); // 3 - Go back to Function
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			process.Continue(); // 4 - Go to the SubFunction
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			localInSubFunction = process.SelectedStackFrame.GetLocalVariableValue("localInSubFunction");
			ObjectDump("localInSubFunction(new)", @localInSubFunction);
			
			process.Continue(); // 5 - Setp out of both functions
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			ObjectDump("localInSubFunction", @localInSubFunction);
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="FunctionVariablesLifetime.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>FunctionVariablesLifetime.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLenght="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="1"
        Expression="argument"
        IsArray="False"
        IsInteger="True"
        IsInvalid="False"
        IsNull="False"
        IsObject="False"
        IsPrimitive="True"
        PrimitiveValue="1"
        Type="System.Int32" />
    </argument>
    <local>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLenght="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="2"
        Expression="local"
        IsArray="False"
        IsInteger="True"
        IsInvalid="False"
        IsNull="False"
        IsObject="False"
        IsPrimitive="True"
        PrimitiveValue="2"
        Type="System.Int32" />
    </local>
    <_x0040_class>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLenght="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="3"
        Expression="this.class"
        IsArray="False"
        IsInteger="True"
        IsInvalid="False"
        IsNull="False"
        IsObject="False"
        IsPrimitive="True"
        PrimitiveValue="3"
        Type="System.Int32" />
    </_x0040_class>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="argument"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </argument>
    <local>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="local"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </local>
    <_x0040_class>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="this.class"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </_x0040_class>
    <localInSubFunction>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLenght="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="4"
        Expression="localInSubFunction"
        IsArray="False"
        IsInteger="True"
        IsInvalid="False"
        IsNull="False"
        IsObject="False"
        IsPrimitive="True"
        PrimitiveValue="4"
        Type="System.Int32" />
    </localInSubFunction>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="argument"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </argument>
    <local>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="local"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </local>
    <_x0040_class>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="this.class"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </_x0040_class>
    <localInSubFunction>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="localInSubFunction"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </localInSubFunction>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="argument"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </argument>
    <local>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="local"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </local>
    <_x0040_class>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="this.class"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </_x0040_class>
    <localInSubFunction>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="localInSubFunction"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </localInSubFunction>
    <localInSubFunction_x0028_new_x0029_>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLenght="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="4"
        Expression="localInSubFunction"
        IsArray="False"
        IsInteger="True"
        IsInvalid="False"
        IsNull="False"
        IsObject="False"
        IsPrimitive="True"
        PrimitiveValue="4"
        Type="System.Int32" />
    </localInSubFunction_x0028_new_x0029_>
    <DebuggingPaused>Break</DebuggingPaused>
    <argument>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="argument"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </argument>
    <local>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="local"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </local>
    <_x0040_class>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="this.class"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </_x0040_class>
    <localInSubFunction>
      <Value
        ArrayDimensions="{Exception: Value is no longer valid}"
        ArrayLenght="{Exception: Value is no longer valid}"
        ArrayRank="{Exception: Value is no longer valid}"
        AsString="{Exception: Value is no longer valid}"
        Expression="localInSubFunction"
        IsArray="{Exception: Value is no longer valid}"
        IsInteger="{Exception: Value is no longer valid}"
        IsInvalid="True"
        IsNull="{Exception: Value is no longer valid}"
        IsObject="{Exception: Value is no longer valid}"
        IsPrimitive="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </localInSubFunction>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT