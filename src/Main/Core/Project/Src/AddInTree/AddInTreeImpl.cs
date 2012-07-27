// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	sealed class AddInTreeImpl : IAddInTree
	{
		IReadOnlyList<AddIn> IAddInTree.AddIns {
			get { return AddInTree.AddIns; }
		}
		
		IReadOnlyList<T> IAddInTree.BuildItems<T>(string path, object caller, bool throwOnNotFound)
		{
			return AddInTree.BuildItems<T>(path, caller, throwOnNotFound);
		}
		
		object IAddInTree.BuildItem(string path, object caller)
		{
			return AddInTree.BuildItem(path, caller);
		}
		
		AddInTreeNode IAddInTree.GetTreeNode(string path, bool throwOnNotFound)
		{
			return AddInTree.GetTreeNode(path, throwOnNotFound);
		}
	}
}
