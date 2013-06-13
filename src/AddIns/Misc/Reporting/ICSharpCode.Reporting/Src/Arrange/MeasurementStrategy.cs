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
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Arrange
{
	/// <summary>
	/// Description of ArrangeStrategy.
	/// </summary>
	public interface IMeasurementStrategy
    {
        Size Measure(IPrintableObject reportItem,Graphics graphics);
    }


	internal class ContainerMeasurementStrategy:IMeasurementStrategy
	{
		public ContainerMeasurementStrategy()
		{
		}
		
		public Size Measure(IPrintableObject reportItem,Graphics graphics)
		{
			return reportItem.Size;
		}
	}
	
	internal class TextBasedMeasurementStrategy:IMeasurementStrategy
	{
		
		public Size Measure(IPrintableObject reportItem, Graphics graphics)
		{
			return MeasurementService.Measure((ITextItem)reportItem,graphics);
		}
	}
}
