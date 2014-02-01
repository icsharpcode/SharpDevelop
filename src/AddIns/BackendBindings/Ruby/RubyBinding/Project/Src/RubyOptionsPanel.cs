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
