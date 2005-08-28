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
	public class AutoHideMenuStripContainer: AutoHideContainer
	{
		protected bool dropDownOpened;
		
		Padding? defaultPadding;
		
		public override bool AutoHide {
			get {
				return base.AutoHide;
			}
			set {
				if (defaultPadding == null) {
					defaultPadding = ((MenuStrip)control).Padding;
				}
				((MenuStrip)control).Padding = value?Padding.Empty:(Padding)defaultPadding;
				base.AutoHide = value;
			}
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
		
		protected override void OnControlMouseLeave(object sender, EventArgs e)
		{
			mouseIn = false;
			if (!dropDownOpened) {
				HideOverlay();
			}
		}
	}
}
