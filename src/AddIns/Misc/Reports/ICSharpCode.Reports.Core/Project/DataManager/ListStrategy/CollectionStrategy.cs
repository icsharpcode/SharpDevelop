// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using SimpleExpressionEvaluator.Utilities;


/// <summary>
/// This Class handles all List's with IList
/// Access to Data is allway's done by using the 'IndexList'
/// </summary>

namespace ICSharpCode.Reports.Core.ListStrategy
{
	
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
		
		#endregion
		
		#region SharpReportCore.IDataViewStrategy interface implementation
		
		public override AvailableFieldsCollection AvailableFields
		{
			get {
				base.AvailableFields.Clear();
				foreach (PropertyDescriptor p in this.listProperties){
					base.AvailableFields.Add (new AbstractColumn(p.Name,p.PropertyType));
				}
				return base.AvailableFields;
			}
		}
		
		public override object Current
		{
			get
			{
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
			//ShowIndexList(gl);
			base.BuildGroup(gl);
		}
		
		
		public override void Sort()
		{
			base.Sort();
			if ((base.ReportSettings.SortColumnsCollection != null)) {
				if (base.ReportSettings.SortColumnsCollection.Count > 0) {
					base.IndexList = this.BuildSortIndex (ReportSettings.SortColumnsCollection);
				} else {
					base.IndexList = this.IndexBuilder(ReportSettings.SortColumnsCollection);
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
		
	
		#region Fill
		
		public override void Fill(int position,ReportItemCollection collection)
		{
			var current = this.CurrentFromPosition(position);
			foreach (IDataItem item in collection)
            {
                FillInternal(current, item);
            }
		}
	
		public override void Fill(IDataItem item)
		{
			FillInternal(Current,item);
		}
		
		
		private void FillInternal(object fillFrom,IDataItem item)
		{
		    
            if (item is BaseDataItem)
			{
				string result = String.Empty;
				PropertyPath path = fillFrom.ParsePropertyPath(item.ColumnName);
				if (path != null)
				{
					var pp = path.Evaluate(fillFrom);
					if (pp != null)
					{
						result = pp.ToString();
					}
				} else 
				{
					result = WrongColumnName(item.ColumnName);
				}
				
				item.DBValue = result;
			}
			else
			{
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
		
		#endregion
			
		static string  WrongColumnName(string propertyName)
		{
			return String.Format(CultureInfo.InvariantCulture, "Error : <{0}> missing!", propertyName);
		}
			
		
		
        public override object CurrentFromPosition (int pos)
        {
        	return this.baseList[pos];
        }

        public override CurrentItemsCollection FillDataRow(int pos)
        {
        	CurrentItemsCollection ci = new CurrentItemsCollection();
        	var obj = CurrentFromPosition(pos);
        	if (obj != null)
        	{
        		CurrentItem currentItem = null;
        		foreach (PropertyDescriptor pd in this.listProperties)
        		{
        		    currentItem = new CurrentItem(pd.Name, pd.PropertyType);
        			PropertyPath prop = obj.ParsePropertyPath(pd.Name);
        			if (prop != null)
        			{
        				var pp = prop.Evaluate(obj);
                        if (pp != null)
                        {
                           currentItem.Value = pp.ToString(); 
                        }
        			}
        			ci.Add(currentItem);
        		}
        	}
        	return ci;
        }
        
        /*
        public override CurrentItemsCollection FillDataRow(int pos)
        {
            CurrentItemsCollection ci = new CurrentItemsCollection();
        	var obj = CurrentFromPosition(pos);
            if (obj != null)
            {
                CurrentItem currentItem = null;
                foreach (PropertyDescriptor pd in this.listProperties)
                {
                    currentItem = new CurrentItem();
                    currentItem.ColumnName = pd.Name;
                    currentItem.DataType = pd.PropertyType;
                    
                    var propValue = FollowPropertyPath(obj,pd.Name);
                    if (propValue != null)
                    {
                        currentItem.Value = propValue.ToString();
                    }
                    else
                    {
                        currentItem.Value = String.Empty;
                    }
                    ci.Add(currentItem);
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
					c = new CurrentItem(pd.Name,pd.PropertyType);
                    var s = pd.GetValue(this.Current);
                    if (s != null)
                    {
                        c.Value = s.ToString();
                    }
                    else
                    {
                        c.Value = String.Empty;
                    }
					ci.Add(c);
				}
			}
			return ci;
		}

		
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
