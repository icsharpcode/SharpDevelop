/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 09.10.2006
 * Time: 09:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace SharpReportCore.Exporters
{
	/// <summary>
	/// Description of BaseLineItem.
	/// </summary>
	public class BaseExportColumn{
		
		BaseStyleDecorator styleDecorator;
		bool isContainer;
		
		#region Constructors
	
		public BaseExportColumn(){
			this.styleDecorator = new BaseStyleDecorator(Color.White,Color.Black);
		}
		
		
		public BaseExportColumn(BaseStyleDecorator itemStyle, bool isContainer)
		{
			this.styleDecorator = itemStyle;
			this.isContainer = isContainer;
		}
		
		#endregion
			
		
		public virtual void DrawItem (Graphics graphics) {
		}
		
		/// <summary>
		/// Fill the Background and draw a (Rectangle)Frame around the Control
		/// </summary>
		/// <param name="graphics"></param>
		
		protected virtual void Decorate (Graphics graphics) {
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			RectangleShape shape = new RectangleShape();
			this.FillShape(graphics,shape);
			this.DrawFrame(graphics,shape);
		}
		/// <summary>
		/// Draw the Backround <see cref="BaseStyleDecorator"></see>
		/// </summary>
		/// <param name="graphics">a vlid graphics object</param>
		/// <param name="shape">the shape to fill</param>
		protected virtual void FillShape (Graphics graphics,BaseShape shape) {
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (shape == null) {
				throw new ArgumentNullException("shape");
			}
			shape.FillShape(graphics,
			                new SolidFillPattern(this.styleDecorator.BackColor),
			                this.styleDecorator.DisplayRectangle);
		}
		
		
		private  void DrawFrame (Graphics graphics, BaseShape shape) {
			if (this.styleDecorator.DrawBorder) {
				shape.DrawShape (graphics,
				                 new BaseLine (this.styleDecorator.ForeColor,
				                               System.Drawing.Drawing2D.DashStyle.Solid,1),
				                 this.styleDecorator.DisplayRectangle);
			}
		}
		
		public virtual BaseStyleDecorator StyleDecorator {
			get {
				return styleDecorator;
			}
			set {
				this.styleDecorator = value;
			}
		}
		
		public bool IsContainer {
			get {
				return isContainer;
			}
			set {
				isContainer = value;
			}
		}
	}
}
