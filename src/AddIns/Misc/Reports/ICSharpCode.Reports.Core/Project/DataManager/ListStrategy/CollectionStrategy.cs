// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

/// <summary>
/// This Class handles all List's with IList
/// Access to Data is allway's done by using the 'IndexList'
/// </summary>

namespace ICSharpCode.Reports.Core {
	
	internal sealed class CollectionStrategy : BaseListStrategy {
		
		private Type	itemType;
		private object firstItem;
		private object current;
		
		private PropertyDescriptorCollection listProperties;
		private DataCollection<object> baseList;
		
		
		#region Constructor
		
		public CollectionStrategy(IList list,ReportSettings reportSettings):base(reportSettings)
		{
			if (list.Count > 0) {
				firstItem = list[0];
				itemType =  firstItem.GetType();
				
				this.baseList = new DataCollection <object>(itemType);
				this.baseList.AddRange(list);
			}
			this.listProperties = this.baseList.GetItemProperties(null);
		}

		#endregion
		
		
		
		#region build sorting
		
		private PropertyDescriptor[] BuildSortProperties (SortColumnCollection col)
		{
			PropertyDescriptor[] sortProperties = new PropertyDescriptor[col.Count];
			PropertyDescriptorCollection c = this.baseList.GetItemProperties(null);
			
			for (int criteriaIndex = 0; criteriaIndex < col.Count; criteriaIndex++){
				PropertyDescriptor descriptor = c.Find (col[criteriaIndex].ColumnName,true);
		
				if (descriptor == null){
					throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
					                                                  "Die Liste enthält keine Spalte [{0}].",
					                                                  col[criteriaIndex].ColumnName));
				}
				sortProperties[criteriaIndex] = descriptor;
			}
			return sortProperties;
		}
		
		
		private  IndexList BuildSortIndex(SortColumnCollection col) 
		{
			IndexList arrayList = new IndexList();
			PropertyDescriptor[] sortProperties = BuildSortProperties (col);
			for (int rowIndex = 0; rowIndex < this.baseList.Count; rowIndex++){
				object rowItem = this.baseList[rowIndex];
				object[] values = new object[col.Count];
				
				// Hier bereits Wertabruf um dies nicht während des Sortierens tun zu müssen.
				for (int criteriaIndex = 0; criteriaIndex < sortProperties.Length; criteriaIndex++){
					object value = sortProperties[criteriaIndex].GetValue(rowItem);
					// Hier auf Verträglichkeit testen um Vergleiche bei Sortierung zu vereinfachen.
					// Muss IComparable und gleicher Typ sein.
					
					if (value != null && value != DBNull.Value)
					{
						if (!(value is IComparable)){
							throw new InvalidOperationException("ReportDataSource:BuildSortArray - > This type doesn't support IComparable." + value.ToString());
						}
						
						values[criteriaIndex] = value;
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
		
		
		
		// if we have no sorting, we build the indexlist as well, so we don't need to
		private IndexList IndexBuilder(SortColumnCollection col)
		{
			IndexList arrayList = new IndexList();
			for (int rowIndex = 0; rowIndex < this.baseList.Count; rowIndex++){
				object[] values = new object[1];
				arrayList.Add(new SortComparer(col, rowIndex, values));
			}
			return arrayList;
		}
	
		
		private void BuildAvailableFields () 
		{
			base.AvailableFields.Clear();
			foreach (PropertyDescriptor p in this.listProperties){
				base.AvailableFields.Add (new AbstractColumn(p.Name,p.PropertyType));
			}
		}
		
		#endregion
		
		#region SharpReportCore.IDataViewStrategy interface implementation
		
		public override AvailableFieldsCollection AvailableFields
		{
			get {
				BuildAvailableFields();
				return base.AvailableFields;
			}
		}
		
		public override object Current
		{
			get {
				return this.baseList[((BaseComparer)base.IndexList[base.CurrentPosition]).ListIndex];
			}
		}
		
		public override int Count
		{
			get {
				return this.baseList.Count;
			}
		}
		
		public override  int CurrentPosition 
		{
			get {
				return base.IndexList.CurrentPosition;
			}
			set {
				base.CurrentPosition = value;
				current = this.baseList[((BaseComparer)base.IndexList[value]).ListIndex];
			}
		}

		
		public override void Group ()
		{
			base.Group();
			IndexList gl = new IndexList("group");
			gl = this.BuildSortIndex (ReportSettings.GroupColumnsCollection);
//			ShowIndexList(gl);
			base.BuildGroup(gl);
		}
		
		
		public override void Sort() 
		{
			base.Sort();		
			if ((base.ReportSettings.SortColumnsCollection != null)) {
				if (base.ReportSettings.SortColumnsCollection.Count > 0) {

					base.IndexList = this.BuildSortIndex (ReportSettings.SortColumnsCollection);
					base.IsSorted = true;
				} else {
					base.IndexList = this.IndexBuilder(ReportSettings.SortColumnsCollection);
					base.IsSorted = false;
				}
			}
		}
		
		
		public override void Reset() 
		{
			this.CurrentPosition = 0;
			base.Reset();
		}
		
		
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
		
		#endregion
		
		public override void Fill(IDataItem item)
		{
			if (current != null) {
				BaseDataItem baseDataItem = item as BaseDataItem;
				if (baseDataItem != null) {
					PropertyDescriptor p = this.listProperties.Find(baseDataItem.ColumnName, true);
					if (p != null) {
						baseDataItem.DBValue = p.GetValue(this.Current).ToString();
					} else {
						baseDataItem.DBValue = string.Format(CultureInfo.InvariantCulture,"<{0}> missing!", baseDataItem.ColumnName);
					}
					return;
				}
				
				//image processing from IList
				BaseImageItem baseImageItem = item as BaseImageItem;
				
				if (baseImageItem != null) {
					PropertyDescriptor p = this.listProperties.Find(baseImageItem.ColumnName, true);
					if (p != null) {
						baseImageItem.Image = p.GetValue(this.Current) as System.Drawing.Image;
					}
					return;
				}
			}
		}

		#region test
		/*
		public override CurrentItems FillDataRow()
		{
			CurrentItems ci = base.FillDataRow();
			DataRow row = this.Current as DataRow;
			
			if (row != null) {
				CurrentItem c = null;
				
				foreach (DataColumn dc in table.Columns)
				{
					c = new CurrentItem();
					c.ColumnName = dc.ColumnName;
					c.Value = row[dc.ColumnName];
					ci.Add(c);
				}
			}
			return ci;
		}
		*/
		
		public override CurrentItemsCollection FillDataRow()
		{
			CurrentItemsCollection ci = base.FillDataRow();
			if (current != null) {
				CurrentItem c = null;
				foreach (PropertyDescriptor pd in this.listProperties)
				{
					c = new CurrentItem();
					c.ColumnName = pd.Name;
					c.DataType = pd.PropertyType;
					c.Value = pd.GetValue(this.Current).ToString();
					ci.Add(c);
				}
			}
			return ci;
		}
		
		#endregion
	
		#region IDisposable
		
		public override void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		
		protected override void Dispose(bool disposing){
			try {
				if (disposing) {
					if (this.baseList != null) {
						this.baseList.Clear();
						this.baseList = null;
					}
					if (this.listProperties != null) {
						this.listProperties = null;
					}
				}
			} finally {
				base.Dispose(disposing);
				// Release unmanaged resources.
				// Set large fields to null.
				// Call Dispose on your base class.
			}
		}
		#endregion
	}
}
