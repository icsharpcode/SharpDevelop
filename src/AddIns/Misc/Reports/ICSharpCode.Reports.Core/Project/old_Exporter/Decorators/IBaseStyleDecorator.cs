// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace ICSharpCode.Reports.Core.old_Exporter
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
