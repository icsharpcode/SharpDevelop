/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.04.2013
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.Arrange;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of ReportContainer.
	/// </summary>
	public class ReportContainer:PrintableItem,IReportContainer
	{
		
		public ReportContainer()
		{
			items = new List<IPrintableObject>();
		}
		
		private List<IPrintableObject> items;
		
		public List<IPrintableObject> Items {
			get { return items; }
		}

		
		public override IExportColumn CreateExportColumn()
		{
			return new ExportContainer(){
			Name = this.Name,
			Size = this.Size,
			Location = this.Location,
			CanGrow = this.CanGrow,
			};
		}
		
		
		public override IMeasurementStrategy MeasurementStrategy()
		{
			return new ContainerMeasurementStrategy();
		}
	}
}
