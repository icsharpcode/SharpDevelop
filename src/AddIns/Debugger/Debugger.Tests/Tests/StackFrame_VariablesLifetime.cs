// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			
			argument = this.CurrentStackFrame.GetArgumentValue(0);
			local = this.CurrentStackFrame.GetLocalVariableValue("local");
			@class = this.CurrentStackFrame.GetThisValue(false).GetFieldValue("class");
			ObjectDump("argument", argument);
			ObjectDump("local", local);
			ObjectDump("@class", @class);
			
			process.Continue(); // 2 - Go to the SubFunction
			localInSubFunction = this.CurrentStackFrame.GetLocalVariableValue("localInSubFunction");
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
			localInSubFunction = this.CurrentStackFrame.GetLocalVariableValue("localInSubFunction");
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
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_VariablesLifetime.exe (Has symbols)</ModuleLoaded>
    <Paused>StackFrame_VariablesLifetime.cs:36,4-36,40</Paused>
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
    <Paused>StackFrame_VariablesLifetime.cs:45,4-45,40</Paused>
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
    <Paused>StackFrame_VariablesLifetime.cs:38,4-38,40</Paused>
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
    <Paused>StackFrame_VariablesLifetime.cs:45,4-45,40</Paused>
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
    <Paused>StackFrame_VariablesLifetime.cs:30,4-30,40</Paused>
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
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
