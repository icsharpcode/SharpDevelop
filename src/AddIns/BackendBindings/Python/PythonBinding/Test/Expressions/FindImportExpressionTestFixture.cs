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
	/// Checks that an import statement is correctly identified.
	/// </summary>
	[TestFixture]
	public class FindImportExpressionTestFixture
	{
		PythonExpressionFinder expressionFinder;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			expressionFinder = new PythonExpressionFinder();
		}

		[Test]
		public void ImportOnly()
		{
			string text = "import";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual("import", result.Expression);			
		}
		
		[Test]
		public void ExpressionContextIsNamespaceForImportExpression()
		{
			string text = "import";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.IsNotNull(result.Context as PythonImportExpressionContext);
		}
		
		/// <summary>
		/// Currently the expression finder returns the full import expression.
		/// It should probably just return the namespace.
		/// </summary>
		[Test]
		public void ImportWithNamespace()
		{
			string text = "import System";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual("import System", result.Expression);
			Assert.IsNotNull(result.Context as PythonImportExpressionContext);
		}
		
		[Test]
		public void MultipleLinesWithImportAndNamespace()
		{
			string text =
				"# Line to ignore\r\n" +
				"import System";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual("import System", result.Expression);
			Assert.IsNotNull(result.Context as PythonImportExpressionContext);
		}		
		
		[Test]
		public void ImportWithExtraWhitespaceBeforeNamespace()
		{
			string text = "import    System";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual(text, result.Expression);
			Assert.IsNotNull(result.Context as PythonImportExpressionContext);
		}
		
		[Test]
		public void FromStatementBeforeImport()
		{
			string text = "from System import Test";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual(text, result.Expression);
			Assert.IsNotNull(result.Context as PythonImportExpressionContext);
		}
		
		/// <summary>
		/// Expressions are case sensitive in Python.
		/// </summary>
		[Test]
		public void UppercaseImportWithNamespace()
		{
			string text = "IMPORT Test";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual("Test", result.Expression);
		}
		
		[Test]
		public void FromStatementWithoutAnyImportPart()
		{
			string text = "from System";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual(text, result.Expression);
			Assert.IsNotNull(result.Context as PythonImportExpressionContext);
		}
	}
}
