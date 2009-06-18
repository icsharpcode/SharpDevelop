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
	
	public class ObjectValue: BaseClass2
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
			ObjectValue val = new ObjectValue();
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
		public void ObjectValue()
		{
			Value val = null;
			
			StartTest("ObjectValue.cs");
			
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
    name="ObjectValue.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ObjectValue.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ObjectValue.cs:32,4-32,40</DebuggingPaused>
    <val>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLength="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="{Debugger.Tests.TestPrograms.ObjectValue}"
        Expression="val"
        IsInvalid="False"
        IsNull="False"
        IsReference="True"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="Debugger.Tests.TestPrograms.ObjectValue" />
    </val>
    <val_members>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="c"
          Expression="val.privateField"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="c"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="d"
          Expression="val.publicFiled"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="d"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="a"
          Expression="val.basePublic"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="a"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="b"
          Expression="val.basePrivate"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="b"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="c"
          Expression="val.PublicProperty"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="c"
          Type="System.String" />
      </Item>
    </val_members>
    <DebuggingPaused>Break ObjectValue.cs:34,4-34,40</DebuggingPaused>
    <val>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLength="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="{Debugger.Tests.TestPrograms.ObjectValue}"
        Expression="val"
        IsInvalid="False"
        IsNull="False"
        IsReference="True"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="Debugger.Tests.TestPrograms.ObjectValue" />
    </val>
    <val_members>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="new private"
          Expression="val.privateField"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="new private"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="d"
          Expression="val.publicFiled"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="d"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="a"
          Expression="val.basePublic"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="a"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="b"
          Expression="val.basePrivate"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="b"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="new private"
          Expression="val.PublicProperty"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="new private"
          Type="System.String" />
      </Item>
    </val_members>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT