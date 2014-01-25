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
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Gui
{
	/// <summary>
	/// Tests the CompilingOptionsPanel.
	/// </summary>
	[TestFixture]
	public class CompilingOptionsPanelTestFixture
	{
		DerivedCompilingOptionsPanel compilingOptionsPanel;
		PythonProject project;
		
		[SetUp]
		public void SetUp()
		{
			PythonMSBuildEngineHelper.InitMSBuildEngine();
			
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.Solution = new Solution(new MockProjectChangeWatcher());
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.pyproj";
			info.RootNamespace = "Test";
			
			project = new PythonProject(info);
			
			compilingOptionsPanel = new DerivedCompilingOptionsPanel();
			compilingOptionsPanel.Owner = project;
			compilingOptionsPanel.LoadPanelContents();
		}
		
		[TearDown]
		public void TearDown()
		{
			compilingOptionsPanel.Dispose();
		}
		
		[Test]
		public void IsHelperInitialised()
		{
			Assert.IsNotNull(compilingOptionsPanel.Helper);
		}
		
		[Test]
		public void SetupFromManifestStreamResourceName()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.Resources.CompilingOptionsPanel.xfrm", compilingOptionsPanel.SetupFromManifestResourceName);
		}		

		[Test]
		public void OutputPathBoundToTextBox()
		{
			Assert.AreEqual("outputPathTextBox", compilingOptionsPanel.GetBoundStringControlName("OutputPath"));
		}
		
		[Test]
		public void OutputPathTextEditBindMode()
		{
			Assert.AreEqual(TextBoxEditMode.EditRawProperty, compilingOptionsPanel.GetBoundControlTextBoxEditMode("OutputPath"));
		}	
		
		[Test]
		public void OutputPathTextBoxLocationButtonCreated()
		{
			Assert.IsTrue(compilingOptionsPanel.IsLocationButtonCreated("outputPathTextBox"));
		}
		
		[Test]
		public void OutputPathBrowseButtonConnected()
		{
			Assert.AreEqual("outputPathTextBox", GetOutputPathBrowseFolderInfo().Target);
		}
		
		[Test]
		public void OutputPathBrowseButtonTextEditMode()
		{
			Assert.AreEqual(TextBoxEditMode.EditRawProperty, GetOutputPathBrowseFolderInfo().TextBoxEditMode);
		}
		
		[Test]
		public void OutputPathBrowseButtonFilter()
		{
			Assert.AreEqual("${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}", GetOutputPathBrowseFolderInfo().Description);
		}	
		
		[Test]
		public void DebugInfoPathBoundToCheckBox()
		{
			Assert.AreEqual("debugInfoCheckBox", compilingOptionsPanel.GetBoundBooleanControlName("DebugInfo"));
		}

		[Test]
		public void DebugInfoTextBoxLocationButtonCreated()
		{
			Assert.IsTrue(compilingOptionsPanel.IsLocationButtonCreated("debugInfoCheckBox"));
		}	
		
		[Test]
		public void AddConfigurationSelectorCalled()
		{
			Assert.IsTrue(compilingOptionsPanel.ConfigurationSelectorAddedToControl);
		}		
		
		[Test]
		public void TargetCpuComboxBoxAdded()
		{
			Assert.IsTrue(compilingOptionsPanel.IsTargetCpuComboBoxCreated);
		}
		
		[Test]
		public void TargetCpuComboxBoxLocationButtonCreated()
		{
			Assert.IsTrue(compilingOptionsPanel.IsLocationButtonCreated("targetCpuComboBox"));
		}	

		BrowseFolderButtonInfo GetOutputPathBrowseFolderInfo()
		{
			return compilingOptionsPanel.GetBrowseFolderButtonInfo("outputPathBrowseButton");
		}		
	}
}
