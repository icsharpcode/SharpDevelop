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
using System.Data;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of WizardHelper.
	/// </summary>
	internal sealed class WizardHelper
	{
		private WizardHelper()
		{
		}
		
		///<summary>
		/// Build a <see cref="ColumnCollection"></see> this collection holds all the fields
		/// comming from the DataSource
		///</summary>
	
		internal static AvailableFieldsCollection AvailableFieldsCollection(DataSet dataSet)
		{
			if (dataSet == null) {
				throw new ArgumentNullException("dataSet");
			}
			
			AvailableFieldsCollection collection = new AvailableFieldsCollection();
			foreach (DataTable dataTable in dataSet.Tables) {
				foreach (DataColumn col in dataTable.Columns) {
					collection.Add (new AbstractColumn(col.ColumnName,col.DataType));
				}
			}
			return collection;
		}
		
		
		internal static ReportItemCollection CreateItemsCollection (DataSet resultDataSet,DataGridViewColumn[] displayCols)
		{
			ReportItemCollection sourceItems = WizardHelper.ReportItemCollection(resultDataSet);
			ReportItemCollection destItems = WizardHelper.ExtractSelectedItems(sourceItems, displayCols);
			return destItems;
		}
		
		
		internal static void SetupGridView(DataGridView grdQuery)
		{
			foreach (DataGridViewColumn dd in grdQuery.Columns)
			{			         			    
				DataGridViewColumnHeaderCheckBoxCell cb = new DataGridViewColumnHeaderCheckBoxCell();
				cb.CheckBoxAlignment = HorizontalAlignment.Left;
				cb.Checked = true;
				dd.HeaderCell = cb;
			}
		
		}
		
		
		internal static  ReportItemCollection ExtractSelectedItems(ReportItemCollection sourceItems,DataGridViewColumn[] displayCols)
		{
			var destItems = new ReportItemCollection();
			foreach (DataGridViewColumn cc in displayCols) {
				DataGridViewColumnHeaderCheckBoxCell hc = (DataGridViewColumnHeaderCheckBoxCell)cc.HeaderCell;
				if (hc.Checked) {
					BaseReportItem br = (BaseReportItem)sourceItems.Find(cc.HeaderText);
					destItems.Add(br);
				}
			}
			return destItems;
		}
		
		
		internal static ReportItemCollection ReportItemCollection (DataSet dataSet)
		{
			if (dataSet == null) {
				throw new ArgumentNullException("dataSet");
			}
		
			if (dataSet.Tables.Count > 1) {
				string s = String.Format(System.Globalization.CultureInfo.InvariantCulture,
				                         "AutoBuildFromDataSet : at this time no more than one table is allowed <{0}>",dataSet.Tables.Count);
				ICSharpCode.Core.MessageService.ShowError(s);
			}
			
			ReportItemCollection destItems = new ReportItemCollection();
			
			foreach (DataTable tbl in dataSet.Tables) {
				foreach (DataColumn col  in tbl.Columns)
				{
					if (col.DataType == typeof(System.Byte[]))
					{
						ICSharpCode.Reports.Core.BaseImageItem rItem = new ICSharpCode.Reports.Core.BaseImageItem();
						InitializeItem(rItem,col);
						rItem.ColumnName = col.ColumnName;
						rItem.ScaleImageToSize = false;
						destItems.Add (rItem);
					} else
					{
						ICSharpCode.Reports.Core.BaseDataItem rItem = new ICSharpCode.Reports.Core.BaseDataItem();
						InitializeItem(rItem,col);
						rItem.DBValue = col.ColumnName;
						rItem.Text = "=[" + col.ColumnName + "]";
						destItems.Add (rItem);
					}
				}
			}
			return destItems;
		}
		
		
		static void  InitializeItem (IDataItem rItem, DataColumn col)
		{
			rItem.ColumnName = col.ColumnName;
			rItem.Name = col.ColumnName;
			rItem.BaseTableName = col.Table.TableName;
			rItem.DataType = col.DataType.ToString();
		}
		
		
		
		internal static ICSharpCode.Reports.Core.BaseTextItem CreateTextItem (string text)
		{
			ICSharpCode.Reports.Core.BaseTextItem textItem = new ICSharpCode.Reports.Core.BaseTextItem();
			textItem.Text = text;
			return textItem;
		}
	}
}
