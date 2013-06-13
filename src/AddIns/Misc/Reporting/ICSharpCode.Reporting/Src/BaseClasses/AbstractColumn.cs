/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2013
 * Time: 20:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Xml.Serialization;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of AbstractColumn.
	/// </summary>
	public  class AbstractColumn
	{
		private Type dataType;
		private string dataTypeName;
		
		public AbstractColumn() {
			this.dataType = typeof(System.String);
		}
		
		
		public AbstractColumn(string columnName, Type dataType){
			this.ColumnName = columnName;
			this.dataType = dataType;
		}
		
		public string ColumnName {get;set;}
		
		
		
		public string DataTypeName {
			get { 
				return this.dataType.ToString();
			}
			set { 
				dataTypeName = value;
				this.dataType = Type.GetType(dataTypeName,true,true);
			}
		}
		
		[XmlIgnoreAttribute]
		public Type DataType {
			get {
				return dataType;
			}
			set {
				dataType = value;
			}
		}
	
	}
}
