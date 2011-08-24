// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;
using ICSharpCode.Reports.Core.Globals;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter
{
	public interface IBaseExportColumn
	{
		void DrawItem(Graphics graphics);
		void DrawItem(PdfWriter pdfWriter,PdfUnitConverter converter);
		IBaseStyleDecorator StyleDecorator { get; set; }
	}
}
