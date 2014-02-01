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
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Interfaces;

/// <summary>
///This class drwas a Circle
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 29.09.2005 11:54:19
/// </remarks>
namespace ICSharpCode.Reports.Core {	
	
	public class BaseCircleItem : BaseGraphicItem, IExportColumnBuilder,ISimpleContainer
	{
		private ReportItemCollection items;
		EllipseShape shape = new EllipseShape();
		
		#region Constructor
		
		public BaseCircleItem():base() 
		{
		}
		
		#endregion
		
		#region IExportColumnBuilder
		
		public IBaseExportColumn CreateExportColumn(){
			IGraphicStyleDecorator style = base.CreateItemStyle(this.shape);
			return new ExportGraphicContainer(style);
		}
	
		
		#endregion
		
		public override void Render(ReportPageEventArgs rpea) 
		{
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			base.Render (rpea);
			Rectangle rect = base.DisplayRectangle;
			
			shape.FillShape(rpea.PrintPageEventArgs.Graphics,
			                new SolidFillPattern(this.BackColor),
			                rect);
			
			shape.DrawShape (rpea.PrintPageEventArgs.Graphics,
			                 base.Baseline(),
			                 rect);
		}
		
			
		public ReportItemCollection Items {
			get {
				if (this.items == null) {
					this.items = new ReportItemCollection();
				}
				return this.items;
			}
		}
	}
}
