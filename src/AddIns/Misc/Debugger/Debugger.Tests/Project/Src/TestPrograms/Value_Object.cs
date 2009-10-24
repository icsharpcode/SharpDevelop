// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class BaseClass2
	{
		public string basePublic = "a";
		string basePrivate = "b";
	}
	
	public class Value_Object: BaseClass2
	{
		string privateField = "c";
		public string publicFiled = "d";
		
		public string PublicProperty {
			get {
				return privateField;
			}
		}
		
		public static void Main()
		{
			Value_Object val = new Value_Object();
			System.Diagnostics.Debugger.Break();
			val.privateField = "new private";
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using Debugger.MetaData;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Value_Object()
		{
			Value val = null;
			
			StartTest("Value_Object.cs");
			
			val = process.SelectedStackFrame.GetLocalVariableValue("val");
			ObjectDump("val", val);
			ObjectDump("val members", val.GetMemberValues());
			
			process.Continue();
			val = process.SelectedStackFrame.GetLocalVariableValue("val");
			ObjectDump("val", val);
			ObjectDump("val members", val.GetMemberValues());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Value_Object.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Value_Object.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Value_Object.cs:32,4-32,40</DebuggingPaused>
    <val>
      <Value
        AsString="{Debugger.Tests.TestPrograms.Value_Object}"
        IsReference="True"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="Debugger.Tests.TestPrograms.Value_Object" />
    </val>
    <val_members>
      <Item>
        <Value
          AsString="c"
          IsReference="True"
          PrimitiveValue="c"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="d"
          IsReference="True"
          PrimitiveValue="d"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="a"
          IsReference="True"
          PrimitiveValue="a"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="b"
          IsReference="True"
          PrimitiveValue="b"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="c"
          IsReference="True"
          PrimitiveValue="c"
          Type="System.String" />
      </Item>
    </val_members>
    <DebuggingPaused>Break Value_Object.cs:34,4-34,40</DebuggingPaused>
    <val>
      <Value
        AsString="{Debugger.Tests.TestPrograms.Value_Object}"
        IsReference="True"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="Debugger.Tests.TestPrograms.Value_Object" />
    </val>
    <val_members>
      <Item>
        <Value
          AsString="new private"
          IsReference="True"
          PrimitiveValue="new private"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="d"
          IsReference="True"
          PrimitiveValue="d"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="a"
          IsReference="True"
          PrimitiveValue="a"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="b"
          IsReference="True"
          PrimitiveValue="b"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="new private"
          IsReference="True"
          PrimitiveValue="new private"
          Type="System.String" />
      </Item>
    </val_members>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT