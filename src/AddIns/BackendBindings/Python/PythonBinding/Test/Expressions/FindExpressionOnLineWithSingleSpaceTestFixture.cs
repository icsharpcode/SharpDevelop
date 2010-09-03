// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class FindExpressionOnLineWithSingleSpaceTestFixture
	{
		ExpressionResult result;
		
		[SetUp]
		public void Init()
		{
			string text = " ";
			PythonExpressionFinder expressionFinder = new PythonExpressionFinder();
			result = expressionFinder.FindExpression(text, 1);
		}
		
		[Test]
		public void ExpressionResultExpressionIsEmptyString()
		{
			Assert.AreEqual(String.Empty, result.Expression);
		}
	}
}
