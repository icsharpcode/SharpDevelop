// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		[Test]
		public void VBNetSimpleDescendentsReferenceTest()
		{
			XmlMemberAccessExpression xmae = ParseUtilVBNet.ParseExpression<XmlMemberAccessExpression>("xml...<ns:Element>");
			Assert.AreEqual("ns:Element", xmae.Identifier);
			Assert.IsTrue(xmae.IsXmlIdentifier);
			Assert.AreEqual(XmlAxisType.Descendents, xmae.AxisType);
			Assert.IsTrue(xmae.TargetObject is IdentifierExpression);
			Assert.AreEqual("xml", ((IdentifierExpression)xmae.TargetObject).Identifier);
		}
		
		[Test]
		public void VBNetSimpleElementReferenceWithDotTest()
		{
			XmlMemberAccessExpression xmae = ParseUtilVBNet.ParseExpression<XmlMemberAccessExpression>(".<ns:MyElement>");
			Assert.AreEqual("ns:MyElement", xmae.Identifier);
			Assert.IsTrue(xmae.IsXmlIdentifier);
			Assert.AreEqual(XmlAxisType.Element, xmae.AxisType);
			Assert.IsTrue(xmae.TargetObject.IsNull);
		}
		
		[Test]
		public void VBNetSimpleAttributeReferenceWithDotTest()
		{
			XmlMemberAccessExpression xmae = ParseUtilVBNet.ParseExpression<XmlMemberAccessExpression>(".@attribute");
			Assert.AreEqual("attribute", xmae.Identifier);
			Assert.IsFalse(xmae.IsXmlIdentifier);
			Assert.AreEqual(XmlAxisType.Attribute, xmae.AxisType);
			Assert.IsTrue(xmae.TargetObject.IsNull);
		}
		
		[Test]
		public void VBNetXmlNameAttributeReferenceWithDotTest()
		{
			XmlMemberAccessExpression xmae = ParseUtilVBNet.ParseExpression<XmlMemberAccessExpression>(".@<ns:attribute>");
			Assert.AreEqual("ns:attribute", xmae.Identifier);
			Assert.IsTrue(xmae.IsXmlIdentifier);
			Assert.AreEqual(XmlAxisType.Attribute, xmae.AxisType);
			Assert.IsTrue(xmae.TargetObject.IsNull);
		}
		
		[Test]
		public void VBNetSimpleDescendentsReferenceWithDotTest()
		{
			XmlMemberAccessExpression xmae = ParseUtilVBNet.ParseExpression<XmlMemberAccessExpression>("...<ns:Element>");
			Assert.AreEqual("ns:Element", xmae.Identifier);
			Assert.IsTrue(xmae.IsXmlIdentifier);
			Assert.AreEqual(XmlAxisType.Descendents, xmae.AxisType);
			Assert.IsTrue(xmae.TargetObject.IsNull);
		}
		#endregion
	}
}
