/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 29.05.2013
 * Time: 20:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of BaseDataItem.
	/// </summary>
	public class BaseDataItem:BaseTextItem,IDataItem
	{
		public BaseDataItem()
		{
		}
		
		public override  IExportColumn CreateExportColumn()
		{
			var exCol = (IExportText)base.CreateExportColumn();
			exCol.Text = DBValue;
			return exCol;
		}
	
		
		public virtual string DBValue {get;set;}
			
		public virtual string ColumnName {get;set;}
		
//		public string DataType {get;set;}
		
	}
}
