/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 10.10.2010
 * Time: 17:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of RendererFactory.
	/// </summary>
	public class PrintRendererFactory
	{
		  
		public static IBaseRenderer  CreateRenderer (BaseReportItem itemToConvert,IDataNavigator dataNavigator,
		                                   ISinglePage singlePage,ILayouter layouter,BaseSection section)
		{

			Type t = itemToConvert.GetType();
			if (t.Equals(typeof(BaseTableItem))) {
				Console.WriteLine("render Table");
				return new RenderTable(dataNavigator,Rectangle.Empty,singlePage,layouter,section);
			}
			
			if (t.Equals(typeof(BaseRowItem))) {
//				return new GroupedRowConverter (dataNavigator,singlePage,layouter);
				Console.WriteLine("render Row");
			}
			return null;
		}
	}
}
