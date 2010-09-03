// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.Profiler.Controller.Data;

namespace Profiler.Tests.Controller.Data
{
	/// <summary>
	/// Stub CallTreeNode for unit tests.
	/// </summary>
	public class CallTreeNodeStub : CallTreeNode
	{
		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}
		
		public override bool Equals(CallTreeNode other)
		{
			throw new NotImplementedException();
		}
		
		public override System.Linq.IQueryable<CallTreeNode> Callers {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override CallTreeNode Merge(System.Collections.Generic.IEnumerable<CallTreeNode> nodes)
		{
			throw new NotImplementedException();
		}
		
		List<CallTreeNodeStub> children = new List<CallTreeNodeStub>();
		CallTreeNode parent;
		
		public override System.Linq.IQueryable<CallTreeNode> Children {
			get {
				return children.AsQueryable();
			}
		}
		
		public sealed class ChildrenCollection : IEnumerable<CallTreeNodeStub>
		{
			CallTreeNodeStub parent;
			
			public ChildrenCollection(CallTreeNodeStub parent)
			{
				this.parent = parent;
			}
			
			public void Add(CallTreeNodeStub node)
			{
				parent.AddChild(node);
			}
			
			public IEnumerator<CallTreeNodeStub> GetEnumerator()
			{
				return parent.children.GetEnumerator();
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		
		public ChildrenCollection AddChildren {
			get { return new ChildrenCollection(this); }
		}
		
		public void AddChild(CallTreeNodeStub node)
		{
			node.parent = this;
			children.Add(node);
		}
		
		public override CallTreeNode Parent {
			get {
				return parent;
			}
		}
		
		public override double TimeSpent {
			get {
				return CpuCyclesSpent;
			}
		}
		
		public override double TimeSpentSelf {
			get {
				return CpuCyclesSpentSelf;
			}
		}
		
		public long CpuCyclesSpentValue;
		
		public override long CpuCyclesSpent {
			get {
				return CpuCyclesSpentValue;
			}
		}
		
		public bool IsActiveAtStartValue;
		
		public override bool IsActiveAtStart {
			get {
				return IsActiveAtStartValue;
			}
		}
		
		public int RawCallCountValue;
		
		public override int RawCallCount {
			get {
				return RawCallCountValue;
			}
		}
		
		public NameMapping NameMappingValue;
		
		public override NameMapping NameMapping {
			get {
				return NameMappingValue;
			}
		}
	}
	
	public class DataSetStub : IProfilingDataSet
	{
		public bool IsFirst { get; set; }
		public CallTreeNode RootNode { get; set; }
	}
}
