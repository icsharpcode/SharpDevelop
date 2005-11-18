// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class TypeOfIsExpressionTests
	{
		#region C#
		[Test]
		public void GenericArrayIsExpression()
		{
			TypeOfIsExpression ce = ParseUtilCSharp.ParseExpression<TypeOfIsExpression>("o is List<string>[]");
			Assert.AreEqual("List", ce.TypeReference.Type);
			Assert.AreEqual("string", ce.TypeReference.GenericTypes[0].Type);
			Assert.AreEqual(new int[] { 0 }, ce.TypeReference.RankSpecifier);
			Assert.IsTrue(ce.Expression is IdentifierExpression);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleTypeOfIsExpression()
		{
			TypeOfIsExpression ce = ParseUtilVBNet.ParseExpression<TypeOfIsExpression>("TypeOf o Is MyObject");
			Assert.AreEqual("MyObject", ce.TypeReference.Type);
			Assert.IsTrue(ce.Expression is IdentifierExpression);
		}
		
		[Test]
		public void VBNetGenericTypeOfIsExpression()
		{
			TypeOfIsExpression ce = ParseUtilVBNet.ParseExpression<TypeOfIsExpression>("TypeOf o Is List(of T)");
			Assert.AreEqual("List", ce.TypeReference.Type);
			Assert.AreEqual("T", ce.TypeReference.GenericTypes[0].Type);
			Assert.IsTrue(ce.Expression is IdentifierExpression);
		}
		#endregion
	}
}
