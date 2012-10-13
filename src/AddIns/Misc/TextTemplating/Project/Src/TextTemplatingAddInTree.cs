// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingAddInTree : IAddInTree
	{
		public IEnumerable<IAddIn> GetAddIns()
		{
			foreach (AddIn addIn in AddInTree.AddIns) {
				yield return new TextTemplatingAddIn(addIn);
			}
		}
		
		public List<IServiceProvider> BuildServiceProviders(string path)
		{
			return AddInTree.BuildItems<IServiceProvider>(path, null);
		}
	}
}
