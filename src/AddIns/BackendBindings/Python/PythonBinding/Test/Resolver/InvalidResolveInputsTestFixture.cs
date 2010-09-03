// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests that the PythonResolver.Resolve method handles 
	/// invalid input parameters.
	/// </summary>
	[TestFixture]
	public class InvalidResolveInputsTestFixture
	{
		static readonly ParseInformation emptyParseInfo = new ParseInformation(new DefaultCompilationUnit(new DefaultProjectContent()));
		
		PythonResolver resolver;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
		}
			
		[Test]
		public void NullFileContent()
		{
			ExpressionResult result = new ExpressionResult("test", new DomRegion(0, 0), null, null);
			Assert.IsNull(resolver.Resolve(result, emptyParseInfo, null));
		}
		
		[Test]
		public void EmptyFileContent()
		{
			ExpressionResult result = new ExpressionResult("test", new DomRegion(0, 0), null, null);
			Assert.IsNull(resolver.Resolve(result, emptyParseInfo, String.Empty));
		}
		
		[Test]
		public void NullParseInfo()
		{
			ExpressionResult result = new ExpressionResult("test", new DomRegion(0, 0), null, null);
			Assert.IsNull(resolver.Resolve(result, null, "test"));
		}
	}
}
