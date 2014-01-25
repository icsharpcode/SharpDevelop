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

namespace ICSharpCode.Reports.Core.Globals
{
	/// <summary>
	/// Description of PdHelper.
	/// </summary>
	
	public class ScreenRectToPdfRectConverter :System.Drawing.RectangleConverter
	{
		ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter;
		
		
		public ScreenRectToPdfRectConverter(ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter)
		{
			if (converter == null) {
				throw new ArgumentNullException("converter");
			}
			this.converter = converter;
		}
		
		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(System.Drawing.Rectangle)) {
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		
		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(iTextSharp.text.Rectangle)){
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		
		public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			throw new NotImplementedException();
		}
		
		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			
			System.Drawing.Rectangle r = (System.Drawing.Rectangle)value ;
			
			float lx  = (float)UnitConverter.FromPixel(r.Left).Point;
			float ly = (float)this.converter.PageTop - UnitConverter.FromPixel(r.Bottom).Point;
			float rx = (float)lx + UnitConverter.FromPixel(r.Width).Point;
			float ry = (float)ly + UnitConverter.FromPixel(r.Height).Point;
			return  new iTextSharp.text.Rectangle(lx,ly,rx,ry);
		}
		
	}
}
