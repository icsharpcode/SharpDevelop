// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace Debugger.Tests
{
	public class ExpressionEvaluatorVisitor_Tests
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
		
		public struct DBBool
		{
			// The three possible DBBool values.

			public static readonly DBBool Null = new DBBool(0);
			public static readonly DBBool False = new DBBool(-1);
			public static readonly DBBool True = new DBBool(1);

			// Private field that stores –1, 0, 1 for False, Null, True.

			sbyte value;

			// Private instance constructor. The value parameter must be –1, 0, or 1.

			DBBool(int value) {
				this.value = (sbyte)value;
			}

			// Properties to examine the value of a DBBool. Return true if this
			// DBBool has the given value, false otherwise.

			public bool IsNull { get { return value == 0; } }

			public bool IsFalse { get { return value < 0; } }

			public bool IsTrue { get { return value > 0; } }

			// Implicit conversion from bool to DBBool. Maps true to DBBool.True and
			// false to DBBool.False.

			public static implicit operator DBBool(bool x) {
				return x? True: False;
			}

			// Explicit conversion from DBBool to bool. Throws an exception if the
			// given DBBool is Null, otherwise returns true or false.

			public static explicit operator bool(DBBool x) {
				if (x.value == 0) throw new InvalidOperationException();
				return x.value > 0;
			}

			// Equality operator. Returns Null if either operand is Null, otherwise
			// returns True or False.

			public static DBBool operator ==(DBBool x, DBBool y) {
				if (x.value == 0 || y.value == 0) return Null;
				return x.value == y.value? True: False;
			}

			// Inequality operator. Returns Null if either operand is Null, otherwise
			// returns True or False.

			public static DBBool operator !=(DBBool x, DBBool y) {
				if (x.value == 0 || y.value == 0) return Null;
				return x.value != y.value? True: False;
			}

			// Logical negation operator. Returns True if the operand is False, Null
			// if the operand is Null, or False if the operand is True.

			public static DBBool operator !(DBBool x) {
				return new DBBool(-x.value);
			}

			// Logical AND operator. Returns False if either operand is False,
			// otherwise Null if either operand is Null, otherwise True.

			public static DBBool operator &(DBBool x, DBBool y) {
				return new DBBool(x.value < y.value? x.value: y.value);
			}

			// Logical OR operator. Returns True if either operand is True, otherwise
			// Null if either operand is Null, otherwise False.

			public static DBBool operator |(DBBool x, DBBool y) {
				return new DBBool(x.value > y.value? x.value: y.value);
			}

			// Definitely true operator. Returns true if the operand is True, false
			// otherwise.

			public static bool operator true(DBBool x) {
				return x.value > 0;
			}

			// Definitely false operator. Returns true if the operand is False, false
			// otherwise.

			public static bool operator false(DBBool x) {
				return x.value < 0;
			}
			
			public override string ToString() {
				if (value > 0) return "DBBool.True";
				if (value < 0) return "DBBool.False";
				return "DBBool.Null";
			}
			
			public override bool Equals(object obj)
			{
				return obj is DBBool && this.value == ((DBBool)obj).value;
			}
			
			public override int GetHashCode()
			{
				return value;
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
			new ExpressionEvaluatorVisitor_Tests().Fun("function argument");
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
			DBBool boo = DBBool.Null;
			var enumerable = Enumerable.Empty<int>();
			
			char[] array = "Hello".ToCharArray();
			char[] array2 = "world".ToCharArray();
			char[][] arrays = new char[][] {array, array2};
			List<char> list = new List<char>(array);
			
			DerivedClass myClass = new DerivedClass();
			BaseClass myClass2 = myClass;
			
			int*[][,] complexType1 = new int*[][,] { new int*[,] { { (int*)0xDA1D } } };
			A<int>.B.C<char>[][,] complexType2 = new A<int>.B.C<char>[0][,];
			
//			System.Threading.Thread bgWork = new System.Threading.Thread(
//				delegate() { WorkerThreadMoved = true; }
//			);
//
//			System.Diagnostics.Debugger.Break();
//			bgWork.Start();
//			System.Threading.Thread.Sleep(100);
			
			System.Diagnostics.Debugger.Break();
			
			i++;
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests
{
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using ICSharpCode.NRefactory.TypeSystem;
	using ICSharpCode.SharpDevelop.Services;
	using NUnit.Framework;
	using Debugger.AddIn;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.TypeSystem.Implementation;

	
	/// <summary>
	/// Description of ExpressionEvaluatorVisitor_Tests.
	/// </summary>
	public partial class DebuggerTests
	{
		
		[Test]
		public void ExpressionEvaluatorVisitor_Tests()
		{
			StartTest();
			
			evalContext = GetResource("ExpressionEvaluatorVisitor_Tests.cs");
			
			AssertEval("i", "4");
			AssertEval("pi", "3.14f");
			AssertEval("hi", "\"hi\"");
			AssertEval("i is int", "true");
			AssertEval("i is string", "false");
			AssertEval("hi + i", "\"hi4\"");
			AssertEval("hi + 1", "\"hi1\"");
			AssertEval("i > 2 ? i : i * 2", "4");
			AssertEval("i < 2 ? i : i * 2", "8");
			AssertEval("DBBool.True.IsTrue", "true");
			AssertEval("i < 2 ? i : i * 2", "8");
			AssertEval("DBBool.True || DBBool.Null", "DBBool.True");
			AssertEval("DBBool.Null || DBBool.True", "DBBool.True");
			AssertEval("DBBool.Null || DBBool.False", "DBBool.Null");
			AssertEval("DBBool.False || DBBool.False", "DBBool.False");
			AssertEval("array", "char[] {'H', 'e', 'l', 'l', 'o'}");
			AssertEval("array.ToList()", "List<char> {'H', 'e', 'l', 'l', 'o'}");
			
			EndTest(false);
		}
		
		string evalContext;
		
		void AssertEval(string expression, string expected)
		{
			NUnit.Framework.Assert.AreEqual(expected, ExpressionEvaluationVisitor.FormatValue(EvalThread, Evaluate(CurrentStackFrame, EvalThread, expression, evalContext)));
		}
		
		static ResolveResult ResolveSnippet(string fileName, TextLocation location, string contextCode, string codeSnippet, ICompilation compilation)
		{
			CSharpParser contextParser = new CSharpParser();
			var cu = contextParser.Parse(contextCode, fileName);
			CSharpAstResolver contextResolver = new CSharpAstResolver(compilation, cu);
			var node = cu.GetNodeAt(location);
			CSharpResolver context;
			if (node != null)
				context = contextResolver.GetResolverStateAfter(node, CancellationToken.None);
			else
				context = new CSharpResolver(compilation);
			CSharpParser parser = new CSharpParser();
			var expr = parser.ParseExpression(codeSnippet);
			Assert.IsFalse(parser.HasErrors);
			CSharpAstResolver snippetResolver = new CSharpAstResolver(context, expr);
			return snippetResolver.Resolve(expr, CancellationToken.None);
		}
		
		static Value Evaluate(StackFrame frame, Thread evalThread, string code, string contextCode)
		{
			if (frame == null || frame.NextStatement == null)
				throw new GetValueException("no stackframe available!");
			var location = frame.NextStatement;
			var debuggerTypeSystem = frame.AppDomain.Compilation;
			var compilation = new SimpleCompilation(debuggerTypeSystem.MainAssembly.UnresolvedAssembly, debuggerTypeSystem.ReferencedAssemblies.Select(a => a.UnresolvedAssembly));
			var rr = ResolveSnippet(location.Filename, new TextLocation(location.StartLine, location.StartColumn),
			                        contextCode, code, compilation);
			return new ExpressionEvaluationVisitor(frame, evalThread, frame.AppDomain.Compilation, true, true).Convert(rr);
		}
	}
}
#endif
