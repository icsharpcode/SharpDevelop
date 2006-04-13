// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 1253 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public partial class CallStackPad
	{
		public bool ShowArgumentNames {
			get {
				return debugger.Properties.Get("ShowArgumentNames", true);
			}
			set {
				debugger.Properties.Set("ShowArgumentNames", value);
			}
		}
		
		public bool ShowArgumentValues {
			get {
				return debugger.Properties.Get("ShowArgumentValues", true);
			}
			set {
				debugger.Properties.Set("ShowArgumentValues", value);
			}
		}
		
		public bool ShowExternalMethods {
			get {
				return debugger.Properties.Get("ShowExternalMethods", false);
			}
			set {
				debugger.Properties.Set("ShowExternalMethods", value);
			}
		}
		
		ContextMenuStrip CreateContextMenuStrip()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			menu.Opening += FillContextMenuStrip;
			return menu;
		}
		
		void FillContextMenuStrip(object sender, CancelEventArgs e)
		{
			ContextMenuStrip menu = sender as ContextMenuStrip;
			menu.Items.Clear();
			
			ToolStripMenuItem argNamesItem;
			argNamesItem = new ToolStripMenuItem();
			argNamesItem.Text = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowArgumentNames");
			argNamesItem.Checked = ShowArgumentNames;
			argNamesItem.Click += delegate {
				ShowArgumentNames = !ShowArgumentNames;
				RefreshPad();
			};
			
			ToolStripMenuItem argValuesItem;
			argValuesItem = new ToolStripMenuItem();
			argValuesItem.Text = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowArgumentValues");
			argValuesItem.Checked = ShowArgumentValues;
			argValuesItem.Click += delegate {
				ShowArgumentValues = !ShowArgumentValues;
				RefreshPad();
			};
			
			ToolStripMenuItem extMethodsItem;
			extMethodsItem = new ToolStripMenuItem();
			extMethodsItem.Text = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowExternalMethods");
			extMethodsItem.Checked = ShowExternalMethods;
			extMethodsItem.Click += delegate {
				ShowExternalMethods = !ShowExternalMethods;
				RefreshPad();
			};
			
			menu.Items.AddRange(new ToolStripItem[] {
			                    	argNamesItem,
			                    	argValuesItem,
			                    	extMethodsItem
			                    });
			
			e.Cancel = false;
		}
	}
}
