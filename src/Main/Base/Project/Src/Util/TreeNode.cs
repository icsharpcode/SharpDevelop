// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
