// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Tests
{
	public class StackFrame_VariablesLifetime
	{
		public int @class = 3;
		
		public static void Main()
		{
			new StackFrame_VariablesLifetime().Function(1);
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
		public void StackFrame_VariablesLifetime()
		{
			Value argument = null;
			Value local    = null;
			Value localInSubFunction = null;
			Value @class   = null;
			
			StartTest(); // 1 - Enter program
			
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
    name="StackFrame_VariablesLifetime.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_VariablesLifetime.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break StackFrame_VariablesLifetime.cs:21,4-21,40</DebuggingPaused>
    <argument>
      <Value
        PrimitiveValue="1"
        Type="System.Int32" />
    </argument>
    <local>
      <Value
        PrimitiveValue="2"
        Type="System.Int32" />
    </local>
    <_x0040_class>
      <Value
        PrimitiveValue="3"
        Type="System.Int32" />
    </_x0040_class>
    <DebuggingPaused>Break StackFrame_VariablesLifetime.cs:30,4-30,40</DebuggingPaused>
    <argument>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </argument>
    <local>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </local>
    <_x0040_class>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </_x0040_class>
    <localInSubFunction>
      <Value
        PrimitiveValue="4"
        Type="System.Int32" />
    </localInSubFunction>
    <DebuggingPaused>Break StackFrame_VariablesLifetime.cs:23,4-23,40</DebuggingPaused>
    <argument>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </argument>
    <local>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </local>
    <_x0040_class>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </_x0040_class>
    <localInSubFunction>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </localInSubFunction>
    <DebuggingPaused>Break StackFrame_VariablesLifetime.cs:30,4-30,40</DebuggingPaused>
    <argument>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </argument>
    <local>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </local>
    <_x0040_class>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </_x0040_class>
    <localInSubFunction>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </localInSubFunction>
    <localInSubFunction_x0028_new_x0029_>
      <Value
        PrimitiveValue="4"
        Type="System.Int32" />
    </localInSubFunction_x0028_new_x0029_>
    <DebuggingPaused>Break StackFrame_VariablesLifetime.cs:15,4-15,40</DebuggingPaused>
    <argument>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </argument>
    <local>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </local>
    <_x0040_class>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </_x0040_class>
    <localInSubFunction>
      <Value
        IsInvalid="True"
        IsReference="{Exception: Value is no longer valid}"
        PrimitiveValue="{Exception: Value is no longer valid}"
        Type="System.Int32" />
    </localInSubFunction>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
