// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Interfaces;
using System.Linq;
using System.Collections.ObjectModel;

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
			
			Point currentPosition = new Point(PrintHelper.DrawingAreaRelativeToParent(this.table.Parent,this.table).Location.X,
			                                  base.SectionBounds.DetailStart.Y);
			
			int defaultLeftPos = currentPosition.X;
			
			Point dataAreaStart = new Point(table.Items[0].Location.X,table.Items[0].Location.Y + currentPosition.Y);
			
			ISimpleContainer headerRow = null;
			Rectangle pageBreakRect = Rectangle.Empty;
			this.table.Items.SortByLocation();
			
			// Header
			
			var simpleContainer = table.Items[0] as ISimpleContainer;
			
			if (simpleContainer.Items.Count > 0) {
				//----
//				do {
				simpleContainer.Location = new Point (simpleContainer.Location.X,simpleContainer.Location.Y);
				simpleContainer.Parent = (BaseReportItem)this.table;
				
				base.SaveSize( new Size (simpleContainer.Size.Width,simpleContainer.Size.Height));
				
				if (PrintHelper.IsTextOnlyRow(simpleContainer) ) {
					Console.WriteLine("Print HeaderRow");
					headerRow = simpleContainer;
					currentPosition = BaseConverter.BaseConvert(exporterCollection,headerRow,defaultLeftPos,currentPosition);
				}

				BaseGroupedRow row = table.Items[1] as BaseGroupedRow;
				
				if (row != null) {
					
					//grouped
					do {
						
						Console.WriteLine("Group detected");
						// GetType child navigator
						IDataNavigator childNavigator = base.DataNavigator.GetChildNavigator();
						
						base.Evaluator.SinglePage.IDataNavigator = childNavigator;
						// Convert Grouping Header
						
						currentPosition = ConvertGroupHeader(exporterCollection,(BaseSection)table.Parent,defaultLeftPos,currentPosition);
						
						childNavigator.Reset();
						childNavigator.MoveNext();
						
						//Convert children
						if (childNavigator != null) {
							do
							{
								StandardPrinter.AdjustBackColor(simpleContainer,GlobalValues.DefaultBackColor);
								simpleContainer = table.Items[2] as ISimpleContainer;
								
								childNavigator.Fill(simpleContainer.Items);
								
								currentPosition = ConvertGroupChilds (exporterCollection,(BaseSection)table.Parent,
								                                      simpleContainer,defaultLeftPos,currentPosition);
								
							}
							while ( childNavigator.MoveNext());
							
							base.Evaluator.SinglePage.IDataNavigator = base.DataNavigator;
						}
					}
					while (base.DataNavigator.MoveNext());
				}
				
				else
				{
					// No Grouping at all
					
					// DataRegion
					
					base.SaveSize(simpleContainer.Size);
					simpleContainer =  table.Items[1] as ISimpleContainer;
					
					do {
						//
						BaseSection section = this.table.Parent as BaseSection;
						PrintHelper.AdjustSectionLocation(section);
						
						pageBreakRect = PrintHelper.CalculatePageBreakRectangle((BaseReportItem)simpleContainer,currentPosition);
						
						if (PrintHelper.IsPageFull(pageBreakRect,base.SectionBounds))
						{
							Console.WriteLine("PageBreak - PageFull");
							base.BuildNewPage(exporterCollection,section);
							
							currentPosition = base.SectionBounds.ReportHeaderRectangle.Location;
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
				AfterConverting (section,list);
				retVal =  new Point (leftPos,offset.Y + groupCollection[0].Size.Height + 20  + (3 *GlobalValues.GapBetweenContainer));
			} else {
				retVal = ConvertStandardRow(exportList,section,groupedRow[0],leftPos,offset);
			}
			return retVal;
		}	
		
		
		private Point ConvertGroupChilds(ExporterCollection mylist, BaseSection section, ISimpleContainer simpleContainer, int defaultLeftPos, Point currentPosition)
		{
			PrepareContainerForConverting(section,simpleContainer);
			Point curPos  = BaseConverter.BaseConvert(mylist,simpleContainer,defaultLeftPos,currentPosition);
			AfterConverting (section,mylist);
			return curPos;
		}
		
		private void AfterConverting (BaseSection section,ExporterCollection mylist)
		{
			Console.WriteLine("AfterConverting");
			StandardPrinter.EvaluateRow(base.Evaluator,mylist);
//			section.Items[0].Size = base.RestoreSize;
//			section.SectionOffset += section.Size.Height + 3 * GlobalValues.GapBetweenContainer;
		}
		
		private  Point ConvertStandardRow(ExporterCollection mylist, BaseSection section, ISimpleContainer simpleContainer, int defaultLeftPos, Point currentPosition)
		{
			base.FillRow(simpleContainer);
			PrepareContainerForConverting(section,simpleContainer);
			Point curPos = BaseConverter.BaseConvert(mylist,simpleContainer,defaultLeftPos,currentPosition);
			AfterConverting (section,mylist);
			return curPos;
		}
	}
}

