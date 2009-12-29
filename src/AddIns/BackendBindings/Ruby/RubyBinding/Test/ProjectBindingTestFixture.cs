// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using NUnit.Framework;

namespace RubyBinding.Tests
{
	/// <summary>
	/// Tests the RubyProjectBinding class.
	/// </summary>
	[TestFixture]
	public class ProjetBindingTestFixture
	{
		RubyProjectBinding projectBinding;
		RubyProject project;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			projectBinding = new RubyProjectBinding();
			ProjectCreateInformation createInfo = new ProjectCreateInformation();
			createInfo.ProjectName = "Ruby";
			createInfo.OutputProjectFileName = @"C:\Projects\Ruby.rbproj";
			createInfo.Solution = new Solution();
			project = projectBinding.CreateProject(createInfo) as RubyProject;
		}
		
		[Test]
		public void Language()
		{
			Assert.AreEqual("Ruby", projectBinding.Language);
		}
		
		[Test]
		public void IsRubyProject()
		{
			Assert.IsNotNull(project);
		}

		[Test]
		public void ProjectName()
		{
			Assert.AreEqual("Ruby", project.Name);
		}
	}
}
