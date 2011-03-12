// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class NamespaceHintTests
	{
		NamespaceHint namespaceHint;
		IProject project;
		FileProjectItem templateFile;
		
		void CreateProjectTemplateFile()
		{
			project = ProjectHelper.CreateProject();
			templateFile = new FileProjectItem(project, ItemType.None, "MyTemplate.tt");
		}
		
		void CreateNamespaceHint()
		{
			namespaceHint = new NamespaceHint(templateFile);
		}
		
		[Test]
		public void ToString_TemplateFileHasCustomToolNamespaceSetToTest_ReturnsTest()
		{
			CreateProjectTemplateFile();
			templateFile.SetMetadata("CustomToolNamespace", "Test");
			CreateNamespaceHint();
			
			string result = namespaceHint.ToString();
			
			Assert.AreEqual("Test", result);
		}
		
		[Test]
		public void ToString_TemplateFileHasNoCustomToolNamespace_ReturnsProjectRootNamespace()
		{
			CreateProjectTemplateFile();
			ProjectHelper.SetProjectRootNamespace(project, "ProjectRootNamespace");
			CreateNamespaceHint();
			
			string result = namespaceHint.ToString();
			
			Assert.AreEqual("ProjectRootNamespace", result);
		}
	}
}