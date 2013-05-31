/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 29.05.2013
 * Time: 20:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reporting.Interfaces
{
	/// <summary>
	/// Description of IDataItem.
	/// </summary>
	public interface IDataItem
	{
		
		string ColumnName { get; set; }
//		string MappingName { get; }
//		string BaseTableName { get; set; }
		string DBValue {get;set;}
		string Name {get;set;}
		string DataType {get;set;}
	}
}
