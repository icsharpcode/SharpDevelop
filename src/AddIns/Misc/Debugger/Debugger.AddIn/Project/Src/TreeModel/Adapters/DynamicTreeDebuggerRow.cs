// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Widgets.TreeGrid;

namespace Debugger.AddIn.TreeModel
{
	public class DynamicTreeDebuggerRow: DynamicTreeRow
	{
		// Columns:
		// 0 = plus sign
		// 1 = icon
		// 2 = text
		// 3 = value
		Process process;
		
		AbstractNode content;
		
		bool isShown;
		bool isExpanded;
		bool childsLoaded;
		
		public AbstractNode Content {
			get { return content; }
		}
		
		public bool IsShown {
			get { return isShown; }
		}
		
		public bool IsExpanded {
			get { return isExpanded; }
		}
		
		public DynamicTreeDebuggerRow(Process process, AbstractNode content)
		{
			this.process = process;
			
			DebuggerGridControl.AddColumns(this.ChildColumns);
			
			this[1].Paint += OnIconPaint;
			this[3].FinishLabelEdit += OnLabelEdited;
			this[3].MouseDown += OnMouseDown;
			
			this.Expanded += delegate { isExpanded = true; };
			this.Collapsed += delegate { isExpanded = false; };
			this.Shown += delegate { isShown = true; };
			this.Hidden += delegate { isShown = false; };
			
			SetContentRecursive(content);
		}
		
		public void SetContentRecursive(AbstractNode content)
		{
			this.content = content;
			
			this[1].Text = ""; // Icon
			this[2].Text = content.Name;
			this[3].Text = content.Text;
			this[3].AllowLabelEdit = (content is ISetText) && ((ISetText)content).CanSetText;
			
			this.ShowPlus = (content.ChildNodes != null);
			this.ShowMinusWhileExpanded = true;
			
			childsLoaded = false;
			if (content.ChildNodes != null && isExpanded) {
				LoadChilds();
			} else {
				this.ChildRows.Clear();
			}
			
			// Repaint and process user commands
			Utils.DoEvents(process);
		}
		
		public void SetChildContentRecursive(IEnumerable<AbstractNode> contentEnum)
		{
			contentEnum = contentEnum ?? new AbstractNode[0];
			
			int index = 0;
			foreach(AbstractNode content in contentEnum) {
				// Add or overwrite existing items
				if (index < ChildRows.Count) {
					// Overwrite
					((DynamicTreeDebuggerRow)ChildRows[index]).SetContentRecursive(content);
				} else {
					// Add
					ChildRows.Add(new DynamicTreeDebuggerRow(process, content));
				}
				index++;
			}
			int count = index;
			// Delete other nodes
			while(ChildRows.Count > count) {
				ChildRows.RemoveAt(count);
			}
		}
		
		/// <summary>
		/// Called when plus is pressed in debugger tooltip.
		/// Sets the data to be show in the next level.
		/// </summary>
		protected override void OnExpanding(DynamicListEventArgs e)
		{
			base.OnExpanding(e);
			if (process.IsRunning) {
				MessageService.ShowMessage(
					"${res:MainWindow.Windows.Debug.LocalVariables.CannotExploreVariablesWhileRunning}",
					"${res:MainWindow.Windows.Debug.LocalVariables}"
				);
				return;
			}
			try {
				LoadChilds();
			} catch (AbortedBecauseDebuggeeResumedException) {
			}
		}
		
		void LoadChilds()
		{
			if (!childsLoaded) {
				childsLoaded = true;
				SetChildContentRecursive(content.ChildNodes);
			}
		}
		
		void OnIconPaint(object sender, ItemPaintEventArgs e)
		{
			if (content.Image != null) {
				e.Graphics.DrawImageUnscaled(content.Image, e.ClipRectangle);
			}
		}
		
		void OnLabelEdited(object sender, DynamicListEventArgs e)
		{
			if (((ISetText)content).CanSetText) {
				((ISetText)content).SetText(((DynamicListItem)sender).Text);
			}
		}
		
		void OnMouseDown(object sender, DynamicListMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && content is IContextMenu) {
				ContextMenuStrip menu = ((IContextMenu)content).GetContextMenu();
				if (menu != null) {
					menu.Show(e.List, e.Location);
				}
			}
		}
	}
}
