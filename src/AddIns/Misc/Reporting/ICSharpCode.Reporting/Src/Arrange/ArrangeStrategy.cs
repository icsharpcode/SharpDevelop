/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.05.2013
 * Time: 19:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.Arrange
{
	/// <summary>
	/// Description of ArrangeStrategy.
	/// </summary>
	/// 
	public interface IArrangeStrategy
    {
        void Arrange(IExportColumn exportColumn);
    }
	
	
	internal class ContainerArrangeStrategy:IArrangeStrategy
	{
		public ContainerArrangeStrategy()
		{
		}
		

		public void Arrange(IExportColumn exportColumn)
		{
			var e = exportColumn as IExportContainer;
			if (e != null) {
				if ((e != null) && (e.ExportedItems != null)) {
				}
				Console.WriteLine("Arrange  Container with {0} items",e.ExportedItems.Count);
			} else {
				throw new ArgumentException("No Container");
			}
		}
	}
	
}
