// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Generic TreeNode with content and children.
	/// </summary>
	public interface ITreeNode<TContent>
	{
		TContent Content { get; }
		
		IEnumerable<ITreeNode<TContent>> Children { get; }
	}
	
	public class TreeNode<TContent> : ITreeNode<TContent>
	{
		public TreeNode(TContent content)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			this.Content = content;
		}
		
		public TContent Content { get; private set; }
		
		public IEnumerable<ITreeNode<TContent>> Children { get; set; }
		
		public override string ToString()
		{
			return string.Format("[TreeNode {0}]", this.Content.ToString());
		}
	}
}
