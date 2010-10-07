// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of TableConverter.
	/// </summary>
	public class GroupedTableConverter:BaseConverter
	{

		private ITableContainer table;
		
		public GroupedTableConverter(IDataNavigator dataNavigator,
		                      ExporterPage singlePage, ILayouter layouter ):base(dataNavigator,singlePage,layouter)
			
		{
		}
		
		
		public override ExporterCollection Convert (BaseReportItem parent,BaseReportItem item)
		{
			if (parent == null) {
				throw new ArgumentNullException("parent");
			}
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			
			ExporterCollection mylist = base.Convert(parent,item);
			this.table = (BaseTableItem)item ;
			this.table.Parent = parent;
			this.table.DataNavigator = base.DataNavigator;
			return ConvertInternal(mylist);
		}
		
		
		
		private ExporterCollection ConvertInternal(ExporterCollection exporterCollection)
		{
			
			BaseSection section = table.Parent as BaseSection;
			
			Point currentPosition = new Point(PrintHelper.DrawingAreaRelativeToParent(this.table.Parent,this.table).Location.X,
			                                  base.SectionBounds.DetailStart.Y);
			
			int defaultLeftPos = currentPosition.X;
			
			Point dataAreaStart = new Point(table.Items[0].Location.X,table.Items[0].Location.Y + currentPosition.Y);
			
			ISimpleContainer headerRow = null;
			Rectangle pageBreakRect = Rectangle.Empty;
			this.table.Items.SortByLocation();
			
			// Header
			
			var simpleContainer = table.Items[0] as ISimpleContainer;
			
			if (simpleContainer.Items.Count > 0)
			{
				simpleContainer.Location = new Point (simpleContainer.Location.X,simpleContainer.Location.Y);
				simpleContainer.Parent = (BaseReportItem)this.table;
				
				base.SaveSize( new Size (simpleContainer.Size.Width,simpleContainer.Size.Height));
				
				if (PrintHelper.IsTextOnlyRow(simpleContainer) ) {
					headerRow = simpleContainer;
					currentPosition = BaseConverter.BaseConvert(exporterCollection,headerRow,defaultLeftPos,currentPosition);
				}

				BaseGroupedRow row = table.Items[1] as BaseGroupedRow;
				
				if (row != null) {
					
					//grouped
					do {
						
						// GetType child navigator
						IDataNavigator childNavigator = base.DataNavigator.GetChildNavigator();
						
						base.Evaluator.SinglePage.IDataNavigator = childNavigator;
						// Convert Grouping Header
						
						currentPosition = ConvertGroupHeader(exporterCollection,section,defaultLeftPos,currentPosition);
						
						childNavigator.Reset();
						childNavigator.MoveNext();
						
						//Convert children
						if (childNavigator != null) {
							do
							{
								StandardPrinter.AdjustBackColor(simpleContainer,GlobalValues.DefaultBackColor);
								simpleContainer = table.Items[2] as ISimpleContainer;
								
								childNavigator.Fill(simpleContainer.Items);
								
								currentPosition = ConvertGroupChilds (exporterCollection,
								                                      section,
								                                      simpleContainer,defaultLeftPos,currentPosition);
								pageBreakRect = PrintHelper.CalculatePageBreakRectangle((BaseReportItem)simpleContainer,currentPosition);
								
								if (PrintHelper.IsPageFull(pageBreakRect,base.SectionBounds))
								{
									currentPosition = base.ForcePageBreak(exporterCollection,section);
									currentPosition = ConvertStandardRow (exporterCollection,section,headerRow,defaultLeftPos,currentPosition);
								}
							}
							while ( childNavigator.MoveNext());
							
							if (PageBreakAfterGroupChange(table) )
							{
								if ( base.DataNavigator.HasMoreData)
								{
									currentPosition = base.ForcePageBreak(exporterCollection,section);
									currentPosition = ConvertStandardRow (exporterCollection,section,headerRow,defaultLeftPos,currentPosition);
								}
								
							}
							
							base.Evaluator.SinglePage.IDataNavigator = base.DataNavigator;
						}
					}
					while (base.DataNavigator.MoveNext());
				}
				
				else
				{
					// No Grouping at all
					
					base.SaveSize(simpleContainer.Size);
					simpleContainer =  table.Items[1] as ISimpleContainer;
					
					do {

						PrintHelper.AdjustSectionLocation(section);
						
						pageBreakRect = PrintHelper.CalculatePageBreakRectangle((BaseReportItem)simpleContainer,currentPosition);
						
						if (PrintHelper.IsPageFull(pageBreakRect,base.SectionBounds))
						{
							currentPosition = base.ForcePageBreak(exporterCollection,section);
							currentPosition = ConvertStandardRow (exporterCollection,section,headerRow,defaultLeftPos,currentPosition);
						}
						
						currentPosition = ConvertStandardRow (exporterCollection,section,simpleContainer,defaultLeftPos,currentPosition);
						
						simpleContainer.Size = base.RestoreSize;
					}
					while (base.DataNavigator.MoveNext());
					
					base.DataNavigator.Reset();
					base.DataNavigator.MoveNext();
					SectionBounds.ReportFooterRectangle =  new Rectangle(SectionBounds.ReportFooterRectangle.Left,
					                                                     currentPosition.Y,
					                                                     SectionBounds.ReportFooterRectangle.Width,
					                                                     SectionBounds.ReportFooterRectangle.Height);				
				}
			}
			return exporterCollection;
		}
			
		protected override Point ForcePageBreak(ExporterCollection exporterCollection, BaseSection section)
		{
			base.ForcePageBreak(exporterCollection, section);
			return base.SectionBounds.ReportHeaderRectangle.Location;
		}
		//Copy from GroupedRow
		
		private Point ConvertGroupHeader(ExporterCollection exportList,BaseSection section,int leftPos,Point offset)
		{
			var retVal = Point.Empty;
			ReportItemCollection groupCollection = null;
			
			var groupedRow  = new Collection<BaseGroupedRow>(table.Items.OfType<BaseGroupedRow>().ToList());
			
			if (groupedRow.Count == 0) {
				groupCollection = section.Items.ExtractGroupedColumns();
				base.DataNavigator.Fill(groupCollection);
				base.FireSectionRendering(section);
				ExporterCollection list = StandardPrinter.ConvertPlainCollection(groupCollection,offset);
				
				StandardPrinter.EvaluateRow(base.Evaluator,list);
				
				exportList.AddRange(list);
				AfterConverting (list);
				retVal =  new Point (leftPos,offset.Y + groupCollection[0].Size.Height + 20  + (3 *GlobalValues.GapBetweenContainer));
			} else {
				retVal = ConvertStandardRow(exportList,section,groupedRow[0],leftPos,offset);
			}
			return retVal;
		}	
		
	}
}

