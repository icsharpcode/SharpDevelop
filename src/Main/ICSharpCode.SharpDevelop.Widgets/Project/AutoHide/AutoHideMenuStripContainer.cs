// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.AutoHide
{
	/// <summary>
	/// AutoHideMenuStripContainer can be used instead of MenuStrip to get a menu
	/// which is automaticaly hiden and shown. It is especially useful in fullscreen.
	/// </summary>
	public class AutoHideMenuStripContainer: AutoHideContainer
	{
		protected bool dropDownOpened;
		
		Padding? defaultPadding;
		
		protected override void Reformat()
		{
			if (defaultPadding == null) {
				defaultPadding = ((MenuStrip)control).Padding;
			}
			((MenuStrip)control).Padding = AutoHide ? Padding.Empty : (Padding)defaultPadding;
			base.Reformat();
		}
		
		public AutoHideMenuStripContainer(MenuStrip menuStrip):base(menuStrip)
		{
			menuStrip.AutoSize = false;
			menuStrip.ItemAdded += OnMenuItemAdded;
			foreach(ToolStripMenuItem menuItem in menuStrip.Items) {
				AddEventHandlersForItem(menuItem);
			}
		}
		
		void OnMenuItemAdded(object sender, EventArgs e)
		{
			AddEventHandlersForItem((ToolStripMenuItem)sender);
		}
		
		void AddEventHandlersForItem(ToolStripMenuItem menuItem)
		{
			menuItem.DropDownOpened += delegate { dropDownOpened = true; };
			menuItem.DropDownClosed += delegate { dropDownOpened = false; if (!mouseIn) ShowOverlay = false; };
		}
		
		protected override void OnControlMouseLeave(object sender, EventArgs e)
		{
			mouseIn = false;
			if (!dropDownOpened) ShowOverlay = false;
		}
	}
}
