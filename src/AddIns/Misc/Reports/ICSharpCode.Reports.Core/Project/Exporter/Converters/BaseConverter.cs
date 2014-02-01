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
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Events;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.Exporter
{

	/// <summary>
	/// Description of BaseConverter.
	/// </summary>
	
	public class BaseConverter:IBaseConverter
	{
		private Size saveSize;
		
		public event EventHandler <NewPageEventArgs> PageFull;
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		public event EventHandler<GroupHeaderEventArgs> GroupHeaderRendering;
		public event EventHandler<GroupFooterEventArgs> GroupFooterRendering;
		public event EventHandler<RowRenderEventArgs> RowRendering;

		
		public BaseConverter(IReportModel reportModel,IDataNavigator dataNavigator,ExporterPage singlePage)
		{
			if (dataNavigator == null) {
				throw new ArgumentNullException("dataNavigator");
			}
			if (singlePage == null) {
				throw new ArgumentNullException("singlePage");
			}
			this.ReportModel = reportModel;

			this.SinglePage = singlePage;
			this.DataNavigator = dataNavigator;
			this.Layouter =  (ILayouter)ServiceContainer.GetService(typeof(ILayouter));
			this.Evaluator = EvaluationHelper.CreateEvaluator(this.SinglePage,this.DataNavigator);
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
			var newPage = new NewPageEventArgs (items,SinglePage.SectionBounds);
			EventHelper.Raise<NewPageEventArgs>(PageFull,this,newPage);
			SinglePage.SectionBounds = newPage.SectionBounds;
		}
		
		#endregion
		
		
		#region Events
		
		protected void FireRowRendering (ISimpleContainer detailRow,IDataNavigator currentNavigator)
		{
			BaseRowItem row = detailRow as BaseRowItem;
			if (row == null) {
				throw new ArgumentException("row");
			}
			RowRenderEventArgs rrea = new RowRenderEventArgs(row,currentNavigator.Current);
			EventHelper.Raise<RowRenderEventArgs>(RowRendering,this,rrea);
		}
		
		
		protected void FireGroupHeaderRendering (GroupHeader groupHeader)
		{
			GroupHeaderEventArgs ghea = new GroupHeaderEventArgs(groupHeader);
			EventHelper.Raise<GroupHeaderEventArgs>(GroupHeaderRendering,this,ghea);
		}
			
		
		protected void FireGroupFooterRendering (GroupFooter groupFooter)
		{
			GroupFooterEventArgs gfea = new GroupFooterEventArgs(groupFooter);
			EventHelper.Raise<GroupFooterEventArgs>(GroupFooterRendering,this,gfea);
		}
		
		
		protected void FireSectionRendering (BaseSection section)
		{
			SectionRenderEventArgs srea = new SectionRenderEventArgs(section,
			                                                         this.SinglePage.PageNumber,
			                                                         this.DataNavigator.CurrentRow,
			                                                         section);
			EventHelper.Raise<SectionRenderEventArgs>(SectionRendering,this,srea);
		}
		
		#endregion
		
		
		protected  static ExporterCollection ConvertItems (ISimpleContainer row,Point offset)		                                          
		{

			IExportColumnBuilder exportLineBuilder = row as IExportColumnBuilder;

			if (exportLineBuilder != null) {
				ExportContainer exportContainer = ExportHelper.ConvertToContainer(row,offset);
				ExporterCollection list = ExportHelper.ConvertPlainCollection(row.Items,exportContainer.StyleDecorator.Location);
				exportContainer.Items.AddRange(list);
				ExporterCollection containerList = new ExporterCollection();
				containerList.Add (exportContainer);
				return containerList;
			}
			return null;
		}
		
		protected void DebugShowSections ()
		{
			Console.WriteLine("\treportheader {0}",SectionBounds.ReportHeaderRectangle);
			Console.WriteLine("\tpageheader {0}",SectionBounds.PageHeaderRectangle);
			Console.WriteLine("\tdetail {0}",SectionBounds.DetailArea);
		}
	
		#region Grouping
		
		protected void ConvertGroupFooter (ISimpleContainer container,ExporterCollection exporterCollection)
		{
            var footers = container.Items.FindGroupFooter();
			if (footers.Count > 0) {				
				Size rowSize = footers[0].Size;
				CurrentPosition = ConvertStandardRow(exporterCollection,(ISimpleContainer)footers[0]);
				FireGroupFooterRendering(footers[0]);
				footers[0].Size = rowSize;
			}
		}
		
		
		protected void PageBreakAfterGroupChange(BaseSection section,ExporterCollection exporterCollection)
		{
			if (CheckPageBreakAfterGroupChange(section) ) {
				
				if (DataNavigator.HasMoreData)
				{
					CurrentPosition = ForcePageBreak (exporterCollection,section);
				}
			}
		}
		
		
		private static bool CheckPageBreakAfterGroupChange(ISimpleContainer container)
		{
            var groupedRows = container.Items.FindGroupHeader();
			if (groupedRows.Count > 0) {
				var groupedRow = groupedRows[0];
				return groupedRow.PageBreakOnGroupChange;
			}
			return false;
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
			this.ParentRectangle = new Rectangle(parent.Location,parent.Size);
			return new ExporterCollection();;
		}
		
		
		
		#endregion
		
		public Point CurrentPosition {get;set;}
		
		public Rectangle ParentRectangle {get;private set;}
			
		public ISinglePage SinglePage {get;private set;}
		
		public SectionBounds SectionBounds
		{
			get 
			{
				return SinglePage.SectionBounds;
			}
		}
			
		public IDataNavigator DataNavigator {get;private set;}
			
		public ILayouter Layouter {get; private set;}
		
		public Graphics Graphics {get;set;}
		
		protected IExpressionEvaluatorFacade Evaluator{get;private set;}
		
		protected IReportModel ReportModel {get; private set;}
		
		protected int DefaultLeftPosition {get;set;}
		
		protected void  SaveSectionSize(Size size)
		{
			this.saveSize = size;
		}
		
		protected Size RestoreSectionSize
		{
			get {return this.saveSize;}
		}
		
		protected	void PrepareContainerForConverting(BaseSection section,ISimpleContainer simpleContainer)
		{
			Console.WriteLine("PrepareContainerForConverting");
			FireSectionRendering(section);
			LayoutHelper.SetLayoutForRow(Graphics,Layouter,simpleContainer);
		}
		
		protected  Point ConvertStandardRow(ExporterCollection mylist,ISimpleContainer simpleContainer)
		{
			Console.WriteLine("ConvertStandardRow");
			var rowSize = simpleContainer.Size;
			
			Point curPos = new Point(DefaultLeftPosition, CurrentPosition.Y);
			ExporterCollection ml = BaseConverter.ConvertItems (simpleContainer, curPos);
			EvaluationHelper.EvaluateRow(Evaluator,ml);
			
			mylist.AddRange(ml);
		
			curPos = new Point (DefaultLeftPosition,CurrentPosition.Y + simpleContainer.Size.Height + 2 * GlobalValues.GapBetweenContainer);
			simpleContainer.Size = rowSize;
			return curPos;
			
		}

		
		protected void AfterConverting (ExporterCollection convertedList)
		{
			EvaluationHelper.EvaluateRow(Evaluator,convertedList);
		}
		
		
		public static Point ConvertContainer(ExporterCollection myList,ISimpleContainer container,int leftPos,Point curPos)
		{
			ExporterCollection ml = BaseConverter.ConvertItems (container, curPos);		
			myList.AddRange(ml);
			return new Point (leftPos,curPos.Y + container.Size.Height);
		}
		
		
		protected static  void FillRow (ISimpleContainer row,IDataNavigator currentNavigator)
		{
			currentNavigator.Fill(row.Items);
		}
		
	}
}
