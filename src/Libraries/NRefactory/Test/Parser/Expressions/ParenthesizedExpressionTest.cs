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
	public class ParenthesizedExpressionTests
	{
		#region C#
		[Test]
		public void CSharpPrimitiveParenthesizedExpression()
		{
			ParenthesizedExpression p = (ParenthesizedExpression)ParseUtilCSharp.ParseExpression("((1))", typeof(ParenthesizedExpression));
			Assert.IsTrue(p.Expression is ParenthesizedExpression);
			p = p.Expression as ParenthesizedExpression;;
			Assert.IsTrue(p.Expression is PrimitiveExpression);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetPrimitiveParenthesizedExpression()
		{
			ParenthesizedExpression p = (ParenthesizedExpression)ParseUtilVBNet.ParseExpression("((1))", typeof(ParenthesizedExpression));
			Assert.IsTrue(p.Expression is ParenthesizedExpression);
			p = p.Expression as ParenthesizedExpression;;
			Assert.IsTrue(p.Expression is PrimitiveExpression);
		}
		#endregion
		
	}
}
