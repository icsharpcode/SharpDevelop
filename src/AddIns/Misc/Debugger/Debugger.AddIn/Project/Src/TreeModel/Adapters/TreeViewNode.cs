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

using Debugger.Util;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.Pads;

namespace Debugger.AddIn.TreeModel
{
	public partial class TreeViewNode: TreeNodeAdv
	{
		static Dictionary<string, bool> expandedNodes = new Dictionary<string, bool>();
		
		LocalVarPad localVarPad;
		AbstractNode content;
		
		bool childsLoaded;
		bool textChanged;
		
		public AbstractNode Content {
			get { return content; }
		}
		
		public bool TextChanged {
			get { return textChanged; }
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
		
		public TreeViewNode(LocalVarPad localVarPad, AbstractNode content): base(localVarPad.LocalVarList, new object())
		{
			this.localVarPad = localVarPad;
			SetContentRecursive(content);
		}
		
		static TimeSpan workTime = TimeSpan.FromMilliseconds(40);
		static DateTime nextRepaintTime = DateTime.MinValue;
		
		public void SetContentRecursive(AbstractNode content)
		{
			this.textChanged =
				this.content != null &&
				this.content.Name == content.Name &&
				this.content.Text != content.Text;
			this.content = content;
			this.IsLeaf = (content.ChildNodes == null);
			childsLoaded = false;
			this.IsExpandedOnce = false;
			if (!IsLeaf && expandedNodes.ContainsKey(this.FullName) && expandedNodes[this.FullName]) {
				LoadChilds();
				this.Expand();
			} else {
				this.Children.Clear();
				this.Collapse();
			}
			// Process user commands
			Utils.DoEvents(localVarPad.Process.DebuggeeState);
			// Repaint
			if (HighPrecisionTimer.Now > nextRepaintTime) {
				using(new PrintTime("Repainting Local Variables Pad")) {
					try {
						this.Tree.EndUpdate();   // Enable painting
						Utils.DoEvents(localVarPad.Process.DebuggeeState); // Paint
					} finally {
						this.Tree.BeginUpdate(); // Disable painting
						nextRepaintTime = HighPrecisionTimer.Now + workTime;
					}
				}
			}
		}
		
		public static void SetContentRecursive(LocalVarPad localVarPad, IList<TreeNodeAdv> childNodes, IEnumerable<AbstractNode> contentEnum)
		{
			contentEnum = contentEnum ?? new AbstractNode[0];
			
			int index = 0;
			foreach(AbstractNode content in contentEnum) {
				// Add or overwrite existing items
				if (index < childNodes.Count) {
					// Overwrite
					((TreeViewNode)childNodes[index]).SetContentRecursive(content);
				} else {
					// Add
					childNodes.Add(new TreeViewNode(localVarPad, content));
				}
				index++;
			}
			int count = index;
			// Delete other nodes
			while(childNodes.Count > count) {
				childNodes.RemoveAt(count);
			}
		}
		
		protected override void OnExpanding()
		{
			base.OnExpanding();
		}
		
		void LoadChilds()
		{
			if (!childsLoaded) {
				childsLoaded = true;
				this.IsExpandedOnce = true;
				SetContentRecursive(localVarPad, this.Children, this.Content.ChildNodes);
			}
		}
		
		protected override void OnExpanded()
		{
			base.OnExpanded();
			expandedNodes[FullName] = true;
			if (localVarPad.Process.IsRunning) {
				MessageService.ShowMessage(
					"${res:MainWindow.Windows.Debug.LocalVariables.CannotExploreVariablesWhileRunning}",
					"${res:MainWindow.Windows.Debug.LocalVariables}"
				);
				return;
			}
			try {
				this.Tree.BeginUpdate();
				LoadChilds();
			} catch (AbortedBecauseDebuggeeResumedException) {
			} finally {
				this.Tree.EndUpdate();
			}
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
	}
}
