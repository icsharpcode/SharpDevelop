// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
				return this.unitTests.Any(test => test.IsActiveAtStart);
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
			return this.unitTests.Aggregate(0, (sum, item) => sum ^= item.GetHashCode());
		}
		
		/// <inheritdoc/>
		public override bool Equals(CallTreeNode other)
		{
			return (other is UnitTestRootCallTreeNode) && (other as UnitTestRootCallTreeNode).unitTests.SequenceEqual(unitTests);
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
				return unitTests.AsQueryable();
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
