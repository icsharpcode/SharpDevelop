// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class MainClass
	{
		public static void Main()
		{
			GenericClass<int, string> gClass = new GenericClass<int, string>();
			gClass.Metod(1, "1!");
			gClass.GenericMethod<bool>(2, "2!");
			GenericClass<int, string>.StaticMetod(3, "3!");
			GenericClass<int, string>.StaticGenericMethod<bool>(4, "4!");
			
			GenericStruct<int, string> gStruct = new GenericStruct<int, string>();
			gStruct.Metod(5, "5!");
			gStruct.GenericMethod<bool>(6, "6!");
			GenericStruct<int, string>.StaticMetod(7, "7!");
			GenericStruct<int, string>.StaticGenericMethod<bool>(8, "8!");
			
			System.Diagnostics.Debugger.Break();
		}
	}
	
	public class GenericClass<V, K>
	{
		public V Prop {
			get {
				return default(V);
			}
		}
		
		public static V StaticProp {
			get {
				return default(V);
			}
		}
		
		public K Metod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public T GenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
		
		public static K StaticMetod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public static T StaticGenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
	}
	
	public struct GenericStruct<V, K>
	{
		public K Metod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public T GenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
		
		public static K StaticMetod(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return k;
		}
		
		public static T StaticGenericMethod<T>(V v, K k)
		{
			System.Diagnostics.Debugger.Break();
			return default(T);
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Generics()
		{
			ExpandProperties(
				"StackFrame.MethodInfo",
				"MemberInfo.DeclaringType"
			);
			StartTest("Generics.cs");
			
			for(int i = 0; i < 8; i++) {
				ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
				ObjectDump("SelectedStackFrame-GetArguments", process.SelectedStackFrame.GetArgumentValues());
				process.Continue();
			}
			ObjectDump("Prop", process.SelectedStackFrame.GetLocalVariableValue("gClass").GetMemberValue("Prop"));
			ObjectDump("StaticProp", process.SelectedStackFrame.GetLocalVariableValue("gClass").GetMemberValue("StaticProp"));
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Generics.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Generics.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Generics.cs:48,4-48,40</DebuggingPaused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="2"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="Metod"
        NextStatement="Generics.cs:48,4-48,40">
        <MethodInfo>
          <MethodInfo
            DeclaringType="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;"
            FullName="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.Metod"
            IsPublic="True"
            Module="Generics.exe"
            Name="Metod"
            ParameterCount="2"
            ParameterTypes="{System.Int32, System.String}"
            ReturnType="System.String">
            <DeclaringType>
              <DebugType
                BaseType="System.Object"
                FullName="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;"
                GenericArguments="{System.Int32, System.String}"
                Kind="Class"
                Module="Generics.exe"
                Name="GenericClass&lt;Int32,String&gt;" />
            </DeclaringType>
          </MethodInfo>
        </MethodInfo>
      </StackFrame>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments
      Capacity="4"
      Count="2">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="1"
          Expression="v"
          PrimitiveValue="1"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="1!"
          Expression="k"
          IsReference="True"
          PrimitiveValue="1!"
          Type="System.String" />
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break Generics.cs:54,4-54,40</DebuggingPaused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="2"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="GenericMethod"
        NextStatement="Generics.cs:54,4-54,40">
        <MethodInfo>
          <MethodInfo
            DeclaringType="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;"
            FullName="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.GenericMethod"
            IsPublic="True"
            Module="Generics.exe"
            Name="GenericMethod"
            ParameterCount="2"
            ParameterTypes="{System.Int32, System.String}"
            ReturnType="System.Object">
            <DeclaringType>
              <DebugType
                BaseType="System.Object"
                FullName="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;"
                GenericArguments="{System.Int32, System.String}"
                Kind="Class"
                Module="Generics.exe"
                Name="GenericClass&lt;Int32,String&gt;" />
            </DeclaringType>
          </MethodInfo>
        </MethodInfo>
      </StackFrame>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments
      Capacity="4"
      Count="2">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="2"
          Expression="v"
          PrimitiveValue="2"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="2!"
          Expression="k"
          IsReference="True"
          PrimitiveValue="2!"
          Type="System.String" />
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break Generics.cs:60,4-60,40</DebuggingPaused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="2"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="StaticMetod"
        NextStatement="Generics.cs:60,4-60,40">
        <MethodInfo>
          <MethodInfo
            DeclaringType="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;"
            FullName="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticMetod"
            IsPublic="True"
            IsStatic="True"
            Module="Generics.exe"
            Name="StaticMetod"
            ParameterCount="2"
            ParameterTypes="{System.Int32, System.String}"
            ReturnType="System.String">
            <DeclaringType>
              <DebugType
                BaseType="System.Object"
                FullName="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;"
                GenericArguments="{System.Int32, System.String}"
                Kind="Class"
                Module="Generics.exe"
                Name="GenericClass&lt;Int32,String&gt;" />
            </DeclaringType>
          </MethodInfo>
        </MethodInfo>
      </StackFrame>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments
      Capacity="4"
      Count="2">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="3"
          Expression="v"
          PrimitiveValue="3"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="3!"
          Expression="k"
          IsReference="True"
          PrimitiveValue="3!"
          Type="System.String" />
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break Generics.cs:66,4-66,40</DebuggingPaused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="2"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="StaticGenericMethod"
        NextStatement="Generics.cs:66,4-66,40">
        <MethodInfo>
          <MethodInfo
            DeclaringType="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;"
            FullName="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticGenericMethod"
            IsPublic="True"
            IsStatic="True"
            Module="Generics.exe"
            Name="StaticGenericMethod"
            ParameterCount="2"
            ParameterTypes="{System.Int32, System.String}"
            ReturnType="System.Object">
            <DeclaringType>
              <DebugType
                BaseType="System.Object"
                FullName="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;"
                GenericArguments="{System.Int32, System.String}"
                Kind="Class"
                Module="Generics.exe"
                Name="GenericClass&lt;Int32,String&gt;" />
            </DeclaringType>
          </MethodInfo>
        </MethodInfo>
      </StackFrame>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments
      Capacity="4"
      Count="2">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="4"
          Expression="v"
          PrimitiveValue="4"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="4!"
          Expression="k"
          IsReference="True"
          PrimitiveValue="4!"
          Type="System.String" />
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break Generics.cs:75,4-75,40</DebuggingPaused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="2"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="Metod"
        NextStatement="Generics.cs:75,4-75,40">
        <MethodInfo>
          <MethodInfo
            DeclaringType="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;"
            FullName="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.Metod"
            IsPublic="True"
            Module="Generics.exe"
            Name="Metod"
            ParameterCount="2"
            ParameterTypes="{System.Int32, System.String}"
            ReturnType="System.String">
            <DeclaringType>
              <DebugType
                BaseType="System.ValueType"
                FullName="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;"
                GenericArguments="{System.Int32, System.String}"
                Kind="ValueType"
                Module="Generics.exe"
                Name="GenericStruct&lt;Int32,String&gt;" />
            </DeclaringType>
          </MethodInfo>
        </MethodInfo>
      </StackFrame>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments
      Capacity="4"
      Count="2">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="5"
          Expression="v"
          PrimitiveValue="5"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="5!"
          Expression="k"
          IsReference="True"
          PrimitiveValue="5!"
          Type="System.String" />
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break Generics.cs:81,4-81,40</DebuggingPaused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="2"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="GenericMethod"
        NextStatement="Generics.cs:81,4-81,40">
        <MethodInfo>
          <MethodInfo
            DeclaringType="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;"
            FullName="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.GenericMethod"
            IsPublic="True"
            Module="Generics.exe"
            Name="GenericMethod"
            ParameterCount="2"
            ParameterTypes="{System.Int32, System.String}"
            ReturnType="System.Object">
            <DeclaringType>
              <DebugType
                BaseType="System.ValueType"
                FullName="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;"
                GenericArguments="{System.Int32, System.String}"
                Kind="ValueType"
                Module="Generics.exe"
                Name="GenericStruct&lt;Int32,String&gt;" />
            </DeclaringType>
          </MethodInfo>
        </MethodInfo>
      </StackFrame>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments
      Capacity="4"
      Count="2">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="6"
          Expression="v"
          PrimitiveValue="6"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="6!"
          Expression="k"
          IsReference="True"
          PrimitiveValue="6!"
          Type="System.String" />
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break Generics.cs:87,4-87,40</DebuggingPaused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="2"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="StaticMetod"
        NextStatement="Generics.cs:87,4-87,40">
        <MethodInfo>
          <MethodInfo
            DeclaringType="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;"
            FullName="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticMetod"
            IsPublic="True"
            IsStatic="True"
            Module="Generics.exe"
            Name="StaticMetod"
            ParameterCount="2"
            ParameterTypes="{System.Int32, System.String}"
            ReturnType="System.String">
            <DeclaringType>
              <DebugType
                BaseType="System.ValueType"
                FullName="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;"
                GenericArguments="{System.Int32, System.String}"
                Kind="ValueType"
                Module="Generics.exe"
                Name="GenericStruct&lt;Int32,String&gt;" />
            </DeclaringType>
          </MethodInfo>
        </MethodInfo>
      </StackFrame>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments
      Capacity="4"
      Count="2">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="7"
          Expression="v"
          PrimitiveValue="7"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="7!"
          Expression="k"
          IsReference="True"
          PrimitiveValue="7!"
          Type="System.String" />
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break Generics.cs:93,4-93,40</DebuggingPaused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="2"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="StaticGenericMethod"
        NextStatement="Generics.cs:93,4-93,40">
        <MethodInfo>
          <MethodInfo
            DeclaringType="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;"
            FullName="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;.StaticGenericMethod"
            IsPublic="True"
            IsStatic="True"
            Module="Generics.exe"
            Name="StaticGenericMethod"
            ParameterCount="2"
            ParameterTypes="{System.Int32, System.String}"
            ReturnType="System.Object">
            <DeclaringType>
              <DebugType
                BaseType="System.ValueType"
                FullName="Debugger.Tests.TestPrograms.GenericStruct&lt;System.Int32,System.String&gt;"
                GenericArguments="{System.Int32, System.String}"
                Kind="ValueType"
                Module="Generics.exe"
                Name="GenericStruct&lt;Int32,String&gt;" />
            </DeclaringType>
          </MethodInfo>
        </MethodInfo>
      </StackFrame>
    </SelectedStackFrame>
    <SelectedStackFrame-GetArguments
      Capacity="4"
      Count="2">
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="8"
          Expression="v"
          PrimitiveValue="8"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{Exception: Value is not an array}"
          ArrayLength="{Exception: Value is not an array}"
          ArrayRank="{Exception: Value is not an array}"
          AsString="8!"
          Expression="k"
          IsReference="True"
          PrimitiveValue="8!"
          Type="System.String" />
      </Item>
    </SelectedStackFrame-GetArguments>
    <DebuggingPaused>Break Generics.cs:28,4-28,40</DebuggingPaused>
    <Prop>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLength="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="0"
        Expression="gClass.Prop"
        PrimitiveValue="0"
        Type="System.Int32" />
    </Prop>
    <StaticProp>
      <Value
        ArrayDimensions="{Exception: Value is not an array}"
        ArrayLength="{Exception: Value is not an array}"
        ArrayRank="{Exception: Value is not an array}"
        AsString="0"
        Expression="Debugger.Tests.TestPrograms.GenericClass&lt;System.Int32,System.String&gt;.StaticProp"
        PrimitiveValue="0"
        Type="System.Int32" />
    </StaticProp>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT