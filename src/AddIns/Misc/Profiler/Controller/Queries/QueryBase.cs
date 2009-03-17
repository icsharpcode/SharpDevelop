// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Profiler.Controller.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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
		public IQueryable<CallTreeNode> Calls { get { return Root.Descendants; } }
		
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
			CallTreeNode node = this.Root;
			
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
