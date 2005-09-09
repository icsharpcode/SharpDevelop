// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using MbUnit.Framework;
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
		
		[Test]
		public void CSharpGenericTypeOfExpressionTest()
		{
			TypeOfExpression toe = (TypeOfExpression)ParseUtilCSharp.ParseExpression("typeof(MyNamespace.N1.MyType<string>)", typeof(TypeOfExpression));
			Assert.AreEqual("MyNamespace.N1.MyType", toe.TypeReference.Type);
			Assert.AreEqual("string", toe.TypeReference.GenericTypes[0].Type);
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
