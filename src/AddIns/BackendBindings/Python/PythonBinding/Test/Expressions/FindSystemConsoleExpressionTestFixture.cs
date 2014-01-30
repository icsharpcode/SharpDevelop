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
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	/// <summary>
	/// Tests the PythonExpressionFinder can find the System.Console
	/// expression in various forms.
	/// </summary>
	[TestFixture]
	public class FindSystemConsoleExpressionTestFixture
	{
		PythonExpressionFinder expressionFinder;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			expressionFinder = new PythonExpressionFinder();
		}

		[Test]
		public void NullTextExpression()
		{
			ExpressionResult result = expressionFinder.FindExpression(null, 10);
			Assert.IsNull(result.Expression);
		}

		/// <summary>
		/// The offset passed to the expression finder points to the
		/// last character in the expression. In the string below this is the
		/// 'e'.
		/// </summary>
		[Test]
		public void SystemConsoleOnly()
		{
			string text = "System.Console";
			AssertSystemConsoleExpressionFound(text, text.Length);
		}
		
		[Test]
		public void MultipleLinesContainingCarriageReturnAndNewLine()
		{
			string text = "\r\nSystem.Console";
			AssertSystemConsoleExpressionFound(text, text.Length);
		}

		[Test]
		public void MultipleLinesContainingCarriageReturn()
		{
			string text = "\rSystem.Console";
			AssertSystemConsoleExpressionFound(text, text.Length);
		}
		
		
		/// <summary>
		/// Should find an empty string since the offset is after the carriage return character.
		/// </summary>
		[Test]
		public void CarriageReturnAfterLastCharacter()
		{
			string text = "System.Console\r";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual(String.Empty, result.Expression);
		}

		[Test]
		public void SpaceBeforeSystemConsoleText()
		{
			string text = " System.Console";
			AssertSystemConsoleExpressionFound(text, text.Length);
		}
		
		[Test]
		public void TabBeforeSystemConsoleText()
		{
			string text = "\tSystem.Console";
			AssertSystemConsoleExpressionFound(text, text.Length);
		}
		
		[Test]
		public void EmptyString()
		{
			string text = String.Empty;
			ExpressionResult result = expressionFinder.FindExpression(text, 0);
			Assert.IsNull(result.Expression);
		}

		[Test]
		public void OffsetTooSmall()
		{
			string text = "a";
			ExpressionResult result = expressionFinder.FindExpression(text, 0);
			Assert.IsNull(result.Expression);
		}
		
		[Test]
		public void OffsetTooLarge()
		{
			string text = "a";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length + 1);
			Assert.IsNull(result.Expression);			
		}
		
		[Test]
		public void NegativeOffset()
		{
			string text = "a";
			ExpressionResult result = expressionFinder.FindExpression(text, -1);
			Assert.IsNull(result.Expression);						
		}
		
		/// <summary>
		/// Checks that the System.Console expression is found in the
		/// text before the specified offset.
		/// </summary>
		void AssertSystemConsoleExpressionFound(string text, int offset)
		{
			ExpressionResult result = expressionFinder.FindExpression(text, offset);
			Assert.AreEqual("System.Console", result.Expression);
		}
	}
}
