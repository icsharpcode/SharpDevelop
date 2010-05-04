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
			ISimpleContainer headerContainer = null;
			
			int defaultLeftPos = PrintHelper.DrawingAreaRelativeToParent(this.baseTable.Parent,this.baseTable).Left;
			
			this.baseTable.Items.SortByLocation();
			
			foreach (ISimpleContainer simpleContainer in this.baseTable.Items)
			{
				if (simpleContainer.Items.Count > 0) {
					simpleContainer.Location = new Point (simpleContainer.Location.X + defaultLeftPos,simpleContainer.Location.Y);
					simpleContainer.Parent = (BaseReportItem)this.baseTable;
					base.SaveSize( new Size (simpleContainer.Size.Width,simpleContainer.Size.Height));
					// Header/FooterRow
				
					if (PrintHelper.IsTextOnlyRow(simpleContainer) ) {
						headerContainer = simpleContainer;
						currentPosition = base.BaseConvert(mylist,headerContainer,defaultLeftPos,currentPosition);
					}
					
					else 
					{
						// DataRegion
						base.SaveSize(simpleContainer.Size);
						do {
							//
							BaseSection section = this.baseTable.Parent as BaseSection;
							section.Location = new Point(section.Location.X,section.SectionOffset );
							base.FillAndLayoutRow(simpleContainer);
							
							if (PrintHelper.IsPageFull(new Rectangle(new Point (simpleContainer.Location.X,currentPosition.Y),simpleContainer.Size),base.SectionBounds)) {
								base.FirePageFull(mylist);
								mylist.Clear();

								currentPosition = base.BaseConvert(mylist,headerContainer,
								                                     defaultLeftPos,
								                                     base.SectionBounds.ReportHeaderRectangle.Location);
							}
							
							currentPosition = base.BaseConvert(mylist,simpleContainer,defaultLeftPos,currentPosition);
							simpleContainer.Size = base.RestoreSize;
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
	}
}
