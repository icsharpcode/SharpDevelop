/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.04.2013
 * Time: 20:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.Arrange;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of BaseExportContainer.
	/// </summary>
	public class ExportContainer:ExportColumn,IExportContainer,IAcceptor
	{
		public ExportContainer()
		{
			exportedItems = new List<IExportColumn>();
		}
		
		List<IExportColumn> exportedItems;
		
		public List<IExportColumn> ExportedItems {
			get { return exportedItems; }
		}
		
		public void Accept(IVisitor visitor)
		{
			visitor.Visit(this);
		}
		
		public override ICSharpCode.Reporting.Arrange.IArrangeStrategy GetArrangeStrategy()
		{
			return new ContainerArrangeStrategy();
		}
	}
}
