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
	public class XmlMemberAccessExpressionTests
	{
		#region C#
		
		// no C# representation
		
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleElementReferenceTest()
		{
			XmlMemberAccessExpression xmae = ParseUtilVBNet.ParseExpression<XmlMemberAccessExpression>("xml.<ns:MyElement>");
			Assert.AreEqual("ns:MyElement", xmae.Identifier);
			Assert.IsTrue(xmae.IsXmlIdentifier);
			Assert.AreEqual(XmlAxisType.Element, xmae.AxisType);
			Assert.IsTrue(xmae.TargetObject is IdentifierExpression);
			Assert.AreEqual("xml", ((IdentifierExpression)xmae.TargetObject).Identifier);
		}
		
		[Test]
		public void VBNetSimpleAttributeReferenceTest()
		{
			XmlMemberAccessExpression xmae = ParseUtilVBNet.ParseExpression<XmlMemberAccessExpression>("xml.@attribute");
			Assert.AreEqual("attribute", xmae.Identifier);
			Assert.IsFalse(xmae.IsXmlIdentifier);
			Assert.AreEqual(XmlAxisType.Attribute, xmae.AxisType);
			Assert.IsTrue(xmae.TargetObject is IdentifierExpression);
			Assert.AreEqual("xml", ((IdentifierExpression)xmae.TargetObject).Identifier);
		}
		
		[Test]
		public void VBNetXmlNameAttributeReferenceTest()
		{
			XmlMemberAccessExpression xmae = ParseUtilVBNet.ParseExpression<XmlMemberAccessExpression>("xml.@<ns:attribute>");
			Assert.AreEqual("ns:attribute", xmae.Identifier);
			Assert.IsTrue(xmae.IsXmlIdentifier);
			Assert.AreEqual(XmlAxisType.Attribute, xmae.AxisType);
			Assert.IsTrue(xmae.TargetObject is IdentifierExpression);
			Assert.AreEqual("xml", ((IdentifierExpression)xmae.TargetObject).Identifier);
		}
		#endregion
	}
}
