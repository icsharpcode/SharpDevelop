// SharpDevelop samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.NAnt.Gui
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
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.NAnt.Resources.NAntAddInOptionPanel.xfrm"));
								
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
