// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
