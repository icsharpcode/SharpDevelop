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
			ISimpleContainer simpleContainer = item as ISimpleContainer;
			this.parent = parent;
			
			simpleContainer.Parent = parent;
			
			PrintHelper.AdjustParent(parent,simpleContainer.Items);
			if (PrintHelper.IsTextOnlyRow(simpleContainer)) {
				ExporterCollection myList = new ExporterCollection();

				base.BaseConvert (myList,simpleContainer,parent.Location.X,
				                  new Point(base.SectionBounds.DetailStart.X,base.SectionBounds.DetailStart.Y));
				
				
				return myList;
			} else {
				return this.ConvertDataRow(simpleContainer);
			}
		}
		
		private ExporterCollection ConvertDataRow (ISimpleContainer simpleContainer)
		{
			ExporterCollection mylist = new ExporterCollection();
			Point currentPosition = new Point(base.SectionBounds.DetailStart.X,base.SectionBounds.DetailStart.Y);
			BaseSection section = parent as BaseSection;
			
			int defaultLeftPos = parent.Location.X;
			
			do {
				section.Location = new Point(section.Location.X,section.SectionOffset );
				section.Size = this.SectionBounds.DetailSectionRectangle.Size;
				base.SaveSize(section.Items[0].Size);
				
				base.FillAndLayoutRow(simpleContainer);
				base.FireSectionRendering(section);
				currentPosition = base.BaseConvert(mylist,simpleContainer,defaultLeftPos,currentPosition);
				
				section.Items[0].Size = base.RestoreSize;
				section.SectionOffset += section.Size.Height + 2 * base.SinglePage.SectionBounds.Gap;

				
				if (PrintHelper.IsPageFull(new Rectangle(new Point (simpleContainer.Location.X,currentPosition.Y), section.Size),base.SectionBounds)) {
					base.FirePageFull(mylist);
					section.SectionOffset = base.SinglePage.SectionBounds.PageHeaderRectangle.Location.Y;
					currentPosition = new Point(base.SectionBounds.PageHeaderRectangle.X,base.SectionBounds.PageHeaderRectangle.Y);
					mylist.Clear();
				}
				
				if (section.DrawBorder == true) {
					BaseRectangleItem br = BasePager.CreateDebugItem (section);
					BaseExportColumn bec = br.CreateExportColumn();
					bec.StyleDecorator.Location = section.Location;
					mylist.Insert(0,bec);
				}
			}
			while (base.DataNavigator.MoveNext());
			
			SectionBounds.ReportFooterRectangle =  new Rectangle(SectionBounds.ReportFooterRectangle.Left,
			                                                     section.Location.Y + section.Size.Height,
			                                                     SectionBounds.ReportFooterRectangle.Width,
			                                                     SectionBounds.ReportFooterRectangle.Height);
			return mylist;
		}
		
	}
}
