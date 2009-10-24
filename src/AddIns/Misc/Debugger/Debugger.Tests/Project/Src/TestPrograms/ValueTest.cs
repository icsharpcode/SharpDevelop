// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class ValueTest
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
		public void ValueTest()
		{
			StartTest("ValueTest.cs");
			
			PrintLocalVariables();
			
			Value array = process.SelectedStackFrame.GetLocalVariableValue("array").GetPermanentReference();
			ObjectDump("array.Length", array.GetMemberValue("Length"));
			ObjectDump("array elements", array.GetArrayElements());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="ValueTest.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ValueTest.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ValueTest.cs:26,4-26,40</DebuggingPaused>
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
    <array_elements>
      <Item>
        <Value
          AsString="0"
          PrimitiveValue="0"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="1"
          PrimitiveValue="1"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="2"
          PrimitiveValue="2"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          AsString="3"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
    </array_elements>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT