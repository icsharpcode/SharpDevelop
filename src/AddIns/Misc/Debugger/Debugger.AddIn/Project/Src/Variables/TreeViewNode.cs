// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

using Aga.Controls.Tree;

using ICSharpCode.Core;

namespace Debugger
{
	public partial class TreeViewNode: TreeNodeAdv
	{
		ListItem content;
		Image icon;
		string name;
		string text;
		bool canEditText;
		string type;
		
		public ListItem Content {
			get {
				return content;
			}
			set {
				if (content != null) {
					content.Changed -= OnContentChanged;
				}
				content = value;
				if (content != null) {
					content.Changed += OnContentChanged;
				}
				Update();
			}
		}
		
		public Image Icon {
			get { return icon; }
		}
		
		public string Name {
			get { return name; }
		}
		
		public string Text {
			get { return text; }
		}
		
		public bool CanEditText {
			get { return canEditText; }
		}
		
		public string Type {
			get { return type; }
		}
		
		void OnContentChanged(object sender, ListItemEventArgs e)
		{
			//Update();
		}
		
		public TreeViewNode(TreeViewAdv tree, ListItem content): base(tree, new object())
		{
			this.Content = content;
		}
		
		public void Update()
		{
			DoApplicationEvents();
			
			DateTime start = Debugger.Util.HighPrecisionTimer.Now;
			
			this.IsLeaf = !Content.HasSubItems;
			this.icon = content.Image;
			this.name = content.Name;
			this.text = content.Text;
			this.canEditText = content.CanEditText;
			this.type = content.Type;
			//DateTime time = Debugger.Util.HighPrecisionTimer.Now;
			//this.type = time.ToLongTimeString() + "." + time.Millisecond.ToString();
			
			DateTime end = Debugger.Util.HighPrecisionTimer.Now;
			
			LoggingService.InfoFormatted("Updated node {0} ({1} ms)", FullName, (end - start).TotalMilliseconds);
			
			if (this.IsExpanded) {
				UpdateNodes(Tree, this.Children, Content.SubItems);
			} else {
				Children.Clear();
				populated = false;
			}
			
			this.Tree.FullUpdate();
		}
		
		public static void UpdateNodes(TreeViewAdv tree, Collection<TreeNodeAdv> collection, IList<ListItem> contents)
		{
			// Add or overwrite existing items
			for(int i = 0; i < contents.Count; i++) {
				if (i < collection.Count) {
					// Overwrite
					((TreeViewNode)collection[i]).Content = contents[i];
				} else {
					// Add
					collection.Add(new TreeViewNode(tree, contents[i]));
				}
			}
			// Delete other nodes
			while(collection.Count > contents.Count) {
				collection.RemoveAt(collection.Count - 1);
			}
			
			tree.FullUpdate();
		}
		
		bool populated = false;
		
		public void OnExpanding()
		{
			if (!populated) {
				foreach(ListItem item in Content.SubItems) {
					Children.Add(new TreeViewNode(Tree, item));
				}
				populated = true;
				this.IsExpandedOnce = true;
				this.Tree.UpdateSelection();
				this.Tree.FullUpdate();
			}
		}
		
		#region DoApplicationEvents()
		
		static DateTime nextDoEventsTime = Debugger.Util.HighPrecisionTimer.Now;
		const double workLoad    = 0.75; // Fraction of getting variables vs. repainting
		const double maxFPS      = 30;   // ms  this prevents too much drawing on good machine
		const double maxWorkTime = 250;  // ms  this ensures minimal response on bad machine
		
		void DoApplicationEvents()
		{
			if (Debugger.Util.HighPrecisionTimer.Now > nextDoEventsTime) {
				DateTime start = Debugger.Util.HighPrecisionTimer.Now;
				Application.DoEvents();
				DateTime end = Debugger.Util.HighPrecisionTimer.Now;
				double doEventsDuration = (end - start).TotalMilliseconds;
				double minWorkTime = 1000 / maxFPS - doEventsDuration; // ms
				double workTime = (doEventsDuration / (1 - workLoad)) * workLoad;
				workTime = Math.Max(minWorkTime, Math.Min(maxWorkTime, workTime)); // Clamp
				nextDoEventsTime = end.AddMilliseconds(workTime);
				double fps = 1000 / (doEventsDuration + workTime);
				LoggingService.InfoFormatted("Rendering: {0} ms => work budget: {1} ms ({2:f1} FPS)", doEventsDuration, workTime, fps);
			}
		}
		
		#endregion
		
		#region Maintain expanded state
		
		static Dictionary<string, bool> expandedNodes = new Dictionary<string, bool>();
		
		string FullName {
			get {
				if (this.Parent != null && this.Parent is TreeViewNode) {
					return ((TreeViewNode)this.Parent).FullName + "." + Content.Name;
				} else {
					return Content.Name;
				}
			}
		}
		
		public void OnExpanded()
		{
			expandedNodes[FullName] = true;
			// Expand children as well
			foreach(TreeViewNode child in Children) {
				string name = child.FullName;
				if (expandedNodes.ContainsKey(name) && expandedNodes[name]) {
					child.IsExpanded = true;
				}
			}
		}
		
		public void OnCollapsed()
		{
			expandedNodes[FullName] = false;
		}
		
		#endregion
	}
}
