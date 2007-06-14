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
		
		ListItem listItem;
		
		Image image;
		bool populated = false;
		bool visible = true;
		
		public ListItem ListItem {
			get {
				return listItem;
			}
		}
		
		public DynamicTreeDebuggerRow(NamedValue val): this(new ValueItem(val))
		{
			
		}
		
		public DynamicTreeDebuggerRow(ListItem listItem)
		{
			if (listItem == null) throw new ArgumentNullException("listItem");
			
			this.listItem = listItem;
			this.listItem.Changed += delegate { Update(); };
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
			
			this.image = listItem.Image;
			this[1].Text = ""; // Icon
			this[2].Text = listItem.Name;
			this[3].Text = listItem.Text;
			this[3].AllowLabelEdit = listItem.CanEditText;
			
			this.ShowPlus = listItem.HasSubItems;
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
			listItem.SetText(((DynamicListItem)sender).Text);
		}
		
		void OnMouseDown(object sender, DynamicListMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) {
				ContextMenuStrip menu = listItem.GetContextMenu();
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
			foreach(ListItem subItem in listItem.SubItems) {
				this.ChildRows.Add(new DynamicTreeDebuggerRow(subItem));
			}
			populated = true;
		}
	}
}
