// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using NUnit.Framework;

namespace PythonBinding.Tests
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
			createInfo.Solution = new Solution();
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
