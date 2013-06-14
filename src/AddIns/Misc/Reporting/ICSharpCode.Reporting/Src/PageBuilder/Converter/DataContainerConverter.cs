/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 13.06.2013
 * Time: 11:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Factories;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.PageBuilder.Converter
{
	/// <summary>
	/// Description of DataContainerConverter.
	/// </summary>

	internal class DataContainerConverter:ContainerConverter
	{
		
		CollectionSource collectionSource;
		
		public DataContainerConverter(Graphics graphics, IReportContainer reportContainer,
		                              Point currentLocation,CollectionSource collectionSource):base(graphics,reportContainer,currentLocation)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (reportContainer == null) {
				throw new ArgumentNullException("reportContainer");
			}
			this.collectionSource = collectionSource;
		}
		/*
		public override IExportContainer Convert(){
			if (collectionSource.Count == 0) {
				return base.Convert();
			}
	
			var exportContainer = CreateExportContainer();
			Console.WriteLine("");
			Console.WriteLine("start CurrentLocation {0}",CurrentLocation);
			var position = Point.Empty;
//			do {
//				FitOnPage(position);
//				collectionSource.Fill(Container.Items);
				var itemsList = CreateConvertedList(exportContainer,position);
				exportContainer.ExportedItems.AddRange(itemsList);
				position = new Point(Container.Location.X,position.Y + Container.Size.Height);
//			}
//			while (collectionSource.MoveNext());
			Console.WriteLine("end CurrentLocation {0}",CurrentLocation);
			Console.WriteLine("");
			ArrangeContainer(exportContainer);
			return exportContainer;
		}
		*/
		/*
		public IExportContainer  aaaConvert(List<IPrintableObject>list,Point position){
			var exportContainer = CreateExportContainer();
			Console.WriteLine("");
			Console.WriteLine("start CurrentLocation {0}",CurrentLocation);
			var itemsList = CreateConvertedList(exportContainer,position);
				exportContainer.ExportedItems.AddRange(itemsList);
			Console.WriteLine("end CurrentLocation {0}",CurrentLocation);
			Console.WriteLine("");
			ArrangeContainer(exportContainer);
			return exportContainer;	
		}
		*/
		
		public List<IExportColumn>  Convert(ExportContainer container,Point position){
//			Console.WriteLine("");
//			Console.WriteLine("start CurrentLocation {0}",CurrentLocation);
			var itemsList = CreateConvertedList(container,position);
//			Console.WriteLine("end CurrentLocation {0}",CurrentLocation);
//			Console.WriteLine("");
			return itemsList;	
		}
	}
}
