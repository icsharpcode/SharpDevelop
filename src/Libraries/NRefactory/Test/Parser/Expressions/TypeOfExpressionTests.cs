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
	public class TypeOfExpressionTests
	{
		#region C#
		[Test]
		public void CSharpSimpleTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(MyNamespace.N1.MyType)", typeof(TypeOfExpression));
			Assert.AreEqual("MyNamespace.N1.MyType", toe.TypeReference.Type);
		}
		#endregion 
		
		#region VB.NET
		[Test]
		public void SimpleTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilVBNet.ParseExpression("GetType(MyNamespace.N1.MyType)", typeof(TypeOfExpression));
			Assert.AreEqual("MyNamespace.N1.MyType", toe.TypeReference.Type);
		}
		#endregion
	}
}
