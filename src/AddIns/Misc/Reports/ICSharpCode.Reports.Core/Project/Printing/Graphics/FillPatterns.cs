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

namespace ICSharpCode.Reports.Core {	
	/// <summary>
	/// Abstract BaseClass for all AbstractFillPatterns
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 12.10.2005 08:57:05
	/// </remarks>
	
	public abstract class AbstractFillPattern
	{
		
		Color color;
		Brush brush;
		iTextSharp.text.BaseColor pdfColor;
		
		protected AbstractFillPattern(Color color) {
			this.color = color;
		}
		
		
		protected AbstractFillPattern(iTextSharp.text.BaseColor pdfColor)
		{
			this.pdfColor = pdfColor;
		}
		
		
		public abstract Brush CreateBrush(RectangleF rect);

		public Color Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		
		public iTextSharp.text.BaseColor PdfColor {
			get { return pdfColor; }
			set { pdfColor = value; }
		}
		
		public Brush Brush {
			get {
				return brush;
			}
			set {
				brush = value;
			}
		}
		
	}
	
	/// <summary>
	/// Solid Fill Pattern
	/// </summary>
	public class SolidFillPattern : AbstractFillPattern {
		
		public SolidFillPattern (Color color) :base(color){
		}
		
		public SolidFillPattern (iTextSharp.text.BaseColor pdfColor):base(pdfColor)
		{
			
		}
		
		public override Brush CreateBrush(RectangleF rect){
			return new SolidBrush(this.Color);
		}
		
	}
}
