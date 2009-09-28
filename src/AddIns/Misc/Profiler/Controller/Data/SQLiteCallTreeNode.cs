// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Profiler.Controller.Data.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// A CallTreeNode based on a ProfilingDataSQLiteProvider.
	/// </summary>
	sealed class SQLiteCallTreeNode : CallTreeNode
	{
		internal int nameId;
		internal int callCount;
		internal ulong cpuCyclesSpent;
		CallTreeNode parent;
		SQLiteQueryProvider provider;
		internal List<int> ids = new List<int>();
		internal bool hasChildren;
		internal int activeCallCount;
		
		/// <summary>
		/// Creates a new CallTreeNode.
		/// </summary>
		public SQLiteCallTreeNode(int nameId, CallTreeNode parent, SQLiteQueryProvider provider)
		{
			this.nameId = nameId;
			this.parent = parent;
			this.provider = provider;
		}

		/// <summary>
		/// Gets a reference to the name, return type and parameter list of the method.
		/// </summary>
		public override NameMapping NameMapping {
			get {
				if (nameId == 0)
					return new NameMapping(0, null, "Merged node", null);
				
				return this.provider.GetMapping(nameId);
			}
		}

		/// <inheritdoc/>
		public override int RawCallCount {
			get {
				return this.callCount;
			}
		}

		/// <summary>
		/// Gets how many CPU cycles where spent inside this method, including sub calls.
		/// </summary>
		public override long CpuCyclesSpent {
			get{
				return (long)this.cpuCyclesSpent;
			}
		}

		/// <summary>
		/// Gets a reference to the parent of this CallTreeNode.
		/// </summary>
		public override CallTreeNode Parent {
			get {
				return this.parent;
			}
		}

		/// <summary>
		/// Returns all children of the current CallTreeNode, sorted by order of first call.
		/// </summary>
		public override IQueryable<CallTreeNode> Children {
			get {
				Expression<Func<SingleCall, bool>> filterLambda = c => this.ids.Contains(c.ParentID);
				return provider.CreateQuery(new MergeByName(new Filter(AllCalls.Instance, filterLambda)));
			}
		}

		/// <summary>
		/// Gets the time spent inside the method (including sub calls) in milliseconds.
		/// </summary>
		public override double TimeSpent {
			get {
				return CpuCyclesSpent / (1000.0 * this.provider.ProcessorFrequency);
			}
		}
		
		public override double TimeSpentSelf {
			get {
				return CpuCyclesSpentSelf / (1000.0 * this.provider.ProcessorFrequency);
			}
		}

		/// <summary>
		/// Gets whether the function call started in a previous data set that's not selected.
		/// </summary>
		public override bool IsActiveAtStart {
			get {
				return activeCallCount > 0;
			}
		}
		
		public override int CallCount {
			get { return callCount + activeCallCount; }
		}
		
		/// <summary>
		/// Merges a collection of CallTreeNodes into one CallTreeNode, all values are accumulated.
		/// </summary>
		/// <param name="nodes">The collection of nodes to process.</param>
		/// <returns>A new CallTreeNode.</returns>
		public override CallTreeNode Merge(IEnumerable<CallTreeNode> nodes)
		{
			SQLiteCallTreeNode mergedNode = new SQLiteCallTreeNode(0, null, this.provider);
			bool initialised = false;
			
			foreach (SQLiteCallTreeNode node in nodes) {
				mergedNode.ids.AddRange(node.ids);
				mergedNode.callCount += node.callCount;
				mergedNode.cpuCyclesSpent += node.cpuCyclesSpent;
				if (!initialised || mergedNode.nameId == node.nameId)
					mergedNode.nameId = node.nameId;
				else
					mergedNode.nameId = 0;
				initialised = true;
			}
			
			return mergedNode;
		}
		
		public override IQueryable<CallTreeNode> Callers {
			get {
				// parent is not null => this node was created by a
				// 'Children' call => all our IDs come from that parent
				if (this.parent != null)
					return (new CallTreeNode[] { this.parent }).AsQueryable();
				
				throw new NotImplementedException();
			}
		}
		
		public override bool Equals(CallTreeNode other)
		{
			if (other is SQLiteCallTreeNode) {
				SQLiteCallTreeNode node = other as SQLiteCallTreeNode;
				
				if (node.ids.Count != this.ids.Count)
					return false;
				
				for (int i = 0; i < this.ids.Count; i++) {
					if (node.ids[i] != this.ids[i])
						return false;
				}
				
				return true;
			}
			
			return false;
		}
		
		public override int GetHashCode()
		{
			// (a[0] * p + a[1]) * p + a[2]
			const int hashPrime = 1000000007;
			int hash = 0;
			
			unchecked {
				foreach (int i in this.ids) {
					hash = hash * hashPrime + i;
				}
			}
			
			return hash;
		}
		
		public override bool HasChildren {
			get { return this.hasChildren; }
		}
	}
}
