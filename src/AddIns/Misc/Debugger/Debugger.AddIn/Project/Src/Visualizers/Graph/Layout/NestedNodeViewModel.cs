// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// ViewModel base for node in tree of properties, to be bound to View (ie. PositionedGraphNodeControl).
	/// </summary>
	public class NestedNodeViewModel : Utils.ITreeNode<NestedNodeViewModel>
	{
		public NestedNodeViewModel(PositionedGraphNode containingNode, NestedNodeViewModel parent)
		{
			if (containingNode == null)
				throw new ArgumentNullException("containingNode");
			
			this.containingNode = containingNode;
			this.parent = parent;
		}
		
		/// <summary>
		/// Path to this content node in the whole <see cref="PositionedGraph"></see>.
		/// </summary>
		public string FullPath
		{
			get { return this.containingNode.ObjectNode.Expression.Code + "/" + this.Path; }
		}
		
		private NestedNodeViewModel parent;
		/// <summary>
		/// Parent node in the content tree. Null if this node is root.
		/// </summary>
		public NestedNodeViewModel Parent
		{
			get { return this.parent; }
		}
		
		private string path;
		/// <summary>
		/// Path to this content node in the content tree of containing <see cref="PositinedGraphNode"></see>.
		/// </summary>
		public string Path
		{
			get { return this.path; }
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
			this.Name = getNestedNodeName(source);
			this.Text = "";			// lazy evaluated later
			this.IsNested = true;
			this.path = this.Parent == null ? this.Name : this.Parent.Path + "." + this.Name;
			this.IsExpanded = (source is ThisNode); // TODO remember expanded nodes
				
			foreach (AbstractNode child in source.Children)
			{
				if (child is PropertyNode)
				{
					var newChild = new PropertyNodeViewModel(this.ContainingNode, this);
					newChild.InitFrom(child);
					this.Children.Add(newChild);
				}
				else
				{
					var newChild = new NestedNodeViewModel(this.ContainingNode, this);
					newChild.InitFrom(child);
					this.Children.Add(newChild);					
				}
			}
		}
		
		private string getNestedNodeName(AbstractNode source)
		{
			if (source is ThisNode)
			{
				return "this";
			}
			
			if (source is NonPublicMembersNode)
			{
				return StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicMembers}");
			}
			
			var sourceBaseClassNode = source as BaseClassNode;
			if (sourceBaseClassNode != null)
			{
				//return StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}") + " " + sourceBaseClassNode.TypeName;
				return sourceBaseClassNode.TypeName;
			}

			throw new ApplicationException("Unknown AbstractNode.");
		}
	}
}
