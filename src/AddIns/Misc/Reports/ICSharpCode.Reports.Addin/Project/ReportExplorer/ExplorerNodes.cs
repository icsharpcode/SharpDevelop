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
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ColumnNode.
	/// </summary>
	
	internal class AbstractFieldsNode : TreeNode {
		
		private string contextMenuAddinTreePath = String.Empty;
		
		protected AbstractFieldsNode(string nodeName):base()
		{
			this.Text = nodeName;
		}
		
		public AbstractFieldsNode(string nodeName,string contextMenuAddinTreePath):base(nodeName)
		{
			this.contextMenuAddinTreePath = contextMenuAddinTreePath;
		}
		
		/// <summary>
		/// Generates a Drag & Drop data object. If this property returns null
		/// the node indicates that it can't be dragged.
		/// </summary>
		public virtual DataObject DragDropDataObject {
			get {
				return null;
			}
		}
		
		
		/// <summary>
		/// Gets the add-in tree path for the context menu. 
		/// </summary>
		/// <remarks>
		/// I choosed to give back the add-in tree path instead of a popup menu 
		/// or a menuitem collection, because I don't want to add a magic library 
		/// or Windows.Forms dependency.
		/// </remarks>
		
		public virtual string ContextMenuAddinTreePath {
			get {
				return contextMenuAddinTreePath;
			}
			set {
				contextMenuAddinTreePath = value;
			}
		}
	}
	
	
	
	internal class SectionNode:AbstractFieldsNode
	{
		public SectionNode (string nodeName):base(nodeName)
		{
		}
	}
	
	
	
	internal class ColumnNode:AbstractFieldsNode
	{
		public ColumnNode(string nodeName,int imageIndex):base(nodeName)
		{
			this.ImageIndex = imageIndex;
			this.SelectedImageIndex = imageIndex;
		}
	}
	
	
	internal class ParameterNode:AbstractFieldsNode
	{
		public ParameterNode(string nodeName,int imageIndex):base(nodeName)
		{
			this.ImageIndex = imageIndex;
			this.SelectedImageIndex = imageIndex;
		}
	}
	
	
	internal class SortColumnNode:AbstractFieldsNode
	{
		ListSortDirection listSortDirection;
		
		public SortColumnNode(string nodeName,string contextMenuPath):this(nodeName,0,contextMenuPath)
		{
			
		}
		
		public SortColumnNode(string nodeName,int imageIndex,string contextMenuPath):base(nodeName,contextMenuPath)
		{
			this.ImageIndex = imageIndex;
			this.SelectedImageIndex = imageIndex;
			this.ContextMenuAddinTreePath = contextMenuPath;
		}
		
		public ListSortDirection SortDirection {
			get { return listSortDirection; }
			set { listSortDirection = value; }
		}
		
		public string FieldName {get;set;}
	}
	
	internal class GroupColumnNode:SortColumnNode
	{
		public  GroupColumnNode (string nodeName,string contextMenuPath):this(nodeName,0,contextMenuPath)
		{
			
		}
		public GroupColumnNode (string nodeName,int imageIndex,string contextMenuPath):base(nodeName,imageIndex,contextMenuPath)
		{
			
		}
	}
}
