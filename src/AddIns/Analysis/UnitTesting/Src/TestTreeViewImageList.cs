// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.UnitTesting
{
	public enum TestTreeViewImageListIndex
	{
		TestNotRun    = 0,
		TestPassed    = 1,
		TestFailed    = 2,
		TestIgnored   = 3
	}
	
	public class TestTreeViewImageList
	{
		static ImageList imageList;
		
		TestTreeViewImageList()
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
			
			AddBitmap("Grey.png");
			AddBitmap("Green.png");
			AddBitmap("Red.png");
			AddBitmap("Yellow.png");
		}
		
		static void AddBitmap(string name)
		{
			string resourceName = String.Concat("ICSharpCode.UnitTesting.Resources.", name);
			Assembly assembly = typeof(TestTreeViewImageList).Assembly;
			imageList.Images.Add(new Bitmap(assembly.GetManifestResourceStream(resourceName)));
		}
	}
}
