// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of RowConverter.
	/// </summary>
	/// 
	
	public class GroupedRowConverter:BaseConverter
	{

		private BaseReportItem parent;
		
		public GroupedRowConverter(IDataNavigator dataNavigator,
		                           ExporterPage singlePage, ILayouter layouter):base(dataNavigator,singlePage,layouter)
		{
		}
		
		public override ExporterCollection Convert(BaseReportItem parent, BaseReportItem item)
		{
			if (parent == null) {
				throw new ArgumentNullException("parent");
			}
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			
			ISimpleContainer simpleContainer = item as ISimpleContainer;
			this.parent = parent;
			
			simpleContainer.Parent = parent;
			
			PrintHelper.AdjustParent(parent,simpleContainer.Items);
			if (PrintHelper.IsTextOnlyRow(simpleContainer)) {
				ExporterCollection myList = new ExporterCollection();

				BaseConverter.BaseConvert (myList,simpleContainer,parent.Location.X,
				                           new Point(base.SectionBounds.DetailStart.X,base.SectionBounds.DetailStart.Y));
				
				return myList;
			} else {
				return this.ConvertDataRow(simpleContainer);
			}
		}
		
		
		private ExporterCollection ConvertDataRow (ISimpleContainer simpleContainer)
		{
			ExporterCollection exporterCollection = new ExporterCollection();
			Point currentPosition = new Point(base.SectionBounds.DetailStart.X,base.SectionBounds.DetailStart.Y);
			BaseSection section = parent as BaseSection;
			
			int defaultLeftPos = parent.Location.X;

			Rectangle pageBreakRect = Rectangle.Empty;
			
			do {
				
				PrintHelper.AdjustSectionLocation (section);
				section.Size = this.SectionBounds.DetailSectionRectangle.Size;
				base.SaveSize(section.Items[0].Size);
				
				// did we have GroupedItems at all
				if (section.Items.IsGrouped) {
					
					// GetType child navigator
					IDataNavigator childNavigator = base.DataNavigator.GetChildNavigator();
					
					base.Evaluator.SinglePage.IDataNavigator = childNavigator;
					// Convert Grouping Header
					
					currentPosition = ConvertGroupHeader(exporterCollection,section,defaultLeftPos,currentPosition);
					
					childNavigator.Reset();
					childNavigator.MoveNext();
					
					//Convert children
					if (childNavigator != null) {
						StandardPrinter.AdjustBackColor(simpleContainer,GlobalValues.DefaultBackColor);
						
						do {
							childNavigator.Fill(simpleContainer.Items);
							
							currentPosition = ConvertGroupChilds (exporterCollection,section,
							                                      simpleContainer,defaultLeftPos,currentPosition);
							pageBreakRect = PrintHelper.CalculatePageBreakRectangle((BaseReportItem)section.Items[1],currentPosition);
							
							if (PrintHelper.IsPageFull(pageBreakRect,base.SectionBounds )) {
								currentPosition = ForcePageBreak (exporterCollection,section);
							}
						}
						while ( childNavigator.MoveNext());
						
						if (PageBreakAfterGroupChange(section) ) {
							
							if ( base.DataNavigator.HasMoreData)
							{
								currentPosition = ForcePageBreak (exporterCollection,section);
							}
						}
						
						base.Evaluator.SinglePage.IDataNavigator = base.DataNavigator;
					}
				}
				else
				{
					// No Grouping at all
					currentPosition = ConvertStandardRow (exporterCollection,section,simpleContainer,defaultLeftPos,currentPosition);
				}
				
				pageBreakRect = PrintHelper.CalculatePageBreakRectangle((BaseReportItem)section.Items[0],currentPosition);
				
				if (PrintHelper.IsPageFull(pageBreakRect,base.SectionBounds)) {
					currentPosition= ForcePageBreak (exporterCollection,section);
				}
				
				ShouldDrawBorder (section,exporterCollection);
			}
			while (base.DataNavigator.MoveNext());
			
			SectionBounds.ReportFooterRectangle =  new Rectangle(SectionBounds.ReportFooterRectangle.Left,
			                                                     section.Location.Y + section.Size.Height,
			                                                     SectionBounds.ReportFooterRectangle.Width,
			                                                     SectionBounds.ReportFooterRectangle.Height);
			return exporterCollection;
		}
		
		
		protected override Point ForcePageBreak(ExporterCollection exporterCollection, BaseSection section)
		{
			base.ForcePageBreak(exporterCollection,section);
			return CalculateStartPosition();
		}
	
		
		private Point CalculateStartPosition()
		{
			return new Point(base.SectionBounds.PageHeaderRectangle.X,base.SectionBounds.PageHeaderRectangle.Y);
		}
		
		
		private Point ConvertGroupHeader(ExporterCollection exportList,BaseSection section,int leftPos,Point offset)
		{
			var retVal = Point.Empty;
			ReportItemCollection groupCollection = null;
			var groupedRows  = BaseConverter.FindGroups(section);
			if (groupedRows.Count == 0) {
				groupCollection = section.Items.ExtractGroupedColumns();
				base.DataNavigator.Fill(groupCollection);
				base.FireSectionRendering(section);
				ExporterCollection list = StandardPrinter.ConvertPlainCollection(groupCollection,offset);
				
				StandardPrinter.EvaluateRow(base.Evaluator,list);
				
				exportList.AddRange(list);
				AfterConverting (list);
				retVal =  new Point (leftPos,offset.Y + groupCollection[0].Size.Height + 20  + (3 *GlobalValues.GapBetweenContainer));
			} else {
				retVal = ConvertStandardRow(exportList,section,groupedRows[0],leftPos,offset);
			}
			return retVal;
		}
	
		
		private static void ShouldDrawBorder (BaseSection section,ExporterCollection list)
		{
			if (section.DrawBorder == true) {
				BaseRectangleItem br = BasePager.CreateDebugItem (section);
				BaseExportColumn bec = br.CreateExportColumn();
				bec.StyleDecorator.Location = section.Location;
				list.Insert(0,bec);
			}
		}
	}
}
