// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger
{
	class ValueItem: ListItem
	{
		NamedValue val;
		
		public NamedValue Value {
			get {
				return val;
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
		
		public override int ImageIndex {
			get {
				if (val.IsObject) {
					return 0; // Class
				} else {
					return 1; // Field
				}
			}
		}
		
		public override string Name {
			get {
				return val.Name;
			}
		}
		
		public override string Text {
			get {
				if (ShowValuesInHexadecimal && val.IsInteger) {
					return String.Format("0x{0:X}", val.PrimitiveValue);
				} else {
					return val.AsString;
				}
			}
		}
		
		public override bool CanEditText {
			get {
				return val.IsInteger && !ShowValuesInHexadecimal;
			}
		}
		
		public override string Type {
			get {
				if (val.Type != null) {
					return val.Type.FullName;
				} else {
					return String.Empty;
				}
			}
		}
		
		public override bool HasSubItems {
			get {
				return val.IsObject || val.IsArray;
			}
		}
		
		public override IList<ListItem> SubItems {
			get {
				List<ListItem> list = new List<ListItem>();
				if (val.IsArray) {
					foreach(NamedValue element in val.GetArrayElements()) {
						list.Add(new ValueItem(element));
					}
				}
				if (val.IsObject) {
					return new BaseTypeItem(val, val.Type).SubItems;
				}
				return list;
			}
		}
		
		public ValueItem(NamedValue val)
		{
			this.val = val;
			
			val.Changed += delegate { OnChanged(new ListItemEventArgs(this)); };
		}
		
		public override bool SetText(string newValue)
		{
			try {
				val.PrimitiveValue = newValue;
				return true;
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
			return false;
		}
		
		public override ContextMenuStrip GetContextMenu()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			
			ToolStripMenuItem copyItem;
			copyItem = new ToolStripMenuItem();
			copyItem.Text = ResourceService.GetString("MainWindow.Windows.Debug.LocalVariables.CopyToClipboard");
			copyItem.Checked = false;
			copyItem.Click += delegate {
				ClipboardWrapper.SetText(this.Text);
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
			
			return menu;
		}
	}
}
