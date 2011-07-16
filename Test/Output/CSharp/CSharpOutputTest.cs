// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.PrettyPrinter;
using NUnit.Framework;
using System.Text;

namespace ICSharpCode.NRefactory.Tests.PrettyPrinter
{
	[TestFixture]
	public class CSharpOutputTest
	{
		void TestProgram(string program)
		{
			TestProgram(program, new PrettyPrintOptions());
		}
		
		void TestProgram(string program, PrettyPrintOptions options)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(program));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CSharpOutputVisitor outputVisitor = new CSharpOutputVisitor();
			outputVisitor.Options = options;
			outputVisitor.VisitCompilationUnit(parser.CompilationUnit, null);
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(StripWhitespace(program), StripWhitespace(outputVisitor.Text));
		}
		
		internal static string StripWhitespace(string text)
		{
			return text.Trim().Replace("\t", "  ").Replace("\r", "");
		}
		
		void TestTypeMember(string program)
		{
			TestTypeMember(program, new PrettyPrintOptions());
		}
		
		void TestTypeMember(string program, PrettyPrintOptions options)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("class A");
			b.AppendLine("{");
			using (StringReader r = new StringReader(program)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("  ");
					b.AppendLine(line);
				}
			}
			b.AppendLine("}");
			TestProgram(b.ToString(), options);
		}
		
		void TestStatement(string statement)
		{
			TestStatement(statement, new PrettyPrintOptions());
		}
		
		void TestStatement(string statement, PrettyPrintOptions options)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("void Method()");
			b.AppendLine("{");
			using (StringReader r = new StringReader(statement)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("  ");
					b.AppendLine(line);
				}
			}
			b.AppendLine("}");
			TestTypeMember(b.ToString(), options);
		}
		
		void TestExpression(string expression)
		{
			// SEMICOLON HACK : without a trailing semicolon, parsing expressions does not work correctly
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(expression + ";"));
			Expression e = parser.ParseExpression();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			Assert.IsNotNull(e, "ParseExpression returned null");
			CSharpOutputVisitor outputVisitor = new CSharpOutputVisitor();
			e.AcceptVisitor(outputVisitor, null);
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(StripWhitespace(expression), StripWhitespace(outputVisitor.Text));
		}
		
		[Test]
		public void Namespace()
		{
			TestProgram("namespace System\n{\n}");
		}
		
		[Test]
		public void CustomEvent()
		{
			TestTypeMember("public event EventHandler Click {\n" +
			               "  add { obj.Click += value; }\n" +
			               "  remove { obj.Click -= value; }\n" +
			               "}");
		}
		
		[Test]
		public void EventWithInitializer()
		{
			TestTypeMember("public event EventHandler Click = delegate { };");
		}
		
		[Test]
		public void Field()
		{
			TestTypeMember("int a;");
		}
		
		[Test]
		public void Method()
		{
			TestTypeMember("void Method()\n{\n}");
		}
		
		[Test]
		public void StaticMethod()
		{
			TestTypeMember("static void Method()\n{\n}");
		}
		
		[Test]
		public void PartialModifier()
		{
			TestProgram("public partial class Foo\n{\n}");
		}
		
		[Test]
		public void GenericClassDefinition()
		{
			TestProgram("public class Foo<T> where T : IDisposable, ICloneable\n{\n}");
		}
		
		[Test]
		public void InterfaceWithOutParameters()
		{
			TestProgram("public interface ITest\n" +
			            "{\n" +
			            "  void Method(out int a, ref double b);\n" +
			            "}");
		}
		
		[Test]
		public void GenericClassDefinitionWithBaseType()
		{
			TestProgram("public class Foo<T> : BaseClass where T : IDisposable, ICloneable\n" +
			            "{\n" +
			            "}");
		}
		
		[Test]
		public void GenericMethodDefinition()
		{
			TestTypeMember("public void Foo<T>(T arg) where T : IDisposable, ICloneable\n" +
			               "{\n" +
			               "}");
		}
		
		[Test]
		public void ArrayRank()
		{
			TestStatement("object[,,] a = new object[1, 2, 3];");
		}
		
		[Test]
		public void JaggedArrayRank()
		{
			TestStatement("object[,][,,] a = new object[1, 2][,,];");
		}
		
		[Test]
		public void ArrayInitializer()
		{
			TestStatement("object[] a = new object[] {\n" +
			              "  1,\n" +
			              "  2,\n" +
			              "  3\n" +
			              "};");
		}
		
		[Test]
		public void IfStatement()
		{
			TestStatement("if (a) {\n" +
			              "  m1();\n" +
			              "} else {\n" +
			              "  m2();\n" +
			              "}");
			
			TestStatement("if (a) {\n" +
			              "  m1();\n" +
			              "} else if (b) {\n" +
			              "  m2();\n" +
			              "} else {\n" +
			              "  m3();\n" +
			              "}");
		}
		
		[Test]
		[Ignore("PlaceElseOnNewLine is currently broken")]
		public void IfStatementSeparateLines()
		{
			PrettyPrintOptions options = new PrettyPrintOptions();
			options.PlaceElseOnNewLine = true;
			options.StatementBraceStyle = BraceStyle.NextLine;
			
			TestStatement("if (a)\n" +
			              "{\n" +
			              "  m1();\n" +
			              "}\n" +
			              "else\n" +
			              "{\n" +
			              "  m2();\n" +
			              "}", options);
			
			TestStatement("if (a)\n" +
			              "{\n" +
			              "  m1();\n" +
			              "}\n" +
			              "else if (b)\n" +
			              "{\n" +
			              "  m2();\n" +
			              "}\n" +
			              "else\n" +
			              "{\n" +
			              "  m3();\n" +
			              "}", options);
		}
		
		[Test]
		[Ignore("Single-line if-else is currently broken.")]
		public void SingleLineIfElseStatement()
		{
			TestStatement("if (a) m1(); else m2();");
		}
		
		[Test]
		public void Assignment()
		{
			TestExpression("a = b");
		}
		
		[Test]
		public void UnaryOperator()
		{
			TestExpression("a = -b");
		}
		
		[Test]
		public void BlockStatements()
		{
			TestStatement("checked {\n}");
			TestStatement("unchecked {\n}");
			TestStatement("unsafe {\n}");
		}
		
		[Test]
		public void ExceptionHandling()
		{
			TestStatement("try {\n" +
			              "  throw new Exception();\n" +
			              "} catch (FirstException e) {\n" +
			              "} catch (SecondException) {\n" +
			              "} catch {\n" +
			              "  throw;\n" +
			              "} finally {\n" +
			              "}");
		}
		
		[Test]
		public void LoopStatements()
		{
			TestStatement("foreach (Type var in col) {\n}");
			TestStatement("while (true) {\n}");
			TestStatement("do {\n} while (true);");
		}
		
		[Test]
		public void Switch()
		{
			TestStatement("switch (a) {\n" +
			              "  case 0:\n" +
			              "  case 1:\n" +
			              "    break;\n" +
			              "  case 2:\n" +
			              "    return;\n" +
			              "  default:\n" +
			              "    throw new Exception();\n" +
			              "}");
		}
		
		[Test]
		public void MultipleVariableForLoop()
		{
			TestStatement("for (int a = 0, b = 0; b < 100; ++b,a--) {\n}");
		}
		
		[Test]
		public void SizeOf()
		{
			TestExpression("sizeof(IntPtr)");
		}
		
		[Test]
		public void ParenthesizedExpression()
		{
			TestExpression("(a)");
		}
		
		[Test]
		public void MethodOnGenericClass()
		{
			TestExpression("Container<string>.CreateInstance()");
		}
		
		[Test]
		public void EmptyStatement()
		{
			TestStatement(";");
		}
		
		[Test]
		public void Yield()
		{
			TestStatement("yield break;");
			TestStatement("yield return null;");
		}
		
		[Test]
		public void Integer()
		{
			TestExpression("16");
		}
		
		[Test]
		public void HexadecimalInteger()
		{
			TestExpression("0x10");
		}
		
		[Test]
		public void LongInteger()
		{
			TestExpression("12L");
		}
		
		[Test]
		public void LongUnsignedInteger()
		{
			TestExpression("12uL");
		}
		
		[Test]
		public void UnsignedInteger()
		{
			TestExpression("12u");
		}
		
		[Test]
		public void Double()
		{
			TestExpression("12.5");
			TestExpression("12.0");
		}
		
		[Test]
		public void StringWithUnicodeLiteral()
		{
			TestExpression(@"""\u0001""");
		}
		
		[Test]
		public void GenericMethodInvocation()
		{
			TestExpression("GenericMethod<T>(arg)");
		}
		
		[Test]
		public void Cast()
		{
			TestExpression("(T)a");
		}
		
		[Test]
		public void AsCast()
		{
			TestExpression("a as T");
		}
		
		[Test]
		public void NullCoalescing()
		{
			TestExpression("a ?? b");
		}
		
		[Test]
		public void SpecialIdentifierName()
		{
			TestExpression("@class");
		}
		
		[Test]
		public void InnerClassTypeReference()
		{
			TestExpression("typeof(List<string>.Enumerator)");
		}
		
		[Test]
		public void GenericDelegate()
		{
			TestProgram("public delegate void Predicate<T>(T item) where T : IDisposable, ICloneable;");
		}
		
		[Test]
		public void Enum()
		{
			TestProgram("enum MyTest\n{\n" +
			            "  Red,\n" +
			            "  Green,\n" +
			            "  Blue,\n" +
			            "  Yellow\n" +
			            "}");
		}
		
		[Test]
		public void EnumWithInitializers()
		{
			TestProgram("enum MyTest\n{\n" +
			            "  Red = 1,\n" +
			            "  Green = 2,\n" +
			            "  Blue = 4,\n" +
			            "  Yellow = 8\n" +
			            "}");
		}
		
		[Test]
		public void SyncLock()
		{
			TestStatement("lock (a) {\n" +
			              "  Work();\n" +
			              "}");
		}
		
		[Test]
		public void Using()
		{
			TestStatement("using (A a = new A()) {\n" +
			              "  a.Work();\n" +
			              "}");
		}
		
		[Test]
		public void AbstractProperty()
		{
			TestTypeMember("public abstract bool ExpectsValue { get; set; }");
			TestTypeMember("public abstract bool ExpectsValue { get; }");
			TestTypeMember("public abstract bool ExpectsValue { set; }");
		}
		
		[Test]
		public void SetOnlyProperty()
		{
			TestTypeMember("public bool ExpectsValue {\n" +
			               "  set { DoSomething(value); }\n" +
			               "}");
		}
		
		[Test]
		public void AbstractMethod()
		{
			TestTypeMember("public abstract void Run();");
			TestTypeMember("public abstract bool Run();");
		}
		
		[Test]
		public void AnonymousMethod()
		{
			TestStatement("Func b = delegate { return true; };");
			TestStatement("Func a = delegate() { return false; };");
		}
		
		[Test]
		public void Interface()
		{
			TestProgram("interface ITest\n" +
			            "{\n" +
			            "  bool GetterAndSetter { get; set; }\n" +
			            "  bool GetterOnly { get; }\n" +
			            "  bool SetterOnly { set; }\n" +
			            "  void InterfaceMethod();\n" +
			            "  string InterfaceMethod2();\n" +
			            "}");
		}
		
		[Test]
		public void IndexerDeclaration()
		{
			TestTypeMember("public string this[int index] {\n" +
			               "  get { return index.ToString(); }\n" +
			               "  set { }\n" +
			               "}");
			TestTypeMember("public string IList.this[int index] {\n" +
			               "  get { return index.ToString(); }\n" +
			               "  set { }\n" +
			               "}");
		}
		
		[Test]
		public void OverloadedConversionOperators()
		{
			TestTypeMember("public static explicit operator TheBug(XmlNode xmlNode)\n{\n}");
			TestTypeMember("public static implicit operator XmlNode(TheBug bugNode)\n{\n}");
		}
		
		[Test]
		public void OverloadedTrueFalseOperators()
		{
			TestTypeMember("public static bool operator true(TheBug bugNode)\n{\n}");
			TestTypeMember("public static bool operator false(TheBug bugNode)\n{\n}");
		}
		
		[Test]
		public void OverloadedOperators()
		{
			TestTypeMember("public static TheBug operator +(TheBug bugNode, TheBug bugNode2)\n{\n}");
			TestTypeMember("public static TheBug operator >>(TheBug bugNode, int b)\n{\n}");
		}
		
		[Test]
		public void OverloadedUnaryOperators()
		{
			TestTypeMember("public static TheBug operator +(TheBug bugNode)\n{\n}");
			TestTypeMember("public static TheBug operator -(TheBug bugNode)\n{\n}");
			TestTypeMember("public static TheBug operator ~(TheBug bugNode)\n{\n}");
			TestTypeMember("public static TheBug operator !(TheBug bugNode)\n{\n}");
		}
		
		[Test]
		public void PropertyWithAccessorAccessModifiers()
		{
			TestTypeMember("public bool ExpectsValue {\n" +
			               "  internal get { }\n" +
			               "  protected set { }\n" +
			               "}");
		}
		
		[Test]
		public void UsingStatementForExistingVariable()
		{
			TestStatement("using (obj) {\n}");
		}
		
		[Test]
		public void NewConstraint()
		{
			TestProgram("public struct Rational<T, O> where O : IRationalMath<T>, new()\n{\n}");
		}
		
		[Test]
		public void StructConstraint()
		{
			TestProgram("public struct Rational<T, O> where O : struct\n{\n}");
		}
		
		[Test]
		public void ClassConstraint()
		{
			TestProgram("public struct Rational<T, O> where O : class\n{\n}");
		}
		
		[Test]
		public void ExtensionMethod()
		{
			TestTypeMember("public static T[] Slice<T>(this T[] source, int index, int count)\n{\n}");
		}
		
		[Test]
		public void FixedStructField()
		{
			TestProgram(@"unsafe struct CrudeMessage
{
	public fixed byte data[256];
}");
		}
		
		[Test]
		public void FixedStructField2()
		{
			TestProgram(@"unsafe struct CrudeMessage
{
	fixed byte data[4 * sizeof(int)], data2[10];
}");
		}
		
		[Test]
		public void ImplicitlyTypedLambda()
		{
			TestExpression("x => x + 1");
		}
		
		[Test]
		public void ImplicitlyTypedLambdaWithBody()
		{
			TestExpression("x => { return x + 1; }");
			TestStatement("Func<int, int> f = x => { return x + 1; };");
		}
		
		[Test]
		public void ExplicitlyTypedLambda()
		{
			TestExpression("(int x) => x + 1");
		}
		
		[Test]
		public void ExplicitlyTypedLambdaWithBody()
		{
			TestExpression("(int x) => { return x + 1; }");
		}
		
		[Test]
		public void LambdaMultipleParameters()
		{
			TestExpression("(x, y) => x * y");
			TestExpression("(x, y) => { return x * y; }");
			TestExpression("(int x, int y) => x * y");
			TestExpression("(int x, int y) => { return x * y; }");
		}
		
		[Test]
		public void LambdaNoParameters()
		{
			TestExpression("() => Console.WriteLine()");
			TestExpression("() => { Console.WriteLine(); }");
		}
		
		[Test]
		public void ObjectInitializer()
		{
			TestExpression("new Point {\n" +
			               "  X = 0,\n" +
			               "  Y = 1\n" +
			               "}");
			TestExpression("new Rectangle {\n" +
			               "  P1 = new Point {\n" +
			               "    X = 0,\n" +
			               "    Y = 1\n" +
			               "  },\n" +
			               "  P2 = new Point {\n" +
			               "    X = 2,\n" +
			               "    Y = 3\n" +
			               "  }\n" +
			               "}");
			TestExpression("new Rectangle(arguments) {\n" +
			               "  P1 = {\n" +
			               "    X = 0,\n" +
			               "    Y = 1\n" +
			               "  },\n" +
			               "  P2 = {\n" +
			               "    X = 2,\n" +
			               "    Y = 3\n" +
			               "  }\n" +
			               "}");
		}
		
		[Test]
		public void CollectionInitializer()
		{
			TestExpression("new List<int> {\n" +
			               "  0,\n" +
			               "  1,\n" +
			               "  2,\n" +
			               "  3,\n" +
			               "  4,\n" +
			               "  5\n" +
			               "}");
			TestExpression(@"new List<Contact> {
  new Contact {
    Name = ""Chris Smith"",
    PhoneNumbers = {
      ""206-555-0101"",
      ""425-882-8080""
    }
  },
  new Contact {
    Name = ""Bob Harris"",
    PhoneNumbers = { ""650-555-0199"" }
  }
}");
		}
		
		[Test]
		public void AnonymousTypeCreation()
		{
			TestExpression("new {\n" +
			               "  obj.Name,\n" +
			               "  Price = 26.9,\n" +
			               "  ident\n" +
			               "}");
		}
		
		[Test]
		public void ImplicitlyTypedArrayCreation()
		{
			TestExpression("new[] {\n" +
			               "  1,\n" +
			               "  10,\n" +
			               "  100,\n" +
			               "  1000\n" +
			               "}");
		}
		
		[Test]
		public void QuerySimpleWhere()
		{
			TestExpression("from n in numbers\n" +
			               "  where n < 5\n" +
			               "  select n");
		}
		
		[Test]
		public void QueryMultipleFrom()
		{
			TestExpression("from c in customers\n" +
			               "  where c.Region == \"WA\"\n" +
			               "  from o in c.Orders\n" +
			               "  where o.OrderDate >= cutoffDate\n" +
			               "  select new {\n" +
			               "    c.CustomerID,\n" +
			               "    o.OrderID\n" +
			               "  }");
		}
		
		[Test]
		public void QuerySimpleOrdering()
		{
			TestExpression("from w in words\n" +
			               "  orderby w\n" +
			               "  select w");
		}
		
		[Test]
		public void QueryComplexOrdering()
		{
			TestExpression("from w in words\n" +
			               "  orderby w.Length descending, w ascending\n" +
			               "  select w");
		}
		
		[Test]
		public void QueryGroupInto()
		{
			TestExpression("from n in numbers\n" +
			               "  group n by n % 5 into g\n" +
			               "  select new {\n" +
			               "    Remainder = g.Key,\n" +
			               "    Numbers = g\n" +
			               "  }");
		}
		
		[Test]
		public void ExternAlias()
		{
			TestProgram("extern alias Name;");
		}
		
		[Test]
		public void Variance()
		{
			TestProgram("interface C<out X, in Y, Z>\n{\n}");
		}
		
		[Test]
		public void OptionalParameters()
		{
			TestTypeMember("void M(int x = 0);");
		}
		
		[Test]
		public void NamedArguments()
		{
			TestExpression("M(x: 1)");
		}
		
		[Test]
		public void TestAttributeWithNamedArgument()
		{
			TestProgram("[assembly: Foo(1, namedArg: 2, prop = 3)]");
		}
	}
}
