// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Displays information about the conversion to MSBuild 3.5 and allows to choose whether
	/// all projects should be converted/whether the target framework should be changed.
	/// </summary>
	public partial class ConvertToMSBuild35Dialog : Form
	{
		public ConvertToMSBuild35Dialog(string newLanguageVersion)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		
		public bool ForceConvertAllProjects {
			get { return !convertAllProjectsCheckBox.Enabled; }
			set {
				convertAllProjectsCheckBox.Enabled = !value;
				if (value)
					convertAllProjectsCheckBox.Checked = true;
			}
		}
		
		public bool ChangeTargetFramework {
			get { return changeTargetFrameworkCheckBox.Checked; }
		}
		
		public bool ConvertAllProjects {
			get { return convertAllProjectsCheckBox.Checked; }
		}
	}
}
