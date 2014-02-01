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
