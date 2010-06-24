// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Generic TreeNode with content and children.
	/// </summary>
	public interface ITreeNode<TContent>
	{
		TContent Content { get; }
	}
}
