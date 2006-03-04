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

namespace ICSharpCode.Svn
{
	public class TortoiseSvnNotFoundForm : XmlForm
	{		
		public TortoiseSvnNotFoundForm()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.Svn.Resources.TortoiseSvnNotFoundForm.xfrm"));
		
			((Label)ControlDictionary["messageLabel"]).Text = StringParser.Parse("TortoiseSVN needs to be installed to execute this action.");
			((PictureBox)ControlDictionary["iconPictureBox"]).Image = ResourceService.GetBitmap("Icons.32x32.Information");
			((LinkLabel)ControlDictionary["linkLabel"]).Click += LinkLabelClicked;
		}
		
		void LinkLabelClicked(object sender, EventArgs e)
		{
			Process.Start("http://tortoisesvn.tigris.org");
		}
	}
}
