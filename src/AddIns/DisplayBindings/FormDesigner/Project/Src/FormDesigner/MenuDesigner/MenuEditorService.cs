//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
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
//	public class MenuEditorService : IMenuEditorService
//	{
//		IDesignerHost             host;
//		bool                      isActive = false;
//		
//		AbstractMenuEditorControl menuEditorControl = null;
//		Menu                      menu;
//		
//		public IDesignerHost Host {
//			get {
//				return host;
//			}
//			set {
//				host = value;
//			}
//		}
//		
//		public AbstractMenuEditorControl MenuEditorControl {
//			get {
//				return menuEditorControl;
//			}
//		}
//		
//		
//		public MenuEditorService(IDesignerHost host)
//		{ 
//			this.host = host;
//			Initialize();
//		}
//		
//		void Initialize()
//		{
//			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
//			selectionService.SelectionChanged += new EventHandler(HandleSelectionChange);
//		}
//		
//		void HandleSelectionChange(object sender, EventArgs e)
//		{
//			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
//			Menu selectedMenu = selectionService.PrimarySelection as Menu;
//			
//			if (selectedMenu != null) {
//				if (menu != selectedMenu && (selectedMenu is MainMenu || selectedMenu is ContextMenu)) {
//					DisposeMenuEditorControl();
//					this.menu = selectedMenu;
//					InitMenuEditorControl();
//				} 
//			} else {
//				if (menuEditorControl != null && selectionService.PrimarySelection != null) {
//					AbstractMenuEditorControl.MenuEditorFocused = false;
//					menuEditorControl.CloseSubMenuEditor();
//					menuEditorControl.Refresh();
//				}
//			}
//		}
//		
//		public void Dispose()
//		{
//		}
//		
//		void DisposeMenuEditorControl()
//		{
//			if (menuEditorControl != null) {
//				menuEditorControl.Dispose();
//				menuEditorControl = null;
//			}
//		}
//
//		void InitMenuEditorControl()
//		{
//			Control rootComponent = host.RootComponent as Control;
//			if (rootComponent != null) {
//				menuEditorControl = (menu is MainMenu) ? (AbstractMenuEditorControl)new MainMenuEditorControl(host, menu) : 
//				                                         (AbstractMenuEditorControl)new ContextMenuEditorControl(host, menu);
//				rootComponent.Parent.Controls.Add(menuEditorControl);
//				menuEditorControl.BringToFront();
//			}
//		}
//
//		
//#region System.Windows.Forms.Design.IMenuEditorService interface implementation
//		public bool IsActive()
//		{
//			return isActive;
//		}
//		
//		public bool MessageFilter(ref System.Windows.Forms.Message m)
//		{
//			return false;
//		}
//		
//		public void SetSelection(System.Windows.Forms.MenuItem item)
//		{
//			Console.WriteLine("!!!TODO!!! SET SELECTION TO : " + item);
//		}
//		
//		public void SetMenu(System.Windows.Forms.Menu menu)
//		{
//			this.menu = menu;
//			isActive  = menu != null;
//			if (isActive) {
//				InitMenuEditorControl();
//			} else {
//				DisposeMenuEditorControl();
//			}
//		}
//		
//		public System.Windows.Forms.Menu GetMenu()
//		{
//			return menu;
//		}
//#endregion
//	}
//}
