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

namespace Debugger
{
	/// <summary>
	/// Description of DebuggerIcons.
	/// </summary>
	public static class DebuggerIcons
	{
		static ImageList imageList;
		
		public static ImageList ImageList {
			get {
				return imageList;
			}
		}
		
		static DebuggerIcons()
		{
			imageList = new ImageList();
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Class"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Field"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Property"));
		}
		
		public static Image GetImage(Variable variable)
		{
			return imageList.Images[GetImageListIndex(variable)];
		}
		
		public static int GetImageListIndex(Variable variable)
		{
			if (variable.Value is ObjectValue) {
				return 0; // Class
			} else if (variable is PropertyVariable){
				return 2; // Property
			} else {
				return 1; // Field
			}
		}
	}
}
