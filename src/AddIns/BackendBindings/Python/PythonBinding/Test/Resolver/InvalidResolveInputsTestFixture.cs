// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
