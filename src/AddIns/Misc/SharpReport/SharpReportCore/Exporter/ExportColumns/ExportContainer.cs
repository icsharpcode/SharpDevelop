/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 09.10.2006
 * Time: 22:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace SharpReportCore.Exporters
{
	/// <summary>
	/// Description of ContainerItem.
	/// </summary>
	public class ExportContainer:BaseExportColumn
	{

		ExporterCollection<BaseExportColumn> items;
		
		#region Constructor
		
		public ExportContainer():base(){
			base.IsContainer = true;
		}
		
		public ExportContainer (BaseStyleDecorator itemStyle):base(itemStyle,true){
		}
		
		#endregion
		
		#region overrides
		
		public override void DrawItem(System.Drawing.Graphics graphics){
			base.DrawItem(graphics);
			base.Decorate(graphics);
		}
		
		#endregion
		
		public void AddLineItem (BaseExportColumn item) {
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			this.items.Add(item);
		}
		
		public ExporterCollection<BaseExportColumn> Items {
			get {
				if (this.items == null) {
					items = new ExporterCollection<BaseExportColumn>();
				}
				return items;
			}
		}
	}
}
