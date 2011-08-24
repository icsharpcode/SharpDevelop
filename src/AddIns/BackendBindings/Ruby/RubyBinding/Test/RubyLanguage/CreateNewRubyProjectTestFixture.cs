// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.RubyLanguage
{
	/// <summary>
	/// Tests the initial properties set in a newly created RubyProject.
	/// </summary>
	[TestFixture]
	public class CreateNewRubyProjectTestFixture
	{
		ProjectCreateInformation info;
		RubyProject project;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			info = new ProjectCreateInformation();
			info.Solution = new Solution(new MockProjectChangeWatcher());
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.rbproj";
			info.RootNamespace = "Test";

			project = new RubyProject(info);
		}
		
		[Test]
		public void Language()
		{
			Assert.AreEqual(RubyProjectBinding.LanguageName, project.Language);
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual(info.ProjectName, project.Name);
		}
		
		[Test]
		public void BuildTargetAdded()
		{
			foreach (ProjectTargetElement target in project.MSBuildProjectFile.Targets) {
				if (target.Name == "Build") {
					return;
				}
			}
			Assert.Fail("Build target does not exist.");
		}
	
		[Test]
		public void ProjectLanguageProperties()
		{
			Assert.AreEqual(RubyLanguageProperties.Default, project.LanguageProperties);
		}
		
		[Test]
		public void DefaultItemTypeForRubyFileNameIsCompile()
		{
			Assert.AreEqual(ItemType.Compile, project.GetDefaultItemType(".rb"));
		}
		
		[Test]
		public void DefaultItemTypeForUpperCaseRubyFileNameIsCompile()
		{
			Assert.AreEqual(ItemType.Compile, project.GetDefaultItemType(".RB"));
		}

		[Test]
		public void DefaultItemTypeForNullRubyFileNameIsCompile()
		{
			Assert.AreEqual(ItemType.None, project.GetDefaultItemType(null));
		}		
	}
}
