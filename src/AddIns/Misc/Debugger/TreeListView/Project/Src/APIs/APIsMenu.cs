using System;
using System.Runtime.InteropServices;

namespace System.Runtime.InteropServices.APIs
{
	public class APIsMenu
	{
		[DllImport("user32.dll")]
		public static extern APIsEnums.MenuItemFlags GetMenuState(
			IntPtr hMenu, int itemID, APIsEnums.MenuItemFlags uFlag);
		[DllImport("user32.dll")]
		public static extern bool GetMenuString(
			IntPtr hMenu, int itemID, System.Text.StringBuilder lpString, int maxlen, APIsEnums.MenuItemFlags uFlag);
		[DllImport("user32.dll")]
		public static extern int GetMenuItemInfo(
			IntPtr hMenu, int itemindex, bool fByPosition, ref APIsStructs.MENUITMEINFO infos);
		[DllImport("user32.dll")]
		public static extern int GetMenuItemID(
			IntPtr hMenu, int index);
		[DllImport("user32.dll")]
		public static extern bool InsertMenuItem(
			IntPtr hMenu, int itemindex, bool fByPosition, ref APIsStructs.MENUITMEINFO infos);
		[DllImport("user32.dll")]
		public static extern int GetMenuItemCount(IntPtr hMenu);
		[DllImport("user32.dll")]
		public static extern IntPtr CreatePopupMenu();
		[DllImport("user32.dll")]
		public static extern bool DestroyMenu(IntPtr hMenu);
		[DllImport("user32.dll")]
		public static extern bool DeleteMenu(
			IntPtr hMenu,
			int index,
			APIsEnums.MenuItemFlags uFlag);
		[DllImport("user32.dll", SetLastError=true)]
		public static extern int TrackPopupMenu(
			IntPtr hmenu,
			APIsEnums.TrackPopupMenuFlags fuFlags,
			int x,
			int y,
			int res,
			IntPtr hwnd,
			IntPtr rect);
	}
}
