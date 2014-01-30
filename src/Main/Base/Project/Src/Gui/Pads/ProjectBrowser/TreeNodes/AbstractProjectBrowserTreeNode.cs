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
		public virtual ISolution Solution {
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
		/// any node is child of a project. Keep in mind that Solution Folders 
		/// and Solution Items do not have a project assigned.
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
			SD.Workbench.GetPad(typeof(PropertyPad)).BringPadToFront();
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
			return StringParser.Parse(question, new StringTagPair("FileName", Text));
		}

		public virtual AbstractProjectBrowserTreeNode GetNodeByRelativePath(string relativePath)
		{

			if (relativePath == Text)
				return this;

			string[] targets = relativePath.Trim('/', '\\').Split('/', '\\');
			if (targets[0] != Text)
				return null;

			if (!this.IsExpanded)
			{
				// the targetNode is not expanded so it's as deep as we can go
				//LoggingService.DebugFormatted("target node '{0};{1}' is not expanded.", targetNode, targetNode.Text);
				return this;
			}

			string currentPath = relativePath.Trim('/', '\\').RemoveFromStart(targets[0]).Trim('/', '\\');
			//LoggingService.Debug("entering depth loop...");
			//LoggingService.DebugFormatted(@"\- looking for '{0}'", relativePath);
			//LoggingService.DebugFormatted(@"\- starting at '{0}'", targetNode != null ? targetNode.Text : "null");

			//LoggingService.Debug("-- looking for: "+target);
			foreach (AbstractProjectBrowserTreeNode node in this.Nodes)
			{
				if (node == null)
				{
					// can happen when the node is currently expanding
					continue;
				}
				AbstractProjectBrowserTreeNode tempNode = node.GetNodeByRelativePath(currentPath);
				if (tempNode != null)
					return tempNode;
			}

			return null;
		}
	}
}
