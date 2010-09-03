// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestAddInTree : IAddInTree
	{
		public List<T> BuildItems<T>(string path, object caller)
		{
			return AddInTree.BuildItems<T>(path, caller);
		}
	}
}
