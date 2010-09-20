// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		PythonAddInOptions options;
		TextBox pythonFileNameTextBox;
		TextBox pythonLibraryPathTextBox;
		
		public PythonOptionsPanel() : this(new PythonAddInOptions())
		{
		}
		
		public PythonOptionsPanel(PythonAddInOptions options)
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
