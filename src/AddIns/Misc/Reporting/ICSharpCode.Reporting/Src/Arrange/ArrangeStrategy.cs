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
			Console.WriteLine("Start arrange------");
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var container = exportColumn as IExportContainer;
			if ((container != null) && (container.ExportedItems.Count > 0)) {
				
				List<IExportColumn> canGrowItems = CreateCangrowList(container);
				if (canGrowItems.Count > 0) {
					var arSize = ArrangeInternal(container);
					Console.WriteLine("Ret from arrange {0}",arSize);
					container.DesiredSize = new Size(arSize.Width,arSize.Height);
				}
			}
			Console.WriteLine("End arrange------");
		}

		
		static List<IExportColumn> CreateCangrowList(IExportContainer container)
		{
			var l1 = new List<IExportColumn>();
			foreach (var element in container.Descendents()) {
				if (element.CanGrow) {
					l1.Add(element);
				}
			}
			return l1;
		}
		
		
		
		static Size ArrangeInternal(IExportContainer container)
		{
			var result = container.DisplayRectangle;
			Console.WriteLine();
			Console.WriteLine("enter arrange for <{0}> with {1}",container.Name,result);
			foreach (var element in container.ExportedItems) {
				var con = element as IExportContainer;
				if (con != null) {
					Console.WriteLine("recursive");
					con.DesiredSize = result.Size;
					ArrangeInternal(con);
				}
				var testRext = new Rectangle(element.DisplayRectangle.Left + result.Left,
				                             element.DisplayRectangle.Top + result.Top,
				                             element.DesiredSize.Width,
				                             element.DesiredSize.Height);
				
				if (!result.Contains(testRext)) {
					Console.WriteLine("No fit do arrange container {0} - elem {1}",result.Bottom,testRext.Bottom);
					Console.WriteLine("{0} - {1}",result.Bottom,testRext.Bottom);
					var r1 = Rectangle.Union(result,testRext);
					result = new Rectangle(result.Left,
					                       result.Top,
					                       container.DisplayRectangle.Width,
					                       element.DisplayRectangle.Size.Height);
					Console.WriteLine("Union {0}",r1);
					Console.WriteLine("{0} - {1}",result.Bottom,testRext.Bottom);
					container.DesiredSize = result.Size;
				} else {
					Console.WriteLine("Nothing to arrange {0} - {1}",result.Bottom,testRext.Bottom);
				}
			}
			return result.Size;
		}
		
		
		/*
		public void a_Arrange(IExportColumn exportColumn){
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var container = exportColumn as IExportContainer;
			if ((container != null) && (container.ExportedItems.Count > 0)) {
				var resizeable = from resize in container.ExportedItems
					where ((resize.CanGrow))
					select resize;
				if (resizeable.Any()) {
					
					//minimun Location
					var maxLocation  = (from p in container.ExportedItems orderby p.Location.Y select p).Last();
					// maximum Size
					var maxBottom  = (from p in container.ExportedItems orderby p.DisplayRectangle.Bottom select p).Last();
					container.DesiredSize = new Size(container.Size.Width,maxLocation.Location.Y + maxBottom.DesiredSize.Height + 5);

				} else {
					container.DesiredSize = container.Size;
				}
			}
		}
		 */
		
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
