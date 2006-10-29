/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier Peter
 * Datum: 29.10.2006
 * Zeit: 14:52
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace SharpReportCore.Exporters
{
	/// <summary>
	/// Description of GraphicStyleDecorator.
	/// </summary>
	public class GraphicStyleDecorator:BaseStyleDecorator
	{
		private BaseShape shape;
		private int thickness = 1;
		private DashStyle dashStyle = DashStyle.Solid;
		
		public GraphicStyleDecorator(BaseShape shape):base(){
			if (shape == null) {
				throw new ArgumentNullException("shape");
			}
			this.shape = shape;
		}
		
		
		public BaseShape Shape {
			get { return shape; }
			set { shape = value; }
		}
		public int Thickness {
			get { return thickness; }
			set { thickness = value; }
		}
		
		public DashStyle DashStyle {
			get { return dashStyle; }
			set { dashStyle = value; }
		}
		
		
	}
}
