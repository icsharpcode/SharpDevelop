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
//	public class SubMenuEditorControl : AbstractMenuEditorControl
//	{
//		AbstractMenuEditorControl owner;
//		
//		public SubMenuEditorControl(IDesignerHost host, AbstractMenuEditorControl owner, Menu menu) : base(host, menu)
//		{
//			isHorizontal   = false;
//			drawMenuGlyphs = true;
//			borderSize     = SystemInformation.FrameBorderSize;
//			SetSize(null, null);
//			this.owner = owner;
//		}
//		
//		public override void SetSize(object sender, EventArgs e)
//		{
//			try {
//				MeasureMenuItems();
//				int maxWidth = (int)base.typeArea.Width + base.borderSize.Width * 2;
//				foreach (RectangleF rect in menuItemAreas) {
//					maxWidth = (int)Math.Max(maxWidth, rect.Width + base.borderSize.Width * 2);
//				}
//				
//				Size = new Size(maxWidth, base.borderSize.Height * 2 + SystemInformation.MenuHeight * (menu.MenuItems.Count + 1));
//				
//				if (subMenuEditor != null) {
//					subMenuEditor.Location = GetSubmenuLocation(this.selectedItem);
//				}
//			} catch (Exception ex)  {
//				Console.WriteLine(ex);
//				if (Parent != null) {
//					Parent.Controls.Remove(this);
//				}
//			}
//		}
//		
//		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
//		{
//			base.OnPaintBackground(pevent);
//			ControlPaint.DrawBorder3D(pevent.Graphics, new Rectangle(0, 0, Width, Height), Border3DStyle.Raised);
//		}
//		
//		protected override Point GetSubmenuLocation(int menuIndex)
//		{
//			RectangleF r;
//			if (menuItemAreas == null || menuIndex >= menu.MenuItems.Count || menuIndex < 0) {
//				r = base.typeArea;
//			} else {
//				r = base.menuItemAreas[menuIndex];
//			}
//			return new Point((int)(Right - 2),
//			                 (int)(Top  + r.Y));
//		}
//		
//		
//		protected override bool ProcessDialogKey(System.Windows.Forms.Keys keyData)
//		{
//			switch (keyData) {
//				case Keys.Up:
//					if (SelectedItem == 0) {
//						if (!(owner is SubMenuEditorControl)) {
//							owner.Focus();
//						} else {
//							SelectedItem = menu.MenuItems.Count;
//						}
//					} else {
//						SelectItem(ItemSelection.Prev);
//					}
//					Refresh();
//					return true;
//				case Keys.Down:
//					if (SelectedItem >= menu.MenuItems.Count) {
//						if (!(owner is SubMenuEditorControl)) {
//							owner.Focus();
//						} else {
//							SelectedItem = 0;
//						}
//					} else {
//						SelectItem(ItemSelection.Next);
//					}
//					Refresh();
//					return true;
//				case Keys.Escape:
//					owner.CloseSubMenuEditor();
//					owner.Focus();
//					return true;
//				case Keys.Left: {
//					AbstractMenuEditorControl topLevel = GetTopLevelMenuControl();
//					if (topLevel == owner) {
//						topLevel.Focus();
//						topLevel.SelectItem(ItemSelection.Prev);
//					} else {
//						goto case Keys.Escape;
//					}
//					return true;
//				}
//				case Keys.Right:
//					if (base.subMenuEditor != null) {
//						subMenuEditor.Focus();
//						subMenuEditor.SelectItem(ItemSelection.First);
//					} else {
//						AbstractMenuEditorControl topLevel = GetTopLevelMenuControl();
//						topLevel.Focus();
//						topLevel.SelectItem(ItemSelection.Next);
//					}
//					return true;
//			}
//			return base.ProcessDialogKey(keyData);
//		}
//		
//		AbstractMenuEditorControl GetTopLevelMenuControl()
//		{
//			AbstractMenuEditorControl  curControl = owner;
//			while (curControl is SubMenuEditorControl) {
//				curControl = ((SubMenuEditorControl)curControl).owner;
//			}
//			return curControl;
//		}
//	}
//}
