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
//using System.Drawing.Drawing2D;
//using System.Collections;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.Windows.Forms;
//using System.Windows.Forms.Design;
//using ICSharpCode.FormDesigner.Services;
//
//using System.Drawing.Design;
//
//namespace ICSharpCode.FormDesigner {
//	
//	public enum ItemSelection {
//		Last,
//		First,
//		Next,
//		Prev
//	}
//	
//	public abstract class AbstractMenuEditorControl : Panel
//	{
//		protected Menu         menu;
//		protected SizeF[]      menuItemSize;
//		protected RectangleF[] menuItemAreas;
//		
//		protected RectangleF   typeArea;
//		protected SizeF        typeSize;
//		protected string       typeAreaText = "Type here";
//		
//		protected IDesignerHost host;
//		protected ItemEditor    itemEditor = null;
//		
//		protected Control       rootControl;
//		protected SubMenuEditorControl subMenuEditor = null;
//		
//		protected int           oldSelectedItem = -1;
//		protected int           selectedItem    = -1;
//		protected int           mouseOverItem   = -1;
//		protected bool          isMouseOverLeftSpacing = false;
//		
//		protected bool          isHorizontal = true;
//		protected bool          canEditMenuItems = true;
//		protected bool          drawMenuGlyphs   = false;
//		protected Size borderSize = new Size(0, 0);
//		
//		public static bool MenuEditorFocused = false;
//		public bool iveTheFocus;
//		
//		public int SelectedItem {
//			get {
//				return this.selectedItem;
//			}
//			set {
//				this.oldSelectedItem = selectedItem;
//				if (selectedItem != value) {
//					selectedItem = value;
//					Refresh();
//				}
//				ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
//				if (menu != null && selectedItem >= 0 && selectedItem < menu.MenuItems.Count) {
//					selectionService.SetSelectedComponents(new object[] { menu.MenuItems[selectedItem] });
//				} else {
//					selectionService.SetSelectedComponents(null);
//				}
//			}
//		}
//		
//		protected override void OnGotFocus(EventArgs e)
//		{
//			base.OnGotFocus(e);
//			MenuEditorFocused = true;
//			iveTheFocus = true;
//			Refresh();
//		}
//		
//		protected override void OnLostFocus(EventArgs e)
//		{
//			base.OnLostFocus(e);
//			iveTheFocus       = false;
////			SelectedItem      = -1;
//			Refresh();
//		}
//		
//		protected override Size DefaultSize {
//			get {
//				return new Size(320, System.Windows.Forms.SystemInformation.MenuHeight + 2);
//			}
//		}
//		
//		protected AbstractMenuEditorControl(IDesignerHost host, Menu menu)
//		{
//			this.host = host;
//			this.menu = menu;
//			rootControl = (Control)host.RootComponent;
//			rootControl.Resize += new EventHandler(SetSize);
//			SetSize(null, null);
//			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
//		}
//		
//		public virtual void SetSize(object sender, EventArgs e)
//		{
//			int boundsLeftRightOffset = 0;
//			int boundsTopOffset = 0;
//			
//			if (rootControl is Form) {
//				Form f = (Form)rootControl;
//				switch (f.FormBorderStyle) {
//					case FormBorderStyle.Fixed3D:
//						boundsLeftRightOffset = SystemInformation.Border3DSize.Width + SystemInformation.FixedFrameBorderSize.Width;
//						break;
//					
//					case FormBorderStyle.FixedDialog:
//					case FormBorderStyle.FixedSingle:
//					case FormBorderStyle.FixedToolWindow:
//						boundsLeftRightOffset = SystemInformation.FixedFrameBorderSize.Width;
//						break;
//					
//					case FormBorderStyle.Sizable:
//					case FormBorderStyle.SizableToolWindow:
//						boundsLeftRightOffset = SystemInformation.FrameBorderSize.Width;
//						break;
//				}
//				
//				switch (f.FormBorderStyle) {
//					case FormBorderStyle.Fixed3D:
//					case FormBorderStyle.FixedDialog:
//					case FormBorderStyle.FixedSingle:
//						boundsTopOffset       = SystemInformation.FixedFrameBorderSize.Height + SystemInformation.CaptionHeight;
//						break;
//					
//					case FormBorderStyle.Sizable:
//						boundsTopOffset       = SystemInformation.FrameBorderSize.Height      + SystemInformation.CaptionHeight;
//						break;
//					
//					case FormBorderStyle.SizableToolWindow:
//						boundsTopOffset       = SystemInformation.FrameBorderSize.Height      + SystemInformation.ToolWindowCaptionHeight;
//						break;
//					case FormBorderStyle.FixedToolWindow:
//						boundsTopOffset       = SystemInformation.FixedFrameBorderSize.Height + SystemInformation.ToolWindowCaptionHeight;
//						break;
//				}
//			}
//			
//			SetBounds(rootControl.Bounds.Left  + boundsLeftRightOffset, 
//			          rootControl.Bounds.Top   + boundsTopOffset, 
//			          rootControl.Bounds.Width - boundsLeftRightOffset * 2,  
//			          System.Windows.Forms.SystemInformation.MenuHeight + 2);
//			Show();
//		}
//		
//		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
//		{
//			Graphics g = pevent.Graphics;
//			g.FillRectangle(new SolidBrush(SystemColors.Menu), 
//			                pevent.ClipRectangle.X, 
//			                pevent.ClipRectangle.Y, 
//			                pevent.ClipRectangle.Width, 
//			                pevent.ClipRectangle.Height);
//		}
//		
//		int GetLeftSpacing()
//		{
//			return drawMenuGlyphs ? SystemInformation.MenuHeight : 0;
//		}
//		int GetRightSpacing()
//		{
//			return drawMenuGlyphs ? SystemInformation.MenuHeight : 0;
//		}
//		int GetLeftRightSpacing()
//		{
//			return GetLeftSpacing() + GetRightSpacing();
//		}
//		
//		protected virtual void MeasureMenuItems()
//		{
//			Graphics g = null;
//			try {
//				g = base.CreateGraphics();
//			} catch (Exception e) {
//				Console.WriteLine("Got exception : " + e.ToString());
//				return;
//			}
//
//			Font f = System.Windows.Forms.SystemInformation.MenuFont;
//			
//			menuItemAreas = new RectangleF[menu.MenuItems.Count];
//			
//			float curX = 0;
//			float curY = 0;
//			float maxWidth = 0;
//			menuItemSize = new SizeF[menu.MenuItems.Count];
//			for (int i = 0; i < menu.MenuItems.Count; ++i) {
//				menuItemSize[i] = g.MeasureString(menu.MenuItems[i].Text + "min", f);
//				menuItemSize[i].Height += 2;
//				maxWidth = Math.Max(maxWidth, menuItemSize[i].Width + GetLeftRightSpacing());
//			}
//			
//			typeSize = g.MeasureString(typeAreaText, f);
//			typeSize.Height += 2;
//			maxWidth = Math.Max(maxWidth, typeSize.Width  + GetLeftRightSpacing());
//			
//			for (int i = 0; i < menu.MenuItems.Count; ++i) {
//				menuItemAreas[i] = new RectangleF(curX + this.borderSize.Width, 
//				                                  curY + this.borderSize.Height, 
//				                                  isHorizontal ? menuItemSize[i].Width : maxWidth,
//				                                  SystemInformation.MenuHeight);
//				
//				if (this.isHorizontal) {
//					curX += menuItemAreas[i].Width + 4;
//				} else {
//					curY += menuItemAreas[i].Height;
//				}
//			}
//			
//			typeArea = new RectangleF(curX + this.borderSize.Width, 
//			                          curY + this.borderSize.Height, 
//			                          isHorizontal ? typeSize.Width : maxWidth, 
//			                          SystemInformation.MenuHeight);
//			
//			g.Dispose();
//		}
//		
//		protected void DrawTypeHere(Graphics g, bool isSelected, bool isMouseOver)
//		{
//			int yOffset = (int)((typeArea.Height - typeSize.Height) / 2);
//			
//			if (isSelected && iveTheFocus) {
//				DrawSelcectionRectangle(g, typeArea, isMouseOver, false, true);
//			} else {
//				g.FillRectangle(new SolidBrush(Color.White),
//				                (int)typeArea.X + 2,
//				                (int)typeArea.Y + yOffset,
//				                (int)typeArea.Width - 4,
//				                (int)typeSize.Height);
//				
//				g.DrawRectangle(new Pen(Color.Gray), 
//				                (int)typeArea.X + 2,
//				                (int)typeArea.Y + yOffset,
//				                (int)typeArea.Width - 4,
//				                (int)typeSize.Height);
//			}
//			
//			
//			g.DrawString(typeAreaText,
//			             SystemInformation.MenuFont,
//			             SystemBrushes.InactiveBorder,
//			             (int)typeArea.X + this.GetLeftSpacing(),
//			             (int)typeArea.Y + yOffset);
//		}
//		
//		protected void DrawSelcectionRectangle(Graphics g, RectangleF drawArea, bool isMouseOver, bool mouseOverLeft, bool alwaysSelectAll)
//		{
//			g.FillRectangle(SystemBrushes.Highlight,
//			                (int)drawArea.X + 2,
//			                (int)drawArea.Y + 2,
//			                (int)drawArea.Width - 3,
//			                (int)drawArea.Height - 3);
//			
//			if (isMouseOver) {
//				if (mouseOverLeft) {
//					g.DrawRectangle(new Pen(Color.White),
//					                (int)drawArea.X + 2,
//					                (int)drawArea.Y + 2,
//					                (int)this.GetLeftSpacing() - 3,
//					                (int)drawArea.Height - 3);
//				} else {
//					if (this.drawMenuGlyphs && !alwaysSelectAll) {
//						g.DrawRectangle(new Pen(Color.White),
//							                (int)drawArea.X + this.GetLeftSpacing(),
//							                (int)drawArea.Y + 2,
//							                (int)drawArea.Width - 1 -  this.GetLeftSpacing() - 1,
//							                (int)drawArea.Height - 3);
//					} else {
//						g.DrawRectangle(new Pen(Color.White),
//							                (int)drawArea.X + 2,
//							                (int)drawArea.Y + 2,
//							                (int)drawArea.Width - 3,
//							                (int)drawArea.Height - 3);
//					}
//				}
//			} 
//			
//			g.DrawRectangle(new Pen(new HatchBrush(HatchStyle.Percent50, SystemColors.Highlight, SystemColors.Menu)),
//			                (int)drawArea.X,
//			                (int)drawArea.Y,
//			                (int)drawArea.Width,
//			                (int)drawArea.Height);
//		}
//		
//		protected void DrawItem(Graphics g, int i, bool isSelected, bool isMouseOver)
//		{
//			Brush drawBrush = SystemBrushes.ControlText;
//			if (isSelected && iveTheFocus) {
//				DrawSelcectionRectangle(g, menuItemAreas[i], isMouseOver, isMouseOverLeftSpacing, menu.MenuItems[i].MenuItems.Count > 0);
//				drawBrush = SystemBrushes.HighlightText;
//			}
//			
//			int yOffset = (int)((menuItemAreas[i].Height - menuItemSize[i].Height) / 2);
//			g.DrawString(menu.MenuItems[i].Text,
//			             SystemInformation.MenuFont,
//			             drawBrush,
//			             (int)menuItemAreas[i].X + this.GetLeftSpacing(),
//			             (int)menuItemAreas[i].Y + yOffset + 1);
//			
//			if (drawMenuGlyphs) {
//				int r = SystemInformation.MenuHeight;
//				if (menu.MenuItems[i].Checked) {
//					ControlPaint.DrawMenuGlyph(g, (int)menuItemAreas[i].X,
//					                              (int)menuItemAreas[i].Y, 
//					                              r, r, MenuGlyph.Checkmark);
//				}
//				if (menu.MenuItems[i].MenuItems.Count > 0) {
//					ControlPaint.DrawMenuGlyph(g, (int)(menuItemAreas[i].Right - r), 
//					                              (int)(menuItemAreas[i].Y), 
//					                              r, r, MenuGlyph.Arrow);
//				}
//			}
//		}
//		
//		protected override void OnPaint(PaintEventArgs pe)
//		{
//			Graphics g = pe.Graphics;
//			Font     f = System.Windows.Forms.SystemInformation.MenuFont;
//			
//			MeasureMenuItems();
//			
//			for (int i = 0; i < menu.MenuItems.Count; ++i) {
//				DrawItem(g, i, this.selectedItem == i, i == mouseOverItem);
//			}
//			
//			if (MenuEditorFocused) {
//				DrawTypeHere(g, this.selectedItem == menu.MenuItems.Count, mouseOverItem == menu.MenuItems.Count);
//				ShowSubMenuEditor();
//			}
//		}
//		
//		public void CloseSubMenuEditor()
//		{
//			if (this.subMenuEditor != null) {
//				try {
//					subMenuEditor.Parent.Controls.Remove(subMenuEditor);
//				} catch (Exception e) {
//					Console.WriteLine(e);
//				}
//				subMenuEditor.Parent = null;
//				try {
//					this.subMenuEditor.Dispose();
//					this.subMenuEditor = null;
//				} catch (Exception e) {
//					Console.WriteLine(e);
//				}
//			}
//		}
//		
//		protected override void Dispose(bool disposing)
//		{
//			base.Dispose(disposing);
//			CloseSubMenuEditor();
//		}
//		
//		protected virtual void ShowSubMenuEditor()
//		{
//			if (this.selectedItem >= 0 && this.selectedItem < menu.MenuItems.Count) {
//				if (menu.MenuItems[this.selectedItem].Checked) {
//					CloseSubMenuEditor();
//				} else {
//					if (subMenuEditor == null || subMenuEditor.menu != menu.MenuItems[this.selectedItem]) {
//						SubMenuEditorControl panel = new SubMenuEditorControl(host, this, menu.MenuItems[this.selectedItem]);
//						panel.Location = GetSubmenuLocation(this.selectedItem);
//						rootControl.Parent.Controls.Add(panel);
//						panel.BringToFront();
//						if (this.subMenuEditor != null) {
//							CloseSubMenuEditor();
//						}
//						this.subMenuEditor = panel;
//					}
//				}
//			} else if (this.selectedItem == menu.MenuItems.Count) {
//				CloseSubMenuEditor();
//			}
//		}
//		
//		protected virtual Point GetSubmenuLocation(int menuIndex)
//		{
//			// implementation for a top leven menu
//			RectangleF r;
//			if (menuItemAreas == null || menuIndex >= menu.MenuItems.Count || menuIndex < 0) {
//				r = typeArea;
//			} else {
//				r = menuItemAreas[menuIndex];
//			}
//			
//			return new Point((int)(Left + r.X),
//			                 (int)(Top  + SystemInformation.MenuHeight));
//		}
//		
//		int GetItemIndex(int x, int y)
//		{
//			if (menuItemAreas != null) {
//				for (int i = 0; i < menuItemAreas.Length; ++i) {
//					if (menuItemAreas[i].Contains(x, y)) {
//						return i;
//					}
//				}
//			}
//			return -1;
//		}
//		
//		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
//		{
//			int itemIndex = GetItemIndex(e.X, e.Y);
//			AbstractMenuEditorControl.MenuEditorFocused = true;
//			
//			Select();
//			if (itemIndex == -1 && typeArea.Contains(e.X, e.Y)) {
//				SelectedItem = menu.MenuItems.Count;
//			} else {
//				SelectedItem = itemIndex;
//			}
//		}
//		
//		protected override void OnMouseLeave(System.EventArgs e)
//		{
//			base.OnMouseLeave(e);
//			if (-1 != mouseOverItem) {
//				mouseOverItem = -1;
//				Refresh();
//			}
//		}
//		
//		
//		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
//		{
//			int itemIndex = GetItemIndex(e.X, e.Y);
//			
//			int newMouseOver;
//			bool newIsMouseOverLeftSpacing = e.X < GetLeftSpacing();
//			 
//			if (itemIndex == -1 && typeArea.Contains(e.X, e.Y)) {
//				newMouseOver = menu.MenuItems.Count;
//				newIsMouseOverLeftSpacing = false;
//			} else {
//				newMouseOver = itemIndex;
//				if (itemIndex >= 0 && itemIndex < menu.MenuItems.Count) {
//					newIsMouseOverLeftSpacing &= menu.MenuItems[itemIndex].MenuItems.Count == 0;
//				}
//			}
//			
//			if (newMouseOver != mouseOverItem || newIsMouseOverLeftSpacing != isMouseOverLeftSpacing) {
//				mouseOverItem = newMouseOver;
//				isMouseOverLeftSpacing = newIsMouseOverLeftSpacing;
//				Refresh();
//			}
//		}
//		
//		
//		protected override void OnMouseUp(MouseEventArgs e)
//		{
//			base.OnMouseUp(e);
//			if (oldSelectedItem == selectedItem) {
//				int itemIndex = GetItemIndex(e.X, e.Y);
//				
//				if (itemIndex == -1 && typeArea.Contains(e.X, e.Y)) {
////					if (menu.MenuItems.Count == selectedItem) {
////						EditMode(menu.MenuItems.Count);
////					}
//				} else {
//					if (itemIndex == selectedItem && itemIndex != -1) {
//						if (isMouseOverLeftSpacing) {
//							menu.MenuItems[itemIndex].Checked = !menu.MenuItems[itemIndex].Checked;
//							Refresh();
//						} else {
//							EditMode(itemIndex);
//						}
//					}
//				} 
//			}
//		}
//		
//		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
//		{
//			base.OnKeyPress(e);
//			EditMode(selectedItem);
//			if (itemEditor != null) {
//				itemEditor.Text = String.Empty;
//				itemEditor.AppendText(e.KeyChar.ToString());
//			}
//		}
//		
//		protected override bool ProcessDialogKey(System.Windows.Forms.Keys keyData)
//		{
//			switch (keyData) {
//				case Keys.Home:
//					SelectItem(ItemSelection.First);
//					return true;
//				case Keys.End:
//					SelectItem(ItemSelection.Last);
//					return true;
//				case Keys.Return:
//					EditMode(this.SelectedItem);
//					return true;
//				case Keys.Left:
//					SelectItem(ItemSelection.Prev);
//					return true;
//				case Keys.Right:
//					SelectItem(ItemSelection.Next);
//					return true;
//				case Keys.Up:
//					if (subMenuEditor != null) {
//						subMenuEditor.Focus();
//						subMenuEditor.SelectItem(ItemSelection.Last);
//					}
//					return true;
//				case Keys.Down:
//					if (subMenuEditor != null) {
//						subMenuEditor.Focus();
//						subMenuEditor.SelectItem(ItemSelection.First);
//					}
//					return true;
//				case Keys.Escape: {
//					SelectedItem = -1;
//					AbstractMenuEditorControl.MenuEditorFocused = false;
//					Refresh();
//					ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
//					selectionService.SetSelectedComponents(new object[] { host.RootComponent });
//					return true;
//				}
//				case Keys.Back:
//				case Keys.Delete: {
//					if (itemEditor != null && !itemEditor.Focused) {
//						DeleteCommand();
//						return true;
//					}
//					break;
//				}
//				
//			}
//			return base.ProcessDialogKey(keyData);
//		}
//		public void DeleteCommand()
//		{
//			if (selectedItem >= 0 && selectedItem < menu.MenuItems.Count) { //  && subMenuEditor == null
//				ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
//				if (selectedItem == 0) {
//					selectionService.SetSelectedComponents(new object[] { });
//				} else {
//					selectionService.SetSelectedComponents(new object[] { menu.MenuItems[selectedItem - 1] });
//				}
//				ComponentChangeService componentChangeService = (ComponentChangeService)host.GetService(typeof(IComponentChangeService));
//				
//				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(menu);
//				System.ComponentModel.PropertyDescriptor myProperty = properties.Find("MenuItems", false);
//				
//				object[] oldArray = new object[menu.MenuItems.Count];
//				menu.MenuItems.CopyTo(oldArray, 0);
//				MenuItem item = menu.MenuItems[selectedItem];
//				
//				DesignerTransaction transaction = host.CreateTransaction("remove menu item");
//				componentChangeService.OnComponentChanging(menu, myProperty);
//				menu.MenuItems.Remove(item);
//				componentChangeService.OnComponentChanged(menu, myProperty, oldArray, menu.MenuItems);
//				
//				host.DestroyComponent(item);
//				transaction.Commit();
//				SetSize(null, null);
//				Refresh();
//			}
//		}
//		
//		public void SelectItem(ItemSelection selection)
//		{
//			switch (selection) {
//				case ItemSelection.Last:
//					SelectedItem =  menu.MenuItems.Count;
//					break;
//				case ItemSelection.First:
//					SelectedItem = 0;
//					break;
//				case ItemSelection.Next:
//					this.SelectedItem  = (SelectedItem == menu.MenuItems.Count ? 0 : selectedItem + 1);
//					break;
//				case ItemSelection.Prev:
//					this.SelectedItem  = (SelectedItem == 0 ? menu.MenuItems.Count : selectedItem - 1);
//					break;
//			}
//		}
//		
//		void EditMode(int itemNumber)
//		{
//			if (itemNumber < 0 || !canEditMenuItems) {
//				return;
//			}
//			try {
//				itemEditor = new ItemEditor(this);
//				RectangleF r = itemNumber >= menu.MenuItems.Count ? this.typeArea : menuItemAreas[itemNumber];
//				SizeF      s = itemNumber >= menu.MenuItems.Count ? this.typeSize : menuItemSize[itemNumber];
//				int yOffset = (int)((r.Height - s.Height) / 2);
//				itemEditor.SetBounds((int)r.X + 2 + this.GetLeftSpacing(),
//				                     (int)r.Y + yOffset, 
//				                     (int)r.Width - 4, 
//				                     (int)r.Height);
//				
//				if (itemNumber >= menu.MenuItems.Count) {
//					DesignerTransaction transaction = host.CreateTransaction("create menu item");
//					ComponentChangeService componentChangeService = (ComponentChangeService)host.GetService(typeof(IComponentChangeService));
//					MenuItem newItem = (MenuItem)host.CreateComponent(typeof(MenuItem));
//					componentChangeService.OnComponentAdding(newItem);
//					
//					newItem.Text = newItem.Site.Name;
//					menu.MenuItems.Add(newItem);
//					componentChangeService.OnComponentAdded(newItem);
//					transaction.Commit();
//					MeasureMenuItems();
//				}
//				itemEditor.SetItem(menu.MenuItems[itemNumber]);
//				base.Controls.Add(itemEditor);
//				MenuEditorFocused = true;
//				itemEditor.Select();
//				MenuEditorFocused = true;
//				Refresh();
//			} catch (Exception e) {
//				Console.WriteLine(e);
//				if (itemEditor != null) {
//					base.Controls.Remove(itemEditor);
//					itemEditor.Dispose();
//					itemEditor = null;
//				}
//			}
//		}
//	}
//}
