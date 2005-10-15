// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	/// Represents a <see cref="NAntBuildTarget"/> in the
	/// <see cref="NAntPadTreeView"/>.
	/// </summary>
	public class NAntBuildTargetTreeNode : TreeNode
	{
		NAntBuildTarget target;
		
		public NAntBuildTargetTreeNode(NAntBuildTarget target)
		{
			if (target.IsDefault) {
				this.Text = String.Concat(target.Name, " [default]");
				this.ImageIndex = NAntPadTreeViewImageList.DefaultTargetImage;
				this.SelectedImageIndex = NAntPadTreeViewImageList.DefaultTargetImage;
				this.ForeColor = Color.Blue;
			} else {
				this.Text = target.Name;
				this.ImageIndex = NAntPadTreeViewImageList.TargetImage;
				this.SelectedImageIndex = NAntPadTreeViewImageList.TargetImage;
			}
			
			this.target = target;
		}
		
		/// <summary>
		/// Gets the <see cref="NAntBuildTarget"/>
		/// associated with this node.
		/// </summary>
		public NAntBuildTarget Target {
			get {
				return target;
			}
		}
	}
}
