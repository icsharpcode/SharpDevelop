// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		private IDataNavigator dataNavigator;
		private BaseSection startSection;

		
		#region Constructor
		
		public BaseTableItem():base()
		{
		}
		
		#endregion
		
		#region IExportColumnBuilder
		
		public IBaseExportColumn CreateExportColumn()
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
			Rectangle rect = base.DisplayRectangle;
			style.Location = new Point(rect.Left,this.Location.Y);
			style.Size = this.Size;
			style.DrawBorder = this.DrawBorder;
			return style;
		}
		
		
		#endregion
		
		private void EvaluateRecursive (IExpressionEvaluatorFacade evaluatorFassade,ReportItemCollection items)
		{
			foreach (var item in items) {
				
				ISimpleContainer  ec = item as ISimpleContainer;
				if (ec != null)
				{
					if (ec.Items.Count > 0) {
						EvaluateRecursive(evaluatorFassade,ec.Items);
					}
				}
				BaseTextItem bt = item as BaseTextItem;
				if (bt != null) {
					bt.Text = evaluatorFassade.Evaluate(bt.Text);
				}
			}
		}
		
		
		#region overrides
		
		public override void Render(ReportPageEventArgs rpea)
		{
			base.Render (rpea);
		}
		
		
		
		public override string ToString(){
			return this.GetType().Name;
		}
		
		#endregion
		
		
		#region Interface implementation of 'ITableContainer'
		
		public void StartLayoutAt (BaseSection section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			this.startSection = section;
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
		
		#endregion
	}
}
