//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.Diagnostics;
//using System.Drawing;
//using System.Collections;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.Windows.Forms;
//using System.Windows.Forms.Design;
//
//using System.Drawing.Design;
//
//namespace ICSharpCode.FormDesigner {
//	
//	public class ContextMenuEditorControl : AbstractMenuEditorControl
//	{
//		public ContextMenuEditorControl(IDesignerHost host, Menu menu) : base(host, menu)
//		{
//			typeAreaText     = "Context Menu";
//			canEditMenuItems = false;
//			MeasureMenuItems();
//		}
//		
//		protected override void OnLostFocus(EventArgs e)
//		{
//			MenuEditorFocused = base.subMenuEditor != null && !subMenuEditor.iveTheFocus;
//			base.OnLostFocus(e);
//		}
//		
//		protected override void MeasureMenuItems()
//		{
//			Graphics g = base.CreateGraphics();
//			Font f = System.Windows.Forms.SystemInformation.MenuFont;
//			
//			typeSize = g.MeasureString(typeAreaText, f);
//			typeArea = new RectangleF(this.borderSize.Width, 
//			                          this.borderSize.Height, 
//			                          typeSize.Width,
//			                          SystemInformation.MenuHeight);
//			g.Dispose();
//		}
//		
//		protected override void OnPaint(PaintEventArgs pe)
//		{
//			MeasureMenuItems();
//			DrawTypeHere(pe.Graphics, this.selectedItem == menu.MenuItems.Count, mouseOverItem == menu.MenuItems.Count);
//			if (MenuEditorFocused) {
//				ShowSubMenuEditor();
//			}
//		}
//		
//		protected override void ShowSubMenuEditor()
//		{
//			if (this.selectedItem >= 0) {
//				SubMenuEditorControl panel = new SubMenuEditorControl(host, this, menu);
//				panel.Location = GetSubmenuLocation(this.selectedItem);
//				panel.Parent = rootControl.Parent;
//				panel.BringToFront();
//				panel.Select();
//				CloseSubMenuEditor();
//				this.subMenuEditor = panel;
//			}
//		}
//	}
//}
