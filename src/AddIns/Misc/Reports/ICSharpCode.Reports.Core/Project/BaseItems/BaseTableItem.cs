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
