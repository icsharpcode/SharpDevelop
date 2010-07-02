// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class XmlLiteralExpressionTests
	{
		#region C#
		
		// no C# representation
		
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleCommentTest()
		{
			XmlLiteralExpression xle = ParseUtilVBNet.ParseExpression<XmlLiteralExpression>("<!-- test -->");
			Assert.IsNotEmpty(xle.Expressions);
			Assert.IsTrue(xle.Expressions[0] is XmlContentExpression);
			XmlContentExpression content = xle.Expressions[0] as XmlContentExpression;
			Assert.AreEqual(XmlContentType.Comment, content.Type);
			Assert.AreEqual(" test ", content.Content);
			Assert.AreEqual(new Location(1,1), content.StartLocation);
			Assert.AreEqual(new Location(14,1), content.EndLocation);
		}
		#endregion
	}
}
