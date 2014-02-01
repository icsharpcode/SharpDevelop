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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of TableConverter.
	/// </summary>
	public class GroupedTableConverter:BaseConverter
	{

		private ITableContainer table;
		
		public GroupedTableConverter(IReportModel reportModel,IDataNavigator dataNavigator,
		                      ExporterPage singlePage):base(reportModel,dataNavigator,singlePage)
			
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
			return ConvertInternal(mylist);
		}
		
		
		
		private ExporterCollection ConvertInternal(ExporterCollection exporterCollection)
		{
			
			BaseSection section = table.Parent as BaseSection;
			
			ISimpleContainer headerRow = null;
			Point dataAreaStart = new Point(table.Items[0].Location.X,table.Items[0].Location.Y + base.CurrentPosition.Y);
			
			base.CurrentPosition = new Point(PrintHelper.DrawingAreaRelativeToParent(this.table.Parent,this.table).Location.X,
			                                 base.SectionBounds.DetailArea.Top);

			base.DefaultLeftPosition = base.CurrentPosition.X;
			
			this.table.Items.SortByLocation();
			
			// Header
			
			var simpleContainer = table.Items[0] as ISimpleContainer;
			Size containerSize = Size.Empty;
			
			if (simpleContainer.Items.Count > 0)
			{
				simpleContainer.Location = new Point (simpleContainer.Location.X,simpleContainer.Location.Y);
				simpleContainer.Parent = (BaseReportItem)this.table;
				
				base.SaveSectionSize(section.Size);
				containerSize = simpleContainer.Size;
				
				if (PrintHelper.IsTextOnlyRow(simpleContainer) ) {
					headerRow = simpleContainer;
					base.PrepareContainerForConverting(section,headerRow);
					base.CurrentPosition = ConvertContainer(exporterCollection,headerRow,base.DefaultLeftPosition,base.CurrentPosition);
				}

				GroupHeader row = table.Items[1] as GroupHeader;
				
				if (row != null) {
					
					//grouped
					do {
						
						// GetType child navigator
						IDataNavigator childNavigator = base.DataNavigator.GetChildNavigator;
						
						base.Evaluator.SinglePage.IDataNavigator = childNavigator;
						// Convert Grouping Header
						
						base.CurrentPosition = ConvertGroupHeader(exporterCollection,section,base.CurrentPosition);
						
						childNavigator.Reset();
						childNavigator.MoveNext();

						//Convert children
						if (childNavigator != null) {
							do
							{
								StandardPrinter.AdjustBackColor(simpleContainer);
								simpleContainer = table.Items[2] as ISimpleContainer;
								containerSize = simpleContainer.Size;
								
								FillRow(simpleContainer,childNavigator);						
								FireRowRendering(simpleContainer,childNavigator);
								PrepareContainerForConverting(section,simpleContainer);		
								base.CurrentPosition = ConvertStandardRow(exporterCollection,simpleContainer);
								
								simpleContainer.Size = containerSize;
								CheckForPageBreak(section,simpleContainer,headerRow,exporterCollection);

							}
							while ( childNavigator.MoveNext());
							
							// GroupFooter
							base.ConvertGroupFooter(table,exporterCollection);
							base.PageBreakAfterGroupChange(section,exporterCollection);
							
							base.Evaluator.SinglePage.IDataNavigator = base.DataNavigator;
						}
					}
					while (base.DataNavigator.MoveNext());
				}
				
				else
				{
					// No Grouping at all
					
					simpleContainer =  table.Items[1] as ISimpleContainer;				
					base.SaveSectionSize(section.Size);
					containerSize = simpleContainer.Size;
						
					do {

						PrintHelper.AdjustSectionLocation(section);
						CheckForPageBreak(section,simpleContainer,headerRow,exporterCollection);
						
						FillRow(simpleContainer,base.DataNavigator);
						FireRowRendering(simpleContainer,base.DataNavigator);
						base.PrepareContainerForConverting(section,simpleContainer);
						
						base.CurrentPosition = ConvertStandardRow (exporterCollection,simpleContainer);
						simpleContainer.Size = containerSize;
						section.Size = base.RestoreSectionSize;
					}
					while (base.DataNavigator.MoveNext());
					base.DataNavigator.Reset();
					base.DataNavigator.MoveNext();
					SectionBounds.ReportFooterRectangle =  new Rectangle(SectionBounds.ReportFooterRectangle.Left,
					                                                     base.CurrentPosition.Y,
					                                                     SectionBounds.ReportFooterRectangle.Width,
					                                                     SectionBounds.ReportFooterRectangle.Height);
				}
			}
			return exporterCollection;
		}
		
		#region Pagebreak
		
		void CheckForPageBreak(BaseSection section,ISimpleContainer container,ISimpleContainer headerRow, ExporterCollection exporterCollection)
		{
			var pageBreakRect = PrintHelper.CalculatePageBreakRectangle((BaseReportItem)container,base.CurrentPosition);
			
			if (PrintHelper.IsPageFull(pageBreakRect,base.SectionBounds))
			{
				base.CurrentPosition = ForcePageBreak(exporterCollection,section);
				base.CurrentPosition = ConvertStandardRow (exporterCollection,headerRow);
			}

		}
		
		
		protected override Point ForcePageBreak(ExporterCollection exporterCollection, BaseSection section)
		{
			base.ForcePageBreak(exporterCollection, section);
			return base.SectionBounds.ReportHeaderRectangle.Location;
		}
		
		
		#endregion
		
		
		#region Grouping
		
		private Point ConvertGroupHeader(ExporterCollection exportList,BaseSection section,Point offset)
		{
			var retVal = Point.Empty;
			var rowSize = Size.Empty;
			ReportItemCollection groupCollection = null;
			
			var groupedRow  = new Collection<GroupHeader>(table.Items.OfType<GroupHeader>().ToList());
			
			if (groupedRow.Count == 0) {
				
				groupCollection = section.Items.ExtractGroupedColumns();
				base.DataNavigator.Fill(groupCollection);
				base.FireSectionRendering(section);
				
				ExporterCollection list = ExportHelper.ConvertPlainCollection(groupCollection,offset);
				EvaluationHelper.EvaluateRow(base.Evaluator,list);
				
				exportList.AddRange(list);
				AfterConverting (list);
				retVal =  new Point (base.DefaultLeftPosition,offset.Y + groupCollection[0].Size.Height + 20  + (3 *GlobalValues.GapBetweenContainer));
				
				
			} else {
				
				rowSize = groupedRow[0].Size;
				FillRow(groupedRow[0],base.DataNavigator);
				base.FireGroupHeaderRendering(groupedRow[0]);
				retVal = ConvertStandardRow(exportList,groupedRow[0]);
				groupedRow[0].Size = rowSize;
			}
			return retVal;
		}	
		
		#endregion
	}
}
