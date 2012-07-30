// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class IISExpressProcessStartInfoTests
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
