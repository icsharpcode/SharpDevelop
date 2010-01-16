// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

using ICSharpCode.Reports.Core.Exporter;

/// <summary>
/// This Class is used for Databased items
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 22.08.2005 00:12:59
/// </remarks>
namespace ICSharpCode.Reports.Core {	
	public class BaseDataItem : BaseTextItem,IDataRenderer,IExportColumnBuilder				
	{
		
		private string columnName;
		private string baseTableName;
		private string dbValue;
//		private string dataType;
		private string nullValue;
		
		#region Constructor
		
		public BaseDataItem():base() 
		{
//			this.dataType = "System.String";
		}
		
		public BaseDataItem(string columnName):base()
		{
			this.columnName = columnName;
//			basataType = "System.String";
		}
		
		#endregion
		
		#region privates
				
		private string CheckForNullValue() 
		{
			if (String.IsNullOrEmpty(this.dbValue)) {
				if (String.IsNullOrEmpty(this.nullValue)) {
					return GlobalValues.UnboundName;
				} else
					return this.nullValue;
			}
			return this.dbValue;
		}
	
		#endregion
		
		#region IExportColumnBuilder  implementation
		
		public new BaseExportColumn  CreateExportColumn()
		{
			string toPrint = CheckForNullValue();
			TextStyleDecorator st = base.CreateItemStyle();
			ExportText item = new ExportText(st,false);
			item.Text = StandardFormatter.FormatOutput(toPrint,
			                              this.FormatString,
			                              base.DataType,
			                              this.nullValue);
			
			return item;
		}
		
		#endregion
		
		public override void Render(ReportPageEventArgs rpea) 
		{
			string toPrint = CheckForNullValue();
			base.Text = StandardFormatter.FormatOutput(toPrint,
			                              this.FormatString,
			                              base.DataType,
			                              this.nullValue);
			base.Render (rpea);
		}
		
		public override string ToString() 
		{
			return this.GetType().Name;
		}
		
		#region Properies
		
		[XmlIgnoreAttribute]
		[Browsable(false)]
		public virtual string DBValue 
		{
			get {
				return dbValue;
			}
			set {
				dbValue = value;
			}
		}
		
		
		public virtual string ColumnName 
		{
			get {
				if (String.IsNullOrEmpty(columnName)) {
					this.columnName = GlobalValues.UnboundName;
				}
				return columnName;
			}
			set {
				columnName = value;
				this.Text = this.columnName;
			}
		}
		
		///<summary>
		/// Mappingname to Datasource
		/// </summary>
		/// 
		
		[XmlIgnoreAttribute]
		[Browsable(true),Category("Databinding"),
		 Description("Mapping Name to DataTable")]
		public string MappingName 
		{
			get {
				return baseTableName + "." + columnName;
			} 
		}
		
		
		public string BaseTableName 
		{
			get {
				return baseTableName;
			}
			set {
				baseTableName = value;
			}
		}
		
		
		[Browsable(true),Category("Databinding"),Description("Display Value for empty Field")]
		public string NullValue {
			get {
				return nullValue;
			}
			set {
				nullValue = value;
			}
		}
		
		#endregion
		
	}
}
