// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of LineStyle.
	/// </summary>
	public class BaseStyleDecorator : IBaseStyleDecorator
	{

		public BaseStyleDecorator() : this(GlobalValues.DefaultBackColor, Color.Black)
		{
		}

		public BaseStyleDecorator(Color backColor, Color foreColor)
		{
			this.BackColor = backColor;
			this.ForeColor = foreColor;
		}

		
		public bool DrawBorder {get;set;}

		public Color BackColor {get;set;}
		
		public Color FrameColor {get;set;}

		public Color ForeColor {get;set;}
		
		public Point Location {get;set;}
			
		public Size Size {get;set;}
		
		public Rectangle DisplayRectangle{get;set;}
		
		public iTextSharp.text.BaseColor PdfBackColor {
			get {return ConvertToPdfBaseColor(this.BackColor);}
		}


		public iTextSharp.text.BaseColor PdfForeColor {
			get {return ConvertToPdfBaseColor(this.ForeColor);}
		}

		
		public iTextSharp.text.BaseColor PdfFrameColor {
			get {return ConvertToPdfBaseColor(FrameColor);}
		}
		
	
		
		private static iTextSharp.text.BaseColor ConvertToPdfBaseColor (Color color)
		{
			return new iTextSharp.text.BaseColor(color.R, color.G, color.B, color.A);
		}
			
	}
}
