/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 26.09.2006
 * Time: 14:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace SharpReportCore.Exporters {
	/// <summary>
	/// Description of LineItem.
	/// </summary>
	public class ExportText :BaseExportColumn{
		string text;
		
		#region Constructors
		
		public ExportText():base(){
		}
		
		public ExportText (BaseStyleDecorator itemStyle,bool isContainer):base(itemStyle,isContainer){
			
		}
		
		#endregion
		
		#region overrides
		
		public override void DrawItem(Graphics graphics){
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			base.DrawItem(graphics);
			base.Decorate(graphics);
			
			TextDrawer.PaintString(graphics,
			                       this.text,this.StyleDecorator);
		}
		
		#endregion
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		public new TextStyleDecorator StyleDecorator {
			get {
				return base.StyleDecorator as TextStyleDecorator;
			}
			set {
				base.StyleDecorator = value;
			}
		}
		
		public override string ToString()
		{
			return this.text;
		}
		
	}
}
