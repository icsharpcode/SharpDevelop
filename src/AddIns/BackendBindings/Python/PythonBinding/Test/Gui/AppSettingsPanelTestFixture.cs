// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// Tests the ApplicationSettingsPanel class.
	/// </summary>
	[TestFixture]
	public class AppSettingsPanelTestFixture
	{
		DerivedApplicationSettingsPanel appSettingsPanel;
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
			
			appSettingsPanel = new DerivedApplicationSettingsPanel();
			appSettingsPanel.Owner = project;
			appSettingsPanel.LoadPanelContents();
		}
		
		[TearDown]
		public void TearDown()
		{
			appSettingsPanel.Dispose();
		}
		
		[Test]
		public void IsHelperInitialised()
		{
			Assert.IsNotNull(appSettingsPanel.Helper);
		}
		
		[Test]
		public void SetupFromManifestStreamResourceName()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.Resources.ApplicationSettingsPanel.xfrm", appSettingsPanel.SetupFromManifestResourceName);
		}

		[Test]
		public void AssemblyNameBoundToTextBox()
		{
			Assert.AreEqual("assemblyNameTextBox", appSettingsPanel.GetBoundStringControlName("AssemblyName"));
		}
		
		[Test]
		public void AssemblyNameTextEditBindMode()
		{
			Assert.AreEqual(TextBoxEditMode.EditEvaluatedProperty, appSettingsPanel.GetBoundControlTextBoxEditMode("AssemblyName"));
		}
		
		[Test]
		public void AssemblyNameTextBoxLocationButtonCreated()
		{
			Assert.IsTrue(appSettingsPanel.IsLocationButtonCreated("assemblyNameTextBox"));
		}
		
		[Test]
		public void RootNamespaceBoundToTextBox()
		{
			Assert.AreEqual("rootNamespaceTextBox", appSettingsPanel.GetBoundStringControlName("RootNamespace"));
		}
		
		[Test]
		public void RootNamespaceTextEditBindMode()
		{
			Assert.AreEqual(TextBoxEditMode.EditEvaluatedProperty, appSettingsPanel.GetBoundControlTextBoxEditMode("RootNamespace"));
		}
		
		[Test]
		public void RootNamespaceTextBoxLocationButtonCreated()
		{
			Assert.IsTrue(appSettingsPanel.IsLocationButtonCreated("rootNamespaceTextBox"));
		}
		
		[Test]
		public void ProjectFolder()
		{
			Assert.AreEqual(project.Directory, appSettingsPanel.Get<TextBox>("projectFolder").Text);
		}
		
		[Test]
		public void ProjectFile()
		{
			Assert.AreEqual(Path.GetFileName(project.FileName), appSettingsPanel.Get<TextBox>("projectFile").Text);
		}
		
		[Test]
		public void ProjectFileTextBoxIsReadOnly()
		{
			Assert.IsTrue(appSettingsPanel.Get<TextBox>("projectFile").ReadOnly);
		}
		
		[Test]
		public void AddConfigurationSelectorCalled()
		{
			Assert.IsTrue(appSettingsPanel.ConfigurationSelectorAddedToControl);
		}
		
		[Test]
		public void OutputTypeBoundToComboBox()
		{
			Assert.AreEqual("outputTypeComboBox", appSettingsPanel.GetBoundEnumControlName("OutputType"));
		}
		
		[Test]
		public void OutputTypeLocationButtonCreated()
		{
			Assert.IsTrue(appSettingsPanel.IsLocationButtonCreated("outputTypeComboBox"));
		}		
		
		[Test]
		public void OutputName()
		{
			Assert.AreEqual("Test.exe", appSettingsPanel.Get<TextBox>("outputName").Text);
		}
		
		[Test]
		public void MainFileBoundToTextBox()
		{
			Assert.AreEqual("mainFileComboBox", appSettingsPanel.GetBoundStringControlName("MainFile"));
		}
		
		[Test]
		public void MainFileTextEditBindMode()
		{
			Assert.AreEqual(TextBoxEditMode.EditEvaluatedProperty, appSettingsPanel.GetBoundControlTextBoxEditMode("MainFile"));
		}
		
		[Test]
		public void MainFileTextBoxLocationButtonCreated()
		{
			Assert.IsTrue(appSettingsPanel.IsLocationButtonCreated("mainFileComboBox"));
		}
		
		[Test]
		public void MainFileBrowseButtonConnected()
		{
			Assert.AreEqual("mainFileComboBox", GetMainFileBrowseInfo().Target);
		}
		
		[Test]
		public void MainFileBrowseButtonTextEditMode()
		{
			Assert.AreEqual(TextBoxEditMode.EditEvaluatedProperty, GetMainFileBrowseInfo().TextBoxEditMode);
		}
		
		[Test]
		public void MainFileBrowseButtonFileFilter()
		{
			Assert.AreEqual("${res:SharpDevelop.FileFilter.AllFiles}|*.*", GetMainFileBrowseInfo().FileFilter);
		}	
		
		[Test]
		public void OutputNameChangedAfterAssemblyNameChanged()
		{
			appSettingsPanel.Get<TextBox>("assemblyName").Text = "PythonTest";
			appSettingsPanel.CallAssemblyNameTextBoxTextChanged();
			Assert.AreEqual("PythonTest.exe", appSettingsPanel.Get<TextBox>("outputName").Text);
		}
		
		[Test]
		public void OutputTypeComboBoxSelectedIndexChanged()
		{
			appSettingsPanel.Get<ComboBox>("outputType").SelectedIndex = 2;
			appSettingsPanel.CallOutputTypeComboBoxSelectedIndexChanged();
			Assert.AreEqual("Test.dll", appSettingsPanel.Get<TextBox>("outputName").Text);
		}

		BrowseButtonInfo GetMainFileBrowseInfo()
		{
			return appSettingsPanel.GetBrowseButtonInfo("mainFileBrowseButton");
		}
	}
}
