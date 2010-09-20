// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
