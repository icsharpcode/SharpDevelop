// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
