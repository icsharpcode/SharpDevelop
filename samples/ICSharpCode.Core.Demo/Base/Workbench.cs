// Copyright (c) 2005 Daniel Grunwald
// Licensed under the terms of the "BSD License", see doc/license.txt

using System;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.Core.WinForms;

namespace Base
{
	/// <summary>
	/// The main form of the application.
	/// </summary>
	public sealed class Workbench : Form
	{
		static Workbench instance;
		
		public static Workbench Instance {
			get {
				return instance;
			}
		}
		
		public static void InitializeWorkbench()
		{
			instance = new Workbench();
		}
		
		MenuStrip menu;
		ToolStrip toolbar;
		Panel contentPanel;
		
		private Workbench()
		{
			// restore form location from last session
			FormLocationHelper.Apply(this, "StartupFormPosition", true);
			
			contentPanel = new Panel();
			contentPanel.Dock = DockStyle.Fill;
			this.Controls.Add(contentPanel);
			
			menu = new MenuStrip();
			MenuService.AddItemsToMenu(menu.Items, this, "/Workbench/MainMenu");
			
			toolbar = ToolbarService.CreateToolStrip(this, "/Workbench/Toolbar");
			
			this.Controls.Add(toolbar);
			this.Controls.Add(menu);
			
			// Start with an empty text file
			ShowContent(new TextViewContent());
			
			// Use the Idle event to update the status of menu and toolbar items.
			Application.Idle += OnApplicationIdle;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				Application.Idle -= OnApplicationIdle;
			}
			base.Dispose(disposing);
		}
		
		void OnApplicationIdle(object sender, EventArgs e)
		{
			// Use the Idle event to update the status of menu and toolbar.
			// Depending on your application and the number of menu items with complex conditions,
			// you might want to update the status less frequently.
			UpdateMenuItemStatus();
		}
		
		/// <summary>Update Enabled/Visible state of items in the main menu based on conditions</summary>
		void UpdateMenuItemStatus()
		{
			foreach (ToolStripItem item in menu.Items) {
				if (item is IStatusUpdate)
					(item as IStatusUpdate).UpdateStatus();
			}
		}
		
		/// <summary>The active view content</summary>
		IViewContent viewContent;
		
		public IViewContent ActiveViewContent {
			get {
				return viewContent;
			}
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (!e.Cancel) {
				e.Cancel = !CloseCurrentContent();
			}
		}
		
		public bool CloseCurrentContent()
		{
			IViewContent content = viewContent;
			if (content != null) {
				if (!content.Close()) {
					return false;
				}
				viewContent = null;
				content.TitleChanged -= OnTitleChanged;
				OnTitleChanged(content, EventArgs.Empty);
				foreach (Control ctl in contentPanel.Controls) {
					ctl.Dispose();
				}
				contentPanel.Controls.Clear();
			}
			return true;
		}
		
		public void ShowContent(IViewContent content)
		{
			if (viewContent != null)
				throw new InvalidOperationException("There is still another content opened.");
			viewContent = content;
			Control ctl = content.Control;
			ctl.Dock = DockStyle.Fill;
			contentPanel.Controls.Add(ctl);
			ctl.Focus();
			
			content.TitleChanged += OnTitleChanged;
			OnTitleChanged(content, EventArgs.Empty);
		}
		
		void OnTitleChanged(object sender, EventArgs e)
		{
			if (viewContent != null) {
				this.Text = viewContent.Title + " - ICSharpCode.Core.Demo";
			} else {
				this.Text = "ICSharpCode.Core.Demo";
			}
		}
	}
}
