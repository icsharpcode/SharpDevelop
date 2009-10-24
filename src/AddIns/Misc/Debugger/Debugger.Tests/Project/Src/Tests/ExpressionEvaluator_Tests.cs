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
			
			public string Foo(double d)
			{
				return "base Foo - double";
			}
			
			public string this[long i] {
				get {
					return "base indexer - long";
				}
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
			public const int ConstInt = 42;
			public const string ConstString = "const string";
			public const object ConstNull = null;
			public const MyEnum ConstEnum = MyEnum.B;
			
			public static string StaticProperty {
				get {
					return "static property";
				}
			}
			
			public static string StaticMethod()
			{
				return "static method";
			}
			
			public string Foo(object o)
			{
				return "derived Foo - object";
			}
			
			new public string Foo(int i)
			{
				return "derived Foo - int";
			}
			
			public string Foo(string s)
			{
				return "derived Foo - string";
			}
			
			public string this[double d] {
				get {
					return "derived indexer - double";
				}
			}
			
			public string this[string s] {
				get {
					return "derived indexer - string";
				}
			}
			
			public string Convert(string s, double d)
			{
				return "converted to " + s + " and " + d;
			}
		}
		
		public enum MyEnum { A = 3, B = 6 };
		
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
			foreach(MemberInfo memberInfo in myClass.Type.GetFieldsAndNonIndexedProperties(DebugType.BindingFlagsAll)) {
				expressions.Add(new IdentifierExpression("myClass").AppendMemberReference((IDebugMemberInfo)memberInfo));
			}
			expressions.Add(new IdentifierExpression("myClass").AppendMemberReference((DebugMethodInfo)myClass.Type.GetMethod("StaticMethod")));
			expressions.Add(new IdentifierExpression("myClass").AppendMemberReference((DebugMethodInfo)((DebugType)myClass.Type.BaseType).GetMethod("Foo", new string[] { "i" }), new PrimitiveExpression(1)));
			expressions.Add(new IdentifierExpression("myClass").AppendMemberReference((DebugMethodInfo)myClass.Type.GetMethod("Foo", new string[] { "i" }), new PrimitiveExpression(1)));
			expressions.Add(new IdentifierExpression("myClass").AppendMemberReference((DebugMethodInfo)myClass.Type.GetMethod("Foo", new string[] { "s" }), new PrimitiveExpression("a")));
			
			foreach(Expression expr in expressions) {
				Eval(expr.PrettyPrint());
			}
			
			string input2 = @"
				myClass.Foo(1.0)
				myClass.Foo(myClass)
				myClass.Foo(1)
				myClass.Foo('abc')
				myClass[1]
				myClass[(long)1]
				myClass[1.0]
				myClass['abc']
				myClass.Convert(1, 2)
				myClass.Convert('abc', 2)
			";
			
			input2 = input2.Replace("'", "\"");
			foreach(string line in input2.Split('\n')) {
				Eval(line.Trim());
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
    <DebuggingPaused>Break ExpressionEvaluator_Tests.cs:127,4-127,40</DebuggingPaused>
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
    <Eval> list.Add(42); list.Add(52); = Error evaluating "list.Add(42);
list.Add(52);
": Incorrect parameter type for 'item'. Excpeted System.Char, seen System.Int32 </Eval>
    <Eval> list = List`1 {'H', 'e', 'l', 'l', 'o'} </Eval>
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
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.ConstInt = 42 </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.ConstString = "const string" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.ConstNull = null </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.ConstEnum = B </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass).name = "derived name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.StaticField = "derived static field" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass).Name = "derived name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.StaticProperty = "static property" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass).name = "base name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.BaseClass.StaticField = "base static field" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass).Name = "base name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.StaticMethod() = "static method" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass).Foo((System.Int32)1) = "base Foo - int" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass).Foo((System.Int32)1) = "derived Foo - int" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass).Foo((System.String)"a") = "derived Foo - string" </Eval>
    <Eval> </Eval>
    <Eval> myClass.Foo(1.0) = "base Foo - double" </Eval>
    <Eval> myClass.Foo(myClass) = "derived Foo - object" </Eval>
    <Eval> myClass.Foo(1) = "derived Foo - int" </Eval>
    <Eval> myClass.Foo("abc") = "derived Foo - string" </Eval>
    <Eval> myClass[1] = Error evaluating "myClass[1]": More then one applicable overload found:
  System.String Item[Double d]
  System.String Item[Int64 i] </Eval>
    <Eval> myClass[(long)1] = "base indexer - long" </Eval>
    <Eval> myClass[1.0] = "derived indexer - double" </Eval>
    <Eval> myClass["abc"] = "derived indexer - string" </Eval>
    <Eval> myClass.Convert(1, 2) = Error evaluating "myClass.Convert(1, 2)": Incorrect parameter type for 's'. Excpeted System.String, seen System.Int32 </Eval>
    <Eval> myClass.Convert("abc", 2) = "converted to abc and 2" </Eval>
    <Eval> </Eval>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT