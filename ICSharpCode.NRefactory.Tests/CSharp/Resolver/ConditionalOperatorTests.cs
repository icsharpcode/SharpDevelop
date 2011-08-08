// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	// assign short name to the fake reflection type
	using dynamic = ICSharpCode.NRefactory.TypeSystem.ReflectionHelper.Dynamic;
	
	[TestFixture]
	public class ConditionalOperatorTests : ResolverTestBase
	{
		void TestOperator(ResolveResult condition, ResolveResult trueExpr, ResolveResult falseExpr,
		                  Conversion conditionConv, Conversion trueConv, Conversion falseConv,
		                  Type expectedResultType)
		{
			var corr = (ConditionalOperatorResolveResult)resolver.ResolveConditional(condition, trueExpr, falseExpr);
			AssertType(expectedResultType, corr);
			AssertConversion(corr.Condition, condition, conditionConv, "Condition Conversion");
			AssertConversion(corr.True, trueExpr, trueConv, "True Conversion");
			AssertConversion(corr.False, falseExpr, falseConv, "False Conversion");
		}
		
		[Test]
		public void PickMoreGeneralOfTheTypes()
		{
			TestOperator(MakeResult(typeof(bool)), MakeResult(typeof(string)), MakeResult(typeof(object)),
			             Conversion.IdentityConversion, Conversion.ImplicitReferenceConversion, Conversion.IdentityConversion,
			             typeof(object));
			
			TestOperator(MakeResult(typeof(bool)), MakeResult(typeof(int)), MakeResult(typeof(long)),
			             Conversion.IdentityConversion, Conversion.ImplicitNumericConversion, Conversion.IdentityConversion,
			             typeof(long));
		}
		
		[Test]
		public void StringAndNull()
		{
			ResolveResult condition = MakeResult(typeof(bool));
			ResolveResult trueExpr = MakeResult(typeof(string));
			var result = (ConditionalOperatorResolveResult)resolver.ResolveConditional(
				condition, trueExpr, MakeConstant(null));
			AssertType(typeof(string), result);
			AssertConversion(result.Condition, condition, Conversion.IdentityConversion, "Condition Conversion");
			AssertConversion(result.True, trueExpr, Conversion.IdentityConversion, "True Conversion");
			Assert.IsTrue(result.False.IsCompileTimeConstant);
			Assert.IsNull(result.False.ConstantValue);
			Assert.AreEqual("System.String", result.False.Type.FullName);
		}
		
		[Test]
		public void NullAndString()
		{
			ResolveResult condition = MakeResult(typeof(bool));
			ResolveResult falseExpr = MakeResult(typeof(string));
			var result = (ConditionalOperatorResolveResult)resolver.ResolveConditional(
				condition, MakeConstant(null), falseExpr);
			AssertType(typeof(string), result);
			AssertConversion(result.Condition, condition, Conversion.IdentityConversion, "Condition Conversion");
			Assert.IsTrue(result.True.IsCompileTimeConstant);
			Assert.IsNull(result.True.ConstantValue);
			Assert.AreEqual("System.String", result.True.Type.FullName);
			AssertConversion(result.False, falseExpr, Conversion.IdentityConversion, "False Conversion");
		}
		
		[Test]
		public void DynamicInArguments()
		{
			TestOperator(MakeResult(typeof(bool)), MakeResult(typeof(dynamic)), MakeResult(typeof(double)),
			             Conversion.IdentityConversion, Conversion.IdentityConversion, Conversion.BoxingConversion,
			             typeof(dynamic));
			
			TestOperator(MakeResult(typeof(bool)), MakeResult(typeof(double)), MakeResult(typeof(dynamic)),
			             Conversion.IdentityConversion, Conversion.BoxingConversion, Conversion.IdentityConversion,
			             typeof(dynamic));
		}
		
		[Test]
		public void DynamicInCondition()
		{
			TestOperator(MakeResult(typeof(dynamic)), MakeResult(typeof(float)), MakeResult(typeof(double)),
			             Conversion.ImplicitDynamicConversion, Conversion.ImplicitNumericConversion, Conversion.IdentityConversion,
			             typeof(double));
		}
		
		[Test]
		public void AllDynamic()
		{
			TestOperator(MakeResult(typeof(dynamic)), MakeResult(typeof(dynamic)), MakeResult(typeof(dynamic)),
			             Conversion.ImplicitDynamicConversion, Conversion.IdentityConversion, Conversion.IdentityConversion,
			             typeof(dynamic));
		}
		
		[Test]
		public void ListOfDynamicAndListOfObject()
		{
			AssertError(typeof(List<object>), resolver.ResolveConditional(
				MakeResult(typeof(bool)), MakeResult(typeof(List<object>)), MakeResult(typeof(List<dynamic>))));
			
			AssertError(typeof(List<dynamic>), resolver.ResolveConditional(
				MakeResult(typeof(bool)), MakeResult(typeof(List<dynamic>)), MakeResult(typeof(List<object>))));
		}
		
		[Test]
		public void Constant()
		{
			AssertConstant(1L, resolver.ResolveConditional(
				MakeConstant(true), MakeConstant(1), MakeConstant(2L)));
			
			AssertConstant(2L, resolver.ResolveConditional(
				MakeConstant(false), MakeConstant(1), MakeConstant(2L)));
		}
		
		[Test]
		public void NotConstantIfFalsePortionNotConstant()
		{
			AssertType(typeof(long), resolver.ResolveConditional(
				MakeConstant(true), MakeConstant(1), MakeResult(typeof(long))));
		}
	}
}
