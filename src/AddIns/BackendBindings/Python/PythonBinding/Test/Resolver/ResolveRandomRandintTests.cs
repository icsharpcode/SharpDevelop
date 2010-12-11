// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveRandomRandintTests : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("self.randomNumber.randint", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"import random\r\n" +
				"\r\n" +
				"class Test:\r\n" +
    			"    def __init__(self):\r\n" +
			    "        self.randomNumber = random.random()\r\n" +
			    "        self.randomNumber.randint\r\n" +
				"\r\n";
		}
			
		[Test]
		public void Resolve_RandomModuleCannotBeFound_NoNullRefererenceExceptionThrown()
		{
			Assert.IsNotNull(resolveResult as PythonMethodGroupResolveResult);
		}
	}
}
