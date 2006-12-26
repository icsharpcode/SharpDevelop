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
using ICSharpCode.SharpDevelop.Widgets.TreeGrid;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DynamicTreeDebuggerRow: DynamicTreeRow
	{
		// Columns:
		// 0 = plus sign
		// 1 = icon
		// 2 = text
		// 3 = value
		
		NamedValue val;
		Image image;
		bool populated = false;
		bool visible = true;
		
		public NamedValue Value {
			get {
				return val;
			}
			set {
				val = value;
			}
		}
		
		public static bool ShowValuesInHexadecimal {
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
		
		public DynamicTreeDebuggerRow(NamedValue val)
		{
			if (val == null) throw new ArgumentNullException("val");
			
			this.val = val;
			this.val.Changed += delegate { Update(); };
			this.Shown += delegate {
				visible = true;
				DoInPausedState( delegate { Update(); } );
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
			
			image = DebuggerIcons.GetImage(val);
			this[1].Text = ""; // Icon
			this[2].Text = val.Name;
			if (ShowValuesInHexadecimal && val.IsInteger) {
				this[3].Text = String.Format("0x{0:X}", val.PrimitiveValue);
			} else {
				this[3].Text = val.AsString;
			}
			this[3].AllowLabelEdit = val.IsInteger && !ShowValuesInHexadecimal;
			
			this.ShowPlus = val.IsObject || val.IsArray;
			this.ShowMinusWhileExpanded = true;
		}
		
		void OnIconPaint(object sender, ItemPaintEventArgs e)
		{
			if (image != null) {
				e.Graphics.DrawImageUnscaled(image, e.ClipRectangle);
			}
		}
		
		void OnLabelEdited(object sender, DynamicListEventArgs e)
		{
			string newValue = ((DynamicListItem)sender).Text;
			try {
				val.PrimitiveValue = newValue;
			} catch (NotSupportedException) {
				string format = ResourceService.GetString("MainWindow.Windows.Debug.LocalVariables.CannotSetValue.BadFormat");
				string msg = String.Format(format, newValue, val.Type.ManagedType.ToString());
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
			if (val.Process.IsPaused) {
				action();
			} else {
				EventHandler<ProcessEventArgs> onDebuggingPaused = null;
				onDebuggingPaused = delegate {
					action();
					val.Process.DebuggingPaused -= onDebuggingPaused;
				};
				val.Process.DebuggingPaused += onDebuggingPaused;
			}
		}
		
		void Populate()
		{
			this.ChildRows.Clear();
			if (val.IsArray) {
				AddCollection(this, val.GetArrayElements());
			}
			if (val.IsObject) {
				AddObjectRows(this, val.Type);
			}
			populated = true;
		}
		
		void AddObjectRows(DynamicTreeRow tree, DebugType type)
		{
			NamedValueCollection publicInstance  = val.GetMembers(type, BindingFlags.Public | BindingFlags.Instance);
			NamedValueCollection publicStatic    = val.GetMembers(type, BindingFlags.Public | BindingFlags.Static);
			NamedValueCollection privateInstance = val.GetMembers(type, BindingFlags.NonPublic | BindingFlags.Instance);
			NamedValueCollection privateStatic   = val.GetMembers(type, BindingFlags.NonPublic | BindingFlags.Static);
			
			if (type.BaseType != null) {
				tree.ChildRows.Add(MakeMenu("Base class", type.BaseType.FullName,
				                            delegate(DynamicTreeRow t) { AddObjectRows(t, type.BaseType); } ));
			}
			
			if (publicStatic.Count > 0) {
				tree.ChildRows.Add(MakeMenu("Static members", String.Empty,
				                            delegate(DynamicTreeRow t) { AddCollection(t, publicStatic); } ));
			}
			
			if (privateInstance.Count > 0 || privateStatic.Count > 0) {
				tree.ChildRows.Add(MakeMenu(
					"Non-public members", String.Empty,
					delegate(DynamicTreeRow t) {
						if (privateStatic.Count > 0) {
							t.ChildRows.Add(MakeMenu("Static members", String.Empty,
							                         delegate(DynamicTreeRow t2) { AddCollection(t2, privateStatic); } ));
						}
						AddCollection(t, privateInstance);
					}));
			}
			
			AddCollection(this, publicInstance);
		}
		
		void AddCollection(DynamicTreeRow tree, NamedValueCollection collection)
		{
			foreach(NamedValue val in collection) {
				tree.ChildRows.Add(new DynamicTreeDebuggerRow(val));
			}
		}
		
		delegate void PopulateMenu(DynamicTreeRow row);
		
		DynamicTreeRow MakeMenu(string name, string text, PopulateMenu populate)
		{
			DynamicTreeRow menu = new DynamicTreeRow();
			DebuggerGridControl.AddColumns(menu.ChildColumns);
			menu[2].Text = name;
			menu[3].Text = text;
			menu.ShowMinusWhileExpanded = true;
			
			bool populated = false;
			menu.Expanding += delegate {
				if (!populated) {
					populate(menu);
					populated = true;
				}
			};
			
			return menu;
		}
	}
}
