// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingPathResolverTests
	{
		TextTemplatingPathResolver pathResolver;
		FakeTextTemplatingVariables fakeTemplatingVariables;
		FakeTextTemplatingEnvironment fakeEnvironment;
		
		void CreatePathResolver()
		{
			fakeTemplatingVariables = new FakeTextTemplatingVariables();
			fakeEnvironment = new FakeTextTemplatingEnvironment();
			pathResolver = new TextTemplatingPathResolver(fakeTemplatingVariables, fakeEnvironment);
		}
		
		void AddEnvironmentVariable(string name, string value)
		{
			fakeEnvironment.AddVariable(name, value);
		}
		
		void AddTemplateVariable(string name, string value)
		{
			fakeTemplatingVariables.AddVariable(name, value);
		}
		
		[Test]
		public void ResolvePath_EnvironmentVariableInPath_ReturnsPathWithEnvironmentVariableExpanded()
		{
			CreatePathResolver();
			AddEnvironmentVariable("windir", @"c:\windows");
			
			string path = pathResolver.ResolvePath("%windir%");
			
			Assert.AreEqual(@"c:\windows", path);
		}
		
		[Test]
		public void ResolvePath_TemplateVariableInPath_ReturnsPathWithTemplateVariableExpanded()
		{
			CreatePathResolver();
			AddTemplateVariable("SolutionDir", @"d:\projects\MyApp\");
			
			string path = pathResolver.ResolvePath("$(SolutionDir)");
			
			Assert.AreEqual(@"d:\projects\MyApp\", path);
		}
	}
}
