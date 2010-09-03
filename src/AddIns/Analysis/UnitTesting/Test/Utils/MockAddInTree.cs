// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockAddInTree : IAddInTree
	{
		Dictionary<string, object> treeItems = new Dictionary<string, object>();
		
		public MockAddInTree()
		{
		}
		
		public List<T> BuildItems<T>(string path, object caller)
		{
			object obj;
			if (treeItems.TryGetValue(path, out obj)) {
				return obj as List<T>;
			}
			throw new TreePathNotFoundException(path);
		}
		
		public void AddItems<T>(string path, List<T> items)
		{
			treeItems.Add(path, items);
		}
	}
}
