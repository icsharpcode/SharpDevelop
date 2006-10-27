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
		
		public ExportText ConvertToLineItems (IItemRenderer r) {
			IExportColumnBuilder lineBuilder = r as IExportColumnBuilder;
			ExportText lineItem = null;
			
			if (lineBuilder != null) {

				lineItem = (ExportText)lineBuilder.CreateExportColumn(this.graphics);
				lineItem.StyleDecorator.Location = new Point(lineItem.StyleDecorator.Location.X,
				                                  lineItem.StyleDecorator.Location.Y + offset);
			} else {
				System.Console.WriteLine("Can't Convert <{0}> to ILineBuilder",r.Name);
			}
			return lineItem;
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
	
