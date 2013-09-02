/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.05.2013
 * Time: 19:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Linq;

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
		public ContainerArrangeStrategy()
		{
		}
		
		
		public void Arrange(IExportColumn exportColumn){
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var container = exportColumn as IExportContainer;
			if ((container != null) && (container.ExportedItems.Count > 0)) {
				var resizeable = from resize in container.ExportedItems
					where ((resize.CanGrow))
					select resize;
				if (resizeable.Any()) {
					
					//minimun Location
//					var minLocation  = (from p in container.ExportedItems orderby p.Location.Y select p).First();
					var maxLocation  = (from p in container.ExportedItems orderby p.Location.Y select p).Last();
					// maximum Size
					var maxBottom  = (from p in container.ExportedItems orderby p.DisplayRectangle.Bottom select p).Last();
					container.DesiredSize = new Size(container.Size.Width,maxLocation.Location.Y + maxBottom.DesiredSize.Height + 5);

				} else {
					container.DesiredSize = container.Size;
				}
			}
		}
		
		
		public void old_Arrange(IExportColumn exportColumn)
		{
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var container = exportColumn as IExportContainer;
			if ((container != null) && (container.ExportedItems.Count > 0)) {
				
				
				BiggestRectangle = FindBiggestRectangle(container);
				var resizeable = from resize in container.ExportedItems
					where ((resize.CanGrow))
					select resize;
               
				if (resizeable.Any()) {
					if (!BiggestRectangle.IsEmpty) {
						var containerRectangle = new Rectangle(container.Location,container.Size);
						var desiredRectangle = Rectangle.Union(containerRectangle,BiggestRectangle);
						container.DesiredSize = new Size(container.Size.Width,desiredRectangle.Size.Height + 5);
					}
				}
			}
		}
		
		
		
		private Rectangle FindBiggestRectangle (IExportContainer container)
		{
		    var rect = Rectangle.Empty;
          
		    foreach (var item in container.ExportedItems
                .Where(item => item.DesiredSize.Height > BiggestRectangle.Size.Height))
		    {
		        rect = new Rectangle(new Point(container.Location.X + item.Location.X,
		                                                   container.Location.Y + item.Location.Y)
		                                         ,item.DesiredSize);
		    }
           return rect;
		}

	    public Rectangle BiggestRectangle {get; private set;}
	}
	
}
