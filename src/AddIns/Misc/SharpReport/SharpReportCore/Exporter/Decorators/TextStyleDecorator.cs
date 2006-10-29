/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier Peter
 * Datum: 29.10.2006
 * Zeit: 14:27
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;

namespace SharpReportCore.Exporters
{
	/// <summary>
	/// Description of TextStyleDecorator.
	/// </summary>
	public class TextStyleDecorator:BaseStyleDecorator
	{
		private Font font;
		
		private StringFormat stringFormat;
		private StringTrimming stringTrimming;
		private ContentAlignment contentAlignment;
		
		public TextStyleDecorator():base()
		{
		}
		
		
		
		public Font Font {
			get {
				return font;
			}
			set {
				font = value;
			}
		}
		
		
		public StringFormat StringFormat {
			get {
				return stringFormat;
			}
			set {
				stringFormat = value;
			}
		}
		
		public StringTrimming StringTrimming {
			get {
				return stringTrimming;
			}
			set {
				stringTrimming = value;
			}
		}
		
		public ContentAlignment ContentAlignment {
			get {
				return contentAlignment;
			}
			set {
				contentAlignment = value;
			}
		}
	}
}
