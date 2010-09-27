// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
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
		
	
		
		#region IBaseConverter
		
		public virtual ExporterCollection Convert(BaseReportItem parent, BaseReportItem item)
		{
			this.parentRectangle = new Rectangle(parent.Location,parent.Size);
			return new ExporterCollection();;
		}
		
		
		public Rectangle ParentRectangle {
			get { return parentRectangle; }
		}
		
		
		public ExporterPage SinglePage {
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
		#endregion
		
		protected void  SaveSize(Size size)
		{
			this.saveSize = size;
		}
		
		
		protected Size RestoreSize
		{
			get {return this.saveSize;}
		}
		
		
		protected IExpressionEvaluatorFacade Evaluator
		{
			get {return this.evaluator;}
		}
		
		
		protected void FillRow (ISimpleContainer row)
		{
			DataNavigator.Fill(row.Items);
		}
		
		
		
		protected	void PrepareContainerForConverting(BaseSection section,ISimpleContainer simpleContainer)
		{
			if (section != null) {
				FireSectionRendering(section);
			}
			LayoutRow(simpleContainer);
		}
		
		
		private void LayoutRow (ISimpleContainer row)
		{
			PrintHelper.SetLayoutForRow(Graphics,Layouter,row);
		}
		
	
		protected static Point BaseConvert(ExporterCollection myList,ISimpleContainer container,int leftPos,Point curPos)
		{
			ExporterCollection ml = BaseConverter.ConvertItems (container, curPos);		
			myList.AddRange(ml);
			return new Point (leftPos,curPos.Y + container.Size.Height + (3 *GlobalValues.GapBetweenContainer));
		}
	}
}
