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
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.wixproj";

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
			Assert.AreEqual(info.ProjectName, project.BaseConfiguration["OutputName"]);
		}
		
		[Test]
		public void OutputType()
		{
			Assert.AreEqual(WixOutputType.package.ToString(), project.BaseConfiguration["OutputType"]);
		}
		
		[Test]
		public void Imports()
		{
			MSBuildImport wixProjectImport = new MSBuildImport(WixProject.DefaultTargetsFile);
			bool found = false;
			foreach (MSBuildImport import in project.Imports) {
				if (import.Project == wixProjectImport.Project) {
					found = true;
					break;
				}
			}
			Assert.IsTrue(found);
		}
		
		[Test]
		public void WixToolPath()
		{
			Assert.AreEqual(@"$(SharpDevelopBinPath)\Tools\Wix", project.BaseConfiguration["WixToolPath"]);
		}
		
		[Test]
		public void ToolPath()
		{
			Assert.AreEqual(@"$(WixToolPath)", project.BaseConfiguration["ToolPath"]);
		}
		
		[Test]
		public void WixMSBuildExtensionsPath()
		{
			Assert.AreEqual(@"$(SharpDevelopBinPath)\Tools\Wix", project.BaseConfiguration["WixMSBuildExtensionsPath"]);
		}
		
		[Test]
		public void DebugConfiguration()
		{
			Assert.AreEqual("Debug", project.BaseConfiguration["Configuration"]);
		}
		
		[Test]
		public void DebugBaseOutputPath()
		{
			Assert.AreEqual(@"obj\", project.Configurations["Debug|*"]["BaseOutputPath"]);
		}
		
		[Test]
		public void DebugIntermediateOutputPath()
		{
			Assert.AreEqual(@"obj\Debug\", project.Configurations["Debug|*"]["IntermediateOutputPath"]);
		}

		[Test]
		public void DebugOutputPath()
		{
			Assert.AreEqual(@"bin\Debug\", project.Configurations["Debug|*"]["OutputPath"]);
		}
				
		[Test]
		public void ReleaseBaseOutputPath()
		{
			Assert.AreEqual(@"obj\", project.Configurations["Release|*"]["BaseOutputPath"]);
		}
		
		[Test]
		public void ReleaseIntermediateOutputPath()
		{
			Assert.AreEqual(@"obj\Release\", project.Configurations["Release|*"]["IntermediateOutputPath"]);
		}

		[Test]
		public void ReleaseOutputPath()
		{
			Assert.AreEqual(@"bin\Release\", project.Configurations["Release|*"]["OutputPath"]);
		}
		
		[Test]
		public void FileName()
		{
			Assert.AreEqual(info.OutputProjectFileName, project.FileName);
		}
	}
}
