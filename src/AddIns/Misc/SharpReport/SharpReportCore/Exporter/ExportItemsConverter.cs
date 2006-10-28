/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 20.09.2006
 * Time: 09:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace SharpReportCore.Exporters{
	
	public class ExportItemsConverter{
		Graphics graphics;
		int offset;

		public ExportItemsConverter (Graphics graphics) {
			this.graphics = graphics;
		}
		
		public BaseExportColumn ConvertToLineItems (IItemRenderer itemRenderer) {
			if (itemRenderer == null) {
				throw new ArgumentNullException("itemRenderer");
			}
	
			IExportColumnBuilder lineBuilder = itemRenderer as IExportColumnBuilder;
			BaseExportColumn lineItem = null;
			if (lineBuilder != null) {
				lineItem = lineBuilder.CreateExportColumn(this.graphics);
				lineItem.StyleDecorator.Location = new Point(lineItem.StyleDecorator.Location.X,
				                                             lineItem.StyleDecorator.Location.Y + offset);
			} else {
				System.Console.WriteLine("ConvertToLineItems:Can't Convert <{0}> to ILineBuilder",itemRenderer.Name);
			}
			return lineItem;
		}
		
		public ExportContainer ConvertToContainer (IItemRenderer itemRenderer) {
			if (itemRenderer == null) {
				throw new ArgumentNullException("itemRenderer");
			}
			IExportColumnBuilder lineBuilder = itemRenderer as IExportColumnBuilder;
	
			if (lineBuilder != null) {
				ExportContainer lineItem = (ExportContainer)lineBuilder.CreateExportColumn(this.graphics);
				lineItem.StyleDecorator.Location = new Point (lineItem.StyleDecorator.Location.X,
				                                              this.offset);
				return lineItem;
			} else {
				System.Console.WriteLine("ConvertToContainer:Can't Convert <{0}> to ILineBuilder",itemRenderer.Name);
			}
			return null;
		}
		
		public int Offset {
			get {
				return offset;
			}
			set {
				offset = value;
			}
		}
	
	}
}
	
