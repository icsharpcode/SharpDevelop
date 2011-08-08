// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	// assign short name to the fake reflection type
	using dynamic = ICSharpCode.NRefactory.TypeSystem.ReflectionHelper.Dynamic;
	
	[TestFixture]
	public class CastTests : ResolverTestBase
	{
		void TestCast(Type targetType, ResolveResult input, Conversion expectedConversion)
		{
			IType type = targetType.ToTypeReference().Resolve(context);
			ResolveResult rr = resolver.ResolveCast(type, input);
			AssertType(targetType, rr);
			Assert.AreEqual(typeof(ConversionResolveResult), rr.GetType());
			var crr = (ConversionResolveResult)rr;
			Assert.AreEqual(expectedConversion, crr.Conversion, "ConversionResolveResult.Conversion");
			Assert.AreSame(input, crr.Input, "ConversionResolveResult.Input");
		}
		
		[Test]
		public void SimpleCast()
		{
			TestCast(typeof(int), MakeResult(typeof(float)), Conversion.ExplicitNumericConversion);
			TestCast(typeof(string), MakeResult(typeof(object)), Conversion.ExplicitReferenceConversion);
			TestCast(typeof(byte), MakeResult(typeof(dynamic)), Conversion.ExplicitDynamicConversion);
			TestCast(typeof(dynamic), MakeResult(typeof(double)), Conversion.BoxingConversion);
		}
		
		[Test]
		public void NullableCasts()
		{
			TestCast(typeof(int), MakeResult(typeof(int?)), Conversion.ExplicitNullableConversion);
			TestCast(typeof(int?), MakeResult(typeof(int)), Conversion.ImplicitNullableConversion);
			
			TestCast(typeof(int?), MakeResult(typeof(long?)), Conversion.ExplicitNullableConversion);
			TestCast(typeof(long?), MakeResult(typeof(int?)), Conversion.ImplicitNullableConversion);
		}
		
		[Test]
		public void ConstantValueCast()
		{
			AssertConstant("Hello", resolver.ResolveCast(ResolveType(typeof(string)), MakeConstant("Hello")));
			AssertConstant((byte)1L, resolver.ResolveCast(ResolveType(typeof(byte)), MakeConstant(1L)));
			AssertConstant(3, resolver.ResolveCast(ResolveType(typeof(int)), MakeConstant(3.1415)));
			AssertConstant(3, resolver.ResolveCast(ResolveType(typeof(int)), MakeConstant(3.99)));
			AssertConstant((short)-3, resolver.ResolveCast(ResolveType(typeof(short)), MakeConstant(-3.99f)));
			AssertConstant(-3L, resolver.ResolveCast(ResolveType(typeof(long)), MakeConstant(-3.5)));
		}
		
		[Test]
		public void OverflowingCast()
		{
			resolver.CheckForOverflow = false;
			AssertConstant(uint.MaxValue, resolver.ResolveCast(ResolveType(typeof(uint)), MakeConstant(-1.6)));
			resolver.CheckForOverflow = true;
			AssertError(typeof(uint), resolver.ResolveCast(ResolveType(typeof(uint)), MakeConstant(-1.6)));
		}
		
		[Test]
		public void FailingStringCast()
		{
			AssertError(typeof(string), resolver.ResolveCast(ResolveType(typeof(string)), MakeConstant(1)));
		}
		
		[Test]
		public void OverflowingCastToEnum()
		{
			resolver.CheckForOverflow = true;
			AssertError(typeof(StringComparison), resolver.ResolveCast(ResolveType(typeof(StringComparison)), MakeConstant(long.MaxValue)));
		}
	}
}
