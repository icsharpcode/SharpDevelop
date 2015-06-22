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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.DataSource;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Data;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.DataManager.Listhandling
{
	/// <summary>
	/// Description of DataSource.
	/// </summary>
	public enum OrderGroup {
		AsIs,
		Sorted,
		Grouped
	}
	
	public class CollectionDataSource:IDataSource
	{
		readonly DataCollection<object> baseList;
		readonly IReportSettings reportSettings;
		readonly PropertyDescriptorCollection listProperties;

	
		public CollectionDataSource(IEnumerable list, IReportSettings reportSettings)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (reportSettings == null)
				throw new ArgumentNullException("reportSettings");
			baseList = CreateBaseList(list);
			CurrentList = baseList;
			
			this.reportSettings = reportSettings;
			this.listProperties = this.baseList.GetItemProperties(null);
			OrderGroup = OrderGroup.AsIs;
		}
	
		
		public Collection<AbstractColumn> AvailableFields {
			get {
				var availableFields = new Collection<AbstractColumn>();
				foreach (PropertyDescriptor p in this.listProperties){
					availableFields.Add (new AbstractColumn(p.Name,p.PropertyType));
				}
				return availableFields;
			}
		}
		
		
		public IList <object> CurrentList {get;private set;}
		
		
		public int Count {
			get {return baseList.Count;}	
		}
		
		public object Current {get; private set;}
		
		public OrderGroup OrderGroup {get; private set;}
		
		public IEnumerable<object> SortedList {get; private set;}
		
		public IEnumerable<IGrouping<object, object>> GroupedList {get;private set;}
		
		#region Sort
		
		void Sort()
		{
			if (reportSettings.SortColumnsCollection.Count > 0) {
				SortedList = SortInternal();
				OrderGroup = OrderGroup.Sorted;
			} else {
				OrderGroup = OrderGroup.AsIs;
				SortedList = CurrentList;
			}

		}
		
		
		IEnumerable<object> SortInternal (){
			IEnumerable<object> sortedList = null;
			var sortProperty = listProperties.Find(reportSettings.SortColumnsCollection[0].ColumnName,true);
			if(reportSettings.SortColumnsCollection.Count == 1) {
				sortedList = baseList.OrderBy(o => o.GetType().GetProperty(sortProperty.Name).GetValue(o, null) );
			}
			return sortedList;
		}
		
		#endregion
		
		
		#region Grouping
		
		void Group()
		{
			OrderGroup = OrderGroup.Grouped;
			GroupedList = GroupInternal();
		}
	
		
		IEnumerable<IGrouping<object, object>> GroupInternal () {
			PropertyDescriptor sortProperty = null;
			var groupProperty = listProperties.Find(reportSettings.GroupColumnsCollection[0].ColumnName,true);
			var groupColumn = (GroupColumn)reportSettings.GroupColumnsCollection[0];
			
			if (groupColumn.GroupSortColumn != null) {
				sortProperty = listProperties.Find(groupColumn.GroupSortColumn.ColumnName,true);
			}
			IEnumerable<IGrouping<object, object>> groupedList;
			if (sortProperty == null) {
				groupedList = baseList.GroupBy(a => a.GetType().GetProperty(groupProperty.Name).GetValue(a, null)).OrderBy(c => c.Key);
			} else {
				groupedList = baseList.OrderBy(o => o.GetType().GetProperty(sortProperty.Name).GetValue(o, null) )
					.GroupBy(a => a.GetType().GetProperty(groupProperty.Name).GetValue(a, null)).OrderBy(c => c.Key);
			}
			return groupedList;
		}
		
		#endregion
		
		public void Bind()
		{
			if (reportSettings.GroupColumnsCollection.Any()) {
				Group();
			} else {
				Sort();
			}
		}
		
		
		#region Fill
		
		public void Fill (List<IPrintableObject> collection, object current) {
			Current = current;
			foreach (var element in collection) {
				var container = element as ReportContainer;
				if (container != null) {
					FillFromList(container.Items);
				} else {
					FillInternal(element);
				}
			}
		}
		
		void FillFromList(List<IPrintableObject> collection)
		{
			foreach (IPrintableObject item in collection) {
				FillInternal(item);
			}
		}


		void FillInternal(IPrintableObject item)
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
		
		
		string ReadValueFromProperty (string columnName) {
			if (String.IsNullOrEmpty(columnName)) {
				return "Missing ColumnName";
			}
			var propertyPath = Current.ParsePropertyPath(columnName);
			try {
				var val = propertyPath.Evaluate(Current);
					return val.ToString();
			} catch (Exception e) {
				Console.WriteLine(" Cant' find <{0}",columnName);
//				throw e;
			}
			return String.Empty;
		}
		
		
		Type SetTypeFromProperty (string columnName) {
			var p = listProperties.Find(columnName,true);
			return p.PropertyType;
		}
		
		#endregion
		
		
		static DataCollection<object> CreateBaseList(IEnumerable source)
		{
			Type elementType = source.AsQueryable().ElementType;
			var list = new DataCollection<object>(elementType);
			list.AddRange(source);
			return list;
		}
	}
}
