// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.DataSource;
using ICSharpCode.Reporting.DataSource.Comparer;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Data;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.DataManager.Listhandling
{
	/// <summary>
	/// Description of DataSource.
	/// </summary>
	public class CollectionDataSource:IDataViewHandling
	{
		readonly DataCollection<object> baseList;
		readonly ReportSettings reportSettings;
		readonly Type elementType;
		readonly PropertyDescriptorCollection listProperties;
	
		public CollectionDataSource(IEnumerable list, Type elementType, ReportSettings reportSettings)
		{
			this.elementType = elementType;
			this.reportSettings = reportSettings;
			baseList = CreateBaseList(list, elementType);
			this.listProperties = this.baseList.GetItemProperties(null);
		}
		
		
		
		public IndexList IndexList {
			get {
				throw new NotImplementedException();
			}
		}
		
		
		public System.Collections.ObjectModel.Collection<ICSharpCode.Reporting.BaseClasses.AbstractColumn> AvailableFields {
			get {
				var availableFields = new Collection<AbstractColumn>();
				foreach (PropertyDescriptor p in this.listProperties){
					availableFields.Add (new AbstractColumn(p.Name,p.PropertyType));
				}
				return availableFields;
			}
		}
		
		
		public int Count {
			get {
				return baseList.Count;
			}
		}
		
		public int CurrentPosition {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public object Current {get; private set;}
			

		
		public void Sort()
		{
			throw new NotImplementedException();
		}
		
		#region Grouping
		
		public void Group()
		{
			groupedList = GroupInternal();
			groupEnumerator = groupedList.GetEnumerator();
			groupEnumerator.MoveNext();
			listEnumerator = groupEnumerator.Current.GetEnumerator();
			listEnumerator.MoveNext();
			Current = listEnumerator.Current;
		}
		
		
		IEnumerable<IGrouping<object, object>> GroupInternal () {
			var properties = listProperties.Find(reportSettings.GroupColumnCollection[0].ColumnName,true);
			var property = listProperties.Find("Randomint",true);
			var groupedList = baseList.OrderBy(o => o.GetType().GetProperty(property.Name).GetValue(o, null) )
				.GroupBy(a => a.GetType().GetProperty(properties.Name).GetValue(a, null)).OrderBy(c => c.Key);
			return groupedList;
		}
		
		#endregion
		
		public void Bind()
		{
			if (reportSettings.GroupColumnCollection.Any()) {
				this.Group();
			} else {
				this.Sort ();
			}
		}
		
		#region Fill
		
		private IEnumerable<IGrouping<object, object>> groupedList;
		private IEnumerator<IGrouping<object, object>> groupEnumerator;
		private IEnumerator<object> listEnumerator;
		
		public void Fill(System.Collections.Generic.List<ICSharpCode.Reporting.Interfaces.IPrintableObject> collection)
		{
			foreach (IPrintableObject item in collection)
			{
				var dbItem = item as IDataItem;
				if (dbItem != null) {
					
					dbItem.DBValue = String.Empty;
					dbItem.DBValue = ReadValueFromProperty(dbItem.ColumnName);
					if (String.IsNullOrEmpty(dbItem.DataType)) {
						dbItem.DataType = SetTypeFromProperty(dbItem.ColumnName).ToString();
					}
				}
			}
		}
		
		
		string ReadValueFromProperty (string columnName) {
			var p = listProperties.Find(columnName,true);
			return p.GetValue(Current).ToString();
		}
		
		
		Type SetTypeFromProperty (string columnName) {
			var p = listProperties.Find(columnName,true);
			return p.PropertyType;
		}
		
		
		public bool MoveNext()
		{
			var canMove = listEnumerator.MoveNext();
			if (! canMove) { 
				var groupCanMove = groupEnumerator.MoveNext();
				if (groupCanMove) {
					listEnumerator = groupEnumerator.Current.GetEnumerator();
					canMove = listEnumerator.MoveNext();
					Current = listEnumerator.Current;
				} else {
					Console.WriteLine("end");
				}
			} else {
				Current = listEnumerator.Current;
			}
			return canMove;
			
		}
		
		#endregion
		
		public void Reset()
		{
			throw new NotImplementedException();
		}
		
		DataCollection<object> CreateBaseList(IEnumerable source, Type elementType)
		{
			var list = new DataCollection<object>(elementType);
			list.AddRange(source);
			return list;
		}
		
		
		PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors){
			if (listAccessors != null && listAccessors.Length > 0){
				var t = this.elementType;

			    t = listAccessors.Aggregate(t,
                    (current, pd) => (Type) PropertyTypeHash.Instance[current, pd.Name]);

			    // if t is null an empty list will be generated
				return ExtendedTypeDescriptor.GetProperties(t);
			}
			return ExtendedTypeDescriptor.GetProperties(elementType);
		}
	}
}
