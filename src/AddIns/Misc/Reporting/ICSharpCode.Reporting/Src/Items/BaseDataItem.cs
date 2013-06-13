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
		
		public virtual string DBValue {get;set;}
			
		public virtual string ColumnName {get;set;}
		
		public string DataType {get;set;}
		
	}
}
