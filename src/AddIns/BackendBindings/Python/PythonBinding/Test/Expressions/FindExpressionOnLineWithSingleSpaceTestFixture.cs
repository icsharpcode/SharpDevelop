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