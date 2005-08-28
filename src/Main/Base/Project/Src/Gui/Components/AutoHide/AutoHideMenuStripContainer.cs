// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// AutoHideMenuStripContainer can be used instead of MenuStrip to get a menu
	/// which is automaticaly hiden and shown. It is especially useful in fullscreen.
	/// </summary>
	public class AutoHideMenuStripContainer: Panel
	{
		MenuStrip menuStrip;
		bool autoHide; // Set in the constructor to fire set_AutoHide
		bool showOnMouseMove = true;
		bool showOnMouseDown = true;
		
		bool mouseIn;
		bool dropDownOpened;
		
		/// <summary>
		/// Gets or sets whether to hide the menu. If set to false, it will behave as normal menu.
		/// </summary>
		public bool AutoHide {
			get {
				return autoHide;
			}
			set {
				autoHide = value;
				if (autoHide) {
					this.Height = 1;
					this.Controls.Clear();
				} else {
					this.Height = menuStrip.PreferredSize.Height;
					menuStrip.Dock = DockStyle.Fill;
					this.Controls.Add(menuStrip);
				}
			}
		}
		
		public bool ShowOnMouseMove {
			get {
				return showOnMouseMove;
			}
			set {
				showOnMouseMove = value;
			}
		}
		
		public bool ShowOnMouseDown {
			get {
				return showOnMouseDown;
			}
			set {
				showOnMouseDown = value;
			}
		}
		
		public Color ActivatorColor {
			get {
				return this.ForeColor;
			}
			set {
				this.ForeColor = value;
			}
		}
		
		public AutoHideMenuStripContainer(MenuStrip menuStrip)
		{
			if (menuStrip == null) {
				throw new ArgumentNullException("menuStrip");
			}
			this.menuStrip = menuStrip;
			this.AutoHide = true;
			this.MouseMove += OnPanelMouseMove;
			this.MouseDown += OnPanelMouseDown;
			menuStrip.MouseEnter += OnMenuMouseEnter;
			menuStrip.MouseLeave += OnMenuMouseLeave;
			menuStrip.ItemAdded += OnMenuItemAdded;
			foreach(ToolStripMenuItem menuItem in menuStrip.Items) {
				AddEventHandlersForItem(menuItem);
			}
		}
		
		void OnPanelMouseMove(object sender, MouseEventArgs e)
		{
			if (showOnMouseMove && autoHide) {
				ShowOverlay();
			}
		}
		
		void OnPanelMouseDown(object sender, MouseEventArgs e)
		{
			if (showOnMouseDown && autoHide) {
				ShowOverlay();
			}
		}
		
		void OnMenuMouseEnter(object sender, EventArgs e)
		{
			mouseIn = true;
		}
		
		void OnMenuMouseLeave(object sender, EventArgs e)
		{
			mouseIn = false;
			if (!dropDownOpened) {
				HideOverlay();
			}
		}
		
		void OnMenuItemAdded(object sender, EventArgs e)
		{
			AddEventHandlersForItem((ToolStripMenuItem)sender);
		}
		
		void AddEventHandlersForItem(ToolStripMenuItem menuItem)
		{
			menuItem.DropDownOpened += OnDropDownOpened;
			menuItem.DropDownClosed += OnDropDownClosed;
		}
		
		void OnDropDownOpened(object sender, EventArgs e)
		{
			dropDownOpened = true;
		}
		
		void OnDropDownClosed(object sender, EventArgs e)
		{
			dropDownOpened = false;
			if (!mouseIn) {
				HideOverlay();
			}
		}
		
		public void ShowOverlay()
		{
			menuStrip.Dock = DockStyle.None;
			menuStrip.Location = this.Location;
			menuStrip.AutoSize = false;
			menuStrip.Size = new Size(this.Width, menuStrip.PreferredSize.Height);
			Parent.Controls.Add(menuStrip);
			menuStrip.BringToFront();
		}
		
		public void HideOverlay()
		{
			if (Parent.Controls.Contains(menuStrip)) {
				Parent.Controls.Remove(menuStrip);
			}
		}
	}
}
