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
		
			if (section.Items.Count > 0) {
				section.Items.SortByLocation();

				IExpressionEvaluatorFacade evaluator = EvaluationHelper.CreateEvaluator(this.SinglePage,this.SinglePage.IDataNavigator);

				Rectangle desiredRectangle = LayoutHelper.CalculateSectionLayout(this.Graphics,section);
				LayoutHelper.FixSectionLayout(desiredRectangle,section);
				
				BaseReportItem oldItem = section.Items[0];
				
				int gap = oldItem.Location.Y;
				foreach (BaseReportItem item in section.Items)
				{
					
					ISimpleContainer simpleContainer = item as ISimpleContainer;
					gap = CalculateGap (oldItem,item);

					if (simpleContainer != null)
					{
						EvaluationHelper.EvaluateReportItems(evaluator,simpleContainer.Items);

						var layouter = (ILayouter)ServiceContainer.GetService(typeof(ILayouter));
						LayoutHelper.SetLayoutForRow(Graphics,layouter, simpleContainer);
						
						Offset = BaseConverter.ConvertContainer(convertedSection,simpleContainer,Offset.X,Offset);
						Offset = new Point(Offset.X,Offset.Y + gap);
					
						//section.Size = CalculatenewSectionSize
						Rectangle rsec = new Rectangle(section.Location,section.Size);
						Rectangle ro = new Rectangle(section.Location,simpleContainer.Size);
						if (!rsec.Contains(ro)) {
							
							Rectangle rnew = new Rectangle(section.Location.X,section.Location.Y,
							                               section.Size.Width , ro.Location.Y + ro.Size.Height +1);
							Console.WriteLine ("{0}",rsec);
							Console.WriteLine ("{0}",ro);
							Console.WriteLine ("{0}",rnew);
							Console.WriteLine(rnew.Contains(ro));
						}
						
					}
					else
					{
						Offset = new Point(Offset.X,Offset.Y  + gap);
						var converteditem = ExportHelper.ConvertLineItem(item,Offset);
						convertedSection.Add(converteditem);
					}
					oldItem = item;
				}
			}
			return convertedSection;
		}
		
/*		
		
//			protected ExporterCollection ConvertSection (BaseSection section,int dataRow)
		private ExporterCollection bbConvertSection (BaseSection section,int dataRow)
		{
			FireSectionRenderEvent (section ,dataRow);
			PrintHelper.AdjustParent(section,section.Items);
			
			var convertedSection = new ExporterCollection();
			Offset = new Point(section.Location.X,section.SectionOffset);
			
			if (section.Items.Count > 0) {
				section.Items.SortByLocation();

				IExpressionEvaluatorFacade evaluator = EvaluationHelper.CreateEvaluator(this.SinglePage,this.SinglePage.IDataNavigator);
				var layouter = (ILayouter)ServiceContainer.GetService(typeof(ILayouter));
				Console.WriteLine("start sec size {0}",section.Size);
				Rectangle desiredRectangle = LayoutHelper.CalculateSectionLayout(this.Graphics,section);
				LayoutHelper.FixSectionLayout(desiredRectangle,section);
				Console.WriteLine("after sectionlayout  sec size {0}",section.Size);
	
	Console.WriteLine(section.Name);
	BaseReportItem oi = section.Items[0];
	var rr = oi.Location.Y;
	
foreach (var element in section.Items)
{
	if (oi != element) {
		rr = CalculateGap(oi,element);
	}
	Console.WriteLine(rr);
	oi = element;
}			
	
	
	
				BaseReportItem oldItem = section.Items[0];
				int gap = oldItem.Location.Y;
				foreach (BaseReportItem item in section.Items)
				{
					ISimpleContainer simpleContainer = item as ISimpleContainer;
					gap = CalculateGap (oldItem,item);

					if (simpleContainer != null)
					{
						EvaluationHelper.EvaluateReportItems(evaluator,simpleContainer.Items);
					
						Offset  = new Point(Offset.X,Offset.Y + item.Size.Height + gap);
//						var layouter = (ILayouter)ServiceContainer.GetService(typeof(ILayouter));
						LayoutHelper.SetLayoutForRow(Graphics,layouter, simpleContainer);
					
//						Console.WriteLine("offset start {0}",Offset);
						
						http://stackoverflow.com/questions/4270541/how-can-i-determine-if-one-rectangle-is-completely-contained-within-another
						
						Rectangle r2 = new Rectangle(0,0,section.Size.Width,section.Size.Height);
//						Rectangle r2 = new Rectangle(section.Location,section.Size);
						Rectangle ro = new Rectangle(simpleContainer.Location,simpleContainer.Size);
						Console.WriteLine (r2.Contains(ro));
						Rectangle r3 = Rectangle.Union(r2,ro);
						Rectangle r4 = Rectangle.Union(ro,r2);
						ro.Intersect(r2);
						if (!r2.Contains(ro)) {
							/*section.Size = new Size (section.Size.Width,r3.Size.Height);	
						}
						
						
						ExportContainer exportContainer = ExportHelper.ConvertToContainer(simpleContainer,Offset);
//						Offset  = new Point(Offset.X,Offset.Y + item.Size.Height + gap);
						ExporterCollection exporterCollection = ExportHelper.ConvertPlainCollection(simpleContainer.Items,exportContainer.StyleDecorator.Location);
						exportContainer.Items.AddRange(exporterCollection);
						convertedSection.Add(exportContainer);
						
					}
					else
					{
						Offset = new Point(Offset.X,Offset.Y  + gap);
						var converteditem = ExportHelper.ConvertLineItem(item,Offset);
						convertedSection.Add(converteditem);
					}
					oldItem = item;
				}
			}
			Console.WriteLine("bbbb sec size {0} new size {1}",section.Size, new Size(section.Size.Width,Offset.Y));
			return convertedSection;
		}
		*/
		
		static int CalculateGap(BaseReportItem oldItem, BaseReportItem item)
		{
				var gap = item.Location.Y - (oldItem.Location.Y + oldItem.Size.Height) ;
						if (gap < 0) {
							gap = 0;
						}
				return gap;
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
			foreach (BaseExportColumn be in items) {
				
				IExportContainer ec = be as IExportContainer;
				if (ec != null)
				{
					if (ec.Items.Count > 0) {
						EvaluateRecursive(evaluatorFassade,ec.Items);
					}
				}
				/*
			IReportExpression expr = be as IReportExpression;
			if (expr != null)
			{
				if (EvaluationHelper.CanEvaluate(expr.Text)) {
						expr.Text = evaluatorFassade.Evaluate(expr.Text);
					}
			}
				 */
				
				ExportText et = be as ExportText;
				
				if ((et != null) && (!String.IsNullOrEmpty(et.Text))) {
					if (EvaluationHelper.CanEvaluate(et.Text)) {
						et.Text = evaluatorFassade.Evaluate(et.Text);
					}
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
