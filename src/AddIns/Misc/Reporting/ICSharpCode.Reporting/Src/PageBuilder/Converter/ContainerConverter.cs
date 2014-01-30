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
