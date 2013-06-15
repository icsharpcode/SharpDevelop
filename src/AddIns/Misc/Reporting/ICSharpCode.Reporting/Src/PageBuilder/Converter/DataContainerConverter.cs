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
		
		
		public List<IExportColumn> Convert(IExportContainer container,Point position){
			var itemsList = CreateConvertedList(container,position);
			ArrangeContainer(container);
			return itemsList;	
		}
	}
}
