/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 29.08.2009
 * Zeit: 09:57
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of TableStrategy.
	/// </summary>
	internal class TableStrategy: BaseListStrategy,IEnumerable<BaseComparer> 
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
			this.Sort ();
			Reset();
		}
		
		
		
		public override void Fill(IReportItem item)
		{
			base.Fill(item);
		
			DataRow row = this.Current as DataRow;
			
			if (row != null) {
				BaseDataItem baseDataItem = item as BaseDataItem;
				if (baseDataItem != null) {
					baseDataItem.DBValue = row[baseDataItem.ColumnName].ToString();
					return;
				}
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
			}
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
			if ((base.ReportSettings.SortColumnCollection != null)) {
				if (base.ReportSettings.SortColumnCollection.Count > 0) {
					base.IndexList = this.BuildSortIndex (base.AbstractCollection);
					base.IsSorted = true;
				} else {
					// if we have no sorting, we build the indexlist as well
					base.IndexList = this.IndexBuilder(base.AbstractCollection);                                  
					base.IsSorted = false;
				}
			}
		}
		
		#endregion
		
		
		private IndexList  BuildSortIndex(Collection<AbstractColumn> col)
		{
			IndexList arrayList = new IndexList();
			
			for (int rowIndex = 0; rowIndex < this.table.Rows.Count; rowIndex++){
				DataRow rowItem = this.table.Rows[rowIndex];
				object[] values = new object[col.Count];
				for (int criteriaIndex = 0; criteriaIndex < col.Count; criteriaIndex++){
					AbstractColumn c = (AbstractColumn)col[criteriaIndex];
					object value = rowItem[c.ColumnName];

					if (value != null && value != DBNull.Value){
						if (!(value is IComparable)){
							throw new InvalidOperationException(value.ToString());
						}
						
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
		
	
		private IndexList IndexBuilder(Collection<AbstractColumn> col)
		{
			IndexList arrayList = new IndexList();
			for (int rowIndex = 0; rowIndex < this.table.Rows.Count; rowIndex++){
				object[] values = new object[1];
				arrayList.Add(new SortComparer(col, rowIndex, values));
			}
			return arrayList;
		}
		
		
		private void BuildAvailableFields ()
		{
			
			base.AvailableFields.Clear();
			for (int i = 0;i < table.Columns.Count ;i ++ ) {
				DataColumn col = this.table.Columns[i];
				base.AvailableFields.Add (new AbstractColumn(col.ColumnName,col.DataType));
			}
		
		}
		
		#region Test
		
		public override CurrentItemsCollection FillDataRow()
		{
			CurrentItemsCollection ci = base.FillDataRow();
			DataRow row = this.Current as DataRow;
			
			if (row != null) {
				CurrentItem c = null;
				foreach (DataColumn dc in table.Columns)
				{
					c = new CurrentItem();
					c.ColumnName = dc.ColumnName;
					c.DataType = dc.DataType;
					c.Value = row[dc.ColumnName];
					ci.Add(c);
				}
			}
			return ci;
		}
		
		
		#endregion
		
		
		#region IEnumerable<BaseComparer>

		IEnumerator IEnumerable.GetEnumerator()
		{
			return((IEnumerable<BaseComparer>)this).GetEnumerator();
		}
		
		IEnumerator<BaseComparer> IEnumerable<BaseComparer>.GetEnumerator()
		{
			IEnumerable<BaseComparer> e = (IEnumerable<BaseComparer>)base.IndexList;
			return e.GetEnumerator();
		}
		#endregion
		
		
		#region Propertys
		
		public override AvailableFieldsCollection AvailableFields {
			get {
				BuildAvailableFields();
				return base.AvailableFields;
			}
		}
		
		
		public override int Count {
			get { return this.table.Rows.Count; }
		}
		
		
		public override object Current {
			get {
				int cr = base.CurrentPosition;
				int li = (base.IndexList[cr] ).ListIndex;
				return this.table.Rows[li];
//				return this.view[((BaseComparer)base.IndexList[base.CurrentRow]).ListIndex];
			}
		}
		
		public override int CurrentPosition {
			get { return base.CurrentPosition; }
			set { base.CurrentPosition = value; }
		}
		
		public override bool IsSorted {
			get { return base.IsSorted; }
			set { base.IsSorted = value; }
		}
		
		#endregion
	}
}
