// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger.Tests
{
	public class ExpressionEvaluator_Tests
	{
		public class BaseClass
		{
			string name = "base name";
			
			public string Name {
				get { return name; }
			}
			
			public static string StaticField = "base static field";
			
			public string Foo(int i)
			{
				return "base Foo - int";
			}
		}
		
		public class DerivedClass: BaseClass
		{
			string name = "derived name";
			
			new public string Name {
				get { return name; }
			}
			
			public char SetterOnlyProperty { set { ; } }
			
			new public static string StaticField = "derived static field";
			
			public static string StaticProperty {
				get {
					return "static property";
				}
			}
			
			public static string StaticMethod()
			{
				return "static method";
			}
			
			new public string Foo(int i)
			{
				return "derived Foo - int";
			}
			
			public string Foo(string s)
			{
				return "derived Foo - string";
			}
		}
		
		string instanceField = "instance field value";
		static string staticField = "static field value";
		
		public static void Main()
		{
			new ExpressionEvaluator_Tests().Fun("function argument");
		}
	
		public void Fun(string arg)
		{
			bool flag = true;
			byte b = 1;
			int i = 4;
			float pi = 3.14f;
			string hi = "hi";
			
			char[] array = "Hello".ToCharArray();
			char[] array2 = "world".ToCharArray();
			char[][] arrays = new char[][] {array, array2};
			List<char> list = new List<char>(array);
			
			DerivedClass myClass = new DerivedClass();
			
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using Debugger.MetaData;
	using System.Reflection;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Ast;
	using ICSharpCode.NRefactory.Visitors;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void ExpressionEvaluator_Tests()
		{
			StartTest();
			
			string input = @"
				b
				i
				pi
				pi - 3
				b + i
				i + b
				b + pi
				pi + b
				hi + pi
				pi + hi
				pi + ' ' + hi
				
				arg
				instanceField
				staticField
				
				(5 + 6) % (1 + 2)
				3 % 2 == 1
				15 & 255
				15 && 255
				b + 3 == i
				b + 4 == i
				true == true
				true == false
				
				array
				arrays
				array[1]
				array[i]
				array[i - 1]
				list
				list[1]
				list[i]
				hi[1]
				'abcd'[2]
				
				list.Add(42); list.Add(52);
				list
				
				i = 10
				-i
				++i
				i++
				i
				i += 1
				i
				~i
				flag
				!flag
			";
			
			input = input.Replace("'", "\"");
			
			foreach(string line in input.Split('\n')) {
				Eval(line.Trim());
			}
			
			// Test member hiding / overloading
			
			Value myClass = process.SelectedStackFrame.GetLocalVariableValue("myClass");
			
			List<Expression> expressions = new List<Expression>();
			foreach(MemberInfo memberInfo in myClass.Type.GetMembers(Debugger.MetaData.DebugType.BindingFlagsAll)) {
				if (memberInfo.Name == typeof(object).Name) continue;
				if (memberInfo.MemberType == MemberTypes.Field || memberInfo.MemberType == MemberTypes.Property)
					expressions.Add(new IdentifierExpression("myClass").AppendMemberReference((IDebugMemberInfo)memberInfo));
			}
			expressions.Add(new IdentifierExpression("myClass").AppendMemberReference((DebugMethodInfo)myClass.Type.GetMethod("StaticMethod")));
			expressions.Add(new IdentifierExpression("myClass").AppendMemberReference((DebugMethodInfo)((DebugType)myClass.Type.BaseType).GetMethod("Foo", new string[] { "i" }), new PrimitiveExpression(1)));
			expressions.Add(new IdentifierExpression("myClass").AppendMemberReference((DebugMethodInfo)myClass.Type.GetMethod("Foo", new string[] { "i" }), new PrimitiveExpression(1)));
			expressions.Add(new IdentifierExpression("myClass").AppendMemberReference((DebugMethodInfo)myClass.Type.GetMethod("Foo", new string[] { "s" }), new PrimitiveExpression("a")));
			
			foreach(Expression expr in expressions) {
				Eval(expr.PrettyPrint());
			}
			
			EndTest();
		}
		
		void Eval(string expr)
		{
			string restultFmted;
			if (string.IsNullOrEmpty(expr)) {
				restultFmted = null;
			} else {
				try {
					Value result = ICSharpCode.NRefactory.Visitors.ExpressionEvaluator.Evaluate(expr, SupportedLanguage.CSharp, process.SelectedStackFrame);
					restultFmted = ICSharpCode.NRefactory.Visitors.ExpressionEvaluator.FormatValue(result);
				} catch (GetValueException e) {
					restultFmted = e.Message;
				}
			}
			if (restultFmted != null) {
				ObjectDump("Eval", " " + expr + " = " + restultFmted + " ");
			} else {
				ObjectDump("Eval", " " + expr);
			}
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="ExpressionEvaluator_Tests.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ExpressionEvaluator_Tests.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ExpressionEvaluator_Tests.cs:88,4-88,40</DebuggingPaused>
    <Eval> </Eval>
    <Eval> b = 1 </Eval>
    <Eval> i = 4 </Eval>
    <Eval> pi = 3.14 </Eval>
    <Eval> pi - 3 = 0.140000104904175 </Eval>
    <Eval> b + i = 5 </Eval>
    <Eval> i + b = 5 </Eval>
    <Eval> b + pi = 4.14000010490417 </Eval>
    <Eval> pi + b = 4.14000010490417 </Eval>
    <Eval> hi + pi = "hi3.14" </Eval>
    <Eval> pi + hi = "3.14hi" </Eval>
    <Eval> pi + " " + hi = "3.14 hi" </Eval>
    <Eval> </Eval>
    <Eval> arg = "function argument" </Eval>
    <Eval> instanceField = "instance field value" </Eval>
    <Eval> staticField = "static field value" </Eval>
    <Eval> </Eval>
    <Eval> (5 + 6) % (1 + 2) = 2 </Eval>
    <Eval> 3 % 2 == 1 = True </Eval>
    <Eval> 15 &amp; 255 = 15 </Eval>
    <Eval> 15 &amp;&amp; 255 = Error evaluating "15 &amp;&amp; 255": Unsupported operator for integers: LogicalAnd </Eval>
    <Eval> b + 3 == i = True </Eval>
    <Eval> b + 4 == i = False </Eval>
    <Eval> true == true = True </Eval>
    <Eval> true == false = False </Eval>
    <Eval> </Eval>
    <Eval> array = Char[] {'H', 'e', 'l', 'l', 'o'} </Eval>
    <Eval> arrays = Char[][] {Char[] {'H', 'e', 'l', 'l', 'o'}, Char[] {'w', 'o', 'r', 'l', 'd'}} </Eval>
    <Eval> array[1] = 'e' </Eval>
    <Eval> array[i] = 'o' </Eval>
    <Eval> array[i - 1] = 'l' </Eval>
    <Eval> list = List`1 {'H', 'e', 'l', 'l', 'o'} </Eval>
    <Eval> list[1] = 'e' </Eval>
    <Eval> list[i] = 'o' </Eval>
    <Eval> hi[1] = 'i' </Eval>
    <Eval> "abcd"[2] = 'c' </Eval>
    <Eval> </Eval>
    <Eval> list.Add(42); list.Add(52);</Eval>
    <Eval> list = List`1 {'H', 'e', 'l', 'l', 'o', '*', '4'} </Eval>
    <Eval> </Eval>
    <Eval> i = 10 = 10 </Eval>
    <Eval> -i = -10 </Eval>
    <Eval> ++i = 11 </Eval>
    <Eval> i++ = 11 </Eval>
    <Eval> i = 12 </Eval>
    <Eval> i += 1 = 13 </Eval>
    <Eval> i = 13 </Eval>
    <Eval> ~i = -14 </Eval>
    <Eval> flag = True </Eval>
    <Eval> !flag = False </Eval>
    <Eval> </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass).name = "derived name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.StaticField = "derived static field" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass).Name = "derived name" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass).SetterOnlyProperty = Error evaluating "((Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass).SetterOnlyProperty": Property does not have a get method </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.StaticProperty = "static property" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass).name = "base name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.BaseClass.StaticField = "base static field" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass).Name = "base name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.StaticMethod() = "static method" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass).Foo((System.Int32)1) = "base Foo - int" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass).Foo((System.Int32)1) = "derived Foo - int" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass).Foo((System.String)"a") = "derived Foo - string" </Eval>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT