/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 18.07.2005
 * Time: 19:51
 */

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
	public class TypeReferenceExpressionTests
	{
		#region C#
		[Test]
		public void IntReferenceExpression()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilCSharp.ParseExpression("int.MaxValue", typeof(FieldReferenceExpression));
			Assert.AreEqual("MaxValue", fre.FieldName);
			Assert.AreEqual("System.Int32", ((TypeReferenceExpression)fre.TargetObject).TypeReference.SystemType);
		}
		
		[Test]
		public void StandaloneIntReferenceExpression()
		{
			// this is propably not what really should be returned for a standalone int
			// reference, but it has to stay consistent because NRefactoryResolver depends
			// on this trick.
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilCSharp.ParseExpression("int", typeof(FieldReferenceExpression), true);
			Assert.AreEqual("", fre.FieldName);
			Assert.AreEqual("System.Int32", ((TypeReferenceExpression)fre.TargetObject).TypeReference.SystemType);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBIntReferenceExpression()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilVBNet.ParseExpression("inTeGer.MaxValue", typeof(FieldReferenceExpression));
			Assert.AreEqual("MaxValue", fre.FieldName);
			Assert.AreEqual("System.Int32", ((TypeReferenceExpression)fre.TargetObject).TypeReference.SystemType);
		}
		
		[Test]
		public void VBStandaloneIntReferenceExpression()
		{
			// this is propably not what really should be returned for a standalone int
			// reference, but it has to stay consistent because NRefactoryResolver depends
			// on this trick.
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilVBNet.ParseExpression("inTeGer", typeof(FieldReferenceExpression), true);
			Assert.AreEqual("", fre.FieldName);
			Assert.AreEqual("System.Int32", ((TypeReferenceExpression)fre.TargetObject).TypeReference.SystemType);
		}
		#endregion
		
	}
}
