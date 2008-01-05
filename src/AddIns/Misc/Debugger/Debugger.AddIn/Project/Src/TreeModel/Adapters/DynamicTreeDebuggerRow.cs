// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Debugger;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Widgets.TreeGrid;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.TreeModel
{
	public class DynamicTreeDebuggerRow: DynamicTreeRow
	{
		// Columns:
		// 0 = plus sign
		// 1 = icon
		// 2 = text
		// 3 = value
		
		AbstractNode content;
		
		bool populated = false;
		bool visible = true;
		
		public AbstractNode Content {
			get { return content; }
		}
		
		public DynamicTreeDebuggerRow(AbstractNode content)
		{
			this.content = content;
			
			this.Shown += delegate {
				visible = true;
				WindowsDebugger.DoInPausedState( delegate { Update(); } );
			};
			this.Hidden += delegate {
				visible = false;
			};
			
			DebuggerGridControl.AddColumns(this.ChildColumns);
			
			this[1].Paint += OnIconPaint;
			this[3].FinishLabelEdit += OnLabelEdited;
			this[3].MouseDown += OnMouseDown;
			
			Update();
		}
		
		void Update()
		{
			if (!visible) return;
			
			this[1].Text = ""; // Icon
			this[2].Text = content.Name;
			this[3].Text = content.Text;
			this[3].AllowLabelEdit = content is ISetText;
			
			this.ShowPlus = content.ChildNodes != null;
			this.ShowMinusWhileExpanded = true;
		}
		
		void OnIconPaint(object sender, ItemPaintEventArgs e)
		{
			if (content.Image != null) {
				e.Graphics.DrawImageUnscaled(content.Image, e.ClipRectangle);
			}
		}
		
		void OnLabelEdited(object sender, DynamicListEventArgs e)
		{
			((ISetText)content).SetText(((DynamicListItem)sender).Text);
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
		
		/// <summary>
		/// Called when plus is pressed in debugger tooltip.
		/// Sets the data to be show in the next level.
		/// </summary>
		protected override void OnExpanding(DynamicListEventArgs e)
		{
			if (!populated) {
				WindowsDebugger.DoInPausedState(delegate { Populate(); });
			}
		}
		
		void Populate()
		{
			this.ChildRows.Clear();
			foreach(AbstractNode childNode in content.ChildNodes) {
				this.ChildRows.Add(new DynamicTreeDebuggerRow(childNode));
			}
			populated = true;
		}
	}
}
