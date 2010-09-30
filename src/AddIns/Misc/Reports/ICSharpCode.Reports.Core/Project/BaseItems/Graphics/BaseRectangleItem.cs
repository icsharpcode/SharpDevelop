// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Exporter;

/// <summary>
/// This class draws a Rectangle
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 29.09.2005 11:57:30
/// </remarks>
namespace ICSharpCode.Reports.Core {	
	public class BaseRectangleItem : BaseGraphicItem,IExportColumnBuilder {
		
		RectangleShape shape = new RectangleShape();
		
		#region Constructor
		
		public BaseRectangleItem() {
		}
		
		#endregion
		
		
		#region IExportColumnBuilder
		
		public BaseExportColumn CreateExportColumn(){
			IGraphicStyleDecorator style = base.CreateItemStyle(this.shape);
			ExportGraphic item = new ExportGraphic(style,false);
			return item as ExportGraphic;
		}
		
		#endregion
		
		public override void Render(ReportPageEventArgs rpea) {
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			base.Render(rpea);
			Rectangle rect = base.DisplayRectangle;
			
			StandardPrinter.FillBackground(rpea.PrintPageEventArgs.Graphics,this.BaseStyleDecorator);
			shape.DrawShape (rpea.PrintPageEventArgs.Graphics,
			                 base.Baseline(),
			                 rect);
		}
		
		
		public override string ToString() {
			return "BaseRectangleItem";
		}
		
	}
}
