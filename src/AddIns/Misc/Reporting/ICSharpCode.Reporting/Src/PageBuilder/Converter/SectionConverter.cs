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
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.PageBuilder.Converter
{
	/// <summary>
	/// Description of SectionConverter.
	/// </summary>
	internal class SectionConverter
	{
		private Point currentPoint;
		private ExportColumnFactory factory;
		
		public SectionConverter(ISection section,Point currentPoint )
		{
			Section = section;
			this.currentPoint = currentPoint;
			factory = new ExportColumnFactory();
		}
		
		public List<IExportColumn> Convert(){
			var l = new List<IExportColumn>();
			foreach (var element in Section.Items) {
				
				var item = factory.CreateItem(element);
				l.Add(item);
			}
			
			return l;
		}
		
		public ISection Section {get; private set;}
	}
}
