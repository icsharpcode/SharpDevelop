// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser.Expression
{
	[TestFixture]
	public class IsExpressionTests
	{
		[Test, Ignore]
		public void GenericArrayIsExpression()
		{
			/* TODO
			TypeOfIsExpression ce = ParseUtilCSharp.ParseExpression<TypeOfIsExpression>("o is List<string>[]");
			Assert.AreEqual("List", ce.TypeReference.Type);
			Assert.AreEqual("System.String", ce.TypeReference.GenericTypes[0].Type);
			Assert.AreEqual(new int[] { 0 }, ce.TypeReference.RankSpecifier);
			Assert.IsTrue(ce.Expression is IdentifierExpression);*/
		}
		
		[Test, Ignore]
		public void NullableIsExpression()
		{
			/* TODO
			TypeOfIsExpression ce = ParseUtilCSharp.ParseExpression<TypeOfIsExpression>("o is int?");
			Assert.AreEqual("System.Nullable", ce.TypeReference.Type);
			Assert.AreEqual("System.Int32", ce.TypeReference.GenericTypes[0].Type);
			Assert.IsTrue(ce.Expression is IdentifierExpression);*/
		}
		
		[Test, Ignore]
		public void NullableIsExpressionInBinaryOperatorExpression()
		{
		/* TODO
			BinaryOperatorExpression boe;
			boe = ParseUtilCSharp.ParseExpression<BinaryOperatorExpression>("o is int? == true");
			TypeOfIsExpression ce = (TypeOfIsExpression)boe.Left;
			Assert.AreEqual("System.Nullable", ce.TypeReference.Type);
			Assert.AreEqual("System.Int32", ce.TypeReference.GenericTypes[0].Type);
			Assert.IsTrue(ce.Expression is IdentifierExpression);
			*/
		}
	}
}
