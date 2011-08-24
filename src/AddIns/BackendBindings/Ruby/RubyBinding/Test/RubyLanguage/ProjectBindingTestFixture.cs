// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.RubyLanguage
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
			createInfo.Solution = new Solution(new MockProjectChangeWatcher());
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
