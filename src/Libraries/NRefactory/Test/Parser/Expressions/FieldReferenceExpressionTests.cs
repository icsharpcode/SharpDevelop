/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 13.09.2004
 * Time: 19:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class FieldReferenceExpressionTests
	{
		#region C#
		[Test]
		public void CSharpSimpleFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilCSharp.ParseExpression("myTargetObject.myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is IdentifierExpression);
			Assert.AreEqual("myTargetObject", ((IdentifierExpression)fre.TargetObject).Identifier);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilVBNet.ParseExpression("myTargetObject.myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is IdentifierExpression);
			Assert.AreEqual("myTargetObject", ((IdentifierExpression)fre.TargetObject).Identifier);
		}
		#endregion
	}
}
