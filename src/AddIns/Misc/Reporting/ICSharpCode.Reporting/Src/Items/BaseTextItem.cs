/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 07.04.2013
 * Time: 18:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of BaseTextItem.
	/// </summary>
	public interface  ITextItem:IReportItem
	{
		Font Font {get;set;}
	}
	
	public class BaseTextItem:ReportItem,ITextItem
	{
		public BaseTextItem(){
		}
	
		public Font Font {get;set;}
		
		public override IExportColumn CreateExportColumn()
		{
			return new ExportText();
		}
	}
}
