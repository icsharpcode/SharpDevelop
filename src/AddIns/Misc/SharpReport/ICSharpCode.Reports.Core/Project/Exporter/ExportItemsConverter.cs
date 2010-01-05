// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	
	public class ExportItemsConverter:IExportItemsConverter
	{
		private int offset;
		private Point parentLocation;

		public ExportItemsConverter ()
		{
		}
		
		public BaseExportColumn ConvertToLineItem (BaseReportItem item)
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}

			IExportColumnBuilder columnBuilder = item as IExportColumnBuilder;
			BaseExportColumn lineItem = null;
			
			if (columnBuilder != null) {
				lineItem = columnBuilder.CreateExportColumn();

				lineItem.StyleDecorator.Location = new Point(this.parentLocation.X + lineItem.StyleDecorator.Location.X,
				                                             lineItem.StyleDecorator.Location.Y + offset);
				
			} 
			return lineItem;
		}
		
		
		public ExportContainer ConvertToContainer (IContainerItem item) 
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			IExportColumnBuilder lineBuilder = item as IExportColumnBuilder;
	
			if (lineBuilder != null) {
				ExportContainer lineItem = (ExportContainer)lineBuilder.CreateExportColumn();
				lineItem.StyleDecorator.Location = new Point (lineItem.StyleDecorator.Location.X,
				                                              this.offset);
				return lineItem;
			}
			return null;
		}
		
		
		public Point ParentLocation {
			get { return parentLocation; }
			set { parentLocation = value; }
		}
		
	
		public int Offset {
			set { offset = value; }
		}
	}
}
