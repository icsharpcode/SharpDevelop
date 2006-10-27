// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Command that adds a new Wix element to the selected WixPackageTreeView node.
	/// </summary>
	public class AddElementCommand : ToolStripMenuItem
	{
		string name;
		
		public AddElementCommand(string name) : base(name)
		{
			this.name = name;
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			PackageFilesView.ActiveView.AddElement(name);
		}
	}
}
