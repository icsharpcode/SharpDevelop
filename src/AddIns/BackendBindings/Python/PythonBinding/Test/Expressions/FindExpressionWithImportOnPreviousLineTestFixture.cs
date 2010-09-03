// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
