// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using Microsoft.Build.Construction;
using System;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

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
			WixBindingTestsHelper.InitMSBuildEngine();
			
			info = new ProjectCreateInformation();
			info.Solution = new Solution(new MockProjectChangeWatcher());
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.wixproj";
			info.RootNamespace = "Test";

			project = new WixProject(info);
		}
		
		[Test]
		public void Language()
		{
			Assert.AreEqual(WixProjectBinding.LanguageName, project.Language);
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual(info.ProjectName, project.Name);
		}
		
		[Test]
		public void OutputName()
		{
			Assert.AreEqual(info.ProjectName, project.GetEvaluatedProperty("OutputName"));
		}
		
		[Test]
		public void OutputType()
		{
			Assert.AreEqual(WixOutputType.Package.ToString(), project.GetEvaluatedProperty("OutputType"));
		}
		
		[Test]
		public void Imports()
		{
			lock (project.SyncRoot) {
				Assert.AreEqual(1, project.MSBuildProjectFile.Imports.Count());
				Assert.AreEqual(WixProject.DefaultTargetsFile, project.MSBuildProjectFile.Imports.Single().Project);
			}
		}

		[Test]
		public void FirstWixTargetsPathCondition()
		{
			ProjectPropertyElement property = GetMSBuildProperty("WixTargetsPath");
			Assert.AreEqual(" '$(WixTargetsPath)' == '' AND '$(MSBuildExtensionsPath32)' != '' ", property.Condition);
		}

		[Test]
		public void FirstWixTargetsPathValue()
		{
			ProjectPropertyElement property = GetMSBuildProperty("WixTargetsPath");
			Assert.AreEqual(@"$(MSBuildExtensionsPath32)\Microsoft\WiX\v3.x\Wix.targets", property.Value);
		}
		
		[Test]
		public void LastWixTargetsPathCondition()
		{
			ProjectPropertyElement property = GetLastMSBuildProperty("WixTargetsPath");
			Assert.AreEqual(" '$(WixTargetsPath)' == '' ", property.Condition);
		}
		
		[Test]
		public void LastWixTargetsPathValue()
		{
			ProjectPropertyElement property = GetLastMSBuildProperty("WixTargetsPath");
			Assert.AreEqual(@"$(MSBuildExtensionsPath)\Microsoft\WiX\v3.x\Wix.targets", property.Value);
		}
		
		[Test]
		public void DebugConfiguration()
		{
			Assert.AreEqual("Debug", project.GetEvaluatedProperty("Configuration"));
		}
		
		[Test]
		public void FileName()
		{
			Assert.AreEqual(info.OutputProjectFileName, project.FileName);
		}
		
		[Test]
		public void AssemblyName()
		{
			Assert.AreEqual("Test", project.AssemblyName);
		}
		
		[Test]
		public void UnknownProperty()
		{
			IWixPropertyValueProvider provider = (IWixPropertyValueProvider)project;
			Assert.IsNull(provider.GetValue("UnknownMSBuildProperty"));
		}
		
		[Test]
		public void ProjectLanguageProperties()
		{
			Assert.AreEqual(LanguageProperties.None, project.LanguageProperties);
		}
		
		/// <summary>
		/// Gets the MSBuild build property with the specified name from the WixProject.
		/// </summary>
		ProjectPropertyElement GetMSBuildProperty(string name)
		{
			foreach (ProjectPropertyGroupElement propertyGroup in project.MSBuildProjectFile.PropertyGroups) {
				foreach (ProjectPropertyElement element in propertyGroup.Properties) {
					if (element.Name == name) {
						return element;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the last MSBuild build property with the specified name from the WixProject.
		/// </summary>
		ProjectPropertyElement GetLastMSBuildProperty(string name)
		{
			ProjectPropertyElement matchedElement = null;
			foreach (ProjectPropertyGroupElement propertyGroup in project.MSBuildProjectFile.PropertyGroups) {
				foreach (ProjectPropertyElement element in propertyGroup.Properties) {
					if (element.Name == name) {
						matchedElement = element;
					}
				}
			}
			return matchedElement;
		}
	}
}
