// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class FolderNode : AssemblyTreeNode
	{
		int openindex;
		int closeindex;
		
		public FolderNode(string name, object attribute, NodeType type,int openindex, int closeindex) : 
			base(name, attribute, type)
		{
			this.openindex = openindex;
			this.closeindex = closeindex;
			OnCollapse();
		}
		
		protected override void SetIcon()
		{
			OnCollapse();
		}
		
		public override void OnExpand()
		{
			ImageIndex  = SelectedImageIndex = closeindex;
		}
		
		public override void OnCollapse()
		{
			ImageIndex  = SelectedImageIndex = openindex;
		}
	}
}
