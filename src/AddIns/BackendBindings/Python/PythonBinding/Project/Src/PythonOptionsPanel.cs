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
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Panel that displays the python options.
	/// </summary>
	public class PythonOptionsPanel : XmlFormsOptionPanel
	{
		AddInOptions options;
		TextBox pythonFileNameTextBox;
		TextBox pythonLibraryPathTextBox;
		
		public PythonOptionsPanel() : this(new AddInOptions())
		{
		}
		
		public PythonOptionsPanel(AddInOptions options)
		{
			this.options = options;
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream("ICSharpCode.PythonBinding.Resources.PythonOptionsPanel.xfrm"));
			
			pythonFileNameTextBox = (TextBox)ControlDictionary["pythonFileNameTextBox"];
			pythonFileNameTextBox.Text = options.PythonFileName;
			
			pythonLibraryPathTextBox = (TextBox)ControlDictionary["pythonLibraryPathTextBox"];
			pythonLibraryPathTextBox.Text = options.PythonLibraryPath;
			
			ConnectBrowseButton("browseButton", "pythonFileNameTextBox", "${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe", TextBoxEditMode.EditRawProperty);
		}
		
		public override bool StorePanelContents()
		{
			options.PythonFileName = pythonFileNameTextBox.Text;
			options.PythonLibraryPath = pythonLibraryPathTextBox.Text;
			return true;
		}
	}
}
