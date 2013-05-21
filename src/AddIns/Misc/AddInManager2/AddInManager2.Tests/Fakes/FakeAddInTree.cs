// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
