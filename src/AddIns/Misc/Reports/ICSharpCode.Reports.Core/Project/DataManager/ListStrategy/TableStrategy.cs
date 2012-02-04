// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Data;

using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.ListStrategy
{
	/// <summary>
	/// Description of TableStrategy.
	/// </summary>

	internal class TableStrategy: BaseListStrategy
	{
		private DataTable table;
		
		public TableStrategy(DataTable table,ReportSettings reportSettings):base(reportSettings)
		{
			if (table == null) {
				throw new ArgumentNullException("table");
			}
			this.table = table;
			
		}
		
		#region Methods
		
		public override void Bind()
		{
			base.Bind();
			if (base.ReportSettings.GroupColumnsCollection.Count > 0) {
				this.Group();
			} else {
				this.Sort ();
			}
			Reset();
		}
		
		
		public override void Fill(int position,ReportItemCollection collection)
		{
			DataRow row = this.table.Rows[position];
			foreach (var item in collection) {
				IDataItem dataItem = item as IDataItem;
				if (dataItem != null) {
					FillInternal (row,dataItem);
				}
			}
		}
		
		
		public override void Fill(IDataItem item)
		{
			DataRow row = this.Current as DataRow;
			if (row != null)
			{
				this.FillInternal (row,item);
			}
		}
		
		
		void FillInternal (DataRow row,IDataItem item)
		{
			if (item != null)
			{
				BaseImageItem bi = item as BaseImageItem;
				if (bi != null) {
					using (System.IO.MemoryStream memStream = new System.IO.MemoryStream()){
						Byte[] val = row[bi.ColumnName] as Byte[];
						if (val != null) {
							if ((val[78] == 66) && (val[79] == 77)){
								memStream.Write(val, 78, val.Length - 78);
							} else {
								memStream.Write(val, 0, val.Length);
							}
							System.Drawing.Image image = System.Drawing.Image.FromStream(memStream);
							bi.Image = image;
						}
					}
				}
				else
				{
					var dataItem = item as BaseDataItem;
					if (dataItem != null) {
						dataItem.DBValue = ExtractDBValue(row,dataItem).ToString();
					}
					return;
				}
			}
		}
		
		
		object ExtractDBValue(DataRow row,BaseDataItem item)
		{
			if (EvaluationHelper.CanEvaluate(item.Expression)) {
				return ExtractFromExpression(item.Expression, row);
			}
			else
			{
				return DBValueFromRow(row, item.ColumnName);
			}
		}
		
		
		public override CurrentItemsCollection FillDataRow(int pos)
		{
			DataRow row = (DataRow) CurrentFromPosition(pos);
			return FillCurrentRow(row);
		}
		
		
		public override CurrentItemsCollection FillDataRow()
		{
			DataRow row =this.Current as DataRow;
			return FillCurrentRow(row);
		}
		
		
		CurrentItemsCollection FillCurrentRow( DataRow row)
		{
			CurrentItemsCollection ci = new CurrentItemsCollection();
			if (row != null) {
				CurrentItem c = null;
				foreach (DataColumn dc in table.Columns) {
					c = new CurrentItem(dc.ColumnName,dc.DataType);
					c.Value = row[dc.ColumnName];
					ci.Add(c);
				}
			}
			return ci;
		}
		
		
		public override bool MoveNext()
		{
			return base.MoveNext();
		}
		
		
		public override void Reset()
		{
			base.Reset();
		}
		
		
		public override void Sort()
		{
			base.Sort();
			if ((base.ReportSettings.SortColumnsCollection != null)) {
				if (base.ReportSettings.SortColumnsCollection.Count > 0) {
					base.IndexList = this.BuildSortIndex (ReportSettings.SortColumnsCollection);
				} else {
					// if we have no sorting, we build the indexlist as well
					base.IndexList = this.UnsortedIndex(ReportSettings.SortColumnsCollection);
				}
			}
		}
		
		
		private IndexList UnsortedIndex(SortColumnCollection col)
		{
			IndexList arrayList = new IndexList();
			for (int rowIndex = 0; rowIndex < this.table.Rows.Count; rowIndex++){
				object[] values = new object[1];
				arrayList.Add(new SortComparer(col, rowIndex, values));
			}
			return arrayList;
		}
		
		
		public override void Group ()
		{
			base.Group();
//			if (ReportSettings.SortColumnsCollection.Count > 0) {
//				ReportSettings.GroupColumnsCollection.AddRange(ReportSettings.SortColumnsCollection);
//			}
			IndexList groupedIndexList = new IndexList("group");
			groupedIndexList = this.BuildSortIndex (ReportSettings.GroupColumnsCollection);
//			ShowIndexList(sortedIndexList);
			BuildGroup(groupedIndexList);
		}
		
		#endregion
		
		
		private IndexList  BuildSortIndex(ColumnCollection col)
		{
			IndexList arrayList = new IndexList();
			
			for (int rowIndex = 0; rowIndex < this.table.Rows.Count; rowIndex++){
				DataRow rowItem = this.table.Rows[rowIndex];
				object[] values = new object[col.Count];
				for (int criteriaIndex = 0; criteriaIndex < col.Count; criteriaIndex++)
				{
					object value = ExtractColumnValue(rowItem,col,criteriaIndex);
					
					if (value != null && value != DBNull.Value)
					{
						values[criteriaIndex] = value;
					}   else {
						values[criteriaIndex] = DBNull.Value;
					}
				}
				
				arrayList.Add(new SortComparer(col, rowIndex, values));
			}
			
			if (arrayList[0].ObjectArray.GetLength(0) == 1) {
				List<BaseComparer> lbc = BaseListStrategy.GenericSorter (arrayList);
				arrayList.Clear();
				arrayList.AddRange(lbc);
			}
			else {
				arrayList.Sort();
			}
			return arrayList;
		}
		
		
		object  ExtractColumnValue(DataRow row,ColumnCollection col, int criteriaIndex)
		{
			AbstractColumn c = (AbstractColumn)col[criteriaIndex];
			object val = null;
			val =  DBValueFromRow(row,c.ColumnName);
			
//			if (!(val is IComparable)){
//				throw new InvalidOperationException(val.ToString());
//			}
			return val;
		}
		
		
		static string  CleanupColumnName(string colName)
		{
			if (colName.StartsWith("=[",StringComparison.InvariantCulture)) {
				return colName.Substring(2, colName.Length - 3);
			}
			return colName;
		}
		
		
		object DBValueFromRow(DataRow row, string colName)
		{
			int pos  =  colName.IndexOf("!",StringComparison.InvariantCultureIgnoreCase);
			if (pos > 0)
			{
				return ExtractFromExpression(colName,row);
					
			} else {
				var expression = CleanupColumnName(colName);
				return row[expression];
			}
		}
		
		
		object ExtractFromExpression(string expression, DataRow row)
		{
			var v = base.ExpressionEvaluator.Evaluate(expression, row);
			return v;
		}
		

		public override object CurrentFromPosition (int pos)
		{
			return this.table.Rows[pos];
		}
		
		
		
		#region Propertys
		
		public override AvailableFieldsCollection AvailableFields {
			get {
				base.AvailableFields.Clear();
				for (int i = 0;i < table.Columns.Count ;i ++ ) {
					DataColumn col = this.table.Columns[i];
					base.AvailableFields.Add (new AbstractColumn(col.ColumnName,col.DataType));
				}
				return base.AvailableFields;
			}
		}
		
		
		public override int Count {
			get { return this.table.Rows.Count; }
		}
		
		
		public override object Current
		{
			get {
				try {
					int cr = base.CurrentPosition;
					int li = (base.IndexList[cr] ).ListIndex;
					return this.table.Rows[li];
				} catch (Exception) {
					throw;
				}
			}
		}
		
		#endregion
	}
}
