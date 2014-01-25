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
using ICSharpCode.Reports.Core.BaseClasses;
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
	
		public ExportContainer(BaseStyleDecorator itemStyle) : base(itemStyle)
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
			items.ForEach(item =>item.DrawItem(graphics));
		}

		
		
		public override void DrawItem(PdfWriter pdfWriter, ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter)
		{
			base.DrawItem(pdfWriter, converter);
			base.Decorate();
			items.ForEach(item =>item.DrawItem(pdfWriter,converter));
			
//			foreach (ICSharpCode.Reports.Core.Exporter.BaseExportColumn baseExportColumn in this.Items)
//			{
//				baseExportColumn.DrawItem(pdfWriter,converter);
//			}
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
