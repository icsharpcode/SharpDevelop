// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
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
	public interface IArrangeStrategy{
		void Arrange(IExportColumn exportColumn);
	}
	
	
	class ContainerArrangeStrategy:IArrangeStrategy{
		
		public void Arrange(IExportColumn exportColumn){
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var container = exportColumn as IExportContainer;
			if ((container != null) && (container.ExportedItems.Any())) {
				List<IExportColumn> canGrowItems = CreateCanGrowList(container);
				if (canGrowItems.Any()) {
					var containerSize = ArrangeInternal(container);
					if (containerSize.Height > container.DesiredSize.Height) {
						container.DesiredSize = new Size(containerSize.Width,containerSize.Height + 15);
					} 
				}
			}
			
			
			var fixedElements = container.ExportedItems.Where(x => !x.CanGrow);
			var growables = container.ExportedItems.Where(x => x.CanGrow);
			
			foreach (var growable in growables) {
				var r = new Rectangle(growable.Location,growable.DesiredSize);
				foreach (var x in fixedElements) {
					var xr = new Rectangle(x.Location,x.DesiredSize);
					if (r.IntersectsWith(xr)) {
						x.Location = new Point(x.Location.X, r.Bottom + 5);
					}
				}
			}
		}

	
		static Size ArrangeInternal(IExportContainer container){
		
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
					
					containerRectangle = new Rectangle(containerRectangle.Left,
					                                   containerRectangle.Top ,
					                                   containerRectangle.Width,
					                                   element.Location.Y + elementRectangle.Size.Height + 5);					                            
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
		
		
		static List<IExportColumn> CreateCanGrowList(IExportContainer container){
			var canGrowList = new List<IExportColumn>();
			foreach (var element in container.Descendents()) {
				if (element.CanGrow) {
					canGrowList.Add(element);
				}
			}
			return canGrowList;
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
