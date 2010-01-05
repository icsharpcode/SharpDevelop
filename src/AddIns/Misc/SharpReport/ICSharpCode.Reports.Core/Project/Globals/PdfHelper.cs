/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 28.07.2007
 * Zeit: 11:02
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using iTextSharp.text;

namespace ICSharpCode.Reports.Core
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
