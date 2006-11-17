// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests the initial properties set in a newly created WixProject.
	/// </summary>
	[TestFixture]
	public class CreateNewWixProjectObjectTestFixture
	{
		ProjectCreateInformation info;
		WixProject project;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			info = new ProjectCreateInformation();
			info.Solution = new Solution();
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.wixproj";
			info.RootNamespace = "Test";

			project = new WixProject(info);
		}
		
		[Test]
		public void Language()
		{
			Assert.AreEqual(WixLanguageBinding.LanguageName, project.Language);
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual(info.ProjectName, project.Name);
		}
		
		[Test]
		public void OutputName()
		{
			Assert.AreEqual(info.ProjectName, project.GetProperty("OutputName"));
		}
		
		[Test]
		public void OutputType()
		{
			Assert.AreEqual(WixOutputType.package.ToString(), project.GetProperty("OutputType"));
		}
		
		[Test]
		public void Imports()
		{
			Assert.AreEqual(1, project.MSBuildProject.Imports.Count);
			Microsoft.Build.BuildEngine.Import[] imports = { null };
			project.MSBuildProject.Imports.CopyTo(imports, 0);
			Assert.AreEqual(imports[0].ProjectPath, WixProject.DefaultTargetsFile);
		}
		
		[Test]
		public void WixToolPath()
		{
			Assert.AreEqual(@"$(SharpDevelopBinPath)\Tools\Wix", project.GetUnevalatedProperty("WixToolPath"));
		}
		
		[Test]
		public void ToolPath()
		{
			Assert.AreEqual(@"$(WixToolPath)", project.GetUnevalatedProperty("ToolPath"));
		}
		
		[Test]
		public void WixMSBuildExtensionsPath()
		{
			Assert.AreEqual(@"$(SharpDevelopBinPath)\Tools\Wix", project.GetUnevalatedProperty("WixMSBuildExtensionsPath"));
		}
		
		[Test]
		public void DebugConfiguration()
		{
			Assert.AreEqual("Debug", project.GetProperty("Configuration"));
		}
		
		[Test]
		public void FileName()
		{
			Assert.AreEqual(info.OutputProjectFileName, project.FileName);
		}
	}
}
