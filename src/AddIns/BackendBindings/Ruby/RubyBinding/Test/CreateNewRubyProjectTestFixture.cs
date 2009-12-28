// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.BuildEngine;
using NUnit.Framework;

namespace RubyBinding.Tests
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
			info.Solution = new Solution();
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.rbproj";
			info.RootNamespace = "Test";

			project = new RubyProject(info);
		}
		
		[Test]
		public void Language()
		{
			Assert.AreEqual(RubyLanguageBinding.LanguageName, project.Language);
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual(info.ProjectName, project.Name);
		}
		
		[Test]
		public void BuildTargetAdded()
		{
			foreach (Target target in project.MSBuildProject.Targets) {
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
