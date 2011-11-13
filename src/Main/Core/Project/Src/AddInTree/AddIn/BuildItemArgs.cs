// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Argument class used for <see cref="IDoozer.BuildItem"/>.
	/// </summary>
	public class BuildItemArgs
	{
		object caller;
		Codon codon;
		IEnumerable<ICondition> conditions;
		AddInTreeNode subItemNode;
		
		public BuildItemArgs(object caller, Codon codon, IEnumerable<ICondition> conditions, AddInTreeNode subItemNode)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			this.caller = caller;
			this.codon = codon;
			this.conditions = conditions ?? Enumerable.Empty<ICondition>();
			this.subItemNode = subItemNode;
		}
		
		/// <summary>
		/// The caller passed to <see cref="AddInTree.BuildItem(string,object)"/>.
		/// </summary>
		public object Caller {
			get { return caller; }
		}
		
		/// <summary>
		/// The codon to build.
		/// </summary>
		public Codon Codon {
			get { return codon; }
		}
		
		/// <summary>
		/// The addin containing the codon.
		/// </summary>
		public AddIn AddIn {
			get { return codon.AddIn; }
		}
		
		/// <summary>
		/// The conditions applied to this item.
		/// </summary>
		public IEnumerable<ICondition> Conditions {
			get { return conditions; }
		}
		
		/// <summary>
		/// The addin tree node containing the sub-items.
		/// Returns null if no sub-items exist.
		/// </summary>
		public AddInTreeNode SubItemNode {
			get { return subItemNode; }
		}
		
		/// <summary>
		/// Builds the sub-items.
		/// Conditions on this node are also applied to the sub-nodes.
		/// </summary>
		public List<T> BuildSubItems<T>()
		{
			if (subItemNode == null)
				return new List<T>();
			else
				return subItemNode.BuildChildItems<T>(caller, conditions);
		}
	}
}
