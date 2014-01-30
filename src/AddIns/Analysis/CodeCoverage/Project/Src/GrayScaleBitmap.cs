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
using System.Drawing.Imaging;

namespace ICSharpCode.CodeCoverage
{
	public class GrayScaleBitmap
	{		
		const int ColorMatrixSize = 5;

		GrayScaleBitmap()
		{
		}
		
		/// <summary>
		/// Converts a bitmap to a grayscale bitmap.
		/// </summary>
		public static Bitmap FromBitmap(Bitmap bitmap)
		{
			return FromBitmap(bitmap, GetGrayScaleColorMatrix());
		}
		
		/// <summary>
		/// Gets a grayscale bitmap and also changes its brightness.
		/// </summary>
		public static Bitmap FromBitmap(Bitmap bitmap, float brightness)
		{
			ColorMatrix m =  new ColorMatrix();
			m.Matrix00 = 1;
			m.Matrix11 = 1;
			m.Matrix22 = 1;
			m.Matrix33 = 1;
			m.Matrix40 = brightness;
			m.Matrix41 = brightness;
			m.Matrix42 = brightness;
			m.Matrix44 = 1;
			
			return FromBitmap(bitmap, Multiply(m, GetGrayScaleColorMatrix()));
		}
		
		static Bitmap FromBitmap(Bitmap bitmap, ColorMatrix colorMatrix)
		{
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetColorMatrix(colorMatrix);
			Bitmap grayBitmap = new Bitmap(bitmap);
			using (Graphics g = Graphics.FromImage(grayBitmap)) {
				g.DrawImage(grayBitmap, new Rectangle(0, 0, grayBitmap.Width, grayBitmap.Height), 0, 0, grayBitmap.Width, grayBitmap.Height, GraphicsUnit.Pixel, imageAttributes);
				return grayBitmap;
			}
		}
		
		static ColorMatrix GetGrayScaleColorMatrix()
		{
			ColorMatrix m = new ColorMatrix();
		    m.Matrix00 = 0.299f;
		    m.Matrix01 = 0.299f;
		    m.Matrix02 = 0.299f;
		    m.Matrix10 = 0.587f;
		    m.Matrix11 = 0.587f;
		    m.Matrix12 = 0.587f;
		    m.Matrix20 = 0.114f;
		    m.Matrix21 = 0.114f;
		    m.Matrix22 = 0.114f; 
		    m.Matrix33 = 1;
		    m.Matrix44 = 1;
		    return m;
		}
		
		static ColorMatrix Multiply(ColorMatrix m, ColorMatrix n)
		{
			ColorMatrix colorMatrix = new ColorMatrix();
			float[] column = new float[ColorMatrixSize];
			for (int j = 0; j < ColorMatrixSize; ++j) {
				for (int k = 0; k < ColorMatrixSize; ++k) {
					column[k] = m[k, j];
				}
				for (int i = 0; i < ColorMatrixSize; ++i) {
					float s = 0;
					for (int k = 0; k < ColorMatrixSize; ++k) {
						s += n[i, k] * column[k];
					}
					colorMatrix[i, j] = s;
				}
			}
			return colorMatrix;
		}
	}
}
