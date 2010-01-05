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
			Rectangle rect = base.DrawingRectangle;
			
			base.FillBackground(rpea.PrintPageEventArgs.Graphics);
			shape.DrawShape (rpea.PrintPageEventArgs.Graphics,
			                 base.Baseline(),
			                 rect);
		}
		
		
		public override string ToString() {
			return "BaseRectangleItem";
		}
		
		
		#region System.IDisposable interface implementation
//		public override void Dispose() {
//			base.Dispose();
//			for (int i = 0; i < arrayList.Count;i ++ ) {
//				IComponent curObj = (IComponent)arrayList[i];
//            curObj.Dispose();
//			}
//			arrayList = null;
//		}
		#endregion
		
		
	}
}
