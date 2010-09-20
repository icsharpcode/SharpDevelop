// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.WinForms;

namespace ICSharpCode.CodeCoverage
{
	public enum CodeCoverageImageListIndex
	{
		Module                    = 0,
		Namespace                 = 2,
		Class                     = 4,
		Method                    = 6,
		MethodWithZeroCoverage    = 7,
		Property                  = 8
	}
	
	public class CodeCoverageImageList
	{
		static ImageList imageList;
		
		CodeCoverageImageList()
		{
		}
		
		public static ImageList ImageList {
			get {
				if (imageList == null) {
					GetImageList();
				}
				return imageList;
			}
		}
		
		static void GetImageList()
		{
			imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			
			AddBitmap(WinFormsResourceService.GetBitmap("Icons.16x16.Library"), 0.2f);
			AddBitmap(WinFormsResourceService.GetBitmap("Icons.16x16.NameSpace"), 0.4f);
			AddBitmap(WinFormsResourceService.GetBitmap("Icons.16x16.Class"), 0.15f);
			AddBitmap(WinFormsResourceService.GetBitmap("Icons.16x16.Method"), 0.2f);
			AddBitmap(WinFormsResourceService.GetBitmap("Icons.16x16.Property"), 0.2f);
		}
		
		static void AddBitmap(Bitmap bitmap, float brightness)
		{
			imageList.Images.Add(bitmap);
			imageList.Images.Add(GrayScaleBitmap.FromBitmap(bitmap, brightness));
		}
	}
}
