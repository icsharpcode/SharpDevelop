// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
