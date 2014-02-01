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
using System.Linq;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Wraps multiple CallTreeNodes that represent unit test methods for proper display in the listview.
	/// </summary>
	public class UnitTestRootCallTreeNode : CallTreeNode
	{
		List<CallTreeNode> unitTests;
		
		/// <summary>
		/// Creates a new UnitTestRootCallTreeNode.
		/// </summary>
		public UnitTestRootCallTreeNode(IEnumerable<CallTreeNode> unitTests)
		{
			if (unitTests != null)
				this.unitTests = new List<CallTreeNode>(unitTests);
		}
		
		/// <inheritdoc/>
		public override NameMapping NameMapping {
			get {
				return new NameMapping(0, null, "Merged node", null);
			}
		}
		
		/// <inheritdoc/>
		public override long CpuCyclesSpent {
			get {
				return 0;
			}
		}
		
		/// <inheritdoc/>
		public override bool IsActiveAtStart {
			get {
				return (unitTests == null) ? false : unitTests.Any(test => test.IsActiveAtStart);
			}
		}
		
		/// <inheritdoc/>
		public override double TimeSpent {
			get {
				return 0;
			}
		}
		
		/// <inheritdoc/>
		public override int RawCallCount {
			get {
				return 0;
			}
		}
		
		/// <inheritdoc/>
		public override CallTreeNode Parent {
			get {
				return null;
			}
		}
		
		/// <inheritdoc/>
		public override CallTreeNode Merge(IEnumerable<CallTreeNode> nodes)
		{
			// throw new ShouldNeverHappenException();
			throw new NotSupportedException("Cannot merge a UnitTestRootCallTreeNode (should never be possible)");
		}
		
		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return (unitTests == null) ? 0 : unitTests.Aggregate(0, (sum, item) => sum ^= item.GetHashCode());
		}
		
		/// <inheritdoc/>
		public override bool Equals(CallTreeNode other)
		{
			UnitTestRootCallTreeNode node = other as UnitTestRootCallTreeNode;
			
			if (node != null && unitTests != null) {
				return node.unitTests.SequenceEqual(unitTests);
			}
			
			return false;
		}
		
		/// <inheritdoc/>
		public override IQueryable<CallTreeNode> Callers {
			get {
				return Enumerable.Empty<CallTreeNode>().AsQueryable();
			}
		}
		
		/// <inheritdoc/>
		public override IQueryable<CallTreeNode> Children {
			get {
				return ((unitTests == null) ? Enumerable.Empty<CallTreeNode>() : unitTests).AsQueryable();
			}
		}

		/// <inheritdoc/>
		public override double TimeSpentSelf {
			get {
				return 0;
			}
		}
	}
}
