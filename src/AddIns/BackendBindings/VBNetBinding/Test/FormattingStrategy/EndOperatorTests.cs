// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextEditor;
using VBNetBinding;
using VBNetBinding.FormattingStrategy;

namespace VBNetBinding.Tests
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
		void RunFormatLineTest(string code, string expectedCode)
		{
			string foo = "As Foo";
			int cursorOffset = code.IndexOf(foo) + foo.Length;
			int line = 2;

			using (TextEditorControl editor = new TextEditorControl()) {
				editor.Document.TextContent = code;
				editor.ActiveTextAreaControl.Caret.Position = editor.Document.OffsetToPosition(cursorOffset);
				VBFormattingStrategy formattingStrategy = new VBFormattingStrategy();
				formattingStrategy.FormatLine(editor.ActiveTextAreaControl.TextArea, line, cursorOffset, '\n');
				
				Assert.AreEqual(expectedCode, editor.Document.TextContent);
			}
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
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
			
			RunFormatLineTest(code, expectedCode);
		}				
		
	}
}
