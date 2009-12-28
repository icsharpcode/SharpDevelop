// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;

namespace RubyBinding.Tests
{
	/// <summary>
	/// Tests the RubyOptionsPanel.
	/// </summary>
	[TestFixture]
	public class RubyOptionsPanelTestFixture
	{
		RubyOptionsPanel optionsPanel;
		Properties properties;
		AddInOptions options;
		TextBox fileNameTextBox;
		
		[SetUp]
		public void SetUp()
		{
			properties = new Properties();
			options = new AddInOptions(properties);
			options.RubyFileName = @"C:\Ruby\ir.exe";
			optionsPanel = new RubyOptionsPanel(options);
			optionsPanel.LoadPanelContents();
			fileNameTextBox = (TextBox)optionsPanel.ControlDictionary["rubyFileNameTextBox"];
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
		public void PanelIsOptionsPanel()
		{
			Assert.IsNotNull(optionsPanel as AbstractOptionPanel);
		}
		
		[Test]
		public void SaveOptions()
		{
			string fileName = @"C:\Program Files\IronRuby\ir.exe";
			fileNameTextBox.Text = fileName;
			optionsPanel.StorePanelContents();
			Assert.AreEqual(fileName, options.RubyFileName);
		}
	}
}
