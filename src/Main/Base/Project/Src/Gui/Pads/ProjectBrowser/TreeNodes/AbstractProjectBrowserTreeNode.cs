using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum FileNodeStatus {
		None = 1,
		InProject = 2,
		Missing = 4,
		BehindFile = 8
	}
	
	public abstract class AbstractProjectBrowserTreeNode : ExtTreeNode, IDisposable
	{
		string                  toolbarAddinTreePath     = null;
		ProjectItem             item;
		protected LinkedList<ProjectItem> subItems = new LinkedList<ProjectItem>();
		
		protected bool autoClearNodes = true;
		protected bool canLabelEdited = true;
		
		public ProjectItem Item {
			get {
				return item;
			}
		}
		
		public LinkedList<ProjectItem> SubItems {
			get {
				return subItems;
			}
		}
		
		/// <returns>
		/// True, if this node can be label edited, false otherwise.
		/// </returns>
		public bool CanLabelEdited {
			get {
				return canLabelEdited;
			}
		}
		
		
		public virtual string ToolbarAddinTreePath {
			get {
				return toolbarAddinTreePath;
			}
			set {
				toolbarAddinTreePath = value;
			}
		}
		
		/// <summary>
		/// Returns the solution in which this node belongs to. This assumes that
		/// any node is child of a solution.
		/// </summary>
		public virtual Solution Solution {
			get {
				AbstractProjectBrowserTreeNode parent = Parent as AbstractProjectBrowserTreeNode;
				if (parent != null) {
					return parent.Solution;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Returns the project in which this node belongs to. This assumes that
		/// any node is child of a project. THIS DON'T WORK ON COMBINE NODES!
		/// (a combine node returns null)
		/// </summary>
		public virtual IProject Project {
			get {
				AbstractProjectBrowserTreeNode parent = Parent as AbstractProjectBrowserTreeNode;
				if (parent != null) {
					return parent.Project;
				}
				return null;
			}
		}		
		
		public AbstractProjectBrowserTreeNode()
		{
			item = null;
		}
		
		public static bool ShowAll {
			get {
				return PropertyService.Get("ProjectBrowser.ShowAll", false);
			}
			set {
				PropertyService.Set("ProjectBrowser.ShowAll", value);
			}
		}
		
		public override void Expanding()
		{
			if (isInitialized) {
				return;
			}
			isInitialized = true;
			if (autoClearNodes) {
				Nodes.Clear();
			}
			Initialize();
			base.UpdateVisibility();
		}
		
		public virtual void ShowProperties()
		{
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(typeof(PropertyPad).FullName);
		}
		
		public static bool IsSomewhereBelow(string path, ProjectItem item)
		{
			return item.Include.StartsWith(path);
		}
		
		public static LinkedListNode<T> Remove<T>(LinkedList<T> list, LinkedListNode<T> item)
		{
			LinkedListNode<T> ret  = item.Next;
			if (item == list.First) {
				list.RemoveFirst();
			} else if (item == list.Last) {
				list.RemoveLast();
			} else {
				list.Remove(item);
			}
			return ret;
		}
		
		/// <summary>
		/// STATIC event called after the initialization of every tree node!
		/// </summary>
		public static event TreeViewEventHandler AfterNodeInitialize;
		
		protected override void Initialize()
		{
			base.Initialize();
			if (AfterNodeInitialize != null)
				AfterNodeInitialize(null, new TreeViewEventArgs(this));
		}
		
		Image overlay;
		
		public Image Overlay {
			get {
				return overlay;
			}
			set {
				if (overlay == value) return;
				overlay = value;
				if (TreeView != null && IsVisible) {
					Rectangle r = this.Bounds;
					r.Width += r.X;
					r.X = 0;
					TreeView.Invalidate(r);
				}
			}
		}
		
		public abstract object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data);
		
		public virtual object AcceptChildren(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			foreach (TreeNode node in Nodes) {
				if (node is AbstractProjectBrowserTreeNode) {
					((AbstractProjectBrowserTreeNode)node).AcceptVisitor(visitor, data);
				}
			}
			return data;
		}
	}
}
