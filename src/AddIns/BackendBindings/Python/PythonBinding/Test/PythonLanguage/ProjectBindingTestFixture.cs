// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.PythonLanguage
{
	/// <summary>
	/// Tests the PythonProjectBinding class.
	/// </summary>
	[TestFixture]
	public class ProjectBindingTestFixture
	{
		PythonProjectBinding projectBinding;
		PythonProject project;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			projectBinding = new PythonProjectBinding();
			ProjectCreateInformation createInfo = new ProjectCreateInformation();
			createInfo.ProjectName = "Python";
			createInfo.OutputProjectFileName = @"C:\Projects\Python.pyproj";
			createInfo.Solution = new Solution(new MockProjectChangeWatcher());
			project = projectBinding.CreateProject(createInfo) as PythonProject;
		}
		
		[Test]
		public void Language()
		{
			Assert.AreEqual("Python", projectBinding.Language);
		}
		
		[Test]
		public void IsPythonProject()
		{
			Assert.IsNotNull(project);
		}

		[Test]
		public void ProjectName()
		{
			Assert.AreEqual("Python", project.Name);
		}
	}
}
