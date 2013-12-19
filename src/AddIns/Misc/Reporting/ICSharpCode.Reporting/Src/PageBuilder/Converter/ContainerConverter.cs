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
	class ContainerConverter : IContainerConverter
	{
		public ContainerConverter(Graphics graphics, Point currentLocation)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			Graphics = graphics;
			CurrentLocation = currentLocation;
		}


		public virtual IExportContainer ConvertToExportContainer(IReportContainer reportContainer)
		{
			var exportContainer = (ExportContainer)reportContainer.CreateExportColumn();
			exportContainer.Location = CurrentLocation;
			return exportContainer;
		}

		
		public List<IExportColumn> CreateConvertedList(List<IPrintableObject> items){                                    
			var itemsList = new List<IExportColumn>();
			foreach (var element in items) {
				var exportColumn = ExportColumnFactory.CreateItem(element);
				var ec = element as IReportContainer;
				if (ec != null) {
					var l = CreateConvertedList(ec.Items);
					((IExportContainer)exportColumn).ExportedItems.AddRange(l);
				}
				
				itemsList.Add(exportColumn);
			}
			return itemsList;
		}

		
		public void SetParent(IExportContainer parent, List<IExportColumn> convertedItems)
		{
			foreach (var item in convertedItems) {
				item.Parent = parent;
			}
		}
		

		protected Point CurrentLocation { get;  set; }
		
		internal Graphics Graphics {get;private set;}
	}
}
