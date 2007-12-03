// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;

namespace PythonBinding.Tests
{
	/// <summary>
	/// Tests the PythonOptionsPanel.
	/// </summary>
	[TestFixture]
	public class PythonOptionsPanelTestFixture
	{
		PythonOptionsPanel optionsPanel;
		Properties properties;
		AddInOptions options;
		TextBox fileNameTextBox;
		
		[SetUp]
		public void SetUp()
		{
			properties = new Properties();
			options = new AddInOptions(properties);
			options.PythonFileName = @"C:\Python\ipy.exe";
			optionsPanel = new PythonOptionsPanel(options);
			optionsPanel.LoadPanelContents();
			fileNameTextBox = (TextBox)optionsPanel.ControlDictionary["pythonFileNameTextBox"];
		}
		
		[TearDown]
		public void TearDown()
		{
			optionsPanel.Dispose();
		}
		
		[Test]
		public void PythonFileNameDisplayed()
		{
			Assert.AreEqual(options.PythonFileName, fileNameTextBox.Text);
		}
		
		[Test]
		public void PanelIsOptionsPanel()
		{
			Assert.IsNotNull(optionsPanel as AbstractOptionPanel);
		}
		
		[Test]
		public void SaveOptions()
		{
			string fileName = @"C:\Program Files\IronPython\ipy.exe";
			fileNameTextBox.Text = fileName;
			optionsPanel.StorePanelContents();
			Assert.AreEqual(fileName, options.PythonFileName);
		}
	}
}
