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

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of RowConverter.
	/// </summary>
	/// 
	
	public class old_RowConverter:BaseConverter
	{

		private BaseReportItem parent;
		
		public old_RowConverter(IDataNavigator dataNavigator,
		                    ExporterPage singlePage,
		                    
		                    ILayouter layouter):base(dataNavigator,singlePage,layouter)
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
				
				PrintHelper.AdjustSectionLocation (section);
				section.Size = this.SectionBounds.DetailSectionRectangle.Size;
				base.SaveSize(section.Items[0].Size);
				Color color = ((BaseReportItem)simpleContainer).BackColor;
				if (base.DataNavigator.HasChildren)
				{
					TestDecorateElement(simpleContainer);
				}
			
				base.FillRow(simpleContainer);
				
				TestPrepareContainerForConverting(simpleContainer);

				base.FireSectionRendering(section);

				currentPosition = base.BaseConvert(mylist,simpleContainer,defaultLeftPos,currentPosition);
				
				TestAfterConverting (mylist,section);
			// Grouping starts
				if (base.DataNavigator.HasChildren) {
					
					((BaseReportItem)simpleContainer).BackColor = color;
					base.DataNavigator.SwitchGroup();
					do {
						((BaseReportItem)simpleContainer).BackColor = color;

						base.DataNavigator.FillChild(simpleContainer.Items);
						TestPrepareContainerForConverting(simpleContainer);

						base.FireSectionRendering(section);

						currentPosition = base.BaseConvert(mylist,simpleContainer,defaultLeftPos,currentPosition);

						TestAfterConverting (mylist,section);
					}
					while ( base.DataNavigator.ChildMoveNext());

				}
			
				// end grouping
		
				if (PrintHelper.IsPageFull(new Rectangle(new Point (simpleContainer.Location.X,currentPosition.Y), section.Size),base.SectionBounds)) {
					base.FirePageFull(mylist);
					section.SectionOffset = base.SinglePage.SectionBounds.PageHeaderRectangle.Location.Y;
					currentPosition = new Point(base.SectionBounds.PageHeaderRectangle.X,base.SectionBounds.PageHeaderRectangle.Y);
					mylist.Clear();
				}
				
				ShouldDrawBorder (section,mylist);
				
			}
			while (base.DataNavigator.MoveNext());
			
			SectionBounds.ReportFooterRectangle =  new Rectangle(SectionBounds.ReportFooterRectangle.Left,
			                                                     section.Location.Y + section.Size.Height,
			                                                     SectionBounds.ReportFooterRectangle.Width,
			                                                     SectionBounds.ReportFooterRectangle.Height);
			return mylist;
		}
		
		
		void TestPrepareContainerForConverting(ISimpleContainer simpleContainer)
		{
			base.LayoutRow(simpleContainer);
		}
		
		
		void TestAfterConverting (ExporterCollection mylist,BaseSection section)
		{
			StandardPrinter.EvaluateRow(base.Evaluator,mylist);
			section.Items[0].Size = base.RestoreSize;
			section.SectionOffset += section.Size.Height + 3 * GlobalValues.GapBetweenContainer;
		}
		
		
		Color TestDecorateElement(ISimpleContainer simpleContainer)
		{
			BaseReportItem i = simpleContainer as BaseReportItem;
			var retval = i.BackColor;
			i.BackColor = System.Drawing.Color.LightGray;
			return retval;
		}
		
		void ShouldDrawBorder (BaseSection section,ExporterCollection list)
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
