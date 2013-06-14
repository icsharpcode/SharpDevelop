/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.04.2013
 * Time: 19:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.Reporting.Factories;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.PageBuilder.Converter
{
	/// <summary>
	/// Description of SectionConverter.
	/// </summary>
	internal class ContainerConverter : IContainerConverter
	{
		
		public ContainerConverter(Graphics graphics, IReportContainer reportContainer, Point currentLocation)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (reportContainer == null) {
				throw new ArgumentNullException("reportContainer");
			}

			Graphics = graphics;
			Container = reportContainer;
			CurrentLocation = currentLocation;
		}


		public virtual IExportContainer Convert()
		{
			
			var exportContainer = CreateExportContainer();
			
			var itemsList = CreateConvertedList(exportContainer,Point.Empty);
			
			exportContainer.ExportedItems.AddRange(itemsList);

			
			ArrangeContainer(exportContainer);

			return exportContainer;
		}

	
		protected ExportContainer CreateExportContainer()
		{
			var exportContainer = (ExportContainer)Container.CreateExportColumn();
			exportContainer.Location = CurrentLocation;
			exportContainer.DesiredSize = Measure(Container);
			return exportContainer;
		}

		
		protected List<IExportColumn> CreateConvertedList(ExportContainer exportContainer,Point position)
		{
			var itemsList = new List<IExportColumn>();
			foreach (var element in Container.Items) {
				var exportColumn = ExportColumnFactory.CreateItem(element);
				exportColumn.Parent = exportContainer;
				exportColumn.Location = new Point(element.Location.X,element.Location.Y + position.Y);
				exportColumn.DesiredSize = Measure(element);
				itemsList.Add(exportColumn);
				Console.WriteLine("Size {0} DesiredSize {1}", exportColumn.Size, exportColumn.DesiredSize);
			}
			return itemsList;
		}

		
		Size Measure(IPrintableObject element)
		{
			var measureStrategy = element.MeasurementStrategy();
			return measureStrategy.Measure(element, Graphics);
		}

		protected void ArrangeContainer(ExportContainer exportContainer)
		{
			Console.WriteLine("calling Container-Arrange");
			var exportArrange = exportContainer.GetArrangeStrategy();
			exportArrange.Arrange(exportContainer);
		}
		
		
		internal IReportContainer Container { get; private set; }

		protected Point CurrentLocation { get;  set; }
		
		internal Graphics Graphics {get;private set;}
	}
}
