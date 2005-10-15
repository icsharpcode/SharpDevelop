// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.NAntAddIn;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace ICSharpCode.NAntAddIn.Gui
{
	/// <summary>
	/// Options panel for the NAnt add-in.
	/// </summary>
	public class NAntAddInOptionPanel : AbstractOptionPanel
	{
		static readonly string commandTextBoxName = "nantCommandTextBox";
		static readonly string argumentsTextBoxName = "argumentsTextBox";
		static readonly string verboseCheckBoxName = "verboseCheckBox";
		static readonly string browseButtonName = "browseButton";
		static readonly string showLogoCheckBoxName = "showLogoCheckBox";
		static readonly string quietCheckBoxName = "quietCheckBox";
		static readonly string debugModeCheckBoxName = "debugModeCheckBox";
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("NAntAddIn.Resources.NAntAddInOptionPanel.xfrm"));
								
			ControlDictionary[commandTextBoxName].Text = AddInOptions.NAntFileName;
			ControlDictionary[argumentsTextBoxName].Text = AddInOptions.NAntArguments;
			((CheckBox)ControlDictionary[verboseCheckBoxName]).Checked = AddInOptions.Verbose;
			((CheckBox)ControlDictionary[showLogoCheckBoxName]).Checked = AddInOptions.ShowLogo;
			((CheckBox)ControlDictionary[quietCheckBoxName]).Checked = AddInOptions.Quiet;
			((CheckBox)ControlDictionary[debugModeCheckBoxName]).Checked = AddInOptions.DebugMode;
			
			ControlDictionary[browseButtonName].Click += new EventHandler(OnBrowse);
		}
		
		public override bool StorePanelContents()
		{					
			AddInOptions.NAntFileName = ControlDictionary[commandTextBoxName].Text;
			AddInOptions.NAntArguments = ControlDictionary[argumentsTextBoxName].Text;
			AddInOptions.Verbose = ((CheckBox)ControlDictionary[verboseCheckBoxName]).Checked;
			AddInOptions.ShowLogo = ((CheckBox)ControlDictionary[showLogoCheckBoxName]).Checked;
			AddInOptions.Quiet = ((CheckBox)ControlDictionary[quietCheckBoxName]).Checked;
			AddInOptions.DebugMode = ((CheckBox)ControlDictionary[debugModeCheckBoxName]).Checked;
			
			return true;
		}
		
		/// <summary>
		/// Allows the user to browse for the NAnt executable.
		/// </summary>
		void OnBrowse(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog  = new OpenFileDialog()) {
				
				openFileDialog.CheckFileExists = true;
				openFileDialog.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				
				if (openFileDialog.ShowDialog() == DialogResult.OK) {
					ControlDictionary[commandTextBoxName].Text = openFileDialog.FileName;
				}
			}			
		}
	}
}
