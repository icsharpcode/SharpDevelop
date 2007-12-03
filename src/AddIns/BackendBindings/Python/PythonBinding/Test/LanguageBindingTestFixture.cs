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
	/// Tests the PythonLanguageBinding class.
	/// </summary>
	[TestFixture]
	public class LanguageBindingTestFixture
	{
		PythonLanguageBinding languageBinding;
		PythonProject project;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			languageBinding = new PythonLanguageBinding();
			ProjectCreateInformation createInfo = new ProjectCreateInformation();
			createInfo.ProjectName = "Python";
			createInfo.OutputProjectFileName = @"C:\Projects\Python.pyproj";
			createInfo.Solution = new Solution();
			project = languageBinding.CreateProject(createInfo) as PythonProject;
		}
		
		[Test]
		public void Language()
		{
			Assert.AreEqual("Python", languageBinding.Language);
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
