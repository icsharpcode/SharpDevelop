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
		
		[Test]
		public void VBNetSimplePreprocessingInstructionTest()
		{
			XmlLiteralExpression xle = ParseUtilVBNet.ParseExpression<XmlLiteralExpression>("<?xml version='1.0'?>");
			Assert.IsNotEmpty(xle.Expressions);
			Assert.IsTrue(xle.Expressions[0] is XmlContentExpression);
			XmlContentExpression content = xle.Expressions[0] as XmlContentExpression;
			Assert.AreEqual(XmlContentType.ProcessingInstruction, content.Type);
			Assert.AreEqual("xml version='1.0'", content.Content);
			Assert.AreEqual(new Location(1,1), content.StartLocation);
			Assert.AreEqual(new Location(22,1), content.EndLocation);
		}
		
		[Test]
		public void VBNetSimpleCDataTest()
		{
			XmlLiteralExpression xle = ParseUtilVBNet.ParseExpression<XmlLiteralExpression>("<![CDATA[<simple> <cdata>]]>");
			Assert.IsNotEmpty(xle.Expressions);
			Assert.IsTrue(xle.Expressions[0] is XmlContentExpression);
			XmlContentExpression content = xle.Expressions[0] as XmlContentExpression;
			Assert.AreEqual(XmlContentType.CData, content.Type);
			Assert.AreEqual("<simple> <cdata>", content.Content);
			Assert.AreEqual(new Location(1,1), content.StartLocation);
			Assert.AreEqual(new Location(29,1), content.EndLocation);
		}
		
		[Test]
		public void VBNetSimpleEmptyElementTest()
		{
			XmlLiteralExpression xle = ParseUtilVBNet.ParseExpression<XmlLiteralExpression>("<Test />");
			Assert.IsNotEmpty(xle.Expressions);
			Assert.IsTrue(xle.Expressions[0] is XmlElementExpression);
			XmlElementExpression element = xle.Expressions[0] as XmlElementExpression;
			Assert.IsFalse(element.NameIsExpression);
			Assert.AreEqual("Test", element.XmlName);
			Assert.IsEmpty(element.Attributes);
			Assert.IsEmpty(element.Children);
			Assert.AreEqual(new Location(1,1), element.StartLocation);
			Assert.AreEqual(new Location(9,1), element.EndLocation);
		}
		#endregion
	}
}
