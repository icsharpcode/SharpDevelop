// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class MethodNode : AssemblyTreeNode
	{
		public MethodNode(IMethod methodinfo2) : base ("", methodinfo2, NodeType.Method)
		{
			if (attribute == null) {
				Text = "no name";
				return;
			}
			Text = MemberNode.GetShortMemberName((IMethod)attribute, false);
		}
		
		protected override void SetIcon()
		{
			if (attribute == null)
				return;
			IMethod methodinfo = (IMethod)attribute;
			
			if (methodinfo.IsPrivate) { // private
				ImageIndex  = SelectedImageIndex = METHODINDEX + 3;
			} else
			if (methodinfo.IsProtected) { // protected
				ImageIndex  = SelectedImageIndex = METHODINDEX + 2;
			} 
			ImageIndex  = SelectedImageIndex = METHODINDEX;
		}
	}
}
