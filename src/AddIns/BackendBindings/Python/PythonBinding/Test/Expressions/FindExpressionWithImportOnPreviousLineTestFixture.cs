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
	[TestFixture]
	public class FindExpressionWithImportOnPreviousLineTestFixture
	{
		ExpressionResult result;
		
		[SetUp]
		public void Init()
		{
			string text = "import\r\n";
			PythonExpressionFinder expressionFinder = new PythonExpressionFinder();
			int offset = 8; // Cursor is just after \r\n on second line.
			result = expressionFinder.FindExpression(text, offset);
		}
		
		[Test]
		public void ExpressionResultExpressionIsEmptyString()
		{
			Assert.AreEqual(String.Empty, result.Expression);
		}
	}
}