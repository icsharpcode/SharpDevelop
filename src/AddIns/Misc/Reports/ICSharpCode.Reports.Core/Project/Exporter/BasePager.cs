// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of BasePager.
	/// </summary>
	public class BasePager:IReportCreator
	{
		private PagesCollection pages;
		private readonly object pageLock = new object();

		public event EventHandler<PageCreatedEventArgs> PageCreated;
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		public event EventHandler<GroupHeaderEventArgs> GroupHeaderRendering;
		public event EventHandler<GroupFooterEventArgs> GroupFooterRendering;
		
		public event EventHandler<RowRenderEventArgs> RowRendering;
		
		#region Constructor
		
		public BasePager(IReportModel reportModel)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			
			this.ReportModel = reportModel;
			
			if (!ServiceContainer.Contains(typeof(ILayouter))) {
				ServiceContainer.AddService<ILayouter>(new Layouter());
			}
			
			this.Graphics = CreateGraphicObject.FromSize(this.ReportModel.ReportSettings.PageSize);
		}
		
		#endregion
		
		#region Create and Init new page
		
		
		protected ExporterPage InitNewPage ()
		{
			bool firstPage;
			this.ReportModel.ReportSettings.LeftMargin = this.ReportModel.ReportSettings.LeftMargin;
			if (this.Pages.Count == 0) {
				firstPage = true;
			} else {
				firstPage = false;
			}
			
			SectionBounds sectionBounds  = new SectionBounds (this.ReportModel.ReportSettings,firstPage);
			ExporterPage sp = ExporterPage.CreateInstance(sectionBounds,this.pages.Count + 1);
			return sp;
		}
		
		
		protected virtual void BuildNewPage ()
		{
			this.SinglePage = this.InitNewPage();
			PrintHelper.InitPage(this.SinglePage,this.ReportModel.ReportSettings);
			this.SinglePage.CalculatePageBounds(this.ReportModel);
		}
		
		#endregion
		
		
		#region Converters

		protected ExporterCollection ConvertSection (BaseSection section,int dataRow)
		{
			FireSectionRenderEvent (section ,dataRow);
			PrintHelper.AdjustParent(section,section.Items);
			PrintHelper.AdjustSectionLocation(section);
			var convertedSection = new ExporterCollection();
			Offset = new Point(section.Location.X,section.SectionOffset);
			Point startOffset = Offset;
			if (section.Items.Count > 0) {
				section.Items.SortByLocation();

				IExpressionEvaluatorFacade evaluator = EvaluationHelper.CreateEvaluator(this.SinglePage,this.SinglePage.IDataNavigator);

				Rectangle desiredRectangle = LayoutHelper.CalculateSectionLayout(this.Graphics,section);
				LayoutHelper.FixSectionLayout(desiredRectangle,section);
				
				GapList gapCalculator = new GapList();
				gapCalculator.CalculateGapList(section);
				int i = 0;
				
				foreach (BaseReportItem item in section.Items)
				{
					ISimpleContainer simpleContainer = item as ISimpleContainer;
					
					Offset = new Point(Offset.X,Offset.Y + gapCalculator.GapBetweenItems[i] );
					
					if (simpleContainer != null)
					{
						EvaluationHelper.EvaluateReportItems(evaluator,simpleContainer.Items);

						var layouter = (ILayouter)ServiceContainer.GetService(typeof(ILayouter));
						LayoutHelper.SetLayoutForRow(Graphics,layouter, simpleContainer);
						
						Offset = BaseConverter.ConvertContainer(convertedSection,simpleContainer,Offset.X,Offset);

						Rectangle rsec = new Rectangle(section.Location,section.Size);
						Rectangle ro = new Rectangle(section.Location,simpleContainer.Size);
						if (!rsec.Contains(ro)) {
							section.Size = new Size(section.Size.Width,Offset.Y - startOffset.Y );
						}
					}
					else
					{
						var converteditem = ExportHelper.ConvertLineItem(item,Offset);
						convertedSection.Add(converteditem);
					}
					i ++;
				}
				Offset = new Point (Offset.X,Offset.Y + gapCalculator.LastGap);
			}
			
			return convertedSection;
		}
		
		
		public static BaseRectangleItem CreateDebugItem (BaseReportItem item)
		{
			BaseRectangleItem debugRectangle = new BaseRectangleItem();
			debugRectangle = new BaseRectangleItem();
			debugRectangle.Location = new Point (0 ,0);
			debugRectangle.Size = new Size(item.Size.Width,item.Size.Height);
			debugRectangle.FrameColor = item.FrameColor;
			return debugRectangle;
		}
		
		#endregion
		
		
		#region Convertion
		
		protected virtual void BuildReportHeader ()
		{
		}
		
		protected virtual void BuildPageHeader ()
		{
		}
		
		protected virtual void BuildDetailInternal (BaseSection section)
		{
		}
		
		protected virtual void BuildPageFooter ()
		{
		}
		
		protected virtual void BuildReportFooter (Rectangle footerRectangle)
		{
		}
		
		
		public virtual void BuildExportList ()
		{
			this.Pages.Clear();
		}
		
		protected virtual void AddPage (ExporterPage page)
		{
		}
		
		#endregion
		
		
		#region After Converting, final step's
		
		protected  void FinishRendering (IDataNavigator dataNavigator)
		{
			if (this.Pages.Count == 0) {
				return;
			}
			
			IExpressionEvaluatorFacade evaluatorFacade = new ExpressionEvaluatorFacade(this.SinglePage);
			
			foreach (ExporterPage p in this.pages)
			{
				p.TotalPages = this.Pages.Count;
				p.IDataNavigator = dataNavigator;
				evaluatorFacade.SinglePage = p;
				EvaluateRecursive(evaluatorFacade,p.Items);
			}
		}
		
		
		private static void EvaluateRecursive (IExpressionEvaluatorFacade evaluatorFassade,ExporterCollection items)
		{
			foreach (BaseExportColumn exportColumn in items) {
				
				IExportContainer ec = exportColumn as IExportContainer;
				if (ec != null)
				{
					if (ec.Items.Count > 0) {
						EvaluateRecursive(evaluatorFassade,ec.Items);
					}
				}

				IReportExpression expressionItem = exportColumn as IReportExpression;
				if (expressionItem != null) {
					EvaluationHelper.EvaluateItem(evaluatorFassade,expressionItem);
				}
			}
		}
		
		
		#endregion
		
		
		#region Event's
		
		
		protected void FireSectionRenderEvent (BaseSection section,int currentRow)
		{
			SectionRenderEventArgs ea =
				new SectionRenderEventArgs(section,
				                           pages.Count,
				                           currentRow,
				                           section);
			
			EventHelper.Raise<SectionRenderEventArgs>(SectionRendering,this,ea);
		}
		
		
		protected void FireGroupHeaderEvent (GroupHeaderEventArgs ghea)
		{
			EventHelper.Raise<GroupHeaderEventArgs>(GroupHeaderRendering,this,ghea);
			
		}
		
		
		protected void FireGroupFooterEvent (GroupFooterEventArgs gfea)
		{
			
			EventHelper.Raise<GroupFooterEventArgs>(GroupFooterRendering,this,gfea);
		}
		
		
		protected void FireRowRenderEvent (RowRenderEventArgs rrea)
		{
			EventHelper.Raise<RowRenderEventArgs>(RowRendering,this,rrea);
		}
		
		protected void FirePageCreated(ExporterPage page)
		{
			EventHelper.Raise<PageCreatedEventArgs>(PageCreated,this,
			                                        new PageCreatedEventArgs(page));
		}
		#endregion
		
		
		protected void DebugShowSections ()
		{
			Console.WriteLine("\toffset {0}",Offset);
			Console.WriteLine("\treportheader {0}",SectionBounds.ReportHeaderRectangle);
			Console.WriteLine("\tpageheader {0}",SectionBounds.PageHeaderRectangle);
			Console.WriteLine("\tdetail {0}",SectionBounds.DetailArea);
		}
		
		#region Property's
		
		protected Point Offset {get;set;}
		
		protected Graphics Graphics {get; private set;}
		
		protected SectionBounds SectionBounds
		{
			get { return SinglePage.SectionBounds; }
		}
		
		public IReportModel ReportModel {get;set;}

		protected ExporterPage SinglePage {get;set;}
		
		public PagesCollection Pages
		{
			get {
				lock(pageLock) {
					if (this.pages == null) {
						this.pages = new PagesCollection();
					}
					return pages;
				}
			}
		}
		#endregion
	}
}
