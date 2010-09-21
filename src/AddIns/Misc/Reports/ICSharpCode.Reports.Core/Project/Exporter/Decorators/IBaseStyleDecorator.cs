// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace ICSharpCode.Reports.Core.Exporter
{
	public interface IBaseStyleDecorator
	{
		bool DrawBorder { get; set;}
		Color BackColor { get; set;}
		Color ForeColor { get; set;}
		Color FrameColor {get;set;}
		Point Location { get; set;}
		Size Size { get; set; }
		Rectangle DisplayRectangle { get;set; }
		
		
		iTextSharp.text.BaseColor PdfBackColor { get; }
		iTextSharp.text.BaseColor PdfFrameColor { get; }
		iTextSharp.text.BaseColor PdfForeColor { get; }
		
	}
}
