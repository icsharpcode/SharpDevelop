// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
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
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).buckets"
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
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).entries"
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
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).count"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="3"
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).version"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="-1"
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).freeList"
          PrimitiveValue="-1"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="0"
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).freeCount"
          PrimitiveValue="0"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{System.Collections.Generic.GenericEqualityComparer&lt;System.String&gt;}"
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).comparer"
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
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).keys"
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
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).values"
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
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict))._syncRoot"
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
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).m_siInfo"
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
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).Comparer"
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
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).Count"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="{KeyCollection&lt;System.String,System.Int32&gt;}"
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).Keys"
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
          Expression="((System.Collections.Generic.Dictionary&lt;System.String, System.Int32&gt;)(dict)).Values"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="ValueCollection&lt;System.String,System.Int32&gt;" />
      </Item>
    </dict_members>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT