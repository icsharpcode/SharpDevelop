// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Exporter;

/// <summary>
/// This Class is used for Databased items
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 22.08.2005 00:12:59
/// </remarks>
namespace ICSharpCode.Reports.Core
{
	public class BaseDataItem : BaseTextItem, IExportColumnBuilder, IDataItem
	{

		#region Constructor

		public BaseDataItem() : base()
		{
		}

		public BaseDataItem(string columnName) : base()
		{
			this.ColumnName = columnName;
		}

		#endregion

		
		#region IExportColumnBuilder  implementation

		public new IBaseExportColumn CreateExportColumn()
		{
			TextStyleDecorator st = base.CreateItemStyle();
			ExportText item = new ExportText(st);
			item.Text = StandardFormatter.FormatOutput(DBValue, this.FormatString, base.DataType, this.NullValue);
			return item;
		}

		#endregion

		public override void Render(ReportPageEventArgs rpea)
		{
			base.Text = StandardFormatter.FormatOutput(DBValue, this.FormatString, base.DataType, this.NullValue);
			base.Render(rpea);
		}

		public override string ToString()
		{
			return this.GetType().Name;
		}

		#region Properies

		[XmlIgnoreAttribute()]
		[Browsable(false)]
		public virtual string DBValue {get;set;}
			
		public virtual string ColumnName {get;set;}
			
		///<summary>
		/// Mappingname to Datasource
		/// </summary>
		/// 

		[XmlIgnoreAttribute()]
		[Browsable(true), Category("Databinding"), Description("Mapping Name to DataTable")]
		public string MappingName {
			get { return BaseTableName + "." + this.ColumnName; }
		}


		public string BaseTableName {get;set;}
		

		[Browsable(true),
		 Category("Databinding"),
		 Description("Display Value for empty Field")]
		public string NullValue {get;set;}
		

		#endregion

	}
}
