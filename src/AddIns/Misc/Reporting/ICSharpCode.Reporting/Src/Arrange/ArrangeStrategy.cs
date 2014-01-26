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
