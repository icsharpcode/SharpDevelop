// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			Assert.AreEqual(ExpressionContext.Importable, result.Context);
		}
		
		[Test]
		public void MultipleLinesWithImportAndNamespace()
		{
			string text = "# Line to ignore\r\nimport System";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual("import System", result.Expression);
			Assert.AreEqual(ExpressionContext.Importable, result.Context);
		}		
		
		[Test]
		public void ImportWithExtraWhitespaceBeforeNamespace()
		{
			string text = "import    System";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual(text, result.Expression);
			Assert.AreEqual(ExpressionContext.Importable, result.Context);
		}
		
		[Test]
		public void FromStatementBeforeImport()
		{
			string text = "from System import Test";
			ExpressionResult result = expressionFinder.FindExpression(text, text.Length);
			Assert.AreEqual(text, result.Expression);
			Assert.AreEqual(ExpressionContext.Importable, result.Context);
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
	}
}
