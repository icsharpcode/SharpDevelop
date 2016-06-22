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
