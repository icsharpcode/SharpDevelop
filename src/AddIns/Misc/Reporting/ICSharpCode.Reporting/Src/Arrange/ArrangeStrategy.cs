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
		

		public void Arrange(IExportColumn exportColumn)
		{
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var container = exportColumn as IExportContainer;
			if ((container != null) && (container.ExportedItems.Count > 0)) {
				
				
				FindBiggestRectangle(container);
				var resizeable = from resize in container.ExportedItems
					where ((resize.CanGrow == true))
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
		
		private void FindBiggestRectangle (IExportContainer container)
		{
		    BiggestRectangle = Rectangle.Empty;
            /*
            foreach (var item in container.ExportedItems)
            {
                if (item.DesiredSize.Height > BiggestRectangle.Size.Height)
                {
                    BiggestRectangle = new Rectangle(new Point(container.Location.X + item.Location.X,
                                                               container.Location.Y + item.Location.Y)
                                                     , item.DesiredSize);
                }
            }
            */
		    foreach (var item in container.ExportedItems
                .Where(item => item.DesiredSize.Height > BiggestRectangle.Size.Height))
		    {
		        BiggestRectangle = new Rectangle(new Point(container.Location.X + item.Location.X,
		                                                   container.Location.Y + item.Location.Y)
		                                         ,item.DesiredSize);
		    }
		}

	    public Rectangle BiggestRectangle {get; private set;}
	}
	
}
