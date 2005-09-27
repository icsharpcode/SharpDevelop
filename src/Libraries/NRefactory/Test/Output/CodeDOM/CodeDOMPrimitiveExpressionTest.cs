// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.Output.CodeDOM.Tests
{
	[TestFixture]
	public class CodeDOMPrimitiveExpressionsTests
	{
		[Test]
		public void TestPrimitiveExpression()
		{
			object output = new PrimitiveExpression(5, "5").AcceptVisitor(new CodeDOMVisitor(), null);
			Assert.IsTrue(output is CodePrimitiveExpression);
			Assert.AreEqual(((CodePrimitiveExpression)output).Value, 5);
		}
	}
}
