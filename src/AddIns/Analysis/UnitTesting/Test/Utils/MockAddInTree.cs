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
		
		public IReadOnlyList<T> BuildItems<T>(string path, object caller, bool throwOnNotFound)
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
		
		public IReadOnlyList<AddIn> AddIns {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Concurrent.ConcurrentDictionary<string, IDoozer> Doozers {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Concurrent.ConcurrentDictionary<string, IConditionEvaluator> ConditionEvaluators {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object BuildItem(string path, object caller)
		{
			throw new NotImplementedException();
		}
		
		public object BuildItem(string path, object caller, IEnumerable<ICondition> additionalConditions)
		{
			throw new NotImplementedException();
		}
		
		public AddInTreeNode GetTreeNode(string path, bool throwOnNotFound)
		{
			throw new NotImplementedException();
		}
	}
}
