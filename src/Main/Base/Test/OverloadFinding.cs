// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class OverloadFinding
	{
//		[Test] public void Simple()
//		{
//			Test("(\"Hallo\")", 0, "(string a)", "(int b)");
//			Test("(2)", 1, "(string a)", "(int b)");
//		}
//		
//		[Test] public void WinForms()
//		{
//			string[] overloads = {"(object a)", "(TextBoxBase a)", "(Control a)", "(RichTextBox a)"};
//			Test("(new RichTextBox())", 3, overloads);
//			Test("(new Control())",     2, overloads);
//			Test("(new TextBox())",     1, overloads);
//			Test("(new Button())",      2, overloads);
//			Test("(3)",                 0, overloads);
//		}
//		
//		[Test] public void Params()
//		{
//			string[] overloads = {"(params int[] a)", "(int a, params int[] b)"};
//			Test("()", 0, overloads);
//			Test("(1)", 1, overloads);
//			Test("(1, 2)", 1, overloads);
//		}
//		
//		[Test] public void IntegerConversion()
//		{
//			string[] overloads = {"<T>(T a)", "(int a)"};
//			Test("(1)", 1, overloads);
//			Test("(short.MaxValue)", 1, overloads);
//			Test("(long.MaxValue)", 0, overloads);
//		}
//		
//		NRefactoryResolverTests nrrt = new NRefactoryResolverTests();
//		
//		void Test(string callExpr, int num, params string[] signatures)
//		{
//			StringBuilder b = new StringBuilder();
//			int lineNumber = 0;
//			++lineNumber; b.AppendLine("using System;");
//			++lineNumber; b.AppendLine("using System.Windows.Forms;");
//			++lineNumber; b.AppendLine("class TestClass {");
//			++lineNumber; b.AppendLine(" void callingMethod() {");
//			++lineNumber; b.AppendLine("   ");
//			int callPosition = lineNumber;
//			++lineNumber; b.AppendLine(" }");
//			int[] positions = new int[signatures.Length];
//			for (int i = 0; i < signatures.Length; i++) {
//				b.Append(" void Method");
//				b.Append(signatures[i]);
//				++lineNumber; b.AppendLine(" {");
//				positions[i] = lineNumber;
//				++lineNumber; b.AppendLine(" }");
//			}
//			b.AppendLine("}");
//			MemberResolveResult mrr = nrrt.Resolve<MemberResolveResult>(b.ToString(), "Method" + callExpr, callPosition);
//			string msg = "wrong overload: ";
//			for (int i = 0; i < positions.Length; i++) {
//				if (positions[i] == mrr.ResolvedMember.Region.BeginLine)
//					msg += signatures[i];
//			}
//			Assert.AreEqual(positions[num], mrr.ResolvedMember.Region.BeginLine, msg);
//		}
	}
}
