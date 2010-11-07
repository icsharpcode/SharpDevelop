// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Events;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.Exporter
{

	/// <summary>
	/// Description of BaseConverter.
	/// </summary>
	
	public class BaseConverter:IBaseConverter
	{
		
		private IDataNavigator dataNavigator;
		private ExporterPage singlePage;
		private SectionBounds sectionBounds;
		private Rectangle parentRectangle;
		private ILayouter layouter;
		private Size saveSize;
		private IExpressionEvaluatorFacade evaluator;
	
		public event EventHandler <NewPageEventArgs> PageFull;
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		public event EventHandler<GroupHeaderEventArgs> GroupHeaderRendering;
		public event EventHandler<RowRenderEventArgs> RowRendering;

		public BaseConverter(IDataNavigator dataNavigator,ExporterPage singlePage,
		                     ILayouter layouter)
		{
			if (dataNavigator == null) {
				throw new ArgumentNullException("dataNavigator");
			}
			if (singlePage == null) {
				throw new ArgumentNullException("singlePage");
			}

			if (layouter == null) {
				throw new ArgumentNullException("layouter");
			}
			this.singlePage = singlePage;
			this.dataNavigator = dataNavigator;
			this.sectionBounds = this.singlePage.SectionBounds;
			this.layouter = layouter;
			this.evaluator = StandardPrinter.CreateEvaluator(this.singlePage,this.dataNavigator);
		}
		
		
		#region PageBreak
		
		protected void BuildNewPage(ExporterCollection myList,BaseSection section)
		{
			FirePageFull(myList);
			section.SectionOffset = SinglePage.SectionBounds.PageHeaderRectangle.Location.Y;
			myList.Clear();
		}
		
		
		protected void FirePageFull (ExporterCollection items)
		{
			EventHelper.Raise<NewPageEventArgs>(PageFull,this,new NewPageEventArgs(items));
		}
		
		
		#endregion
		
		protected void FireRowRendering (ISimpleContainer detailRow)
		{
			BaseRowItem row = detailRow as BaseRowItem;
			if (row == null) {
				throw new ArgumentException("row");
			}
			RowRenderEventArgs rrea = new RowRenderEventArgs(row);
			EventHelper.Raise<RowRenderEventArgs>(RowRendering,this,rrea);
		}
		
		
		protected void FireGroupHeaderRendering (BaseGroupedRow groupHeader)
		{
			GroupHeaderEventArgs ghea = new GroupHeaderEventArgs(groupHeader);
			EventHelper.Raise<GroupHeaderEventArgs>(GroupHeaderRendering,this,ghea);
		}
			
		
		
		protected void FireSectionRendering (BaseSection section)
		{
			SectionRenderEventArgs srea = new SectionRenderEventArgs(section,
			                                                         this.SinglePage.PageNumber,
			                                                         this.dataNavigator.CurrentRow,
			                                                         section);
			EventHelper.Raise<SectionRenderEventArgs>(SectionRendering,this,srea);
		}
		
		
		
		protected  static ExporterCollection ConvertItems (ISimpleContainer row,Point offset)		                                          
		{

			IExportColumnBuilder exportLineBuilder = row as IExportColumnBuilder;

			if (exportLineBuilder != null) {

				ExportContainer lineItem = StandardPrinter.ConvertToContainer(row,offset);
				
				StandardPrinter.AdjustBackColor(row);
				ExporterCollection list = StandardPrinter.ConvertPlainCollection(row.Items,offset);
					
				lineItem.Items.AddRange(list);
				
				ExporterCollection containerList = new ExporterCollection();
				containerList.Add (lineItem);
				return containerList;
			}
			return null;
		}
		
	
		#region Grouping
		
		protected Point ConvertGroupChilds(ExporterCollection mylist, BaseSection section, ISimpleContainer simpleContainer, int defaultLeftPos, Point currentPosition)
		{
			PrepareContainerForConverting(section,simpleContainer);
			FireRowRendering(simpleContainer);
			Point curPos  = BaseConverter.BaseConvert(mylist,simpleContainer,defaultLeftPos,currentPosition);
			AfterConverting (section,mylist);
			return curPos;
		}
		
		
		protected  bool PageBreakAfterGroupChange(ISimpleContainer container)
		{
			var groupedRows  = BaseConverter.FindGroups(container);
			if (groupedRows.Count > 0) {
				var groupedRow = groupedRows[0];
				return groupedRow.PageBreakOnGroupChange;
			}
			return false;
		}
		
		
		protected static Collection<BaseGroupedRow> FindGroups (ISimpleContainer container)
		{
			return new Collection<BaseGroupedRow>(container.Items.OfType<BaseGroupedRow>().ToList());
		}
		
		
		protected virtual Point ForcePageBreak(ExporterCollection exporterCollection, BaseSection section)
		{
			BuildNewPage(exporterCollection,section);
			return Point.Empty;
		}
		
		#endregion
		
		#region IBaseConverter
		
		public virtual ExporterCollection Convert(BaseReportItem parent, BaseReportItem item)
		{
			this.parentRectangle = new Rectangle(parent.Location,parent.Size);
			return new ExporterCollection();;
		}
		
		public Point CurrentPosition {get;set;}
		
		#endregion
		
		public Rectangle ParentRectangle {
			get { return parentRectangle; }
		}
		
		
		public ISinglePage SinglePage {
			get { return singlePage; }
		}
		
		
		public SectionBounds SectionBounds {
			get { return sectionBounds; }
		}
		
		public IDataNavigator DataNavigator {
			get { return dataNavigator; }
		}
		
		
		public ILayouter Layouter {
			get { return layouter; }
		}
		
		public Graphics Graphics {get;set;}
		
		
		
		protected void  SaveSectionSize(Size size)
		{
			this.saveSize = size;
		}
		
		
		protected Size RestoreSectionSize
		{
			get {return this.saveSize;}
		}
		
		
		protected IExpressionEvaluatorFacade Evaluator
		{
			get {return this.evaluator;}
		}
		
		
		protected	void PrepareContainerForConverting(BaseSection section,ISimpleContainer simpleContainer)
		{
			FireSectionRendering(section);
			LayoutRow(simpleContainer);
		}
		
		
		protected void AfterConverting (BaseSection section,ExporterCollection convertedList)
		{
			StandardPrinter.EvaluateRow(Evaluator,convertedList);
		}
		
		
		
		protected  Point ConvertStandardRow(ExporterCollection mylist, BaseSection section, ISimpleContainer simpleContainer, int defaultLeftPos, Point currentPosition)
		{
			var rowSize = simpleContainer.Size;
			FillRow(simpleContainer);
			PrepareContainerForConverting(section,simpleContainer);
			Point curPos = BaseConverter.BaseConvert(mylist,simpleContainer,defaultLeftPos,currentPosition);
			AfterConverting (section,mylist);
			simpleContainer.Size = rowSize;
			return curPos;
		}
		
		
		protected static Point BaseConvert(ExporterCollection myList,ISimpleContainer container,int leftPos,Point curPos)
		{
			ExporterCollection ml = BaseConverter.ConvertItems (container, curPos);		
			myList.AddRange(ml);
			return new Point (leftPos,curPos.Y + container.Size.Height + (3 *GlobalValues.GapBetweenContainer));
		}
		
		
		private void FillRow (ISimpleContainer row)
		{
			DataNavigator.Fill(row.Items);
		}
		
		
		private void LayoutRow (ISimpleContainer row)
		{
			PrintHelper.SetLayoutForRow(Graphics,Layouter,row);
		}
	}
}
