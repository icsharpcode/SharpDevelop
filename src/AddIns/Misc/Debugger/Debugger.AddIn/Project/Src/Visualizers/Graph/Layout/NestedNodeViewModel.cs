// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.AddIn.Visualizers.Utils;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// ViewModel base for node in tree of properties, to be bound to View (ie. PositionedGraphNodeControl).
	/// </summary>
	public class NestedNodeViewModel : Utils.ITreeNode<NestedNodeViewModel>
	{
		public NestedNodeViewModel(PositionedGraphNode containingNode)
		{
			if (containingNode == null)
				throw new ArgumentNullException("containingNode");
			
			this.containingNode = containingNode;
		}
		
		/// <summary>
		/// Name displayed in GUI.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Text displayed in GUI.
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// Is this expandable node?
		/// </summary>
		public bool IsNested { get; set; }
		/// <summary>
		/// Does this node have any children?
		/// </summary>
		public bool HasChildren { get { return this.Children.Count > 0; } }
		
		// if we bound this ViewModel to a TreeView, this would not be needed,
		// it is added "artificially" to support PositionedGraphNodeControl
		public bool IsExpanded { get; set; }	
		
		private List<NestedNodeViewModel> children = new List<NestedNodeViewModel>();		
		public List<NestedNodeViewModel> Children { get { return this.children; } }
	
		PositionedGraphNode containingNode;
		/// <summary>
		/// PositionedGraphNode that contains this PropertyNodeViewModel.
		/// </summary>
		public PositionedGraphNode ContainingNode
		{
			get { return this.containingNode; }
		}
		
		/// <summary>
		/// Show a button to expand property associated with this node?
		/// </summary>
		public bool ShowExpandPropertyButton
		{
			get 
			{
				var thisAsPropertyNode = this as PropertyNodeViewModel;
				if (thisAsPropertyNode == null) 
				{
					// this is NestedNodeViewModel -> no property, don't show expand button
					return false;
				}
				else
				{
					// this is PositionedNodeViewModel -> show expand button when appropriate
					PositionedNodeProperty property = thisAsPropertyNode.Property;
					return (!property.IsAtomic && !property.IsNull);
				}
			}
		}
		
		/// <summary>
		/// Returns flattened subtree.
		/// </summary>
		public IEnumerable<NestedNodeViewModel> FlattenAll()
		{
			return Utils.TreeFlattener.Flatten(this);
		}
		
		/// <summary>
		/// Returns flattened subtree, skipping children of collapsed nodes.
		/// </summary>
		public IEnumerable<NestedNodeViewModel> FlattenExpanded()
		{
			return Utils.TreeFlattener.FlattenSelectChildrenIf(this, (node) => { return node.IsExpanded; });
		}
		
		/// <summary>
		/// Returns properties nodes from this tree.
		/// </summary>
		public IEnumerable<PositionedNodeProperty> FlattenProperties()
		{
			return Utils.TreeFlattener.Flatten(this).Where((node) => { return  node is PropertyNodeViewModel; }).
				Select(	(propertyNode) => { return ((PropertyNodeViewModel)propertyNode).Property; });
		}
		
		#region Utils.ITreeNode implementation
		IEnumerable<NestedNodeViewModel> ITreeNode<NestedNodeViewModel>.Children
		{
			get { return this.Children; }
		}
		#endregion
		
		public virtual void InitFrom(AbstractNode source)
		{
			if (!(source is NestedNode))
				throw new InvalidOperationException("NestedNodeViewModel must initialize from NestedNode");

			NestedNode nestedSource = (NestedNode)source;
			this.Name = nestedSource.NodeType == NestedNodeType.ThisNode ? "this" : "base";	  // TODO
			this.Text = "";
			this.IsNested = true;
			this.IsExpanded = (nestedSource.NodeType == NestedNodeType.ThisNode); // TODO remember expanded nodes
				
			foreach (AbstractNode child in nestedSource.Children)
			{
				if (child is NestedNode)
				{
					var newChild = new NestedNodeViewModel(this.ContainingNode);
					newChild.InitFrom(child);
					this.Children.Add(newChild);
				} 
				else if (child is PropertyNode)
				{
					var newChild = new PropertyNodeViewModel(this.ContainingNode);
					newChild.InitFrom(child);
					this.Children.Add(newChild);
				}
			}
		}
	}
}
