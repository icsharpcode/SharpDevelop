/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 30.05.2008
 * Zeit: 16:19
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Data;
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
	
		internal static AvailableFieldsCollection AbstractColumnsFromDataSet(DataSet dataSet)
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
		
		
		internal static ReportItemCollection DataItemsFromDataSet (DataSet dataSet)
		{
			if (dataSet == null) {
				throw new ArgumentNullException("dataSet");
			}
		
			if (dataSet.Tables.Count > 1) {
				string s = String.Format(System.Globalization.CultureInfo.InvariantCulture,
				                         "AutoBuildFromDataSet : at this time no more than one table is allowed <{0}>",dataSet.Tables.Count);
				ICSharpCode.Core.MessageService.ShowError(s);
			}
			
			ReportItemCollection itemCol = new ReportItemCollection();
			
			foreach (DataTable tbl in dataSet.Tables) {
				foreach (DataColumn col  in tbl.Columns) {
					if (col.DataType == typeof(System.Byte[])) {
						ICSharpCode.Reports.Core.BaseImageItem rItem = new ICSharpCode.Reports.Core.BaseImageItem();
						rItem.ColumnName = col.ColumnName;
						rItem.BaseTableName = tbl.TableName;
						rItem.Name = col.ColumnName;
						rItem.ScaleImageToSize = false;
						itemCol.Add (rItem);
					} else {
						ICSharpCode.Reports.Core.BaseDataItem rItem = new ICSharpCode.Reports.Core.BaseDataItem();
						rItem.ColumnName = col.ColumnName;
						rItem.DBValue = col.ColumnName;
						rItem.BaseTableName = tbl.TableName;
						rItem.DataType = col.DataType.ToString();
						rItem.Name = col.ColumnName;
						rItem.Text = "=[" + col.ColumnName + "]";
						itemCol.Add (rItem);
					}
				}
			}
			return itemCol;
		}
		
		
		internal static ICSharpCode.Reports.Core.BaseTextItem CreateTextItem (string text)
		{
			ICSharpCode.Reports.Core.BaseTextItem textItem = new ICSharpCode.Reports.Core.BaseTextItem();
			textItem.Text = text;
			return textItem;
		}
	}
}
