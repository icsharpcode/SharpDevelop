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
	public class BaseReferenceExpressionTests
	{
		#region C#
		[Test]
		public void CSharpBaseReferenceExpressionTest1()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilCSharp.ParseExpression("base.myField", typeof(FieldReferenceExpression));
			Assert.IsTrue(fre.TargetObject is BaseReferenceExpression);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetBaseReferenceExpressionTest1()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilVBNet.ParseExpression("MyBase.myField", typeof(FieldReferenceExpression));
			Assert.IsTrue(fre.TargetObject is BaseReferenceExpression);
		}
		#endregion
	}
}
