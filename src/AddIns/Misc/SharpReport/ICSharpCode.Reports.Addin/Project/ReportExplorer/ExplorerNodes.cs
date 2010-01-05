/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 16.08.2008
 * Zeit: 14:28
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
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
//		public Image a_IconImage {
//			get {
//				return iconImage;
//			}
//			set {
//				iconImage = value;
//			}
//		}
		
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
	
	
	internal class FunctionNode:AbstractFieldsNode
	{
		public FunctionNode(string nodeName,int imageIndex):base(nodeName)
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
}
