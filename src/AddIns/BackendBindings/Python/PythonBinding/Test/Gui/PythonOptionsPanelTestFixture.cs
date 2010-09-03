// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using NUnit.Framework;

namespace PythonBinding.Tests.Gui
{
	/// <summary>
	/// Tests the PythonOptionsPanel.
	/// </summary>
	[TestFixture]
	public class PythonOptionsPanelTestFixture
	{
		PythonOptionsPanel optionsPanel;
		Properties properties;
		PythonAddInOptions options;
		TextBox fileNameTextBox;
		TextBox pythonLibraryPathTextBox;
		
		[SetUp]
		public void SetUp()
		{
			properties = new Properties();
			options = new PythonAddInOptions(properties);
			options.PythonFileName = @"C:\Python\ipy.exe";
			options.PythonLibraryPath = @"C:\Python26\lib";
			optionsPanel = new PythonOptionsPanel(options);
			optionsPanel.LoadPanelContents();
			fileNameTextBox = (TextBox)optionsPanel.ControlDictionary["pythonFileNameTextBox"];
			pythonLibraryPathTextBox = (TextBox)optionsPanel.ControlDictionary["pythonLibraryPathTextBox"];
		}
		
		[TearDown]
		public void TearDown()
		{
			optionsPanel.Dispose();
		}
		
		[Test]
		public void PythonFileNameDisplayed()
		{
			Assert.AreEqual(@"C:\Python\ipy.exe", fileNameTextBox.Text);
		}
		
		[Test]
		public void PythonLibraryPathIsDisplayed()
		{
			Assert.AreEqual(@"C:\Python26\lib", pythonLibraryPathTextBox.Text);
		}
		
		[Test]
		public void PanelIsOptionsPanel()
		{
			Assert.IsNotNull(optionsPanel as XmlFormsOptionPanel);
		}
		
		[Test]
		public void SavingOptionsUpdatesIronPythonFileName()
		{
			string fileName = @"C:\Program Files\IronPython\ipy.exe";
			fileNameTextBox.Text = fileName;
			optionsPanel.StorePanelContents();
			Assert.AreEqual(fileName, options.PythonFileName);
		}
		
		[Test]
		public void SavingOptionsUpdatesIronPythonLibraryPath()
		{
			string path = @"c:\Program Files\Python\lib";
			pythonLibraryPathTextBox.Text = path;
			optionsPanel.StorePanelContents();
			Assert.AreEqual(path, options.PythonLibraryPath);
		}
	}
}
