// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Reports.Expressions.ReportingLanguage;
using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core {
	/// <summary>
	/// Description of TableItem.
	/// </summary>
	public class BaseTableItem :BaseReportItem,ITableContainer,IExportColumnBuilder
	{
		
		private ReportItemCollection items;
		private SectionBounds sectionBounds;
		private IDataNavigator dataNavigator;
		private IExpressionEvaluatorFacade expressionEvaluatorFacade;
		private ISinglePage singlePage;
		private ILayouter layouter;
		
		
		#region Constructor
		
		public BaseTableItem():base()
		{
		}
		
		#endregion
		
		#region IExportColumnBuilder
		
		public ICSharpCode.Reports.Core.Exporter.BaseExportColumn CreateExportColumn()
		{
			BaseStyleDecorator st = this.CreateItemStyle();
			ExportContainer item = new ExportContainer(st);
			return item;
		}
		
		protected BaseStyleDecorator CreateItemStyle ()
		{
			BaseStyleDecorator style = new BaseStyleDecorator();
			style.BackColor = this.BackColor;
			style.ForeColor = this.ForeColor;
			Rectangle rect = base.DrawingRectangle;
			style.Location = new Point(rect.Left,this.Location.Y);
			style.Size = this.Size;
			style.DrawBorder = this.DrawBorder;
			return style;
		}
		
		
		#endregion
		
		private void EvaluateRecursive (IExpressionEvaluatorFacade evaluatorFassade,ReportItemCollection items)
		{
			foreach (BaseReportItem be in items) {
				
				IContainerItem  ec = be as IContainerItem;
				if (ec != null)
				{
					if (ec.Items.Count > 0) {
						EvaluateRecursive(evaluatorFassade,ec.Items);
					}
				}
				BaseTextItem bt = be as BaseTextItem;
				if (bt != null) {
					bt.Text = evaluatorFassade.Evaluate(bt.Text);
				}
			}
		}
		
		
		#region overrides
		
		public override void Render(ReportPageEventArgs rpea)
		{
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
//			
			Point saveLocation = this.Location;
			Point currentPosition = new Point(this.SectionBounds.DetailStart.X,rpea.LocationAfterDraw.Y);

			Point tableStart = currentPosition;
			base.Render(rpea);

			int defaultLeftPos = PrintHelper.DrawingAreaRelativeToParent(this.Parent,this).Left;
			this.Items.SortByLocation();

			foreach (BaseRowItem row in this.items)
			{
				if (row != null)
				{
					row.Parent = this;
					if (PrintHelper.IsTextOnlyRow(row) )
					{
						currentPosition = this.PrintRow (rpea,row,currentPosition,defaultLeftPos);
						this.Location = saveLocation;
					}
					else {
						do {
							if (AbstractRenderer.IsPageFull(new Rectangle(currentPosition,row.Size),sectionBounds)) {
								this.Location = saveLocation;
								AbstractRenderer.PageBreak(rpea);
								return;
							}
							currentPosition = this.PrintRow (rpea,row,currentPosition,defaultLeftPos);
							
						}
						while (this.dataNavigator.MoveNext());
						this.dataNavigator.Reset();
						this.dataNavigator.MoveNext();
					}
				}
			}
			if (this.DrawBorder) {
				Border border = new Border(new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
				border.DrawBorder(rpea.PrintPageEventArgs.Graphics,
				                  new Rectangle(this.Parent.Location.X,tableStart.Y,
				                                this.Parent.Size.Width,currentPosition.Y + 5));
			}
			rpea.LocationAfterDraw = new Point(rpea.LocationAfterDraw.X,rpea.LocationAfterDraw.Y + 20);
			base.NotifyAfterPrint (rpea.LocationAfterDraw);
		}
			
			
		private Point PrintRow (ReportPageEventArgs rpea,BaseRowItem row,Point drawAt, int leftX)
		{
			int extend = row.Size.Height - row.Items[0].Size.Height;
			Rectangle saveRec = new Rectangle (row.Location,row.Size);
			row.Location = new Point (leftX,drawAt.Y);
			
			this.dataNavigator.Fill(row.Items);
			EvaluateRecursive (this.expressionEvaluatorFacade,row.Items);
			
			PrintHelper.SetLayoutForRow(rpea.PrintPageEventArgs.Graphics,layouter,row);

			row.Render (rpea);
			
			Point retVal = new Point (leftX,drawAt.Y + row.Size.Height +10);
			//reset values
			row.Size = new Size(saveRec.Size.Width,saveRec.Size.Height);
			row.Location = saveRec.Location;
			
			return retVal;
		}
		
		
		public override string ToString(){
			return this.GetType().Name;
		}
		
		#endregion
		
		public SectionBounds SectionBounds{
			get {return this.sectionBounds;}
		}
			
			
			#region Interface implementation of 'ITableContainer'
			
			public void RenderTable (BaseReportItem parent,SectionBounds sectionBounds,ReportPageEventArgs rpea,ILayouter layouter)
			{
				
				this.sectionBounds = sectionBounds;
				this.Parent = parent;
				this.layouter = layouter;
				this.Render (rpea);
			}
			
			
			public ReportItemCollection Items {
				get {
					if (this.items == null) {
						this.items = new ReportItemCollection();
					}
					return items;
				}
			}

			
			public IDataNavigator DataNavigator {
				set { dataNavigator = value;
					if (this.dataNavigator.CurrentRow < 0 ) {
						this.dataNavigator.MoveNext();
					}
				}
			}
			
			
			public IExpressionEvaluatorFacade ExpressionEvaluatorFacade {
				set { this.expressionEvaluatorFacade = value; }
			}
			
			
			public ISinglePage SinglePage {
				set { singlePage = value; }
			}
			
			#endregion
		}
	}
