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
using System.Linq;
using ICSharpCode.Profiler.Controller.Data;

namespace ICSharpCode.Profiler.Controller.Queries
{
	/// <summary>
	/// Provides a skeleton for all queries compiled at runtime.
	/// </summary>
	public abstract class QueryBase
	{
		/// <summary>
		/// Gets the <see cref="ProfilingDataProvider"/> that is the data source for this query.
		/// </summary>
		public ProfilingDataProvider Provider { get; internal set; }
		
		/// <summary>
		/// Gets the start index of the selected range of datasets.
		/// </summary>
		public int StartDataSetIndex { get; internal set; }
		
		/// <summary>
		/// Gets the end index of the selected range of datasets.
		/// </summary>
		public int EndDataSetIndex { get; internal set; }
		
		/// <summary>
		/// Gets the root node (usually invisible node that has all threads as children).
		/// </summary>
		public CallTreeNode Root { get; internal set; }
		
		/// <summary>
		/// Returns all calls.
		/// </summary>
		public IQueryable<CallTreeNode> Calls { get { return Provider.GetAllCalls(StartDataSetIndex, EndDataSetIndex); } }
		
		/// <summary>
		/// Returns all functions.
		/// </summary>
		public IQueryable<CallTreeNode> Functions { get { return Provider.GetFunctions(StartDataSetIndex, EndDataSetIndex); } }
		
		/// <summary>
		/// Returns all threads.
		/// </summary>
		public IQueryable<CallTreeNode> Threads { get { return Root.Children; } }
		
		/// <summary>
		/// Returns the CallTreeNode described by an array of nameIds.
		/// </summary>
		public CallTreeNode GetNodeByPath(params int[] nameIds)
		{
			CallTreeNode node = Root;
			
			for (int i = 1; i < nameIds.Length; i++) {
				node = node.Children.FirstOrDefault(n => n.NameMapping.Id == nameIds[i]);
				if (node == null)
					return null;
			}
			
			return node;
		}
		
		/// <summary>
		/// Helper for merging multiple CallTreeNodes in PQL.
		/// </summary>
		public CallTreeNode Merge(params CallTreeNode[] nodes)
		{
			if (nodes.Length == 0 || nodes[0] == null)
				return null;
			return nodes.Merge();
		}
		
		/// <summary>
		/// Executes a query created at runtime.
		/// </summary>
		public abstract object Execute();
	}
}
