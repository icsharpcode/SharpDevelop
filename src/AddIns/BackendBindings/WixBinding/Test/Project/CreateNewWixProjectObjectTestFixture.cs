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
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using NUnit.Framework;
using Rhino.Mocks;
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
			SD.InitializeForUnitTests();
			WixBindingTestsHelper.InitMSBuildEngine();
			
			info = new ProjectCreateInformation(MockSolution.Create(), new FileName(@"C:\Projects\Test\Test.wixproj"));
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
			Assert.AreEqual(info.FileName, project.FileName);
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
		
		/// <summary>
		/// Gets the MSBuild build property with the specified name from the WixProject.
		/// </summary>
		ProjectPropertyElement GetMSBuildProperty(string name)
		{
			lock (project.SyncRoot) {
				foreach (ProjectPropertyGroupElement propertyGroup in project.MSBuildProjectFile.PropertyGroups) {
					foreach (ProjectPropertyElement element in propertyGroup.Properties) {
						if (element.Name == name) {
							return element;
						}
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
			lock (project.SyncRoot) {
				foreach (ProjectPropertyGroupElement propertyGroup in project.MSBuildProjectFile.PropertyGroups) {
					foreach (ProjectPropertyElement element in propertyGroup.Properties) {
						if (element.Name == name) {
							matchedElement = element;
						}
					}
				}
			}
			return matchedElement;
		}
	}
}
