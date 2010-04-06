/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 21.12.2008
 * Zeit: 11:22
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of TableConverter.
	/// </summary>
	public class TableConverter:BaseConverter
	{

		private ITableContainer baseTable;
		
		public TableConverter(IDataNavigator dataNavigator,
		                      ExporterPage singlePage,
		                      IExportItemsConverter exportItemsConverter,
		                      ILayouter layouter ):base(dataNavigator,singlePage,exportItemsConverter,layouter)
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
			this.baseTable = (BaseTableItem)item ;
			this.baseTable.Parent = parent;
			this.baseTable.DataNavigator = base.DataNavigator;
			return ConvertInternal(mylist);
		}
		
		
		private ExporterCollection ConvertInternal(ExporterCollection mylist)
		{
			Point currentPosition = new Point(base.SectionBounds.DetailStart.X,base.SectionBounds.DetailStart.Y);
			Point dataAreaStart = new Point(baseTable.Items[0].Location.X,baseTable.Items[0].Location.Y + currentPosition.Y);
			BaseRowItem headerRow = null;
			int defaultLeftPos = PrintHelper.DrawingAreaRelativeToParent(this.baseTable.Parent,this.baseTable).Left;
			
			this.baseTable.Items.SortByLocation();
			
			foreach (BaseRowItem row in this.baseTable.Items)
			{
				if (row.Items.Count > 0) {
					row.Location = new Point (row.Location.X + defaultLeftPos,row.Location.Y);
					row.Parent = (BaseReportItem)this.baseTable;
					base.KeepSize =  new Size (row.Size.Width,row.Size.Height);
					// Header/FooterRow
				
					if (PrintHelper.IsTextOnlyRow(row) ) {
						headerRow = row;
						currentPosition = InternalConvertRow(mylist,headerRow,defaultLeftPos,currentPosition);
					}
					
					else 
					{
						// DataRegion
						base.KeepSize = new Size (row.Size.Width,row.Size.Height);
						do {
							//
							BaseSection section = this.baseTable.Parent as BaseSection;
							section.Location = new Point(section.Location.X,section.SectionOffset );
							base.DoRow(row);
							
							if (PrintHelper.IsPageFull(new Rectangle(new Point (row.Location.X,currentPosition.Y),row.Size),base.SectionBounds)) {
								base.FirePageFull(mylist);
								mylist.Clear();
								currentPosition = InternalConvertRow(mylist,headerRow,
								                                   defaultLeftPos,
								                                   base.SectionBounds.ReportHeaderRectangle.Location);
							}
							
							currentPosition = InternalConvertRow(mylist,row,defaultLeftPos,currentPosition);
							row.Size = base.KeepSize;
						}
						while (base.DataNavigator.MoveNext());
						//Allway's reset the DataNavigator
						base.DataNavigator.Reset();
						base.DataNavigator.MoveNext();
						SectionBounds.ReportFooterRectangle =  new Rectangle(SectionBounds.ReportFooterRectangle.Left,
						                                                     currentPosition.Y,
						                                                     SectionBounds.ReportFooterRectangle.Width,
						                                                     SectionBounds.ReportFooterRectangle.Height);
					}
				}
			}
			return mylist;
		}
		
		
		private Point InternalConvertRow(ExporterCollection myList,BaseRowItem row,int leftPos,Point curPos	)
		{
			ExporterCollection ml = base.ConvertItems (this.baseTable.Parent,row, curPos);
			myList.AddRange(ml);
			return new Point (leftPos,curPos.Y + row.Size.Height + (3 *GlobalValues.GapBetweenContainer));
		}

	}
}
