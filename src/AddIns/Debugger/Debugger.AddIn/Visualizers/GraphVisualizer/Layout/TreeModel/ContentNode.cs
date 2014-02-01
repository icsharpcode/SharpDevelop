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
using System.Windows.Media;

using Debugger.AddIn.Visualizers.Utils;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// ViewModel base for node in tree of properties, to be bound to View (ie. PositionedGraphNodeControl).
	/// </summary>
	public class ContentNode : Utils.ITreeNode<ContentNode>
	{
		public ContentNode(PositionedNode containingNode, ContentNode parent)
		{
			if (containingNode == null)
				throw new ArgumentNullException("containingNode");
			this.containingNode = containingNode;
			this.parent = parent;
		}
		
		/// <summary>
		/// Icon next to the name - didn't add much value
		/// </summary>
		public ImageSource MemberIcon { get; set; }
		
		/// <summary>
		/// Path to this content node in the whole <see cref="PositionedGraph"></see>.
		/// </summary>
		public string FullPath
		{
			get { return this.containingNode.ObjectNode.Expression.Expr + "/" + this.Path; }
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
	
		PositionedNode containingNode;
		/// <summary>
		/// PositionedGraphNode that contains this PropertyNodeViewModel.
		/// </summary>
		public PositionedNode ContainingNode
		{
			get { return this.containingNode; }
		}
		
		/// <summary>
		/// Show a button to expand property associated with this node?
		/// </summary>
		public virtual bool ShowExpandPropertyButton
		{
			get  {
				// this is ContentNode -> no property, don't show expand button
				return false;
			}
		}
		
		/// <summary>
		/// Is this an expanded Property node?
		/// </summary>
		public virtual bool IsPropertyExpanded
		{
			get  {
				// this is ContentNode -> no property, don't show expand button
				return false;
			}
			set { throw new InvalidOperationException("Cannot set IsPropertyExpanded on " + typeof(ContentNode).Name); }
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
			return Utils.TreeFlattener.FlattenSelectChildrenIf(this, node => node.IsExpanded);
		}
		
		/// <summary>
		/// Returns flattened subtree (excluding root), skipping children of collapsed nodes.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ContentNode> FlattenChildrenExpanded()
		{
			return Utils.TreeFlattener.FlattenSelectChildrenIf(this.Children, node => node.IsExpanded);
		}
		
		/// <summary>
		/// Returns properties nodes from this tree.
		/// </summary>
		public IEnumerable<PositionedNodeProperty> FlattenProperties()
		{
			return Utils.TreeFlattener.Flatten(this).Where(node => node is ContentPropertyNode).
				Select(propertyNode => ((ContentPropertyNode)propertyNode).Property);
		}
		
		#region Utils.ITreeNode implementation
		IEnumerable<ContentNode> ITreeNode<ContentNode>.Children
		{
			get { return this.Children; }
		}
		#endregion
		
		public virtual void InitOverride(AbstractNode source, Expanded expanded)
		{
			this.Name = GetContentNodeName(source);
			this.Text = "";			// lazy evaluated later
			this.IsNested = true;
			this.path = this.Parent == null ? this.Name : this.Parent.Path + "." + this.Name;
			this.IsExpanded = (source is ThisNode) || expanded.ContentNodes.IsExpanded(this);
				
			foreach (AbstractNode child in source.Children) {
				ContentNode newChild = child is PropertyNode ?
					new ContentPropertyNode(this.ContainingNode, this) :
					new ContentNode(this.ContainingNode, this);
				newChild.InitOverride(child, expanded);
				this.Children.Add(newChild);
			}
		}
		
		string GetContentNodeName(AbstractNode source)
		{
			if (source is ThisNode)	{
				return "this";
			}
			if (source is NonPublicMembersNode)	{
				return StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicMembers}");
			}
			if (source is RawViewNode)	{
				return "Raw View";
			}
			var sourceBaseClassNode = source as BaseClassNode;
			if (sourceBaseClassNode != null) {
				string baseClassString = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}");
				return string.Format("{0} ({1})", sourceBaseClassNode.TypeName, baseClassString);
			}
			throw new ApplicationException("Unknown AbstractNode: " + source.GetType());
		}
	}
}
