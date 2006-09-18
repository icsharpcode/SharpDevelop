// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.TreeGrid;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DynamicTreeDebuggerRow:DynamicTreeRow
	{
		// Columns:
		// 0 = plus sign
		// 1 = icon
		// 2 = text
		// 3 = value
		
		Variable variable;
		Image image;
		bool populated = false;
		bool dirty = true;
		
		public Variable Variable {
			get {
				return variable;
			}
			set {
				variable = value;
			}
		}
		
		public bool ShowValuesInHexadecimal {
			get {
				return ((WindowsDebugger)DebuggerService.CurrentDebugger).Properties.Get("ShowValuesInHexadecimal", false);
			}
			set {
				((WindowsDebugger)DebuggerService.CurrentDebugger).Properties.Set("ShowValuesInHexadecimal", value);
			}
		}
		
		public DynamicTreeDebuggerRow()
		{
		}
		
		public DynamicTreeDebuggerRow(Variable variable)
		{
			if (variable == null) throw new ArgumentNullException("variable");
			
			this.variable = variable;
			this.Shown += delegate {
				this.variable.Changed += Update;
				dirty = true;
				DoInPausedState( delegate { Update(); } );
			};
			this.Hidden += delegate {
				this.variable.Changed -= Update;
			};
			
			DebuggerGridControl.AddColumns(this.ChildColumns);
			
			this[1].Paint += OnIconPaint;
			this[3].FinishLabelEdit += OnLabelEdited;
			this[3].MouseDown += OnMouseDown;
			
			Update();
		}
		
		void Update(object sender, ProcessEventArgs e)
		{
			dirty = true;
			Update();
		}
		
		void Update()
		{
			if (!dirty) return;
			
			image = DebuggerIcons.GetImage(variable);
			this[1].Text = ""; // Icon
			this[2].Text = variable.Name;
			if (ShowValuesInHexadecimal && variable.Value is PrimitiveValue && variable.Value.IsInteger) {
				this[3].Text = String.Format("0x{0:X}", (variable.Value as PrimitiveValue).Primitive);
			} else {
				this[3].Text = variable.Value.AsString;
			}
			this[3].AllowLabelEdit = variable.Value is PrimitiveValue &&
			                         variable.Value.ManagedType != typeof(string) &&
			                         !ShowValuesInHexadecimal;
			ObjectValue objValue = variable.Value as ObjectValue;
			if (objValue != null) {
				objValue.ToStringText.Changed -= Update;
				objValue.ToStringText.Changed += Update;
				this[3].Text = objValue.ToStringText.Value.AsString;
			}
			
			this.ShowPlus = variable.Value.MayHaveSubVariables;
			this.ShowMinusWhileExpanded = true;
			
			dirty = false;
		}
		
		void OnIconPaint(object sender, ItemPaintEventArgs e)
		{
			if (image != null) {
				e.Graphics.DrawImageUnscaled(image, e.ClipRectangle);
			}
		}
		
		void OnLabelEdited(object sender, DynamicListEventArgs e)
		{
			PrimitiveValue val = (PrimitiveValue)variable.Value;
			string newValue = ((DynamicListItem)sender).Text;
			try {
				val.Primitive = newValue;
			} catch (NotSupportedException) {
				string format = ResourceService.GetString("MainWindow.Windows.Debug.LocalVariables.CannotSetValue.BadFormat");
				string msg = String.Format(format, newValue, val.ManagedType.ToString());
				MessageService.ShowMessage(msg ,"${res:MainWindow.Windows.Debug.LocalVariables.CannotSetValue.Title}");
			} catch (COMException) {
				// COMException (0x80131330): Cannot perfrom SetValue on non-leaf frames.
				// Happens if trying to set value after exception is breaked
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.LocalVariables.CannotSetValue.UnknownError}",
				                           "${res:MainWindow.Windows.Debug.LocalVariables.CannotSetValue.Title}");
			}
		}
		
		void OnMouseDown(object sender, DynamicListMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) {
				ContextMenuStrip menu = new ContextMenuStrip();
				
				ToolStripMenuItem copyItem;
				copyItem = new ToolStripMenuItem();
				copyItem.Text = ResourceService.GetString("MainWindow.Windows.Debug.LocalVariables.CopyToClipboard");
				copyItem.Checked = false;
				copyItem.Click += delegate {
					ClipboardWrapper.SetText(((DynamicListItem)sender).Text);
				};
				
				ToolStripMenuItem hewView;
				hewView = new ToolStripMenuItem();
				hewView.Text = ResourceService.GetString("MainWindow.Windows.Debug.LocalVariables.ShowInHexadecimal");
				hewView.Checked = ShowValuesInHexadecimal;
				hewView.Click += delegate {
					ShowValuesInHexadecimal = !ShowValuesInHexadecimal;
				};
				
				menu.Items.AddRange(new ToolStripItem[] {
				                    	copyItem,
				                    	hewView
				                    });
				
				menu.Show(e.List, e.Location);
			}
		}
		
		/// <summary>
		/// Called when plus is pressed in debugger tooltip.
		/// Sets the data to be show in the next level.
		/// </summary>
		protected override void OnExpanding(DynamicListEventArgs e)
		{
			if (!populated) {
				DoInPausedState(delegate { Populate(); });
			}
		}
		
		void DoInPausedState(MethodInvoker action)
		{
			if (Variable.Process.IsPaused) {
				action();
			} else {
				EventHandler<ProcessEventArgs> onDebuggingPaused = null;
				onDebuggingPaused = delegate {
					action();
					Variable.Process.DebuggingPaused -= onDebuggingPaused;
				};
				Variable.Process.DebuggingPaused += onDebuggingPaused;
			}
		}
		
		void Populate()
		{
			Fill(this, Variable.Value.SubVariables);
			populated = true;
		}
		
		static void Fill(DynamicTreeRow row, VariableCollection collection)
		{
			row.ChildRows.Clear();
			foreach(VariableCollection sub in collection.SubCollections) {
				VariableCollection subCollection = sub;
				
				DynamicTreeRow subMenu = new DynamicTreeRow();
				DebuggerGridControl.AddColumns(subMenu.ChildColumns);
				subMenu[2].Text = subCollection.Name;
				subMenu[3].Text = subCollection.Value;
				subMenu.ShowMinusWhileExpanded = true;
				subMenu.ShowPlus = !subCollection.IsEmpty;
				
				EventHandler<DynamicListEventArgs> populate = null;
				populate = delegate {
					Fill(subMenu, subCollection);
					subMenu.Expanding -= populate;
				};
				subMenu.Expanding += populate;
				
				row.ChildRows.Add(subMenu);
			}
			foreach(Variable variable in collection.Items) {
				row.ChildRows.Add(new DynamicTreeDebuggerRow(variable));
			}
		}
	}
}
