// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class BinaryOperatorTests : ResolverTestBase
	{
		[Test]
		public void Multiplication()
		{
			AssertType(typeof(int), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Multiply, MakeResult(typeof(int)), MakeResult(typeof(int))));
			
			AssertType(typeof(float), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Multiply, MakeResult(typeof(int)), MakeConstant(0.0f)));
			
			AssertConstant(3.0f, resolver.ResolveBinaryOperator(
				BinaryOperatorType.Multiply, MakeConstant(1.5f), MakeConstant(2)));
			
			AssertConstant(6, resolver.ResolveBinaryOperator(
				BinaryOperatorType.Multiply, MakeConstant((byte)2), MakeConstant((byte)3)));
			
			AssertType(typeof(long?), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Multiply, MakeResult(typeof(uint?)), MakeResult(typeof(int?))));
			
			AssertError(typeof(decimal), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Multiply, MakeResult(typeof(float)), MakeResult(typeof(decimal))));
		}
		
		[Test]
		public void Addition()
		{
			AssertType(typeof(int?), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeResult(typeof(short)), MakeResult(typeof(byte?))));
			
			AssertConstant(3.0, resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeConstant(1.0f), MakeConstant(2.0)));
			
			AssertConstant(StringComparison.Ordinal, resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeConstant(StringComparison.InvariantCulture), MakeConstant(2)));
			
			AssertConstant(StringComparison.OrdinalIgnoreCase, resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeConstant((short)3), MakeConstant(StringComparison.InvariantCulture)));
			
			AssertConstant("Text", resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeConstant("Te"), MakeConstant("xt")));
			
			AssertConstant("", resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeConstant(null), resolver.ResolveCast(ResolveType(typeof(string)), MakeConstant(null))));
			
			AssertError(typeof(ReflectionHelper.Null), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeConstant(null), MakeConstant(null)));
			
			AssertType(typeof(Action), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeResult(typeof(Action)), MakeResult(typeof(Action))));
			
			AssertType(typeof(Action<string>), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeResult(typeof(Action<object>)), MakeResult(typeof(Action<string>))));
			
			Assert.IsTrue(resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeResult(typeof(Action<int>)), MakeResult(typeof(Action<long>))).IsError);
			
			AssertType(typeof(StringComparison?), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeResult(typeof(StringComparison?)), MakeResult(typeof(int))));
			
			AssertType(typeof(StringComparison?), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeResult(typeof(int?)), MakeResult(typeof(StringComparison))));
		}
		
		[Test]
		public void AdditionWithOverflow()
		{
			resolver.IsCheckedContext = false;
			AssertConstant(int.MinValue, resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeConstant(int.MaxValue), MakeConstant(1)));
			
			resolver.IsCheckedContext = true;
			AssertError(typeof(int), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Add, MakeConstant(int.MaxValue), MakeConstant(1)));
		}
		
		
		[Test]
		public void Subtraction()
		{
			AssertType(typeof(int?), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeResult(typeof(short)), MakeResult(typeof(byte?))));
			
			AssertConstant(-1.0, resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeConstant(1.0f), MakeConstant(2.0)));
			
			AssertConstant(StringComparison.InvariantCulture, resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeConstant(StringComparison.Ordinal), MakeConstant(2)));
			
			AssertConstant(3, resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeConstant(StringComparison.OrdinalIgnoreCase), MakeConstant(StringComparison.InvariantCulture)));
			
			Assert.IsTrue(resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeConstant("Te"), MakeConstant("xt")).IsError);
			
			AssertType(typeof(Action), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeResult(typeof(Action)), MakeResult(typeof(Action))));
			
			AssertType(typeof(Action<string>), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeResult(typeof(Action<object>)), MakeResult(typeof(Action<string>))));
			
			Assert.IsTrue(resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeResult(typeof(Action<int>)), MakeResult(typeof(Action<long>))).IsError);
			
			AssertType(typeof(StringComparison?), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeResult(typeof(StringComparison?)), MakeResult(typeof(int))));
			
			AssertType(typeof(int?), resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeResult(typeof(StringComparison?)), MakeResult(typeof(StringComparison))));
			
			Assert.IsTrue(resolver.ResolveBinaryOperator(
				BinaryOperatorType.Subtract, MakeResult(typeof(int?)), MakeResult(typeof(StringComparison))).IsError);
		}
		
		
		[Test]
		public void ShiftTest()
		{
			AssertConstant(6, resolver.ResolveBinaryOperator(
				BinaryOperatorType.ShiftLeft, MakeConstant(3), MakeConstant(1)));
			
			AssertConstant(ulong.MaxValue >> 2, resolver.ResolveBinaryOperator(
				BinaryOperatorType.ShiftRight, MakeConstant(ulong.MaxValue), MakeConstant(2)));
			
			AssertType(typeof(int?), resolver.ResolveBinaryOperator(
				BinaryOperatorType.ShiftLeft, MakeResult(typeof(ushort?)), MakeConstant(1)));
		}
		
		void Bla(X x, ushort? a) {
			var y = x + x;
			var z = a << 1;
			y.ToString();
			z.ToString();
		}
		class X { public static implicit operator string(X x) { return null ; } }
	}
}
