// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class InvocationExpressionTests
	{
		void CheckSimpleInvoke(InvocationExpression ie)
		{
			Assert.AreEqual(0, ie.Arguments.Count);
			Assert.IsTrue(ie.TargetObject is IdentifierExpression);
			Assert.AreEqual("myMethod", ((IdentifierExpression)ie.TargetObject).Identifier);
		}
		
		void CheckGenericInvoke(InvocationExpression expr)
		{
			Assert.AreEqual(1, expr.Arguments.Count);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			IdentifierExpression ident = (IdentifierExpression)expr.TargetObject;
			Assert.AreEqual("myMethod", ident.Identifier);
			Assert.AreEqual(1, ident.TypeArguments.Count);
			Assert.AreEqual("System.Char", ident.TypeArguments[0].Type);
		}
		
		void CheckGenericInvoke2(InvocationExpression expr)
		{
			Assert.AreEqual(0, expr.Arguments.Count);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			IdentifierExpression ident = (IdentifierExpression)expr.TargetObject;
			Assert.AreEqual("myMethod", ident.Identifier);
			Assert.AreEqual(2, ident.TypeArguments.Count);
			Assert.AreEqual("T", ident.TypeArguments[0].Type);
			Assert.IsFalse(ident.TypeArguments[0].IsKeyword);
			Assert.AreEqual("System.Boolean", ident.TypeArguments[1].Type);
			Assert.IsTrue(ident.TypeArguments[1].IsKeyword);
		}
		
		
		#region C#
		[Test]
		public void CSharpSimpleInvocationExpressionTest()
		{
			CheckSimpleInvoke(ParseUtilCSharp.ParseExpression<InvocationExpression>("myMethod()"));
		}
		
		[Test]
		public void CSharpGenericInvocationExpressionTest()
		{
			CheckGenericInvoke(ParseUtilCSharp.ParseExpression<InvocationExpression>("myMethod<char>('a')"));
		}
		
		[Test]
		public void CSharpGenericInvocation2ExpressionTest()
		{
			CheckGenericInvoke2(ParseUtilCSharp.ParseExpression<InvocationExpression>("myMethod<T,bool>()"));
		}
		
		[Test]
		public void CSharpAmbiguousGrammarGenericMethodCall()
		{
			InvocationExpression ie = ParseUtilCSharp.ParseExpression<InvocationExpression>("F(G<A,B>(7))");
			Assert.IsTrue(ie.TargetObject is IdentifierExpression);
			Assert.AreEqual(1, ie.Arguments.Count);
			ie = (InvocationExpression)ie.Arguments[0];
			Assert.AreEqual(1, ie.Arguments.Count);
			Assert.IsTrue(ie.Arguments[0] is PrimitiveExpression);
			IdentifierExpression ident = (IdentifierExpression)ie.TargetObject;
			Assert.AreEqual("G", ident.Identifier);
			Assert.AreEqual(2, ident.TypeArguments.Count);
		}
		
		[Test]
		public void CSharpAmbiguousGrammarNotAGenericMethodCall()
		{
			BinaryOperatorExpression boe = ParseUtilCSharp.ParseExpression<BinaryOperatorExpression>("F<A>+y");
			Assert.AreEqual(BinaryOperatorType.GreaterThan, boe.Op);
			Assert.IsTrue(boe.Left is BinaryOperatorExpression);
			Assert.IsTrue(boe.Right is UnaryOperatorExpression);
		}
		
		[Test]
		public void CSharpInvalidNestedInvocationExpressionTest()
		{
			// this test was written because this bug caused the AbstractASTVisitor to crash
			
			InvocationExpression expr = ParseUtilCSharp.ParseExpression<InvocationExpression>("WriteLine(myMethod(,))", true);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			Assert.AreEqual("WriteLine", ((IdentifierExpression)expr.TargetObject).Identifier);
			
			Assert.AreEqual(1, expr.Arguments.Count); // here a second null parameter was added incorrectly
			
			Assert.IsTrue(expr.Arguments[0] is InvocationExpression);
			CheckSimpleInvoke((InvocationExpression)expr.Arguments[0]);
		}
		
		[Test]
		public void NestedInvocationPositions()
		{
			InvocationExpression expr = ParseUtilCSharp.ParseExpression<InvocationExpression>("a.B().C(args)");
			Assert.AreEqual(new Location(8, 1), expr.StartLocation);
			Assert.AreEqual(new Location(14, 1), expr.EndLocation);
			MemberReferenceExpression mre = (MemberReferenceExpression)expr.TargetObject;
			Assert.AreEqual(new Location(6, 1), mre.StartLocation);
			Assert.AreEqual(new Location(8, 1), mre.EndLocation);
			
			Assert.AreEqual(new Location(4, 1), mre.TargetObject.StartLocation);
			Assert.AreEqual(new Location(6, 1), mre.TargetObject.EndLocation);
		}
		
		[Test]
		public void InvocationOnGenericType()
		{
			InvocationExpression expr = ParseUtilCSharp.ParseExpression<InvocationExpression>("A<T>.Foo()");
			MemberReferenceExpression mre = (MemberReferenceExpression)expr.TargetObject;
			Assert.AreEqual("Foo", mre.MemberName);
			TypeReferenceExpression tre = (TypeReferenceExpression)mre.TargetObject;
			Assert.AreEqual("A", tre.TypeReference.Type);
			Assert.AreEqual("T", tre.TypeReference.GenericTypes[0].Type);
		}
		
		[Test]
		public void InvocationOnInnerClassInGenericType()
		{
			InvocationExpression expr = ParseUtilCSharp.ParseExpression<InvocationExpression>("A<T>.B.Foo()");
			MemberReferenceExpression mre = (MemberReferenceExpression)expr.TargetObject;
			Assert.AreEqual("Foo", mre.MemberName);
			MemberReferenceExpression mre2 = (MemberReferenceExpression)mre.TargetObject;
			Assert.AreEqual("B", mre2.MemberName);
			TypeReferenceExpression tre = (TypeReferenceExpression)mre2.TargetObject;
			Assert.AreEqual("A", tre.TypeReference.Type);
			Assert.AreEqual("T", tre.TypeReference.GenericTypes[0].Type);
		}
		
		[Test]
		public void InvocationOnGenericInnerClassInGenericType()
		{
			InvocationExpression expr = ParseUtilCSharp.ParseExpression<InvocationExpression>("A<T>.B.C<U>.Foo()");
			MemberReferenceExpression mre = (MemberReferenceExpression)expr.TargetObject;
			Assert.AreEqual("Foo", mre.MemberName);
			TypeReferenceExpression tre = (TypeReferenceExpression)mre.TargetObject;
			InnerClassTypeReference ictr = (InnerClassTypeReference)tre.TypeReference;
			Assert.AreEqual("B.C", ictr.Type);
			Assert.AreEqual(1, ictr.GenericTypes.Count);
			Assert.AreEqual("U", ictr.GenericTypes[0].Type);
			
			Assert.AreEqual("A", ictr.BaseType.Type);
			Assert.AreEqual(1, ictr.BaseType.GenericTypes.Count);
			Assert.AreEqual("T", ictr.BaseType.GenericTypes[0].Type);
		}
		
		[Test]
		public void InvocationWithNamedArgument()
		{
			InvocationExpression expr = ParseUtilCSharp.ParseExpression<InvocationExpression>("a(arg: ref v)");
			Assert.AreEqual(1, expr.Arguments.Count);
			NamedArgumentExpression nae = (NamedArgumentExpression)expr.Arguments[0];
			Assert.AreEqual("arg", nae.Name);
			DirectionExpression dir = (DirectionExpression)nae.Expression;
			Assert.AreEqual(FieldDirection.Ref, dir.FieldDirection);
			Assert.IsInstanceOf<IdentifierExpression>(dir.Expression);
			
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleInvocationExpressionTest()
		{
			CheckSimpleInvoke(ParseUtilVBNet.ParseExpression<InvocationExpression>("myMethod()"));
		}
		
		[Test]
		public void VBNetGenericInvocationExpressionTest()
		{
			CheckGenericInvoke(ParseUtilVBNet.ParseExpression<InvocationExpression>("myMethod(Of Char)(\"a\"c)"));
		}
		
		[Test]
		public void VBNetGenericInvocation2ExpressionTest()
		{
			CheckGenericInvoke2(ParseUtilVBNet.ParseExpression<InvocationExpression>("myMethod(Of T, Boolean)()"));
		}
		
		[Test]
		public void PrimitiveExpression1Test()
		{
			InvocationExpression ie = ParseUtilVBNet.ParseExpression<InvocationExpression>("546.ToString()");
			Assert.AreEqual(0, ie.Arguments.Count);
		}
		
		[Test]
		public void VBInvocationOnGenericType()
		{
			InvocationExpression expr = ParseUtilVBNet.ParseExpression<InvocationExpression>("A(Of T).Foo()");
			MemberReferenceExpression mre = (MemberReferenceExpression)expr.TargetObject;
			Assert.AreEqual("Foo", mre.MemberName);
			IdentifierExpression tre = (IdentifierExpression)mre.TargetObject;
			Assert.AreEqual("A", tre.Identifier);
			Assert.AreEqual("T", tre.TypeArguments[0].Type);
		}
		
		[Test]
		public void VBInvocationOnInnerClassInGenericType()
		{
			InvocationExpression expr = ParseUtilVBNet.ParseExpression<InvocationExpression>("A(Of T).B.Foo()");
			MemberReferenceExpression mre = (MemberReferenceExpression)expr.TargetObject;
			Assert.AreEqual("Foo", mre.MemberName);
			MemberReferenceExpression mre2 = (MemberReferenceExpression)mre.TargetObject;
			Assert.AreEqual("B", mre2.MemberName);
			IdentifierExpression tre = (IdentifierExpression)mre2.TargetObject;
			Assert.AreEqual("A", tre.Identifier);
			Assert.AreEqual("T", tre.TypeArguments[0].Type);
		}
		
		[Test]
		public void VBInvocationOnGenericInnerClassInGenericType()
		{
			InvocationExpression expr = ParseUtilVBNet.ParseExpression<InvocationExpression>("A(Of T).B.C(Of U).Foo()");
			MemberReferenceExpression mre = (MemberReferenceExpression)expr.TargetObject;
			Assert.AreEqual("Foo", mre.MemberName);
			
			MemberReferenceExpression mre2 = (MemberReferenceExpression)mre.TargetObject;
			Assert.AreEqual("C", mre2.MemberName);
			Assert.AreEqual("U", mre2.TypeArguments[0].Type);
			
			MemberReferenceExpression mre3 = (MemberReferenceExpression)mre2.TargetObject;
			Assert.AreEqual("B", mre3.MemberName);
			
			IdentifierExpression tre = (IdentifierExpression)mre3.TargetObject;
			Assert.AreEqual("A", tre.Identifier);
			Assert.AreEqual("T", tre.TypeArguments[0].Type);
		}
		
		#endregion
	}
}
