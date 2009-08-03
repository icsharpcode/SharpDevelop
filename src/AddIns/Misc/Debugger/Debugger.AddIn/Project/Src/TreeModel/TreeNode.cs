// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// A node in the variable tree.
	/// The node is imutable.
	/// </summary>
	public class TreeNode: IComparable<TreeNode>, ITreeNode
	{
		Image  image = null;
		string name  = string.Empty;
		string text  = string.Empty;
		string type  = string.Empty;
		IEnumerable<TreeNode> childNodes = null;
		
		public Image Image {
			get { return image; }
			protected set { image = value; }
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public virtual string Text
		{
			get { return text; }
			protected set { text = value; }
		}
		
		public virtual string Type {
			get { return type; }
			protected set { type = value; }
		}
		
		public virtual IEnumerable<TreeNode> ChildNodes {
			get { return childNodes; }
			protected set { childNodes = value; }
		}
		
		IEnumerable<ITreeNode> ITreeNode.ChildNodes {
			get { return this.childNodes; }
		}
		
		public TreeNode()
		{
		}
		
		public TreeNode(Image image, string name, string text, string type, IEnumerable<TreeNode> childNodes)
		{
			this.image = image;
			this.name = name;
			this.text = text;
			this.type = type;
			this.childNodes = childNodes;
		}
		
		public int CompareTo(TreeNode other)
		{
			return this.Name.CompareTo(other.Name);
		}
	}
}
