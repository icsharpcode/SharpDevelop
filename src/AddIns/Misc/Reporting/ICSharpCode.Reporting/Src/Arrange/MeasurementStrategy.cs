/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.04.2013
 * Time: 19:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Arrange
{
	/// <summary>
	/// Description of ArrangeStrategy.
	/// </summary>
	public interface IMeasurementStrategy
    {
        Size Measure(IExportColumn exportColumn,Graphics graphics);
    }


	class ContainerMeasurementStrategy:IMeasurementStrategy
	{
		
		public Size Measure(IExportColumn exportColumn,Graphics graphics)
		{
			var items = ((ExportContainer)exportColumn).ExportedItems;
			
			foreach (var element in items) {
				var tbi = element as IExportText;
				if (tbi != null) {
					element.DesiredSize = MeasurementService.Measure(tbi,graphics);
				}
				Console.WriteLine("Measure -> {0} - {1}",element.Size,element.DesiredSize);
			}
			return exportColumn.DesiredSize;
		}
	}
	
	internal class TextBasedMeasurementStrategy:IMeasurementStrategy
	{
		
		public Size Measure(IExportColumn exportColumn, Graphics graphics)
		{
			return MeasurementService.Measure((IExportText)exportColumn,graphics);
		}
	}
}
