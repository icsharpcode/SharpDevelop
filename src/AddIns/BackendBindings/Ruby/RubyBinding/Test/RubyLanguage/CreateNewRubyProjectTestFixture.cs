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
