/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.04.2013
 * Time: 19:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Arrange
{
	/// <summary>
	/// Description of ArrangeStrategy.
	/// </summary>
	 public interface IArrangeStrategy
    {
        void Arrange(IPrintableObject reportItem);
    }

	
	public class ContainerArrangeStrategy:IArrangeStrategy
	{
		public ContainerArrangeStrategy()
		{
		}
		
		public void Arrange(IPrintableObject reportItem)
		{
			Console.WriteLine("Arrange {0}",reportItem.Name);
		}
	}
}
