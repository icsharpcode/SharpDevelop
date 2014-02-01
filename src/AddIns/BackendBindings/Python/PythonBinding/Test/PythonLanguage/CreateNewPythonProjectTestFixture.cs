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
using System.Linq;
using System.Text;

using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.PythonLanguage
{
	/// <summary>
	/// Tests the initial properties set in a newly created PythonProject.
	/// </summary>
	[TestFixture]
	public class CreateNewPythonProjectTestFixture
	{
		ProjectCreateInformation info;
		PythonProject project;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonMSBuildEngineHelper.InitMSBuildEngine();
			
			info = new ProjectCreateInformation();
			info.Solution = new Solution(new MockProjectChangeWatcher());
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.pyproj";
			info.RootNamespace = "Test";

			project = new PythonProject(info);
		}
		
		[Test]
		public void Language()
		{
			Assert.AreEqual(PythonProjectBinding.LanguageName, project.Language);
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual(info.ProjectName, project.Name);
		}
		
		[Test]
		public void Imports()
		{
			string[] paths = GetImportPaths();
			Assert.Contains(PythonProject.DefaultTargetsFile, paths, "Could not find Python default target. Actual imports: " + GetArrayAsString(paths));
			// using MSBuild.Construction, we only see the direct imports
			//Assert.Contains(@"$(MSBuildBinPath)\Microsoft.Common.targets", paths, "Could not find Microsoft.Common.targets. Actual imports: " + GetArrayAsString(paths));
		}

		[Test]
		public void ProjectLanguageProperties()
		{
			Assert.AreEqual(PythonLanguageProperties.Default, project.LanguageProperties);
		}
		
		[Test]
		public void DefaultItemTypeForPythonFileNameIsCompile()
		{
			Assert.AreEqual(ItemType.Compile, project.GetDefaultItemType(".py"));
		}
		
		[Test]
		public void DefaultItemTypeForUpperCasePythonFileNameIsCompile()
		{
			Assert.AreEqual(ItemType.Compile, project.GetDefaultItemType(".PY"));
		}

		[Test]
		public void DefaultItemTypeForNullPythonFileNameIsCompile()
		{
			Assert.AreEqual(ItemType.None, project.GetDefaultItemType(null));
		}

		/// <summary>
		/// Gets the import paths from the project.
		/// </summary>
		string[] GetImportPaths()
		{
			lock (project.SyncRoot) {
				return project.MSBuildProjectFile.Imports.Select(i=>i.Project).ToArray();
			}
		}
		
		/// <summary>
		/// Takes the import paths in the project and creates a string with each import
		/// on a new line.
		/// </summary>
		string GetImportPathsAsText()
		{
			return GetArrayAsString(GetImportPaths());
		}
		
		string GetArrayAsString(string[] array)
		{
			StringBuilder text = new StringBuilder();
			foreach (string item in array) {
				text.AppendLine(item);
			}
			return text.ToString();
		}
	}
}
