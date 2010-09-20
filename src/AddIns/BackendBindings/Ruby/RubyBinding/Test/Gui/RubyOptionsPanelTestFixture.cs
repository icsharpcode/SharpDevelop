// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using NUnit.Framework;

namespace RubyBinding.Tests.Gui
{
	/// <summary>
	/// Tests the RubyOptionsPanel.
	/// </summary>
	[TestFixture]
	public class RubyOptionsPanelTestFixture
	{
		RubyOptionsPanel optionsPanel;
		Properties properties;
		RubyAddInOptions options;
		TextBox fileNameTextBox;
		TextBox rubyLibraryPathTextBox;
		
		[SetUp]
		public void SetUp()
		{
			properties = new Properties();
			options = new RubyAddInOptions(properties);
			options.RubyFileName = @"C:\Ruby\ir.exe";
			options.RubyLibraryPath = @"C:\Ruby\lib";
			optionsPanel = new RubyOptionsPanel(options);
			optionsPanel.LoadPanelContents();
			fileNameTextBox = (TextBox)optionsPanel.ControlDictionary["rubyFileNameTextBox"];
			rubyLibraryPathTextBox = (TextBox)optionsPanel.ControlDictionary["rubyLibraryPathTextBox"];
		}
		
		[TearDown]
		public void TearDown()
		{
			optionsPanel.Dispose();
		}
		
		[Test]
		public void RubyFileNameDisplayed()
		{
			Assert.AreEqual(options.RubyFileName, fileNameTextBox.Text);
		}
		
		[Test]
		public void RubyLibraryPathDisplayed()
		{
			Assert.AreEqual(options.RubyLibraryPath, rubyLibraryPathTextBox.Text);
		}
		
		[Test]
		public void PanelIsOptionsPanel()
		{
			Assert.IsNotNull(optionsPanel as XmlFormsOptionPanel);
		}
		
		[Test]
		public void SavingOptionsUpdatesRubyFileName()
		{
			string fileName = @"C:\Program Files\IronRuby\ir.exe";
			fileNameTextBox.Text = fileName;
			optionsPanel.StorePanelContents();
			Assert.AreEqual(fileName, options.RubyFileName);
		}
		
		[Test]
		public void SavingOptionsUpdatesRubyLibraryPath()
		{
			string path = @"c:\Program Files\Python\lib";
			rubyLibraryPathTextBox.Text = path;
			optionsPanel.StorePanelContents();
			Assert.AreEqual(path, options.RubyLibraryPath);
		}
	}
}
