// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public class A<T> {
			public class B {
				public class C<U> {
					
				}
			}
		}
		
		public static void Main()
		{
			new ExpressionEvaluator_Tests().Fun("function argument");
		}
		
		static bool WorkerThreadMoved = false;
	
		public unsafe void Fun(string arg)
		{
			bool flag = true;
			byte b = 1;
			int i = 4;
			int* iPtr = &i;
			float pi = 3.14f;
			string hi = "hi";
			string emptyString = "";
			
			char[] array = "Hello".ToCharArray();
			char[] array2 = "world".ToCharArray();
			char[][] arrays = new char[][] {array, array2};
			List<char> list = new List<char>(array);
			
			DerivedClass myClass = new DerivedClass();
			BaseClass myClass2 = myClass;
			
			int*[][,] complexType1 = new int*[][,] { new int*[,] { { (int*)0xDA1D } } };
			A<int>.B.C<char>[][,] complexType2 = new A<int>.B.C<char>[0][,];
			
			System.Threading.Thread bgWork = new System.Threading.Thread(
				delegate() { WorkerThreadMoved = true; }
			);
			
			System.Diagnostics.Debugger.Break();
			bgWork.Start();
			System.Threading.Thread.Sleep(100);
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
	using NUnit.Framework;
	
	public partial class DebuggerTests
	{
		string expressionsInput = @"
				b; i; *i; *iPtr; pi
				pi - 3; pi + b; i + b; (uint)2 - 3; ((uint)2 - 3).GetType() ; (ulong)2 - 3 ; (b + b).GetType()
				1 << 4; 7 << -1; 1 << (uint)2; 1.0 & 2.0; System.Int32.MaxValue + 1; (uint)2 - (uint)3; 1 / 0
				hi + hi;  hi + ''#'';  hi + pi;  hi + null; emptyString; ''''
				hi + ''#'' == ''hi#''; hi + ''#'' == (object) ''hi#''; hi == (string)null; hi == null; hi == 1; null == null
				
				(5 + 6) % (1 + 2); 3 % 2 == 1
				15 & 255; 15 && 255; (ulong)1 + (long)1 /* invalid */
				b + 3 == i; b + 4 == i
				true == true; true == false
				
				i = 10; -i; ++i; i++; +i; i += 1; ~i; i = 4
				-(byte)1; (-(byte)1).GetType(); -(uint)1; (-(uint)1).GetType(); -(ulong)1 /* invalid */
				-2147483648 /* int.MinValue */; (-2147483648).GetType(); -(-2147483648)
				-9223372036854775808 /* long.MinValue */; (-9223372036854775808).GetType(); -(-9223372036854775808)
				-1.0; ~1.0; !1; flag; !flag
				
				arg; instanceField; staticField
				
				array; arrays; array[1]; array[i]; array[i - 1]
				new char[3]
				new char[b] {'a'}
				new char[3] {'a', 'b', 'c'}
				new char[] {'a', 'b', 'c'}
				new char[][] { new char[] { 'a', 'b' }, new char[] { 'c', 'd' } }
				new char[5] {'a', 'b', 'c'}
				new char[1,2]
				new char[pi]
				list; list[1]; list[i]; hi[1]; ''abcd''[2]
				
				list.Add((char)42); list.Add((char)52); list
				list = new System.Collections.Generic.List<char>(array2); list
				
				(Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass
				(Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass2
				(string)i
				(string)hi
				(int)hi
			";
		
		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore("Test fails randomly (race condition)")]
		public void ExpressionEvaluator_Tests()
		{
			StartTest();
			process.SelectedStackFrame.StepOver();
			process.SelectedStackFrame.StepOver(); // Start worker thread
			
			EvalAll(expressionsInput);
			
			// Test member hiding / overloading
			
			Value myClass = SelectedStackFrame.GetLocalVariableValue("myClass").GetPermanentReference();
			Expression myClassExpr = SelectedStackFrame.MethodInfo.GetLocalVariable(SelectedStackFrame.IP, "myClass").GetExpression();
			
			List<Expression> expressions = new List<Expression>();
			foreach(MemberInfo memberInfo in myClass.Type.GetFieldsAndNonIndexedProperties(DebugType.BindingFlagsAll)) {
				expressions.Add(myClassExpr.AppendMemberReference((IDebugMemberInfo)memberInfo));
			}
			expressions.Add(myClassExpr.AppendMemberReference((DebugMethodInfo)myClass.Type.GetMethod("StaticMethod")));
			expressions.Add(myClassExpr.AppendMemberReference((DebugMethodInfo)((DebugType)myClass.Type.BaseType).GetMethod("Foo", new string[] { "i" }), new PrimitiveExpression(1)));
			expressions.Add(myClassExpr.AppendMemberReference((DebugMethodInfo)myClass.Type.GetMethod("Foo", new string[] { "i" }), new PrimitiveExpression(1)));
			expressions.Add(myClassExpr.AppendMemberReference((DebugMethodInfo)myClass.Type.GetMethod("Foo", new string[] { "s" }), new PrimitiveExpression("a")));
			
			foreach(Expression expr in expressions) {
				Eval(expr.PrettyPrint());
			}
			
			string input2 = @"
				myClass.Foo(1.0)
				myClass.Foo(myClass)
				myClass.Foo(1)
				myClass.Foo(''abc'')
				myClass[1]
				myClass[(long)1]
				myClass[1.0]
				myClass[''abc'']
				myClass.Convert(1, 2)
				myClass.Convert(''abc'', 2)
			";
			
			EvalAll(input2);
			
			// Test type round tripping
			
			foreach(DebugLocalVariableInfo locVar in process.SelectedStackFrame.MethodInfo.GetLocalVariables()) {
				if (locVar.Name.StartsWith("complexType")) {
					TypeReference complexTypeRef = locVar.LocalType.GetTypeReference();
					string code = "typeof(" + complexTypeRef.PrettyPrint() + ")";
					TypeOfExpression complexTypeRefRT = (TypeOfExpression)ExpressionEvaluator.Parse(code, SupportedLanguage.CSharp);
					DebugType type = complexTypeRefRT.TypeReference.ResolveType(process.SelectedStackFrame.AppDomain);
					string status = locVar.LocalType.FullName == type.FullName ? "ok" : "fail";
					ObjectDumpToString("TypeResulution", string.Format(" {0} = {1} ({2})", code, type.FullName, status));
				}
			}
			
			// Type equality
			
			DebugLocalVariableInfo loc = SelectedStackFrame.MethodInfo.GetLocalVariable(SelectedStackFrame.IP, "list");
			Type locType = loc.LocalType;
			Type valType = loc.GetValue(SelectedStackFrame).Type;
			ObjectDump("TypesIdentitcal", object.ReferenceEquals(locType, valType));
			ObjectDump("TypesEqual", locType == valType);
			
			ObjectDump("WorkerThreadMoved", process.SelectedStackFrame.GetThisValue().GetMemberValue("WorkerThreadMoved").AsString());
			process.Continue();
			ObjectDump("WorkerThreadMoved", process.SelectedStackFrame.GetThisValue().GetMemberValue("WorkerThreadMoved").AsString());
			
			EndTest();
		}
		
		void EvalAll(string exprs)
		{
			exprs = exprs.Replace("''", "\"");
			foreach(string line in exprs.Split('\n', ';')) {
				Eval(line.Trim());
			}
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
				restultFmted = restultFmted.Replace("\0", "\\0");
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
    <DebuggingPaused>Break ExpressionEvaluator_Tests.cs:143,4-143,40</DebuggingPaused>
    <DebuggingPaused>StepComplete ExpressionEvaluator_Tests.cs:144,4-144,19</DebuggingPaused>
    <DebuggingPaused>StepComplete ExpressionEvaluator_Tests.cs:145,4-145,39</DebuggingPaused>
    <Eval> </Eval>
    <Eval> b = 1 </Eval>
    <Eval> i = 4 </Eval>
    <Eval> *i = Target object is not a pointer </Eval>
    <Eval> *iPtr = 4 </Eval>
    <Eval> pi = 3.14 </Eval>
    <Eval> pi - 3 = 0.1400001 </Eval>
    <Eval> pi + b = 4.14 </Eval>
    <Eval> i + b = 5 </Eval>
    <Eval> (uint)2 - 3 = -1 </Eval>
    <Eval> ((uint)2 - 3).GetType() = System.Int64 </Eval>
    <Eval> (ulong)2 - 3 = Can not use the binary operator Subtract on types System.UInt64 and System.Int32 </Eval>
    <Eval> (b + b).GetType() = System.Int32 </Eval>
    <Eval> 1 &lt;&lt; 4 = 16 </Eval>
    <Eval> 7 &lt;&lt; -1 = -2147483648 </Eval>
    <Eval> 1 &lt;&lt; (uint)2 = Can not use the binary operator ShiftLeft on types System.Int32 and System.UInt32 </Eval>
    <Eval> 1.0 &amp; 2.0 = Can not use the binary operator BitwiseAnd on types System.Double and System.Double </Eval>
    <Eval> System.Int32.MaxValue + 1 = Arithmetic operation resulted in an overflow. </Eval>
    <Eval> (uint)2 - (uint)3 = Arithmetic operation resulted in an overflow. </Eval>
    <Eval> 1 / 0 = Attempted to divide by zero. </Eval>
    <Eval> hi + hi = "hihi" </Eval>
    <Eval> hi + "#" = "hi#" </Eval>
    <Eval> hi + pi = "hi3.14" </Eval>
    <Eval> hi + null = "hi" </Eval>
    <Eval> emptyString = "" </Eval>
    <Eval> "" = "" </Eval>
    <Eval> hi + "#" == "hi#" = True </Eval>
    <Eval> hi + "#" == (object) "hi#" = False </Eval>
    <Eval> hi == (string)null = False </Eval>
    <Eval> hi == null = False </Eval>
    <Eval> hi == 1 = Can not use the binary operator Equality on types System.String and System.Int32 </Eval>
    <Eval> null == null = True </Eval>
    <Eval> </Eval>
    <Eval> (5 + 6) % (1 + 2) = 2 </Eval>
    <Eval> 3 % 2 == 1 = True </Eval>
    <Eval> 15 &amp; 255 = 15 </Eval>
    <Eval> 15 &amp;&amp; 255 = Can not use the binary operator LogicalAnd on types System.Int32 and System.Int32 </Eval>
    <Eval> (ulong)1 + (long)1 /* invalid */ = Can not use the binary operator Add on types System.UInt64 and System.Int64 </Eval>
    <Eval> b + 3 == i = True </Eval>
    <Eval> b + 4 == i = False </Eval>
    <Eval> true == true = True </Eval>
    <Eval> true == false = False </Eval>
    <Eval> </Eval>
    <Eval> i = 10 = 10 </Eval>
    <Eval> -i = -10 </Eval>
    <Eval> ++i = 11 </Eval>
    <Eval> i++ = 11 </Eval>
    <Eval> +i = 12 </Eval>
    <Eval> i += 1 = 13 </Eval>
    <Eval> ~i = -14 </Eval>
    <Eval> i = 4 = 4 </Eval>
    <Eval> -(byte)1 = -1 </Eval>
    <Eval> (-(byte)1).GetType() = System.Int32 </Eval>
    <Eval> -(uint)1 = -1 </Eval>
    <Eval> (-(uint)1).GetType() = System.Int64 </Eval>
    <Eval> -(ulong)1 /* invalid */ = Can not use the unary operator Minus on type System.UInt64 </Eval>
    <Eval> -2147483648 /* int.MinValue */ = -2147483648 </Eval>
    <Eval> (-2147483648).GetType() = System.Int32 </Eval>
    <Eval> -(-2147483648) = Arithmetic operation resulted in an overflow. </Eval>
    <Eval> -9223372036854775808 /* long.MinValue */ = -9223372036854775808 </Eval>
    <Eval> (-9223372036854775808).GetType() = System.Int64 </Eval>
    <Eval> -(-9223372036854775808) = Arithmetic operation resulted in an overflow. </Eval>
    <Eval> -1.0 = -1 </Eval>
    <Eval> ~1.0 = Can not use the unary operator BitNot on type System.Double </Eval>
    <Eval> !1 = Can not use the unary operator Not on type System.Int32 </Eval>
    <Eval> flag = True </Eval>
    <Eval> !flag = False </Eval>
    <Eval> </Eval>
    <Eval> arg = "function argument" </Eval>
    <Eval> instanceField = "instance field value" </Eval>
    <Eval> staticField = "static field value" </Eval>
    <Eval> </Eval>
    <Eval> array = Char[] {'H', 'e', 'l', 'l', 'o'} </Eval>
    <Eval> arrays = Char[][] {Char[] {'H', 'e', 'l', 'l', 'o'}, Char[] {'w', 'o', 'r', 'l', 'd'}} </Eval>
    <Eval> array[1] = 'e' </Eval>
    <Eval> array[i] = 'o' </Eval>
    <Eval> array[i - 1] = 'l' </Eval>
    <Eval> new char[3] = Char[] {'\0', '\0', '\0'} </Eval>
    <Eval> new char[b] {'a'} = Char[] {'a'} </Eval>
    <Eval> new char[3] {'a', 'b', 'c'} = Char[] {'a', 'b', 'c'} </Eval>
    <Eval> new char[] {'a', 'b', 'c'} = Char[] {'a', 'b', 'c'} </Eval>
    <Eval> new char[][] { new char[] { 'a', 'b' }, new char[] { 'c', 'd' } } = Char[][] {Char[] {'a', 'b'}, Char[] {'c', 'd'}} </Eval>
    <Eval> new char[5] {'a', 'b', 'c'} = Incorrect initializer length </Eval>
    <Eval> new char[1,2] = Multi-dimensional arrays are not suppored </Eval>
    <Eval> new char[pi] = Integer expected </Eval>
    <Eval> list = List`1 {'H', 'e', 'l', 'l', 'o'} </Eval>
    <Eval> list[1] = 'e' </Eval>
    <Eval> list[i] = 'o' </Eval>
    <Eval> hi[1] = 'i' </Eval>
    <Eval> "abcd"[2] = 'c' </Eval>
    <Eval> </Eval>
    <Eval> list.Add((char)42)</Eval>
    <Eval> list.Add((char)52)</Eval>
    <Eval> list = List`1 {'H', 'e', 'l', 'l', 'o', '*', '4'} </Eval>
    <Eval> list = new System.Collections.Generic.List&lt;char&gt;(array2) = List`1 {'w', 'o', 'r', 'l', 'd'} </Eval>
    <Eval> list = List`1 {'w', 'o', 'r', 'l', 'd'} </Eval>
    <Eval> </Eval>
    <Eval> (Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass = Debugger.Tests.ExpressionEvaluator_Tests+DerivedClass </Eval>
    <Eval> (Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass)myClass2 = Debugger.Tests.ExpressionEvaluator_Tests+DerivedClass </Eval>
    <Eval> (string)i = Can not cast System.Int32 to System.String </Eval>
    <Eval> (string)hi = "hi" </Eval>
    <Eval> (int)hi = Can not cast System.String to System.Int32 </Eval>
    <Eval> </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.ConstInt = 42 </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.ConstString = "const string" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.ConstNull = null </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.ConstEnum = B </Eval>
    <Eval> myClass.name = "derived name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.StaticField = "derived static field" </Eval>
    <Eval> myClass.Name = "derived name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.StaticProperty = "static property" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass).name = "base name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.BaseClass.StaticField = "base static field" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass).Name = "base name" </Eval>
    <Eval> Debugger.Tests.ExpressionEvaluator_Tests.DerivedClass.StaticMethod() = "static method" </Eval>
    <Eval> ((Debugger.Tests.ExpressionEvaluator_Tests.BaseClass)myClass).Foo(1) = "base Foo - int" </Eval>
    <Eval> myClass.Foo(1) = "derived Foo - int" </Eval>
    <Eval> myClass.Foo("a") = "derived Foo - string" </Eval>
    <Eval> </Eval>
    <Eval> myClass.Foo(1.0) = "base Foo - double" </Eval>
    <Eval> myClass.Foo(myClass) = "derived Foo - object" </Eval>
    <Eval> myClass.Foo(1) = "derived Foo - int" </Eval>
    <Eval> myClass.Foo("abc") = "derived Foo - string" </Eval>
    <Eval> myClass[1] = More then one applicable overload found:
  String Debugger.Tests.ExpressionEvaluator_Tests+DerivedClass.Item[Double d]
  String Debugger.Tests.ExpressionEvaluator_Tests+BaseClass.Item[Int64 i] </Eval>
    <Eval> myClass[(long)1] = "base indexer - long" </Eval>
    <Eval> myClass[1.0] = "derived indexer - double" </Eval>
    <Eval> myClass["abc"] = "derived indexer - string" </Eval>
    <Eval> myClass.Convert(1, 2) = Incorrect parameter type for 's'. Excpeted System.String, seen System.Int32 </Eval>
    <Eval> myClass.Convert("abc", 2) = "converted to abc and 2" </Eval>
    <Eval> </Eval>
    <TypeResulution> typeof(System.Int32*[][,]) = System.Int32*[,][] (ok)</TypeResulution>
    <TypeResulution> typeof(Debugger.Tests.ExpressionEvaluator_Tests.A&lt;System.Int32&gt;.B.C&lt;System.Char&gt;[][,]) = Debugger.Tests.ExpressionEvaluator_Tests+A`1+B+C`1[System.Int32,System.Char][,][] (ok)</TypeResulution>
    <TypesIdentitcal>True</TypesIdentitcal>
    <TypesEqual>True</TypesEqual>
    <WorkerThreadMoved>False</WorkerThreadMoved>
    <DebuggingPaused>Break ExpressionEvaluator_Tests.cs:146,4-146,40</DebuggingPaused>
    <WorkerThreadMoved>True</WorkerThreadMoved>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
