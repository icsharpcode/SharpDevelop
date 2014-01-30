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
using System.Collections.Concurrent;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	public class FakeAddInTree : IAddInTree
	{
		private List<AddIn> _addIns;
		private ConcurrentDictionary<string, IDoozer> _doozers;
		private ConcurrentDictionary<string, IConditionEvaluator> _conditionEvaluators;
		
		public FakeAddInTree()
		{
			_addIns = new List<AddIn>();
			_doozers = new ConcurrentDictionary<string, IDoozer>();
			_conditionEvaluators = new ConcurrentDictionary<string, IConditionEvaluator>();
		}
		
		public System.Collections.Generic.IReadOnlyList<AddIn> AddIns
		{
			get
			{
				return _addIns.AsReadOnly();
			}
		}
		
		public System.Collections.Concurrent.ConcurrentDictionary<string, IDoozer> Doozers
		{
			get
			{
				return _doozers;
			}
		}
		
		public System.Collections.Concurrent.ConcurrentDictionary<string, IConditionEvaluator> ConditionEvaluators
		{
			get
			{
				return _conditionEvaluators;
			}
		}
		
		public System.Collections.Generic.IReadOnlyList<T> BuildItems<T>(string path, object parameter, bool throwOnNotFound)
		{
			return new List<T>().AsReadOnly();
		}
		
		public object BuildItem(string path, object parameter)
		{
			return null;
		}
		
		public object BuildItem(string path, object parameter, System.Collections.Generic.IEnumerable<ICondition> additionalConditions)
		{
			return null;
		}
		
		public AddInTreeNode GetTreeNode(string path, bool throwOnNotFound)
		{
			return null;
		}
	}
}
