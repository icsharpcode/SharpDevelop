// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class BaseClass
	{
		string name = "base name";
		
		public string Name {
			get { return name; }
		}
		
		public string Value = "base value";
		
		public string Foo(int i)
		{
			return "base-int";
		}
	}
	
	public class TestClass: BaseClass
	{
		string name = "derived name";
		
		new public string Name {
			get { return name; }
		}
		
		new public string Value = "derived value";
		
		string field = "field value";
		string[] array = {"one", "two", "three"};
		
		public static void Main()
		{
			new TestClass().Test("argValue");
		}
		
		public void Test(string arg)
		{
			int i = 0;
			string[] array = {"one", "two", "three"};
			string[,] array2 = {{"A","B"},{"C","D"}};
			string BaseClass = "baseClassString";
			
			System.Diagnostics.Debugger.Break();
		}
		
		public void Test(int arg)
		{
			
		}
		
		public void Test(TestClass[] arg)
		{
			
		}
		
		public new string Foo(int i)
		{
			return "deriv-int";
		}
		
		public string Foo(string s)
		{
			return "deriv-string";
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using ICSharpCode.NRefactory.Ast;

	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Expressions()
		{
			StartTest("Expressions.cs");
			
			ObjectDump("Arguments", process.SelectedStackFrame.GetArgumentValues());
			ObjectDump("LocalVariables", process.SelectedStackFrame.GetLocalVariableValues());
			ObjectDump("this", process.SelectedStackFrame.GetThisValue().GetMemberValues());
			ObjectDump("methods", process.SelectedStackFrame.MethodInfo.DeclaringType.GetMethods());
			Value thisVal = process.SelectedStackFrame.GetThisValue().GetPermanentReference();
			
			Expression baseMethod = new ThisReferenceExpression().AppendMemberReference(thisVal.Type.BaseType.GetMethod("Foo", "i"), new PrimitiveExpression(1));
			Expression derivedMethod = new ThisReferenceExpression().AppendMemberReference(thisVal.Type.GetMethod("Foo", "i"), new PrimitiveExpression(1));
			Expression overloadMethod = new ThisReferenceExpression().AppendMemberReference(thisVal.Type.GetMethod("Foo", "s"), new PrimitiveExpression("a"));
			
			ObjectDump("BaseMethod", baseMethod.PrettyPrint());
			ObjectDump("BaseMethod-Eval", baseMethod.Evaluate(process));
			ObjectDump("HiddenMethod", derivedMethod.PrettyPrint());
			ObjectDump("HiddenMethod-Eval", derivedMethod.Evaluate(process));
			ObjectDump("OverloadMethod", overloadMethod.PrettyPrint());
			ObjectDump("OverloadMethod-Eval", overloadMethod.Evaluate(process));
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Expressions.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Expressions.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Expressions.cs:53,4-53,40</DebuggingPaused>
    <Arguments
      Capacity="4"
      Count="1">
      <Item>
        <Value
          AsString="argValue"
          IsReference="True"
          PrimitiveValue="argValue"
          Type="System.String" />
      </Item>
    </Arguments>
    <LocalVariables
      Capacity="4"
      Count="4">
      <Item>
        <Value
          AsString="0"
          PrimitiveValue="0"
          Type="System.Int32" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{3}"
          ArrayLength="3"
          ArrayRank="1"
          AsString="{System.String[]}"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{2, 2}"
          ArrayLength="4"
          ArrayRank="2"
          AsString="{System.String[,]}"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[,]" />
      </Item>
      <Item>
        <Value
          AsString="baseClassString"
          IsReference="True"
          PrimitiveValue="baseClassString"
          Type="System.String" />
      </Item>
    </LocalVariables>
    <this>
      <Item>
        <Value
          AsString="derived name"
          IsReference="True"
          PrimitiveValue="derived name"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="derived value"
          IsReference="True"
          PrimitiveValue="derived value"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="field value"
          IsReference="True"
          PrimitiveValue="field value"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          ArrayDimensions="{3}"
          ArrayLength="3"
          ArrayRank="1"
          AsString="{System.String[]}"
          IsReference="True"
          PrimitiveValue="{Exception: Value is not a primitive type}"
          Type="System.String[]" />
      </Item>
      <Item>
        <Value
          AsString="base name"
          IsReference="True"
          PrimitiveValue="base name"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="base value"
          IsReference="True"
          PrimitiveValue="base value"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="derived name"
          IsReference="True"
          PrimitiveValue="derived name"
          Type="System.String" />
      </Item>
      <Item>
        <Value
          AsString="base name"
          IsReference="True"
          PrimitiveValue="base name"
          Type="System.String" />
      </Item>
    </this>
    <methods
      Capacity="8"
      Count="8">
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.get_Name"
          IsPublic="True"
          IsSpecialName="True"
          Module="Expressions.exe"
          Name="get_Name"
          ReturnType="System.String"
          StepOver="True" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.Main"
          IsPublic="True"
          IsStatic="True"
          Module="Expressions.exe"
          Name="Main" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.Test"
          IsPublic="True"
          LocalVariableNames="{array, array2, BaseClass, i}"
          Module="Expressions.exe"
          Name="Test"
          ParameterCount="1"
          ParameterTypes="{System.String}" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.Test"
          IsPublic="True"
          Module="Expressions.exe"
          Name="Test"
          ParameterCount="1"
          ParameterTypes="{System.Int32}" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.Test"
          IsPublic="True"
          Module="Expressions.exe"
          Name="Test"
          ParameterCount="1"
          ParameterTypes="{Debugger.Tests.TestPrograms.TestClass[]}" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.Foo"
          IsPublic="True"
          Module="Expressions.exe"
          Name="Foo"
          ParameterCount="1"
          ParameterTypes="{System.Int32}"
          ReturnType="System.String" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass.Foo"
          IsPublic="True"
          Module="Expressions.exe"
          Name="Foo"
          ParameterCount="1"
          ParameterTypes="{System.String}"
          ReturnType="System.String" />
      </Item>
      <Item>
        <MethodInfo
          DeclaringType="Debugger.Tests.TestPrograms.TestClass"
          FullName="Debugger.Tests.TestPrograms.TestClass..ctor"
          IsPublic="True"
          IsSpecialName="True"
          Module="Expressions.exe"
          Name=".ctor" />
      </Item>
    </methods>
    <BaseMethod>((Debugger.Tests.TestPrograms.BaseClass)(this)).Foo((System.Int32)(1))</BaseMethod>
    <BaseMethod-Eval>
      <Value
        AsString="base-int"
        IsReference="True"
        PrimitiveValue="base-int"
        Type="System.String" />
    </BaseMethod-Eval>
    <HiddenMethod>((Debugger.Tests.TestPrograms.TestClass)(this)).Foo((System.Int32)(1))</HiddenMethod>
    <HiddenMethod-Eval>
      <Value
        AsString="deriv-int"
        IsReference="True"
        PrimitiveValue="deriv-int"
        Type="System.String" />
    </HiddenMethod-Eval>
    <OverloadMethod>((Debugger.Tests.TestPrograms.TestClass)(this)).Foo((System.String)("a"))</OverloadMethod>
    <OverloadMethod-Eval>
      <Value
        AsString="deriv-string"
        IsReference="True"
        PrimitiveValue="deriv-string"
        Type="System.String" />
    </OverloadMethod-Eval>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT