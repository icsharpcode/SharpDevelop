// *****************************************************************************
// 
//  Copyright 2004, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  WinFormsUI Library Version 1.0
// *****************************************************************************

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	internal class DrawHelper
	{
		public static GraphicsPath CalculateGraphicsPathFromBitmap(Bitmap bitmap)
		{
			return CalculateGraphicsPathFromBitmap(bitmap, Color.Empty);
		}

		// From http://edu.cnzz.cn/show_3281.html
		public static GraphicsPath CalculateGraphicsPathFromBitmap(Bitmap bitmap, Color colorTransparent) 
		{ 
			GraphicsPath graphicsPath = new GraphicsPath(); 
			if (colorTransparent == Color.Empty)
				colorTransparent = bitmap.GetPixel(0, 0); 

			for(int row = 0; row < bitmap.Height; row ++) 
			{ 
				int colOpaquePixel = 0;
				for(int col = 0; col < bitmap.Width; col ++) 
				{ 
					if(bitmap.GetPixel(col, row) != colorTransparent) 
					{ 
						colOpaquePixel = col; 
						int colNext = col; 
						for(colNext = colOpaquePixel; colNext < bitmap.Width; colNext ++) 
							if(bitmap.GetPixel(colNext, row) == colorTransparent) 
								break;
 
						graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1)); 
						col = colNext; 
					} 
				} 
			} 
			return graphicsPath; 
		} 
	}
}
