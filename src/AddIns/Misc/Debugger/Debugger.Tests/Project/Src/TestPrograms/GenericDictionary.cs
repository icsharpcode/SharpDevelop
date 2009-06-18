// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger.Tests.TestPrograms
{
	public class GenericDictionary
	{
		public static void Main()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			dict.Add("one",1);
			dict.Add("two",2);
			dict.Add("three",3);
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
		public void GenericDictionary()
		{
			StartTest("GenericDictionary.cs");
			
			ObjectDump("dict", process.SelectedStackFrame.GetLocalVariableValue("dict"));
			ObjectDump("dict members", process.SelectedStackFrame.GetLocalVariableValue("dict").GetMemberValues());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="GenericDictionary.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>GenericDictionary.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break GenericDictionary.cs:21,4-21,40</DebuggingPaused>
    <dict>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLength="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="{System.Collections.Generic.Dictionary&lt;System.String,System.Int32&gt;}"
        Expression="dict"
        IsInvalid="False"
        IsNull="False"
        IsReference="True"
        PrimitiveValue="{Exception: Value is not a primitive type}"
        Type="System.Collections.Generic.Dictionary&lt;System.String,System.Int32&gt;" />
    </dict>
    <dict_members>
      <Item>
        <Value
          ArrayDimensions="{3}"
          ArrayLength="3"
          ArrayRank="1"
          AsString="{System.Int32[]}"
          Expression="dict.buckets"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Int32[]" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{3}"
          ArrayLength="3"
          ArrayRank="1"
          AsString="{Entry&lt;System.String,System.Int32&gt;[]}"
          Expression="dict.entries"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="Entry&lt;System.String,System.Int32&gt;[]" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="3"
          Expression="dict.count"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="3"
          Expression="dict.version"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="-1"
          Expression="dict.freeList"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="-1"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0"
          Expression="dict.freeCount"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="0"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Collections.Generic.GenericEqualityComparer&lt;System.String&gt;}"
          Expression="dict.comparer"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Collections.Generic.GenericEqualityComparer&lt;System.String&gt;" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is null}"
          ArrayLength="{Exception: Value is null}"
          ArrayRank="{Exception: Value is null}"
          AsString="null"
          Expression="dict.keys"
          IsInvalid="False"
          IsNull="True"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="KeyCollection&lt;System.String,System.Int32&gt;" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is null}"
          ArrayLength="{Exception: Value is null}"
          ArrayRank="{Exception: Value is null}"
          AsString="null"
          Expression="dict.values"
          IsInvalid="False"
          IsNull="True"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="ValueCollection&lt;System.String,System.Int32&gt;" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is null}"
          ArrayLength="{Exception: Value is null}"
          ArrayRank="{Exception: Value is null}"
          AsString="null"
          Expression="dict._syncRoot"
          IsInvalid="False"
          IsNull="True"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Object" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is null}"
          ArrayLength="{Exception: Value is null}"
          ArrayRank="{Exception: Value is null}"
          AsString="null"
          Expression="dict.m_siInfo"
          IsInvalid="False"
          IsNull="True"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Runtime.Serialization.SerializationInfo" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Collections.Generic.GenericEqualityComparer&lt;System.String&gt;}"
          Expression="dict.Comparer"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.Collections.Generic.GenericEqualityComparer&lt;System.String&gt;" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="3"
          Expression="dict.Count"
          IsInvalid="False"
          IsNull="False"
          IsReference="False"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{KeyCollection&lt;System.String,System.Int32&gt;}"
          Expression="dict.Keys"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="KeyCollection&lt;System.String,System.Int32&gt;" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{ValueCollection&lt;System.String,System.Int32&gt;}"
          Expression="dict.Values"
          IsInvalid="False"
          IsNull="False"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="ValueCollection&lt;System.String,System.Int32&gt;" />
      </Item>
    </dict_members>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT