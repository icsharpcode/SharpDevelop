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

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Panel that displays the python options.
	/// </summary>
	public class PythonOptionsPanel : AbstractOptionPanel
	{
		AddInOptions options;
		TextBox pythonFileNameTextBox;
		
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
			
			ConnectBrowseButton("browseButton", "pythonFileNameTextBox", "${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe", TextBoxEditMode.EditRawProperty);
		}
		
		public override bool StorePanelContents()
		{
			options.PythonFileName = pythonFileNameTextBox.Text;
			return true;
		}
	}
}
