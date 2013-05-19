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
			var e = exportColumn as IExportContainer;
			if ((e != null) && (e.ExportedItems.Count > 0)) {
				
				Console.WriteLine("Arrange  Container {0} ",e.Name);
				Console.WriteLine("Container-Size {0}",e.Size);

				BiggestRectangle = Rectangle.Empty;
				
				foreach (var element in e.ExportedItems) {
					if (element.Size.Height > BiggestRectangle.Size.Height) {
						BiggestRectangle = new Rectangle(element.Location,element.Size);
					}
				}
				if (!BiggestRectangle.IsEmpty) {
					Console.WriteLine("BiggestRectangle {0}",BiggestRectangle.ToString());
					var r = Rectangle.Union(new Rectangle(e.Location,e.Size),BiggestRectangle);
					Console.WriteLine("Sorrounding {0}",r.ToString());
					e.DesiredSize = new Size(e.Size.Width,BiggestRectangle.Bottom + 2);
					Console.WriteLine("Container-Desired_siz {0}",e.DesiredSize);
					Console.WriteLine("new rect {0}",new Rectangle(e.Location,e.DesiredSize));
				}
			}
		}
		
		public Rectangle BiggestRectangle {get; private set;}
	}
	
}
