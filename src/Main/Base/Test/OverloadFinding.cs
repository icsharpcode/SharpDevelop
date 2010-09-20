// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class OverloadFinding
	{
		[Test] public void Simple()
		{
			Test("(\"Hallo\")", 0, "(string a)", "(int b)");
			Test("(2)", 1, "(string a)", "(int b)");
		}
		
		[Test] public void WinForms()
		{
			string[] overloads = {"(object a)", "(TextBoxBase a)", "(Control a)", "(RichTextBox a)"};
			Test("(new RichTextBox())", 3, overloads);
			Test("(new Control())",     2, overloads);
			Test("(new TextBox())",     1, overloads);
			Test("(new Button())",      2, overloads);
			Test("(3)",                 0, overloads);
		}
		
		[Test] public void Params()
		{
			string[] overloads = {"(params int[] a)", "(int a, params int[] b)"};
			Test("()", 0, overloads);
			Test("(1)", 1, overloads);
			Test("(1, 2)", 1, overloads);
		}
		
		[Test] public void IntegerConversion()
		{
			string[] overloads = {"<T>(T a)", "(int a)"};
			Test("(1)", 1, overloads);
			Test("(short.MaxValue)", 0, overloads);
			Test("(long.MaxValue)", 0, overloads);
		}
		
		[Test] public void NullForReferenceTypes()
		{
			string[] overloads = {"(int a)", "(string a)"};
			Test("(null)", 1, overloads);
		}
		
		[Test] public void NullForNullableType()
		{
			string[] overloads = {"(int a)", "(int? a)"};
			Test("(null)", 1, overloads);
		}
		
		[Test] public void Generic()
		{
			string program = "class T<A> {}   class T<A, B> {}";
			string[] overloads = {"(T<int> a)", "(T<int, string> a)", "(T<char, string> a)"};
			Test("(new T<int>())", program, 0, overloads);
			Test("(new T<int, string>())", program, 1, overloads);
			Test("(new T<char, string>())", program, 2, overloads);
		}
		
		NRefactoryResolverTests nrrt = new NRefactoryResolverTests();
		
		void Test(string callExpr, int num, params string[] signatures)
		{
			Test(callExpr, "", num, signatures);
		}
		
		void Test(string callExpr, string extraCode, int num, params string[] signatures)
		{
			StringBuilder b = new StringBuilder();
			int lineNumber = 0;
			++lineNumber; b.AppendLine("using System;");
			++lineNumber; b.AppendLine("using System.Windows.Forms;");
			++lineNumber; b.AppendLine("class TestClass {");
			++lineNumber; b.AppendLine(" void callingMethod() {");
			++lineNumber; b.AppendLine("   ");
			int callPosition = lineNumber;
			++lineNumber; b.AppendLine(" }");
			int[] positions = new int[signatures.Length];
			for (int i = 0; i < signatures.Length; i++) {
				b.Append(" void Method");
				b.Append(signatures[i]);
				++lineNumber; b.AppendLine(" {");
				positions[i] = lineNumber;
				++lineNumber; b.AppendLine(" }");
			}
			b.AppendLine("}");
			b.Append(extraCode);
			MemberResolveResult mrr = nrrt.Resolve<MemberResolveResult>(b.ToString(), "Method" + callExpr, callPosition);
			string msg = "wrong overload: ";
			for (int i = 0; i < positions.Length; i++) {
				if (positions[i] == mrr.ResolvedMember.Region.BeginLine)
					msg += signatures[i];
			}
			Assert.AreEqual(positions[num], mrr.ResolvedMember.Region.BeginLine, msg);
		}
		
		[Test]
		public void MultipleOverloadsWithImplicitLambda()
		{
			string program = @"class MainClass {
	void Main() {
		M(x=>x.ToUpper());
	}
	delegate R Func<T, R>(T arg);
	int M(Func<int, int> f){ /* whatever ... */ }
	string M(Func<string, string> f){ /* whatever ... */ }
}";
			var mrr = nrrt.Resolve<MemberResolveResult>(program, "M(x=>x.ToUpper())", 3, 3, ExpressionContext.Default);
			Assert.AreEqual("System.String", mrr.ResolvedType.DotNetName);
		}
		
		[Test]
		public void MultipleOverloadsWithImplicitLambda2()
		{
			string program = @"class MainClass {
	void Main() {
		M(x=>x.Length);
	}
	delegate R Func<T, R>(T arg);
	int M(Func<int, int> f){ /* whatever ... */ }
	string M(Func<string, int> f){ /* whatever ... */ }
}";
			var mrr = nrrt.Resolve<MemberResolveResult>(program, "M(x=>x.Length)", 3, 3, ExpressionContext.Default);
			Assert.AreEqual("System.String", mrr.ResolvedType.DotNetName);
		}
		
		[Test]
		public void MultipleOverloadsWithImplicitLambda3()
		{
			string program = @"class MainClass {
	void Main() {
		M(x=>x+x);
	}
	delegate R Func<T, R>(T arg);
	string M(Func<string, int> f){ /* whatever ... */ }
	int M(Func<int, int> f){ /* whatever ... */ }
}";
			var mrr = nrrt.Resolve<MemberResolveResult>(program, "M(x=>x+x)", 3, 3, ExpressionContext.Default);
			Assert.AreEqual("System.Int32", mrr.ResolvedType.DotNetName);
		}
	}
}
