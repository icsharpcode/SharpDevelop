// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	[Flags]
	public enum FileNodeStatus {
		None = 1,
		InProject = 2,
		Missing = 4,
		BehindFile = 8,
		Link = 16,
	}
	
	public abstract class AbstractProjectBrowserTreeNode : ExtTreeNode, IDisposable
	{
		string                  toolbarAddinTreePath     = null;
		
		protected bool autoClearNodes = true;
		
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
		/// STATIC event called after the a new tree node was added to the project browser.
		/// </summary>
		public static event TreeViewEventHandler OnNewNode;
		
		bool isNewNode = true;
		
		public override void Refresh()
		{
			base.Refresh();
			if (isNewNode) {
				isNewNode = false;
				if (OnNewNode != null)
					OnNewNode(null, new TreeViewEventArgs(this));
			}
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
		
		protected string GetQuestionText(string question)
		{
			return StringParser.Parse(question, new string[,] {{"FileName", Text}});
		}
	}
}
