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
	public class ContentNode : Utils.ITreeNode<ContentNode>
	{
		public ContentNode(PositionedGraphNode containingNode, ContentNode parent)
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
		
		private ContentNode parent;
		/// <summary>
		/// Parent node in the content tree. Null if this node is root.
		/// </summary>
		public ContentNode Parent
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
		
		private List<ContentNode> children = new List<ContentNode>();		
		public List<ContentNode> Children { get { return this.children; } }
	
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
		public virtual bool ShowExpandPropertyButton
		{
			get 
			{
				// this is NestedNodeViewModel -> no property, don't show expand button
				return false;
			}
		}
		
		/// <summary>
		/// Returns flattened subtree.
		/// </summary>
		public IEnumerable<ContentNode> FlattenAll()
		{
			return Utils.TreeFlattener.Flatten(this);
		}
		
		/// <summary>
		/// Returns flattened subtree (including root), skipping children of collapsed nodes.
		/// </summary>
		public IEnumerable<ContentNode> FlattenExpanded()
		{
			return Utils.TreeFlattener.FlattenSelectChildrenIf(this, (node) => { return node.IsExpanded; });
		}
		
		/// <summary>
		/// Returns flattened subtree (excluding root), skipping children of collapsed nodes.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ContentNode> FlattenChildrenExpanded()
		{
			return Utils.TreeFlattener.FlattenSelectChildrenIf(this.Children, (node) => { return node.IsExpanded; });
		}
		
		/// <summary>
		/// Returns properties nodes from this tree.
		/// </summary>
		public IEnumerable<PositionedNodeProperty> FlattenProperties()
		{
			return Utils.TreeFlattener.Flatten(this).Where((node) => { return  node is ContentPropertyNode; }).
				Select(	(propertyNode) => { return ((ContentPropertyNode)propertyNode).Property; });
		}
		
		#region Utils.ITreeNode implementation
		IEnumerable<ContentNode> ITreeNode<ContentNode>.Children
		{
			get { return this.Children; }
		}
		#endregion
		
		public virtual void InitFrom(AbstractNode source, Expanded expanded)
		{
			this.Name = getContentNodeName(source);
			this.Text = "";			// lazy evaluated later
			this.IsNested = true;
			this.path = this.Parent == null ? this.Name : this.Parent.Path + "." + this.Name;
			this.IsExpanded = (source is ThisNode) || expanded.ContentNodes.IsExpanded(this);
				
			foreach (AbstractNode child in source.Children)
			{
				if (child is PropertyNode)
				{
					var newChild = new ContentPropertyNode(this.ContainingNode, this);
					newChild.InitFrom(child, expanded);
					this.Children.Add(newChild);
				}
				else
				{
					var newChild = new ContentNode(this.ContainingNode, this);
					newChild.InitFrom(child, expanded);
					this.Children.Add(newChild);					
				}
			}
		}
		
		private string getContentNodeName(AbstractNode source)
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
