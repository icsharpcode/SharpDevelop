// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcTextTemplateAssemblyResolverTests
	{
		MvcTextTemplateAssemblyResolver resolver;
		
		void CreateResolver()
		{
			resolver = new MvcTextTemplateAssemblyResolver();
		}
		
		[Test]
		public void ResolvePath_FullPathToAssemblyPassed_ReturnsFullPathToAssembly()
		{
			CreateResolver();
			string expectedAssemblyPath = @"d:\projects\MyProject\bin\debug\MyProject.dll";
			
			string resolvedAssemblyPath = resolver.ResolvePath(expectedAssemblyPath);
			
			Assert.AreEqual(expectedAssemblyPath, resolvedAssemblyPath);
		}
	}
}
