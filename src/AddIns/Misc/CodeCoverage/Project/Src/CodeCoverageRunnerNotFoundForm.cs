// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageRunnerNotFoundForm : XmlForm
	{		
		public CodeCoverageRunnerNotFoundForm()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.CodeCoverage.Resources.CodeCoverageRunnerNotFoundForm.xfrm"));
		
			((Label)ControlDictionary["messageLabel"]).Text = "Unable to locate the NCover console application.\n\nIf NCover is installed, please specify the location of the application in the Code Coverage options.";
			((PictureBox)ControlDictionary["iconPictureBox"]).Image = ResourceService.GetBitmap("Icons.32x32.Information");
			((LinkLabel)ControlDictionary["linkLabel"]).Click += LinkLabelClicked;
		}
		
		void LinkLabelClicked(object sender, EventArgs e)
		{
			Process.Start("http://ncover.org");
		}
	}
}
