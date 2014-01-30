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
using System.Collections.Generic;

using Hornung.ResourceToolkit;
using Hornung.ResourceToolkit.Resolver;
using NUnit.Framework;

namespace ResourceToolkit.Tests.CSharp
{
	[TestFixture]
	public sealed class ICSharpCodeCoreResourceResolverTests : AbstractCSharpResourceResolverTestFixture
	{
		readonly IResourceResolver resolver = new ICSharpCodeCoreResourceResolver();
		
		IResourceResolver Resolver {
			get { return this.resolver; }
		}
		
		// ********************************************************************************************************************************
		
		const string CodeResourceAccess = @"class A {
	string B() {
		return ""${res:TestKey}"";
	}
}";
		
		[Test]
		public void ResourceAccessStart()
		{
			ResourceResolveResult rrr = Resolve(CodeResourceAccess, 2, 10, null);
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", "TestKey", null, null);
		}
		
		[Test]
		public void ResourceAccessAtKey()
		{
			ResourceResolveResult rrr = Resolve(CodeResourceAccess, 2, 17, null);
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", "TestKey", null, null);
		}
		
		[Test]
		public void ResourceAccessAtEndOfKey()
		{
			ResourceResolveResult rrr = Resolve(CodeResourceAccess, 2, 23, null);
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", "TestKey", null, null);
		}
		
		[Test]
		public void AfterKey()
		{
			ResourceResolveResult rrr = Resolve(CodeResourceAccess, 2, 24, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		[Test]
		public void NoCompletionWrongChar()
		{
			ResourceResolveResult rrr = Resolve(CodeResourceAccess, 2, 15, 'x');
			TestHelper.CheckNoReference(rrr);
		}
		
		[Test]
		public void ResourceAccessAtKeyWithCache()
		{
			NRefactoryAstCacheService.EnableCache();
			
			ResourceResolveResult rrr = Resolve(CodeResourceAccess, 2, 17, null);
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", "TestKey", null, null);
			
			rrr = Resolve(CodeResourceAccess, 2, 17, null);
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", "TestKey", null, null);
			
			NRefactoryAstCacheService.DisableCache();
		}
		
		// ********************************************************************************************************************************
		
		const string CodeBoundaryTests = @"//${res
class A {
	string B() {
		return ""${res:TestKey}"";
		return ""${res:"";
	}
}
//${res";
		
		[Test]
		public void StartOfDocument()
		{
			ResourceResolveResult rrr = Resolve(CodeBoundaryTests, 0, 0, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		[Test]
		public void StartOfDocumentTag()
		{
			ResourceResolveResult rrr = Resolve(CodeBoundaryTests, 0, 3, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		[Test]
		public void MiddleOfDocument()
		{
			ResourceResolveResult rrr = Resolve(CodeBoundaryTests, 3, 10, null);
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", "TestKey", null, null);
		}
		
		[Test]
		public void MiddleOfDocumentIncompleteTag()
		{
			ResourceResolveResult rrr = Resolve(CodeBoundaryTests, 4, 10, null);
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", null, null, null);
		}
		
		[Test]
		public void EndOfDocumentTag()
		{
			ResourceResolveResult rrr = Resolve(CodeBoundaryTests, 7, 3, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		[Test]
		public void EndOfDocument()
		{
			ResourceResolveResult rrr = Resolve(CodeBoundaryTests, 7, 6, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		[Test]
		public void AfterEndOfDocument()
		{
			ResourceResolveResult rrr = Resolve(CodeBoundaryTests, 8, 0, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		// ********************************************************************************************************************************
		
		const string CodeAddinFile = @"<AddIn name=""Test"">
	<Path name=""/TestPath"">
		<String id=""TestId"" text=""${res:TestKey}""/>
	</Path>
</AddIn>";
		
		[Test]
		public void ResourceAccessInAddinFile()
		{
			ResourceResolveResult rrr = Resolve("test.addin", CodeAddinFile, 2, 34, null, false);
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", "TestKey", null, null);
		}
		
		// ********************************************************************************************************************************
		
		[Test]
		public void ResolverSupportsCSharpFiles()
		{
			Assert.IsTrue(this.Resolver.SupportsFile("a.cs"));
		}
		
		[Test]
		public void ResolverSupportsAddInFiles()
		{
			Assert.IsTrue(this.Resolver.SupportsFile("Test.AddIn"));
		}
		
		[Test]
		public void ResolverSupportsXfrmFiles()
		{
			Assert.IsTrue(this.Resolver.SupportsFile("Test.XFrm"));
		}
		
		[Test]
		public void CSharpFilePatterns()
		{
			List<string> patterns = new List<string>(this.Resolver.GetPossiblePatternsForFile("a.cs"));
			Assert.Contains("${res:", patterns);
			Assert.AreEqual(1, patterns.Count, "Incorrect number of resource access patterns for C# files.");
		}
		
		[Test]
		public void ResolverUnsupportedExtension()
		{
			Assert.IsFalse(this.Resolver.SupportsFile("Test.resx"));
		}
		
		[Test]
		public void UnsupportedFilePatterns()
		{
			IEnumerable<string> list = this.Resolver.GetPossiblePatternsForFile("a.resx");
			Assert.IsNotNull(list, "IResourceResolver.GetPossiblePatternsForFile must not return null.");
			
			List<string> patterns = new List<string>(list);
			Assert.AreEqual(0, patterns.Count);
		}
	}
}
