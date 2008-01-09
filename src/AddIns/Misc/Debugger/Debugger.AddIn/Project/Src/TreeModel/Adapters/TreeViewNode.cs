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
		static Dictionary<string, bool> expandedNodes = new Dictionary<string, bool>();
		
		AbstractNode content;
		
		bool childsLoaded;
		
		public AbstractNode Content {
			get { return content; }
		}
		
		string FullName {
			get {
				if (this.Parent != null && this.Parent is TreeViewNode) {
					return ((TreeViewNode)this.Parent).FullName + "." + Content.Name;
				} else {
					return Content.Name;
				}
			}
		}
		
		public TreeViewNode(TreeViewAdv tree, AbstractNode content): base(tree, new object())
		{
			SetContentRecursive(content);
		}
		
		public void SetContentRecursive(AbstractNode content)
		{
			this.content = content;
			this.IsLeaf = (content.ChildNodes == null);
			childsLoaded = false;
			if (content.ChildNodes != null && expandedNodes.ContainsKey(this.FullName) && expandedNodes[this.FullName]) {
				LoadChilds();
				this.Expand();
			} else {
				this.Children.Clear();
				this.Collapse();
			}
			this.Tree.Invalidate();
		}
		
		public static void SetContentRecursive(TreeViewAdv tree, IList<TreeNodeAdv> childNodes, IEnumerable<AbstractNode> contentEnum)
		{
			contentEnum = contentEnum ?? new AbstractNode[0];
			
			DoEvents();
			int index = 0;
			foreach(AbstractNode content in contentEnum) {
				// Add or overwrite existing items
				if (index < childNodes.Count) {
					// Overwrite
					((TreeViewNode)childNodes[index]).SetContentRecursive(content);
				} else {
					// Add
					childNodes.Add(new TreeViewNode(tree, content));
				}
				DoEvents();
				index++;
			}
			int count = index;
			// Delete other nodes
			while(childNodes.Count > count) {
				childNodes.RemoveAt(count);
			}
			DoEvents();
		}
		
		protected override void OnExpanding()
		{
			base.OnExpanding();
			LoadChilds();
		}
		
		void LoadChilds()
		{
			if (!childsLoaded) {
				childsLoaded = true;
				SetContentRecursive(this.Tree, this.Children, this.Content.ChildNodes);
				this.IsExpandedOnce = true;
			}
		}
		
		protected override void OnExpanded()
		{
			base.OnExpanded();
			expandedNodes[FullName] = true;
		}
		
		protected override void OnCollapsing()
		{
			base.OnCollapsing();
		}
		
		protected override void OnCollapsed()
		{
			base.OnCollapsed();
			expandedNodes[FullName] = false;
		}
		
		
		static DateTime nextDoEventsTime = Debugger.Util.HighPrecisionTimer.Now;
		const double workLoad    = 0.75; // Fraction of getting variables vs. repainting
		const double maxFPS      = 30;   // this prevents too much drawing on good machine
		const double maxWorkTime = 250;  // ms  this ensures minimal response on bad machine
		
		static void DoEvents()
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
				// LoggingService.InfoFormatted("Rendering: {0} ms => work budget: {1} ms ({2:f1} FPS)", doEventsDuration, workTime, fps);
			}
		}
	}
}
