// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using NUnit.Framework;

namespace ICSharpCode.VBNetBinding.Tests
{
	/// <summary>
	/// Tests that Operator overrides have "End Operator" added after the user presses the return key.
	/// </summary>
	[TestFixture]
	public class EndOperatorTests
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, "VBNetBindingTests");
			}
		}

		/// <summary>
		/// Checks that when the user presses the return key after the Operator line that the
		/// expected code is generated.
		/// </summary>
		void RunFormatLineTest(string code, string expectedCode, int expectedOffset)
		{
			string foo = "As Foo\r\n";
			int cursorOffset = code.IndexOf(foo) + foo.Length;

			AvalonEditTextEditorAdapter editor = new AvalonEditTextEditorAdapter(new TextEditor());
			editor.Document.Text = code;
			editor.Caret.Offset = cursorOffset;
			VBNetFormattingStrategy formattingStrategy = new VBNetFormattingStrategy();
			formattingStrategy.FormatLine(editor, '\n');
			
			Assert.AreEqual(expectedCode, editor.Document.Text);
			Assert.AreEqual(expectedOffset, editor.Caret.Offset);
		}
		
		[Test]
		public void AdditionOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator +(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator +(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator +(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
		
		[Test]
		public void AdditionOperatorWithExistingEndOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator +(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"\tEnd Operator\r\n" +
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator +(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator +(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}

		[Test]
		public void ExistingEndOperatorHasWhitespaceInbetween()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator +(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"\tEnd      Operator\r\n" +
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator +(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd      Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator +(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}

		[Test]
		public void AdditionOperatorHasWhitespaceAfterOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator + (ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator + (ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator + (ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
		
		[Test]
		public void AdditionOperatorWithNoWhitespace()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator+(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator+(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator+(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
		
		[Test]
		public void SubtractionOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator -(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator -(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator -(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}

		[Test]
		public void IsTrueOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator IsTrue(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator IsTrue(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator IsTrue(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
		
		[Test]
		public void MultiplicationOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator *(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator *(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator *(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}

		[Test]
		public void DivisionOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator /(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator /(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator /(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}

		[Test]
		public void IntegerDivisionOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator \\(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator \\(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator \\(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
		
		[Test]
		public void StringConcatenationOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator &(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator &(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator &(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
		
		[Test]
		public void ExponentationcationOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator ^(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator ^(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator ^(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
		
		[Test]
		public void EqualityOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator =(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator =(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator =(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
		
		[Test]
		public void GreaterThanOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator >(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator >(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator >(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}

		[Test]
		public void LessThanOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator <(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator <(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator <(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
		
		[Test]
		public void InequalityOperator()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared Operator <>(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared Operator <>(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\t\r\n" +
				"\tEnd Operator\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared Operator <>(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
		
		/// <summary>
		/// Check that a method that starts with "Operator" is ignored.
		/// </summary>
		[Test]
		public void MethodStartsWithOperatorString()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Shared OperatorIgnore(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Shared OperatorIgnore(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
				"\t\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Shared OperatorIgnore(ByVal lhs As Foo, ByVal rhs As Foo) As Foo\r\n" +
			                      "\t").Length;
			
			RunFormatLineTest(code, expectedCode, expectedOffset);
		}
	}
}
