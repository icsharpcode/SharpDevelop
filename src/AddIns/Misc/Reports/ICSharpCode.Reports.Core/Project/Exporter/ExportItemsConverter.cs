// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/*
	public class ExportItemsConverter:IExportItemsConverter
	{
	
		public ExportItemsConverter ()
		{
		}
		
		
		public static ExportContainer ConvertToContainer (BaseReportItem parent,ISimpleContainer item,Point offset) 
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			IExportColumnBuilder lineBuilder = item as IExportColumnBuilder;
	
			if (lineBuilder != null) {
				ExportContainer lineItem = (ExportContainer)lineBuilder.CreateExportColumn();
				
				lineItem.StyleDecorator.Location = new Point (offset.X + lineItem.StyleDecorator.Location.X,
				                                              offset.Y);
				
				lineItem.StyleDecorator.DisplayRectangle = new Rectangle(lineItem.StyleDecorator.Location,
				                                                         lineItem.StyleDecorator.Size);
				
				return lineItem;
			}
			return null;
		}
		
		//public Rectangle ParentRectangle {get;set;}
	}
	*/
}
