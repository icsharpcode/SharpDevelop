// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Panel that displays the Ruby options.
	/// </summary>
	public class RubyOptionsPanel : XmlFormsOptionPanel
	{
		RubyAddInOptions options;
		TextBox rubyFileNameTextBox;
		TextBox rubyLibraryPathTextBox;
		
		public RubyOptionsPanel() : this(new RubyAddInOptions())
		{
		}
		
		public RubyOptionsPanel(RubyAddInOptions options)
		{
			this.options = options;
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream("ICSharpCode.RubyBinding.Resources.RubyOptionsPanel.xfrm"));
			
			rubyFileNameTextBox = (TextBox)ControlDictionary["rubyFileNameTextBox"];
			rubyFileNameTextBox.Text = options.RubyFileName;
			rubyLibraryPathTextBox = (TextBox)ControlDictionary["rubyLibraryPathTextBox"];
			rubyLibraryPathTextBox.Text = options.RubyLibraryPath;
			
			ConnectBrowseButton("browseButton", "rubyFileNameTextBox", "${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe", TextBoxEditMode.EditRawProperty);
		}
		
		public override bool StorePanelContents()
		{
			options.RubyFileName = rubyFileNameTextBox.Text;
			options.RubyLibraryPath = rubyLibraryPathTextBox.Text;
			return true;
		}
	}
}
