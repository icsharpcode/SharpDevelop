// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
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

namespace Debugger.AddIn.TreeModel
{
	public partial class TreeViewNode: TreeNodeAdv
	{
		AbstractNode content;
		
		bool populated = false;
		
		public AbstractNode Content {
			get { return content; }
			set { content = value; }
		}
		
		public TreeViewNode(TreeViewAdv tree, AbstractNode content): base(tree, new object())
		{
			this.content = content;
			
			this.IsLeaf = content.ChildNodes == null;
		}
		
		public void OnExpanding()
		{
			if (!populated) {
				foreach(AbstractNode childNode in this.Content.ChildNodes) {
					Children.Add(new TreeViewNode(Tree, childNode));
				}
				populated = true;
				this.IsExpandedOnce = true;
				this.Tree.UpdateSelection();
				this.Tree.FullUpdate();
			}
		}
		
		public static void OverwriteNodes(TreeViewAdv tree, Collection<TreeNodeAdv> treeNodes, IEnumerable<AbstractNode> modelNodes)
		{
			modelNodes = modelNodes ?? new AbstractNode[0];
			
			int index = 0;
			foreach(AbstractNode modelNode in modelNodes) {
				// Add or overwrite existing items
				if (index < treeNodes.Count) {
					// Overwrite
					((TreeViewNode)treeNodes[index]).Content = modelNode;
				} else {
					// Add
					treeNodes.Add(new TreeViewNode(tree, modelNode));
				}
				index++;
			}
			int count = index;
			// Delete other nodes
			while(treeNodes.Count > count) {
				treeNodes.RemoveAt(count);
			}
			
			tree.FullUpdate();
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
