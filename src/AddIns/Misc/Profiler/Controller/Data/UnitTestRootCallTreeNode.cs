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
		
		/// <summary>
		/// Gets a reference to the name, return type and parameter list of the method.
		/// </summary>
		public override NameMapping NameMapping {
			get {
				return new NameMapping(0, null, "Merged node", null);
			}
		}
		
		public override long CpuCyclesSpent {
			get {
				return 0;
			}
		}
		
		public override bool IsActiveAtStart {
			get {
				return this.unitTests.Any(test => test.IsActiveAtStart);
			}
		}
		
		public override double TimeSpent {
			get {
				return 0;
			}
		}
		
		public override int RawCallCount {
			get {
				return 0;
			}
		}
		
		public override CallTreeNode Parent {
			get {
				return null;
			}
		}
		
		public override CallTreeNode Merge(System.Collections.Generic.IEnumerable<CallTreeNode> nodes)
		{
			// throw new ShouldNeverHappenException();
			throw new NotSupportedException("Cannot merge a UnitTestRootCallTreeNode (should never be possible)");
		}
		
		public override int GetHashCode()
		{
			return this.unitTests.Aggregate(0, (sum, item) => sum ^= item.GetHashCode());
		}
		
		public override bool Equals(CallTreeNode other)
		{
			return (other is UnitTestRootCallTreeNode) && (other as UnitTestRootCallTreeNode).unitTests.SequenceEqual(unitTests);
		}
		
		public override IQueryable<CallTreeNode> Callers {
			get {
				return Enumerable.Empty<CallTreeNode>().AsQueryable();
			}
		}
		
		public override IQueryable<CallTreeNode> Children {
			get {
				return unitTests.AsQueryable();
			}
		}
	}
}
