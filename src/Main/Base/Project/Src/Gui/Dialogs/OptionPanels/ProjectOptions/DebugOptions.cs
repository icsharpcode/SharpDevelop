// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

using RadioBinding = System.Collections.Generic.KeyValuePair<ICSharpCode.SharpDevelop.Project.StartAction, System.Windows.Forms.RadioButton>;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class DebugOptions : AbstractProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.DebugOptions.xfrm");
			ConnectBrowseButton("startExternalProgramBrowseButton", "startExternalProgramTextBox", "${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			ConnectBrowseFolder("workingDirectoryBrowseButton", "workingDirectoryTextBox");
			
			InitializeHelper();
			
			helper.BindRadioEnum("StartAction",
			                     new RadioBinding(StartAction.Project, Get<RadioButton>("startProject")),
			                     new RadioBinding(StartAction.Program, Get<RadioButton>("startExternalProgram")),
			                     new RadioBinding(StartAction.StartURL, Get<RadioButton>("startBrowserInURL")));
			
			Get<RadioButton>("startExternalProgram").CheckedChanged += UpdateEnabledStates;
			Get<RadioButton>("startBrowserInURL").CheckedChanged += UpdateEnabledStates;
			
			helper.BindString("startExternalProgramTextBox", "StartProgram");
			helper.BindString("startBrowserInURLTextBox", "StartURL");
			helper.BindString("commandLineArgumentsTextBox", "StartArguments");
			helper.BindString("workingDirectoryTextBox", "StartWorkingDirectory");
			
			UpdateEnabledStates(this, EventArgs.Empty);
		}
		
		void UpdateEnabledStates(object sender, EventArgs e)
		{
			Get<TextBox>("startExternalProgram").Enabled = Get<Button>("startExternalProgramBrowse").Enabled = Get<RadioButton>("startExternalProgram").Checked;
			Get<TextBox>("startBrowserInURL").Enabled    = Get<RadioButton>("startBrowserInURL").Checked;
		}
	}
}
