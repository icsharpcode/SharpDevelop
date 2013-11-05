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
		object parameter;
		Codon codon;
		IReadOnlyCollection<ICondition> conditions;
		AddInTreeNode subItemNode;
		
		public BuildItemArgs(object parameter, Codon codon, IReadOnlyCollection<ICondition> conditions, AddInTreeNode subItemNode)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			this.parameter = parameter;
			this.codon = codon;
			this.conditions = conditions;
			this.subItemNode = subItemNode;
		}
		
		/// <summary>
		/// The parameter passed to <see cref="IAddInTree.BuildItem(string,object)"/>.
		/// </summary>
		public object Parameter {
			get { return parameter; }
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
		/// The whole AddIn tree.
		/// </summary>
		public IAddInTree AddInTree {
			get { return codon.AddIn.AddInTree; }
		}
		
		/// <summary>
		/// The conditions applied to this item.
		/// </summary>
		public IReadOnlyCollection<ICondition> Conditions {
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
				return subItemNode.BuildChildItems<T>(parameter, conditions);
		}
	}
}
