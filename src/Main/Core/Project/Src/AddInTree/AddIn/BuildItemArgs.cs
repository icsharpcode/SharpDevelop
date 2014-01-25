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
