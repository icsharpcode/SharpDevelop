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
using System.Diagnostics;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class IISExpressProcessStartInfoTests : MvcTestsBase
	{
		WebProject project;
		TestableProject testableProject;
		ProcessStartInfo processStartInfo;
		
		void CreateWebProject(string name)
		{
			string fileName = @"d:\projects\MyProject\MyProject.csproj";
			testableProject = TestableProject.CreateProject(fileName, name);
			project = new WebProject(testableProject);
		}
		
		void CreateProcessStartInfo()
		{
			processStartInfo = IISExpressProcessStartInfo.Create(project);
		}
		
		[Test]
		public void Create_ProjectNameHasNoSpaces_ProcessInfoHasSiteArgument()
		{
			CreateWebProject("MyProject");
			
			CreateProcessStartInfo();
			
			Assert.AreEqual("/site:MyProject", processStartInfo.Arguments);
		}
		
		[Test]
		public void Create_ProjectNameHasSpaces_ProcessInfoHasSiteArgumentWithProjectNameInQuotes()
		{
			CreateWebProject("My Project");
			
			CreateProcessStartInfo();
			
			Assert.AreEqual("/site:\"My Project\"", processStartInfo.Arguments);
		}
	}
}
