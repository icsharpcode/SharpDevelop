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
			IContainerItem row = item as IContainerItem;
			PrintHelper.AdjustParent(parent,row.Items);
			if (PrintHelper.IsTextOnlyRow(item as BaseRowItem)) {
				return this.ConvertTextOnlyRow(parent, item);
			} else {
				return this.ConvertDataRow(parent,item);
			}
		}
		
		
		
		private ExporterCollection ConvertDataRow (BaseReportItem parent, BaseReportItem item)
		{
			ExporterCollection mylist = new ExporterCollection();
			BaseSection section = parent as BaseSection;
			Size containerSize = new Size (section.Items[0].Size.Width,section.Items[0].Size.Height);
			IContainerItem row = section.Items[0] as IContainerItem;
			do {
				section.Location = new Point(section.Location.X,section.SectionOffset );
				base.DataNavigator.Fill(row.Items);
				
				//
				section.Size = this.SectionBounds.DetailSectionRectangle.Size;
				
				PrintHelper.SetLayoutForRow(base.Graphics,base.Layouter,row);
			
				mylist.AddRange(this.ConvertTextOnlyRow(parent,item));
				section.SectionOffset += section.Size.Height + 2 * base.SinglePage.SectionBounds.Gap;
				Rectangle r = new Rectangle(section.Location.X,section.SectionOffset,
				                            section.Size.Width,section.Size.Height);
				
				if (base.IsPageFull(r)) {
					base.FirePageFull(mylist);
					section.Location = new Point(section.Location.X,SectionBounds.DetailStart.Y );
					section.SectionOffset = SectionBounds.DetailStart.Y;
					mylist.Clear();
				}
				section.Items[0].Size = containerSize;
			}
			
			while (base.DataNavigator.MoveNext());
			
			SectionBounds.ReportFooterRectangle =  new Rectangle(SectionBounds.ReportFooterRectangle.Left,
			                                                     section.Location.Y + section.Size.Height,
			                                                     SectionBounds.ReportFooterRectangle.Width,
			                                                     SectionBounds.ReportFooterRectangle.Height);
			return mylist;
		}
		
	
		private ExporterCollection ConvertTextOnlyRow(BaseReportItem parent,
		                                              BaseReportItem item)                                            
		{
			ExporterCollection mylist = base.Convert(parent,item);
			this.baseRowItem = item as BaseRowItem;

			Point drawAt = base.ParentRectangle.Location;
			baseRowItem.Location = new Point (base.ParentRectangle.Left,baseRowItem.Location.Y);
			ExporterCollection ml = this.ConvertItems (parent,baseRowItem, drawAt);			
			drawAt = new Point (base.ParentRectangle.Left,drawAt.Y + baseRowItem.Size.Height);
			mylist.AddRange(ml);
			return mylist;
		}
	}
}
