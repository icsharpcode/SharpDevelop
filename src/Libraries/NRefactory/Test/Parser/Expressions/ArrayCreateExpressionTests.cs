/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 17.09.2004
 * Time: 14:22
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
	public class ArrayCreateExpressionTests
	{
		#region C#
		[Test]
		public void CSharpArrayCreateExpressionTest1()
		{
			ArrayCreateExpression ace = (ArrayCreateExpression)ParseUtilCSharp.ParseExpression("new int[5]", typeof(ArrayCreateExpression));
			Assert.AreEqual("int", ace.CreateType.Type);
			Assert.AreEqual(1, ace.Parameters.Count);
			// TODO: overwork ArrayCreateExpression.
//			Assert.AreEqual(null, ace.ArrayInitializer);
//			
//			Assert.IsTrue(ace.Parameters[0] is PrimitiveExpression);
//			PrimitiveExpression pe = (PrimitiveExpression)ace.Parameters[0];
//			Assert.AreEqual(5, (int)pe.Value);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetArrayCreateExpressionTest1()
		{
			ArrayCreateExpression ace = (ArrayCreateExpression)ParseUtilVBNet.ParseExpression("new Integer() {1, 2, 3, 4}", typeof(ArrayCreateExpression));
			
			Assert.AreEqual("Integer", ace.CreateType.Type);
			Assert.AreEqual(0, ace.Parameters.Count);
			// TODO: overwork ArrayCreateExpression.
//			Assert.AreEqual(null, ace.ArrayInitializer);
//			
//			Assert.IsTrue(ace.Parameters[0] is PrimitiveExpression);
//			PrimitiveExpression pe = (PrimitiveExpression)ace.Parameters[0];
//			Assert.AreEqual(5, (int)pe.Value);
		}
		#endregion
		
	}
}
