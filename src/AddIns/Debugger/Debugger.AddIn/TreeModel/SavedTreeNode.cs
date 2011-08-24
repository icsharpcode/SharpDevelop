// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// Description of SavedTreeNode.
	/// </summary>
	public class SavedTreeNode : TreeNode
	{			
		public override bool CanSetText { 
			get { return true; }
		}
		
		public SavedTreeNode(IImage image, string fullname, string text)
			: base(null)
		{
			base.IconImage = image;
			FullName = fullname;
			Text = text;
		}
		
		public override bool SetText(string newValue) { 
			Text = newValue;
			return false;
		}
	}
}
