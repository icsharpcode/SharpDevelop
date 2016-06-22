/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 04.04.2014
 * Time: 20:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using ICSharpCode.Reporting.Addin.Designer;
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	/// 
	[Designer(typeof(DataItemDesigner))]
	public class BaseDataItem:BaseTextItem
	{
		const string datatypeOfTheUnderlyingColumn = "Datatype of the underlying Column";
		const string tableName = "TableName";
		const string showIfColumnvalueIsEmpty = "Show if Column is empty";

		public BaseDataItem() {
			TypeDescriptor.AddProvider(new DataItemTypeProvider(), typeof(BaseDataItem));
		}
		
		
		[Category("Databinding"), Description(datatypeOfTheUnderlyingColumn)]
		public string ColumnName {get;set;}
		
		
		[Category("Databinding"), Description(showIfColumnvalueIsEmpty)]
		public string NullValue {get;set;}
	}
}
