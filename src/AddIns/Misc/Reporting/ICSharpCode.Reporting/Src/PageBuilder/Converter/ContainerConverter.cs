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
using System.Linq;
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
		public ContainerConverter(Point currentLocation)
		{
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
	}
}
