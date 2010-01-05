// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of ContainerItem.
	/// </summary>
	public class ExportContainer:BaseExportColumn
	{

		ExporterCollection items;
		
		#region Constructor
		
		public ExportContainer():base(){
			base.IsContainer = true;
		}
		
		public ExportContainer (BaseStyleDecorator itemStyle):base(itemStyle,true){
		}
		
		#endregion
		
		#region overrides
		
		
		public override void DrawItem(System.Drawing.Graphics graphics){
			base.Decorate(graphics);
		}
		
		public override void DrawItem(PdfWriter pdfWriter, ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter)
		{
			base.DrawItem(pdfWriter, converter);
			base.Decorate();
		}
		
		
		#endregion
		
		public void AddLineItem (BaseExportColumn item) {
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			this.items.Add(item);
		}
		
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
