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
