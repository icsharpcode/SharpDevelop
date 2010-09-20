// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveSelfTests : ResolveTestFixtureBase
	{
		IClass fooClass;
		
		protected override ExpressionResult GetExpressionResult()
		{
			fooClass = compilationUnit.Classes[0];
			return new ExpressionResult("self", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"class Foo(self)\r\n" +
				"    def bar(self):\r\n" +
				"        self\r\n" +
				"\r\n";
		}
		
		[Test]
		public void Resolve_ExpressionIsSelf_ResolveResultResolvedTypeUnderlyingClassReturnsFooClass()
		{
			IClass underlyingClass = resolveResult.ResolvedType.GetUnderlyingClass();
			Assert.AreEqual(fooClass, underlyingClass);
		}
		
		[Test]
		public void Resolve_ExpressionIsSelf_ResolveResultCallingClassReturnsFooClass()
		{
			IClass underlyingClass = resolveResult.CallingClass;
			Assert.AreEqual(fooClass, underlyingClass);
		}
	}
}
