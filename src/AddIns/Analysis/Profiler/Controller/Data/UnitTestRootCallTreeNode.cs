// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		List<CallTreeNode> unitTests = null;
		
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
				return (this.unitTests == null) ? false : this.unitTests.Any(test => test.IsActiveAtStart);
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
		public override CallTreeNode Merge(System.Collections.Generic.IEnumerable<CallTreeNode> nodes)
		{
			// throw new ShouldNeverHappenException();
			throw new NotSupportedException("Cannot merge a UnitTestRootCallTreeNode (should never be possible)");
		}
		
		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return (this.unitTests == null) ? 0 : this.unitTests.Aggregate(0, (sum, item) => sum ^= item.GetHashCode());
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
