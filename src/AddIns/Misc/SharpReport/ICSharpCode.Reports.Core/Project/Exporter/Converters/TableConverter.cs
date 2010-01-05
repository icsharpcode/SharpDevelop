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
			
			int defaultLeftPos = PrintHelper.DrawingAreaRelativeToParent(this.baseTable.Parent,this.baseTable).Left;
			
			this.baseTable.Items.SortByLocation();
			
			foreach (BaseRowItem row in this.baseTable.Items)
			{
				if (row.Items.Count > 0) {
					row.Location = new Point (row.Location.X + defaultLeftPos,row.Location.Y);
					row.Parent = (BaseReportItem)this.baseTable;
					Size rowSize = new Size (row.Size.Width,row.Size.Height);
					// Header/FooterRow
					if (PrintHelper.IsTextOnlyRow(row) ) {
						PrintHelper.SetLayoutForRow(base.Graphics,base.Layouter,row);
						currentPosition = DoConvert (mylist,row,defaultLeftPos,currentPosition);
					}
					
					else {
						// DataRegion
						rowSize = new Size (row.Size.Width,row.Size.Height);
						do {
							//
							BaseSection section = this.baseTable.Parent as BaseSection;
							section.Location = new Point(section.Location.X,section.SectionOffset );
							base.DataNavigator.Fill(row.Items);
							
							PrintHelper.SetLayoutForRow(base.Graphics,base.Layouter,row);
							
							if (this.IsPageFull(new Rectangle(row.Location,row.Size))) {
								base.FirePageFull(mylist);
								currentPosition = new Point(base.SectionBounds.DetailStart.X,base.SectionBounds.DetailStart.Y);
								mylist.Clear();
								if (row != null) {
									currentPosition = this.DoConvert (mylist,row,defaultLeftPos,currentPosition);
								}
							}
							currentPosition = this.DoConvert(mylist,row,defaultLeftPos,currentPosition);
							row.Size = rowSize;
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
		
		
		private Point DoConvert(ExporterCollection mylist,BaseRowItem row,int x,Point dp)                      
		{
			ExporterCollection ml = base.ConvertItems (this.baseTable.Parent,row, dp);
			mylist.AddRange(ml);
			return new Point (x,dp.Y + row.Size.Height + (3 *GlobalValues.GapBetweenContainer));
		}
		
	}
}
