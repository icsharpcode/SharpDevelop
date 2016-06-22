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

using Hornung.ResourceToolkit;
using Hornung.ResourceToolkit.Resolver;
using NUnit.Framework;

namespace ResourceToolkit.Tests.CSharp
{
	[TestFixture]
	public sealed class ICSharpCodeCoreNRefactoryResourceResolverTests : AbstractCSharpResourceResolverTestFixture
	{
		protected override void DoSetUp()
		{
			base.DoSetUp();
			
			const string ICSharpCodeCoreCode = @"namespace ICSharpCode.Core
{
	public class ResourceService
	{
		public static string GetString(string key)
		{
			return """";
		}
	}
}
";
			this.EnlistTestFile("ICSharpCodeCore.cs", ICSharpCodeCoreCode, true);
		}
		
		// ********************************************************************************************************************************
		
		const string CodeClass = @"class A {
	void B() {
		ICSharpCode.Core.ResourceService.GetString(""TestKey"");
		ICSharpCode.Core.ResourceService.GetString
	}
}";
		
		[Test]
		public void GetStringClass()
		{
			ResourceResolveResult rrr = Resolve(CodeClass, 2, 46, null);
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void GetStringClassCompletion()
		{
			ResourceResolveResult rrr = Resolve(CodeClass, 3, 44, '(');
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", null, "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeUsing = @"using ICSharpCode.Core;
class A {
	void B() {
		ResourceService.GetString(""TestKey"");
		ResourceService.GetString
	}
}";
		
		[Test]
		public void GetStringUsing()
		{
			ResourceResolveResult rrr = Resolve(CodeUsing, 3, 29, null);
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void GetStringUsingCompletion()
		{
			ResourceResolveResult rrr = Resolve(CodeUsing, 4, 27, '(');
			TestHelper.CheckReference(rrr, "[ICSharpCodeCoreHostResourceSet]", null, "A", "A.B");
		}
	}
}
