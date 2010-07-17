// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.Reports.Core.Exporter
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
		private string dataType;
		private string formatString;
		
		
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
		
		public string DataType {
			get { return dataType; }
			set { dataType = value; }
		}
		
		public string FormatString {
			get { return formatString; }
			set { formatString = value; }
		}
		
	}
}
