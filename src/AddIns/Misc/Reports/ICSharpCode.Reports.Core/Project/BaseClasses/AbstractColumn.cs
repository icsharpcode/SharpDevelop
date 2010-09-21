// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;	
using System.Xml;
using System.Xml.Serialization;	
/// <summary>
/// This class is used as an Abstract Class for all Columns like, AvailableFields,Sorting,Grouping etc.
/// 
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 26.05.2005 16:19:45
/// </remarks>
/// 
namespace ICSharpCode.Reports.Core {
	
	public  class AbstractColumn
	{
		private string columnName;
		private Type dataType;
		private string dataTypeName;
		
		public AbstractColumn() {
			this.dataType = typeof(System.String);
			this.columnName = string.Empty;
		}
		
		public AbstractColumn(string columnName){
			this.columnName = columnName;
			this.dataType = typeof(System.String);
		}
		
		public AbstractColumn(string columnName, Type dataType){
			this.columnName = columnName;
			this.dataType = dataType;
		}
		
		public string ColumnName {
			get {
				return columnName;
			}
			set {
				columnName = value;
			}
		}
		
		
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
