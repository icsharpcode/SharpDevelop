// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
			
			PrintLocalVariables();
			
			Value array = process.SelectedStackFrame.GetLocalVariableValue("array").GetPermanentReference();
			ObjectDump("array.Length", array.GetMemberValue("Length"));
			ObjectDump("array", array);
			
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
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Value_Tests.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Value_Tests.cs:26,4-26,40</DebuggingPaused>
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
    </LocalVariables>
    <array.Length>
      <Value
        AsString="4"
        PrimitiveValue="4"
        Type="System.Int32" />
    </array.Length>
    <array>
      <Value
        ArrayDimensions="{2, 2}"
        ArrayLength="4"
        ArrayRank="2"
        AsString="{System.Int32[,]}"
        GetArrayElements="{0, 1, 2, 3}"
        IsReference="True"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="System.Int32[,]" />
    </array>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT