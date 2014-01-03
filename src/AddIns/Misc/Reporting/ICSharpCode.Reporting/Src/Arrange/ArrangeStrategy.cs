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
			foreach (var element in container.ExportedItems) {
				var elementRectangle = new Rectangle(element.DisplayRectangle.Left + containerRectangle.Left,
				                             element.DisplayRectangle.Top + containerRectangle.Top,
				                             element.DesiredSize.Width,
				                             element.DesiredSize.Height);
				if (!containerRectangle.Contains(elementRectangle)) {
					containerRectangle = Rectangle.Union(containerRectangle,elementRectangle);
				}
			}
			return containerRectangle.Size;
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
		
		
		static Size old_ArrangeInternal(IExportContainer container)
		{
			var result = container.DisplayRectangle;
			Console.WriteLine();
			Console.WriteLine("enter arrange for <{0}> with {1}",container.Name,result);
			if (container.Name.Contains("Det")) {
			    	Console.WriteLine(container.Name);
			}
			foreach (var element in container.ExportedItems) {
				var con = element as IExportContainer;
				if (con != null) {
					Console.WriteLine("recursive");
//					con.DesiredSize = result.Size;
					ArrangeInternal(con);
				}
				var testRext = new Rectangle(element.DisplayRectangle.Left + result.Left,
				                             element.DisplayRectangle.Top + result.Top,
				                             element.DesiredSize.Width,
				                             element.DesiredSize.Height);
				Console.WriteLine("<<<<<<<{0}",element.DisplayRectangle);
				if (!result.Contains(testRext)) {
//					Console.WriteLine("No fit do arrange container  {0} - elem {1}",result.Bottom,testRext.Bottom);
//					Console.WriteLine("{0} - {1}",result.Bottom,testRext.Bottom);
					var r1 = Rectangle.Union(result,testRext);
					result = new Rectangle(result.Left,
					                       result.Top,
					                       container.DisplayRectangle.Width,
					                       element.DisplayRectangle.Size.Height);
					Console.WriteLine("Union {0}",r1);
					Console.WriteLine("{0} - {1}",result.Bottom,testRext.Bottom);
					result = r1;
//					container.DesiredSize = result.Size;
//					container.DesiredSize = r1.Size;
				} else {
					Console.WriteLine("Nothing to arrange {0} - {1}",result.Bottom,testRext.Bottom);
				}
			}
			Console.WriteLine("Retval for {0} - {1}",container.Name,result);
			return result.Size;
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
