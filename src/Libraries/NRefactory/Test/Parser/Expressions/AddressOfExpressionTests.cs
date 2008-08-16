// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="markuspalme@gmx.de"/>
//     <version>$Revision: 3125 $</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class AddressOfExpressionTests
	{
		#region VB.NET
		
		[Test]
		public void SimpleAddressOfExpressionTest()
		{
			AddressOfExpression ae = ParseUtilVBNet.ParseExpression<AddressOfExpression>("AddressOf t");
			Assert.IsNotNull(ae);
			Assert.IsInstanceOfType(typeof(IdentifierExpression), ae.Expression);
			Assert.AreEqual("t", ((IdentifierExpression)ae.Expression).Identifier, "t");
		}
		
		[Test]
		public void GenericAddressOfExpressionTest()
		{
			AddressOfExpression ae = ParseUtilVBNet.ParseExpression<AddressOfExpression>("AddressOf t(Of X)");
			Assert.IsNotNull(ae);
			Assert.IsInstanceOfType(typeof(IdentifierExpression), ae.Expression);
			Assert.AreEqual("t", ((IdentifierExpression)ae.Expression).Identifier, "t");
			Assert.AreEqual(1, ((IdentifierExpression)ae.Expression).TypeArguments.Count);
			Assert.AreEqual("X", ((IdentifierExpression)ae.Expression).TypeArguments[0].Type);
		}
		
		#endregion
		
		#region C#
		
		// no C# representation
		
		#endregion
	}
}
