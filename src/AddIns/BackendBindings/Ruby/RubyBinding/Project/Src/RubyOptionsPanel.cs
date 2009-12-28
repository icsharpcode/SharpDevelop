// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Panel that displays the Ruby options.
	/// </summary>
	public class RubyOptionsPanel : AbstractOptionPanel
	{
		AddInOptions options;
		TextBox rubyFileNameTextBox;
		
		public RubyOptionsPanel() : this(new AddInOptions())
		{
		}
		
		public RubyOptionsPanel(AddInOptions options)
		{
			this.options = options;
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream("ICSharpCode.RubyBinding.Resources.RubyOptionsPanel.xfrm"));
			
			rubyFileNameTextBox = (TextBox)ControlDictionary["rubyFileNameTextBox"];
			rubyFileNameTextBox.Text = options.RubyFileName;
			
			ConnectBrowseButton("browseButton", "rubyFileNameTextBox", "${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe", TextBoxEditMode.EditRawProperty);
		}
		
		public override bool StorePanelContents()
		{
			options.RubyFileName = rubyFileNameTextBox.Text;
			return true;
		}
	}
}
