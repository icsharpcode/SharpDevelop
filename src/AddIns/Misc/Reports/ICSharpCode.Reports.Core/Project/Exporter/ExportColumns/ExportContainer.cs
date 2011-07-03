// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of ContainerItem.
	/// </summary>
	public class ExportContainer : BaseExportColumn, IExportContainer
	{

		ExporterCollection items;

		#region Constructor

		public ExportContainer() : base()
		{
			base.IsContainer = true;
		}

		public ExportContainer(BaseStyleDecorator itemStyle) : base(itemStyle, true)
		{
		}

		#endregion

		#region overrides

		public override void DrawItem(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			base.DrawItem(graphics);
			base.Decorate(graphics);
			
			foreach (ICSharpCode.Reports.Core.Exporter.BaseExportColumn baseExportColumn in items) 
			{
				baseExportColumn.DrawItem(graphics);
			}
		}

		
		
		public override void DrawItem(PdfWriter pdfWriter, ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter)
		{
			base.DrawItem(pdfWriter, converter);
			base.Decorate();
			foreach (ICSharpCode.Reports.Core.Exporter.BaseExportColumn baseExportColumn in this.Items)
			{
				baseExportColumn.DrawItem(pdfWriter,converter);
			}
		}

		#endregion

		
		public ExporterCollection Items {
			get {
				if (this.items == null) {
					items = new ExporterCollection();
				}
				return items;
			}
		}
	}
}
