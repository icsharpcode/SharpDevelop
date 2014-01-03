/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.05.2013
 * Time: 19:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.Arrange
{
	/// <summary>
	/// Description of ArrangeStrategy.
	/// </summary>
	/// 
	public interface IArrangeStrategy
	{
		void Arrange(IExportColumn exportColumn);
	}
	
	
	internal class ContainerArrangeStrategy:IArrangeStrategy
	{
		
		public void Arrange(IExportColumn exportColumn){
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var container = exportColumn as IExportContainer;
			if ((container != null) && (container.ExportedItems.Count > 0)) {
				List<IExportColumn> canGrowItems = CreateCanGrowList(container);
				if (canGrowItems.Count > 0) {
					var containerSize = ArrangeInternal(container);
					if (containerSize.Height > container.DesiredSize.Height) {
						container.DesiredSize = new Size(containerSize.Width,containerSize.Height);
					} 
				}
			}
		}

		
		static Size ArrangeInternal(IExportContainer container)
		{
			var containerRectangle = container.DisplayRectangle;
			Rectangle elementRectangle = Rectangle.Empty;
			
			foreach (var element in container.ExportedItems) {
				var con = element as IExportContainer;
				if (con != null) {
					var keep = containerRectangle;
					con.DesiredSize = ArrangeInternal(con);
					elementRectangle  = AdujstRectangles(keep,con.DisplayRectangle);
					containerRectangle = keep;
					
				} else {
					elementRectangle = AdujstRectangles(containerRectangle,element.DisplayRectangle);
				}
				
				if (!containerRectangle.Contains(elementRectangle)) {
					containerRectangle = Rectangle.Union(containerRectangle,elementRectangle);
				}
			}
			return containerRectangle.Size;
		}
		
		
		static Rectangle AdujstRectangles (Rectangle container,Rectangle element) {
			return new Rectangle(container.Left + element.Left,
			                     container.Top + element.Top,
			                     element.Size.Width,
			                     element.Size.Height);
		}
		
		
		static List<IExportColumn> CreateCanGrowList(IExportContainer container)
		{
			var l1 = new List<IExportColumn>();
			foreach (var element in container.Descendents()) {
				if (element.CanGrow) {
					l1.Add(element);
				}
			}
			return l1;
		}
	}
	
	
	static class Extensions {
		
		public static IEnumerable<IExportColumn> Descendents(this IExportContainer node) {
			if (node == null) throw new ArgumentNullException("node");
			if(node.ExportedItems.Count > 0) {
				foreach (var child in node.ExportedItems) {
					var cont = child as IExportContainer;
					if (cont != null) {
						foreach (var desc in Descendents(cont)) {
							yield return desc;
						}
					}
					yield return child;
				}
			}
		}
	}
}
