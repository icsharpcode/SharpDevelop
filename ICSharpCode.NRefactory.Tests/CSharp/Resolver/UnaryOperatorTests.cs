// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class UnaryOperatorTests
	{
		IProjectContent mscorlib = CecilLoaderTests.Mscorlib;
		CSharpResolver resolver;
		
		ConstantResolveResult MakeConstant(object value)
		{
			return new ConstantResolveResult(value.GetType().ToTypeReference().Resolve(mscorlib), value);
		}
		
		void AssertConstant(object expectedValue, ResolveResult rr)
		{
			Assert.IsFalse(rr.IsError, rr.ToString() + " is an error");
			Assert.IsTrue(rr.IsCompileTimeConstant, rr.ToString() + " is not a compile-time constant");
			Assert.AreEqual(expectedValue, rr.ConstantValue);
			Assert.AreEqual(expectedValue.GetType(), rr.ConstantValue.GetType(), "ResolveResult.ConstantValue has wrong Type");
			Assert.AreEqual(expectedValue.GetType().ToTypeReference().Resolve(mscorlib), rr.Type, "ResolveResult.Type is wrong");
		}
		
		[SetUp]
		public void SetUp()
		{
			resolver = new CSharpResolver(CecilLoaderTests.Mscorlib);
		}
		
		[Test]
		public void TestUnaryPlus()
		{
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((sbyte)1)));
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((byte)1)));
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((short)1)));
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((ushort)1)));
			AssertConstant(65, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant('A')));
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant(1)));
			AssertConstant((uint)1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((uint)1)));
			AssertConstant(1L, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((long)1)));
			AssertConstant((ulong)1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((ulong)1)));
		}
		
		[Test]
		public void TestUnaryMinus()
		{
			AssertConstant(-1, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(1)));
			AssertConstant(-1L, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant((uint)1)));
			AssertConstant(-2147483648L, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(2147483648)));
			AssertConstant(-1.0f, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(1.0f)));
			AssertConstant(-1.0, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(1.0)));
			AssertConstant(1m, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(-1m)));
			AssertConstant(-65, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant('A')));
		}
		
		[Test]
		public void TestUnaryMinusUncheckedOverflow()
		{
			AssertConstant(-2147483648, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(-2147483648)));
		}
		
		[Test]
		public void TestUnaryMinusCheckedOverflow()
		{
			resolver.IsCheckedContext = true;
			ResolveResult rr = resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(-2147483648));
			Assert.AreEqual("System.Int32", rr.Type.DotNetName);
			Assert.IsTrue(rr.IsError);
			Assert.IsFalse(rr.IsCompileTimeConstant);
		}
		
		[Test]
		public void TestBitwiseNot()
		{
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant(-2)));
			AssertConstant(~'A', resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant('A')));
			AssertConstant(~(sbyte)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((sbyte)1)));
			AssertConstant(~(byte)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((byte)1)));
			AssertConstant(~(short)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((short)1)));
			AssertConstant(~(ushort)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((ushort)1)));
			AssertConstant(~(uint)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((uint)1)));
			AssertConstant(~(long)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((long)1)));
			AssertConstant(~(ulong)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((ulong)1)));
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant(1.0)).IsError);
		}
		
		[Test]
		public void TestLogicalNot()
		{
			AssertConstant(true, resolver.ResolveUnaryOperator(UnaryOperatorType.Not, MakeConstant(false)));
			AssertConstant(false, resolver.ResolveUnaryOperator(UnaryOperatorType.Not, MakeConstant(true)));
		}
		
		void Test(char a)
		{
			var x = -a;
			x.GetType();
		}
	}
	
	class X
	{
		//public static implicit operator int(X a) { return 0; }
		public static implicit operator LoaderOptimization(X a) { return 0; }
	}
}
