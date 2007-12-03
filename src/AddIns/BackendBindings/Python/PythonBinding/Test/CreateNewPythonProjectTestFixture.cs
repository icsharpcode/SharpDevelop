// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
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
		public void TwoImports()
		{
			Assert.AreEqual(2, project.MSBuildProject.Imports.Count);
		}
		
		[Test]
		public void Imports()
		{
			Microsoft.Build.BuildEngine.Import[] imports = {null, null};
			project.MSBuildProject.Imports.CopyTo(imports, 0);
			
			string[] paths = new string[] {imports[0].ProjectPath, imports[1].ProjectPath};
			
			Assert.Contains(PythonProject.DefaultTargetsFile, paths);
			Assert.Contains(@"$(MSBuildBinPath)\Microsoft.Common.targets", paths);
		}

		[Test]
		public void ProjectLanguageProperties()
		{
			Assert.AreEqual(LanguageProperties.None, project.LanguageProperties);
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
	}
}
