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
	internal class ContainerConverter
	{
		private Graphics graphics;
		
		public ContainerConverter(Graphics graphics,IReportContainer reportContainer,Point currentLocation )
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (reportContainer == null) {
				throw new ArgumentNullException("reportContainer");
			}
			
			this.graphics = graphics;
			Container = reportContainer;
			CurrentLocation = currentLocation;
		}
		
		
		public IExportContainer Convert() {
			var containerStrategy = Container.MeasurementStrategy ();
			var exportContainer = (ExportContainer)Container.CreateExportColumn();
	
			exportContainer.Location = CurrentLocation;
			exportContainer.DesiredSize = containerStrategy.Measure(Container,graphics);
			
			var itemsList = new List<IExportColumn>();
			
			foreach (var element in Container.Items) {
				var item = ExportColumnFactory.CreateItem(element);
				item.Parent = exportContainer;
				var measureStrategy = element.MeasurementStrategy();
				item.DesiredSize = measureStrategy.Measure(element,graphics);
				
				itemsList.Add(item);
				Console.WriteLine("Size {0} DesiredSize {1}",item.Size,item.DesiredSize);
			}
			exportContainer.ExportedItems.AddRange(itemsList);
			
			Console.WriteLine("calling Container-Arrange");
			var exportArrange = exportContainer.GetArrangeStrategy();
			exportArrange.Arrange(exportContainer);
			
			return exportContainer;
		}
			
		internal IReportContainer Container {get; private set;}
		
		internal Point CurrentLocation {get; private set;}
	}
}
