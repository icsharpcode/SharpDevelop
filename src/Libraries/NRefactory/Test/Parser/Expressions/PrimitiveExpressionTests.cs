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
	public class PrimitiveExpressionTests
	{
		#region C#
		[Test]
		public void CSharpHexIntegerTest1()
		{
			InvocationExpression invExpr = (InvocationExpression)ParseUtilCSharp.ParseExpression("0xAFFE.ToString()", typeof(InvocationExpression));
			Assert.AreEqual(0, invExpr.Parameters.Count);
			Assert.IsTrue(invExpr.TargetObject is FieldReferenceExpression);
			FieldReferenceExpression fre = invExpr.TargetObject as FieldReferenceExpression;
			Assert.AreEqual("ToString", fre.FieldName);
			
			Assert.IsTrue(fre.TargetObject is PrimitiveExpression);
			PrimitiveExpression pe = fre.TargetObject as PrimitiveExpression;
			
			Assert.AreEqual("0xAFFE", pe.StringValue);
			Assert.AreEqual(0xAFFE, (int)pe.Value);
			
		}
		
		[Test]
		public void CSharpDoubleTest1()
		{
			PrimitiveExpression pe = (PrimitiveExpression)ParseUtilCSharp.ParseExpression(".5e-06;", typeof(PrimitiveExpression));
			Assert.AreEqual(".5e-06", pe.StringValue);
			Assert.AreEqual(.5e-06, (double)pe.Value);
		}
		
		[Test]
		public void CSharpCharTest1()
		{
			PrimitiveExpression pe = (PrimitiveExpression)ParseUtilCSharp.ParseExpression("'\\u0356';", typeof(PrimitiveExpression));
			Assert.AreEqual("'\\u0356'", pe.StringValue);
			Assert.AreEqual('\u0356', (char)pe.Value);
		}
		
		[Test]
		public void CSharpStringTest1()
		{
			PrimitiveExpression pe = (PrimitiveExpression)ParseUtilCSharp.ParseExpression("\"\\n\\t\\u0005 Hello World !!!\";", typeof(PrimitiveExpression));
			Assert.AreEqual("\"\\n\\t\\u0005 Hello World !!!\"", pe.StringValue);
			Assert.AreEqual("\n\t\u0005 Hello World !!!", (string)pe.Value);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void PrimitiveExpression1Test()
		{
			InvocationExpression ie = (InvocationExpression)ParseUtilVBNet.ParseExpression("546.ToString()", typeof(InvocationExpression));
		}
		#endregion
	}
}
