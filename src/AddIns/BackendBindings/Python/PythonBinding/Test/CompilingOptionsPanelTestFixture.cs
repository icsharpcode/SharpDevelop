// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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

namespace PythonBinding.Tests
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
			MSBuildEngineHelper.InitMSBuildEngine();
			
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.Solution = new Solution();
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.pyproj";
			info.RootNamespace = "Test";
			
			project = new PythonProject(info);
			
			compilingOptionsPanel = new DerivedCompilingOptionsPanel();
			compilingOptionsPanel.CustomizationObject = project;
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
