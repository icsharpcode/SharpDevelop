// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace NRefactoryToBooConverter.Tests
{
	[TestFixture]
	public class ExpressionTests : TestHelper
	{
		[Test]
		public void Null()
		{
			TestExpr("null", "null");
		}
		
		[Test]
		public void This()
		{
			TestExpr("this", "self");
		}
		
		[Test]
		public void Super()
		{
			TestExpr("base.Member", "super.Member");
		}
		
		[Test]
		public void CastExpression()
		{
			TestExpr("(TargetType)var", "(var cast TargetType)");
		}
		
		[Test]
		public void TryCastExpression()
		{
			TestExpr("var as TargetType", "(var as TargetType)");
		}
		
		[Test]
		public void TypeTestExpression()
		{
			TestExpr("var is TargetType", "(var isa TargetType)");
		}
		
		[Test]
		public void CastExpressionComplexType()
		{
			TestExpr("(List<T>[,])var", "(var cast (List[of T], 2))");
		}
		
		[Test]
		public void TryCastExpressionComplexType()
		{
			TestExpr("var as List<T>[,]", "(var as (List[of T], 2))");
		}
		
		[Test]
		public void TypeTestExpressionComplexType()
		{
			TestExpr("var is List<T>[,]", "(var isa (List[of T], 2))");
		}
		
		[Test]
		public void Reference()
		{
			TestExpr("c", "c");
		}
		
		[Test]
		public void MemberReference()
		{
			TestExpr("c.d", "c.d");
		}
		
		[Test]
		public void MethodCall()
		{
			TestExpr("a()", "a()");
		}
		
		[Test]
		public void MethodCallWithParameter()
		{
			TestExpr("a(4)", "a(4)");
		}
		
		[Test]
		public void ObjectConstruction()
		{
			TestExpr("new MainClass()", "MainClass()");
		}
		
		[Test]
		public void GenericObjectConstruction()
		{
			TestExpr("new List<Element>()", "List[of Element]()");
		}
		
		[Test]
		public void QualifiedConstruction()
		{
			TestExpr("new System.DefaultComparer()", "System.DefaultComparer()");
		}
		
		[Test]
		public void QualifiedGenericObjectConstruction()
		{
			TestExpr("new Namespace.List<System.Element>()", "Namespace.List[of System.Element]()");
		}
		
		[Test]
		public void Integer()
		{
			TestExpr("1", "1");
		}
		
		[Test]
		public void Double()
		{
			TestExpr("1.0", "1.0");
		}
		
		[Test]
		public void Single()
		{
			TestExpr("1f", "1.0F");
		}
		
		[Test]
		public void UnaryMinus()
		{
			TestExpr("-(1)", "(-1)");
		}
		
		[Test]
		public void UnaryBitNegation()
		{
			TestExpr("~1", "(~1)");
		}
		
		[Test]
		public void UnaryLogicalNot()
		{
			TestExpr("!false", "(not false)");
		}
		
		[Test]
		public void UnaryPlus()
		{
			TestExpr("+(1)", "1");
		}
		
		[Test]
		public void PostIncrement()
		{
			TestExpr("i++", "(i++)");
		}
		
		[Test]
		public void PostDecrement()
		{
			TestExpr("i--", "(i--)");
		}
		
		[Test]
		public void Increment()
		{
			TestExpr("++i", "(++i)");
		}
		
		[Test]
		public void Decrement()
		{
			TestExpr("--i", "(--i)");
		}
		
		[Test]
		public void ArrayAccess()
		{
			TestExpr("arr[i]", "arr[i]");
		}
		
		[Test]
		public void MultidimensionalArrayAccess()
		{
			TestExpr("arr[x,y]", "arr[x, y]");
		}
		
		[Test]
		public void CreateArray()
		{
			TestExpr("new int[4]", "array(System.Int32, 4)");
		}
		
		[Test]
		public void CreateNestedArray()
		{
			TestExpr("new int[4][]", "array(typeof((System.Int32)), 4)");
		}
		
		[Test]
		public void CreateMultidimensionalArray()
		{
			TestExpr("new int[2, 3]", "matrix(System.Int32, 2, 3)");
		}
		
		[Test]
		public void CreateEmptyArray()
		{
			TestExpr("new int[] { }", "(of System.Int32: ,)");
		}
		
		[Test]
		public void CreateArrayWithOneElement()
		{
			TestExpr("new int[] { 1 }", "(of System.Int32: 1)");
		}
		
		[Test]
		public void CreateArrayWithTwoElements()
		{
			TestExpr("new int[] { 1 , 2 }", "(of System.Int32: 1, 2)");
		}
		
		[Test]
		public void AnonymousMethod()
		{
			TestExpr("delegate { }", "{ return }");
		}
		
		[Test]
		public void AnonymousMethodWithEmptyParameterList()
		{
			TestExpr("delegate () { }", "{ return }");
		}
		
		[Test]
		public void AnonymousMethodWithParameters()
		{
			TestExpr("delegate (int a, string b) { }", "{ a as System.Int32, b as System.String | return }");
		}
		
		[Test]
		public void Conditional()
		{
			TestExpr("a ? b : c", "(b if a else c)");
		}
		
		[Test]
		public void NullCoalescing()
		{
			TestExpr("a ?? b", "(a or b)");
		}
	}
}
