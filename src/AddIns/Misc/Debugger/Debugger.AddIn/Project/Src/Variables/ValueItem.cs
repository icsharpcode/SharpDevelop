// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
#region License
//  
//  Copyright (c) 2007, ic#code
//  
//  All rights reserved.
//  
//  Redistribution  and  use  in  source  and  binary  forms,  with  or without
//  modification, are permitted provided that the following conditions are met:
//  
//  1. Redistributions  of  source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//  
//  2. Redistributions  in  binary  form  must  reproduce  the  above copyright
//     notice,  this  list  of  conditions  and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//  
//  3. Neither the name of the ic#code nor the names of its contributors may be
//     used  to  endorse or promote products derived from this software without
//     specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND  ANY  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE  DISCLAIMED.   IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//  LIABLE  FOR  ANY  DIRECT,  INDIRECT,  INCIDENTAL,  SPECIAL,  EXEMPLARY,  OR
//  CONSEQUENTIAL  DAMAGES  (INCLUDING,  BUT  NOT  LIMITED  TO,  PROCUREMENT OF
//  SUBSTITUTE  GOODS  OR  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION)  HOWEVER  CAUSED  AND  ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT,  STRICT  LIABILITY,  OR  TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING  IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//  
#endregion

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
