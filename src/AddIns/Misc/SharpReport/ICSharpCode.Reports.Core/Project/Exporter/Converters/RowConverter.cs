/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 02.01.2009
 * Zeit: 17:33
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of RowConverter.
	/// </summary>
	/// 
	
	public class RowConverter:BaseConverter
	{
		private BaseRowItem baseRowItem;
		private BaseReportItem parent;
		
		public RowConverter(IDataNavigator dataNavigator,
		                    ExporterPage singlePage,
		                    IExportItemsConverter exportItemsConverter,
		                    ILayouter layouter):base(dataNavigator,singlePage,exportItemsConverter,layouter)
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
			this.parent = parent;
			
			this.baseRowItem = item as BaseRowItem;
			this.baseRowItem.Parent = parent;
			
			PrintHelper.AdjustParent(parent,this.baseRowItem.Items);
			if (PrintHelper.IsTextOnlyRow(this.baseRowItem)) {
				ExporterCollection myList = new ExporterCollection();
				 this.InternalConvertRow(myList, item,parent.Location.X,
				                        new Point(base.SectionBounds.DetailStart.X,base.SectionBounds.DetailStart.Y));
				return myList;
			} else {
				return this.ConvertDataRow(item);
			}
		}
		
		private ExporterCollection ConvertDataRow (BaseReportItem item)
		{
			ExporterCollection mylist = new ExporterCollection();
			Point currentPosition = new Point(base.SectionBounds.DetailStart.X,base.SectionBounds.DetailStart.Y);
			BaseSection section = parent as BaseSection;
			IContainerItem row = section.Items[0] as IContainerItem;
			int defaultLeftPos = parent.Location.X;
				
			do {
				section.Location = new Point(section.Location.X,section.SectionOffset );
				section.Size = this.SectionBounds.DetailSectionRectangle.Size;
				base.KeepSize = new Size (section.Items[0].Size.Width,section.Items[0].Size.Height);
				
				base.DoRow(row as BaseRowItem);
				base.FireSectionRendering(section);
				
				currentPosition = this.InternalConvertRow(mylist,item,defaultLeftPos,currentPosition);
				section.Items[0].Size = base.KeepSize;
				
				section.SectionOffset += section.Size.Height + 2 * base.SinglePage.SectionBounds.Gap;

				
				Rectangle r = new Rectangle(new Point (((BaseRowItem)row).Location.X,currentPosition.Y), section.Size);
				
				if (PrintHelper.IsPageFull(r,base.SectionBounds)) {
					base.FirePageFull(mylist);
					section.SectionOffset = base.SinglePage.SectionBounds.PageHeaderRectangle.Location.Y;
					currentPosition = new Point(base.SectionBounds.PageHeaderRectangle.X,base.SectionBounds.PageHeaderRectangle.Y);
					mylist.Clear();
				}
			}
			
			while (base.DataNavigator.MoveNext());
			
			SectionBounds.ReportFooterRectangle =  new Rectangle(SectionBounds.ReportFooterRectangle.Left,
			                                                     section.Location.Y + section.Size.Height,
			                                                     SectionBounds.ReportFooterRectangle.Width,
			                                                     SectionBounds.ReportFooterRectangle.Height);
			return mylist;
		}
		                                   
		
		private Point InternalConvertRow(ExporterCollection myList,BaseReportItem item,int leftPos,Point curPos)                                                   
		{
			return base.BaseConvert(myList,parent,item,leftPos,curPos);
			/*
			this.baseRowItem = item as BaseRowItem;
			baseRowItem.Location = new Point (leftPos,baseRowItem.Location.Y);	
			ExporterCollection ml = this.ConvertItems (this.parent,baseRowItem, curPos);		
			myList.AddRange(ml);
			return new Point (leftPos,curPos.Y + this.baseRowItem.Size.Height + (3 *GlobalValues.GapBetweenContainer));
			                                                                       * */
		}
	}
}
