// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class ConditionalExpressionTests
	{
		#region C#
		[Test]
		public void CSharpConditionalExpressionTest()
		{
			ConditionalExpression ce = (ConditionalExpression)ParseUtilCSharp.ParseExpression("a == b ? a() : a.B", typeof(ConditionalExpression));
			
			Assert.IsTrue(ce.Condition is BinaryOperatorExpression);
			Assert.IsTrue(ce.TrueExpression is InvocationExpression);
			Assert.IsTrue(ce.FalseExpression is FieldReferenceExpression);
		}
		
		[Test]
		public void CSharpConditionalIsExpressionTest()
		{
			// (as is b?) ERROR (conflict with nullables, SD2-419)
			ConditionalExpression ce = (ConditionalExpression)ParseUtilCSharp.ParseExpression("a is b ? a() : a.B", typeof(ConditionalExpression));
			
			Assert.IsTrue(ce.Condition is BinaryOperatorExpression);
			Assert.IsTrue(ce.TrueExpression is InvocationExpression);
			Assert.IsTrue(ce.FalseExpression is FieldReferenceExpression);
		}
		#endregion
		
		#region VB.NET
		// No VB.NET representation
		#endregion
	}
}
