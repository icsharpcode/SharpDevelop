// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.Svn
{
	public class TortoiseSvnNotFoundForm : BaseSharpDevelopForm
	{
		public TortoiseSvnNotFoundForm()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.Svn.Resources.TortoiseSvnNotFoundForm.xfrm"));
			
			((Label)ControlDictionary["messageLabel"]).Text = StringParser.Parse("${res:AddIns.Subversion.TortoiseSVNRequired}");
			((PictureBox)ControlDictionary["iconPictureBox"]).Image = ResourceService.GetBitmap("Icons.32x32.Information");
			((LinkLabel)ControlDictionary["linkLabel"]).Click += LinkLabelClicked;
		}
		
		void LinkLabelClicked(object sender, EventArgs e)
		{
			try {
				Process.Start("http://tortoisesvn.tigris.org");
			} catch (Exception ex) {
				LoggingService.Warn("Cannot start http://tortoisesvn.tigris.org", ex);
			}
		}
	}
}
