// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Description of ExtFolderNode.
	/// </summary>
	public class ExtFolderNode : ExtTreeNode
	{
		string closedIcon = null;
		string openedIcon = null;
		
		public string ClosedIcon {
			get {
				return closedIcon;
			}
			set {
				closedIcon = value;
				if (closedIcon != null && !IsExpanded) {
					SetIcon(closedIcon);
				}
			}
		}
		
		public string OpenedIcon {
			get {
				return openedIcon;
			}
			set {
				openedIcon = value;
				if (openedIcon != null && IsExpanded) {
					SetIcon(openedIcon);
				}
			}
		}
		
		public ExtFolderNode()
		{
		}
		
		public override void Refresh() 
		{
			base.Refresh();
			if (Nodes.Count == 0) {
				SetIcon(ClosedIcon);
			} else if (IsExpanded) {
				SetIcon(OpenedIcon);
			}
		}
		public override void Expanding()
		{
			base.Expanding();
			if (openedIcon != null) {
				SetIcon(openedIcon);
			}
		}
		
		public override void Collapsing()
		{
			base.Collapsing();
			if (closedIcon != null) {
				SetIcon(closedIcon);
			}
		}
	}
}
