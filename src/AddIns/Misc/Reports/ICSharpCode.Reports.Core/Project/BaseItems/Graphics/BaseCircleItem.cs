// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;

/// <summary>
///This class drwas a Circle
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 29.09.2005 11:54:19
/// </remarks>
namespace ICSharpCode.Reports.Core {	
	
	public class BaseCircleItem : BaseGraphicItem, IExportColumnBuilder
	{
		EllipseShape shape = new EllipseShape();
		
		#region Constructor
		
		public BaseCircleItem():base() 
		{
		}
		
		#endregion
		
		#region IExportColumnBuilder
		
		public BaseExportColumn CreateExportColumn()
		{
			IGraphicStyleDecorator style = base.CreateItemStyle(this.shape);
			ExportGraphic item = new ExportGraphic(style,false);
			return item as ExportGraphic;
		}
		
		#endregion
		
		public override void Render(ReportPageEventArgs rpea) 
		{
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			base.Render (rpea);
			Rectangle rect = base.DrawingRectangle;
			
			shape.FillShape(rpea.PrintPageEventArgs.Graphics,
			                new SolidFillPattern(this.BackColor),
			                rect);
			
			shape.DrawShape (rpea.PrintPageEventArgs.Graphics,
			                 base.Baseline(),
			                 rect);
		}
	}
}
