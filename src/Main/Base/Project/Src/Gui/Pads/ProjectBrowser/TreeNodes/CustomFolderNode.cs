// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class CustomFolderNode : AbstractProjectBrowserTreeNode
	{
		string closedImage = null;
		string openedImage = null;
		
		public string ClosedImage {
			get {
				return closedImage;
			}
			set {
				closedImage = value;
				if (!IsExpanded) {
					SetIcon(closedImage);
				}
				
			}
		}
		
		public string OpenedImage {
			get {
				return openedImage;
			}
			set {
				openedImage = value;
				if (IsExpanded) {
					SetIcon(openedImage);
				}
			}
		}

		public CustomFolderNode()
		{
		}
		
		protected void UpdateIcon()
		{
			if (Nodes.Count == 0) {
				SetIcon(ClosedImage);
			} else if (IsExpanded) {
				SetIcon(openedImage);
			}
		}
		
		public override void Refresh() 
		{
			base.Refresh();
			UpdateIcon();
		}
		
		public override void Expanding()
		{
			if (openedImage != null) {
				SetIcon(openedImage);
			}
			base.Expanding();
			if (Nodes.Count == 0) {
				SetIcon(ClosedImage);
			}
		}
		
		public override void Collapsing()
		{
			if (closedImage != null) {
				SetIcon(closedImage);
			}
			base.Collapsing();
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
