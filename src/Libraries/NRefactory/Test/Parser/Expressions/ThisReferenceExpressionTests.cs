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
	public class ThisReferenceExpressionTests
	{
		#region C#
		[Test]
		public void CSharpThisReferenceExpressionTest1()
		{
			ThisReferenceExpression tre = (ThisReferenceExpression)ParseUtilCSharp.ParseExpression("this", typeof(ThisReferenceExpression));
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetThisReferenceExpressionTest1()
		{
			ThisReferenceExpression ie = (ThisReferenceExpression)ParseUtilVBNet.ParseExpression("Me", typeof(ThisReferenceExpression));
		}
		#endregion
	}
}
