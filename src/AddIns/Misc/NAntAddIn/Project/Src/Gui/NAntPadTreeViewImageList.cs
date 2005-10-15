// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Windows.Forms;

namespace ICSharpCode.NAntAddIn.Gui
{
	/// <summary>
	/// Represents the images that are used in the 
	/// <see cref="NAntPadTreeView"/>.
	/// </summary>
	public class NAntPadTreeViewImageList
	{
		/// <summary>
		/// The NAnt build file image index.
		/// </summary>
		public static int BuildFileImage = 0;
		
		/// <summary>
		/// The NAnt build target image index.
		/// </summary>
		public static int TargetImage = 1;
		
		/// <summary>
		/// The NAnt default build target image index.
		/// </summary>
		public static int DefaultTargetImage = 2;
		
		/// <summary>
		/// The error icon displayed when the build file has errors.
		/// </summary>
		public static int BuildFileErrorImage = 3;
		
		/// <summary>
		/// The error icon displayed as the first target when the build file has errors.
		/// </summary>
		public static int TargetErrorImage = 4;
		
		NAntPadTreeViewImageList()
		{
		}
		
		/// <summary>
		/// Creates an image list to be used in the 
		/// <see cref="NAntPadTreeView"/>.</summary>
		public static ImageList GetImageList()
		{
			ImageList imageList = new ImageList();
			imageList.Images.Add(IconService.GetBitmap("NAntAddIn.Icons.16x16.BuildFile"));
			imageList.Images.Add(IconService.GetBitmap("NAntAddIn.Icons.16x16.BuildTarget"));
			imageList.Images.Add(IconService.GetBitmap("NAntAddIn.Icons.16x16.DefaultBuildTarget"));
			imageList.Images.Add(IconService.GetBitmap("NAntAddIn.Icons.16x16.BuildFileError"));
			imageList.Images.Add(IconService.GetBitmap("NAntAddIn.Icons.16x16.BuildTargetError"));
			
			return imageList;
		}
	}
}
