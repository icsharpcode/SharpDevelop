// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using MSBuild = Microsoft.Build.BuildEngine;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests
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
			MSBuildEngineHelper.InitMSBuildEngine();
			
			info = new ProjectCreateInformation();
			info.Solution = new Solution();
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.pyproj";
			info.RootNamespace = "Test";

			project = new PythonProject(info);
		}
		
		[Test]
		public void Language()
		{
			Assert.AreEqual(PythonLanguageBinding.LanguageName, project.Language);
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
			Assert.Contains(@"$(MSBuildBinPath)\Microsoft.Common.targets", paths, "Could not find Microsoft.Common.targets. Actual imports: " + GetArrayAsString(paths));
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
			int count = project.MSBuildProject.Imports.Count;
			Microsoft.Build.BuildEngine.Import[] imports = new Microsoft.Build.BuildEngine.Import[count];
			project.MSBuildProject.Imports.CopyTo(imports, 0);
			
			string[] paths = new string[count];
			for (int i = 0; i < count; ++i) {
				Microsoft.Build.BuildEngine.Import import = imports[i];
				paths[i] = import.ProjectPath;
			}
			return paths;
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
