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
	public class Value_Tests
	{
		public static void Main()
		{
			bool b = true;
			int i = 5;
			string s = "five";
			double d = 5.5;
			int[,] array = new int[2,2];
			array[0,0] = 0;
			array[0,1] = 1;
			array[1,0] = 2;
			array[1,1] = 3;
			char[,] lbArray = (char[,])Array.CreateInstance(typeof(char), new int[] { 2, 2 }, new int[] { 10, 20 });
			lbArray[10, 20] = 'a';
			lbArray[10, 21] = 'b';
			lbArray[11, 20] = 'c';
			lbArray[11, 21] = 'd';
			
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Value_Tests()
		{
			StartTest();
			
			DumpLocalVariables();
			
			Value array = this.CurrentStackFrame.GetLocalVariableValue("array").GetPermanentReference(this.EvalThread);
			ObjectDump("array.Length", array.GetPropertyValue(this.EvalThread, "Length"));
			ObjectDump("array", array);
			
			Value lbArray = this.CurrentStackFrame.GetLocalVariableValue("lbArray").GetPermanentReference(this.EvalThread);
			ObjectDump("lbArray", lbArray);
			ObjectDump("lbArray-10-20", lbArray.GetArrayElement(new uint[] {10, 20}));
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Value_Tests.cs">
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Value_Tests.exe (Has symbols)</ModuleLoaded>
    <Paused>Value_Tests.cs:42,4-42,40</Paused>
    <LocalVariables>
      <Item>
        <LocalVariable
          Name="b"
          Type="System.Boolean"
          Value="True" />
      </Item>
      <Item>
        <LocalVariable
          Name="i"
          Type="System.Int32"
          Value="5" />
      </Item>
      <Item>
        <LocalVariable
          Name="s"
          Type="System.String"
          Value="five" />
      </Item>
      <Item>
        <LocalVariable
          Name="d"
          Type="System.Double"
          Value="5.5" />
      </Item>
      <Item>
        <LocalVariable
          Name="array"
          Type="System.Int32[,]"
          Value="{System.Int32[,]}" />
      </Item>
      <Item>
        <LocalVariable
          Name="lbArray"
          Type="System.Char[,]"
          Value="{System.Char[,]}" />
      </Item>
    </LocalVariables>
    <array.Length>
      <Value
        PrimitiveValue="4"
        Type="System.Int32" />
    </array.Length>
    <array>
      <Value
        ArrayBaseIndicies="{0, 0}"
        ArrayDimensions="{2, 2}"
        ArrayLength="4"
        ArrayRank="2"
        IsReference="True"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="System.Int32[,]" />
    </array>
    <lbArray>
      <Value
        ArrayBaseIndicies="{10, 20}"
        ArrayDimensions="{2, 2}"
        ArrayLength="4"
        ArrayRank="2"
        IsReference="True"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="System.Char[,]" />
    </lbArray>
    <lbArray-10-20>
      <Value
        PrimitiveValue="a"
        Type="System.Char" />
    </lbArray-10-20>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
