/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 26.09.2006
 * Time: 14:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace SharpReportCore.Exporters {
	/// <summary>
	/// Description of LineItem.
	/// </summary>
	public class ExportText :BaseExportColumn{
		string text;

		public ExportText():base(){
		}
		

		public ExportText (BaseStyleDecorator itemStyle,bool isContainer):base(itemStyle,isContainer){
			
		}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		public override BaseStyleDecorator StyleDecorator {
			get {
				return base.StyleDecorator;
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
