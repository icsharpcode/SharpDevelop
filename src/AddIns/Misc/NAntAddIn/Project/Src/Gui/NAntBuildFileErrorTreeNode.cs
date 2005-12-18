// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NAntAddIn;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.NAntAddIn.Gui
{
	/// <summary>
	/// Represents a <see cref="NAntBuildFile"/> error in the
	/// <see cref="NAntPadTreeView"/>.
	/// </summary>
	public class NAntBuildFileErrorTreeNode : TreeNode
	{
		NAntBuildFileError buildFileError;
		public NAntBuildFileErrorTreeNode(NAntBuildFileError error)
		{
			this.Text = error.Message;
			this.ImageIndex = NAntPadTreeViewImageList.TargetErrorImage;
			this.SelectedImageIndex = NAntPadTreeViewImageList.TargetErrorImage;
			this.buildFileError = error;
		}
		
		public NAntBuildFileError Error {
			get {
				return buildFileError;
			}
		}
		
	}
}
