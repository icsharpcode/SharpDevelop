// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	abstract class AbstractNode : TreeNode
	{
		protected NodeOptionPanel panel;
		protected string ResNodeName(string resName)
		{
			try {
				return ResourceService.GetString("Dialog.HighlightingEditor.TreeView." + resName);
			} catch {
				return resName;
			}
		}
		
		public NodeOptionPanel OptionPanel {
			get {
				return panel;
			}
		}
		
		public abstract void UpdateNodeText();
		
		// should be made abstract when implementing ToXml()
		public abstract void WriteXml(XmlWriter writer);
	}
}
