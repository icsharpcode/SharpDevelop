/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier Helmut
 * Datum: 27.10.2006
 * Zeit: 22:43
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;

namespace SharpReportCore.Exporters
{
	/// <summary>
	/// Description of ExportGraphic.
	/// </summary>
	public class ExportGraphic:BaseExportColumn
	{
		public ExportGraphic():base()
		{
		}
		
		public ExportGraphic (BaseStyleDecorator itemStyle,bool isContainer):base(itemStyle,isContainer){
			
		}
		
		public override void DrawItem(Graphics graphics){
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			base.DrawItem(graphics);
			
			GraphicStyleDecorator style = base.StyleDecorator as GraphicStyleDecorator;
			if (style != null) {
				base.FillShape(graphics,style.Shape);
				style.Shape.DrawShape(graphics,
				                      new BaseLine (style.ForeColor,style.DashStyle,style.Thickness),
				                      style.DisplayRectangle);
			}
		}
		
	
		
		public new GraphicStyleDecorator StyleDecorator {
			get{
				return base.StyleDecorator as GraphicStyleDecorator;
			}
		}

	}
}
