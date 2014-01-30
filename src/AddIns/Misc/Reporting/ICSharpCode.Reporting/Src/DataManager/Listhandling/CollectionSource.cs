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
using System.Globalization;
using System.Linq;
using System.Reflection;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.DataSource;
using ICSharpCode.Reporting.DataSource.Comparer;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Data;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.DataManager.Listhandling
{
	/// <summary>
	/// Description of CollectionHandling.
	/// </summary>
	internal class CollectionSource:IDataViewHandling
	{

		private PropertyDescriptorCollection listProperties;
		private DataCollection<object> baseList;
		private ReportSettings reportSettings;
		private Type itemType;
		
		public CollectionSource(IList list,ReportSettings reportSettings)
		{
			
			if (list.Count > 0) {
				itemType = list[0].GetType();
				this.baseList = new DataCollection <object>(itemType);
				this.baseList.AddRange(list);
			}
			this.reportSettings = reportSettings;
			this.listProperties = this.baseList.GetItemProperties(null);
			IndexList = new IndexList();
		}
		
		public  int Count
		{
			get {
				return this.baseList.Count;
			}
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
		
		public object Current {
			get {
				return baseList[((BaseComparer)IndexList[CurrentPosition]).ListIndex];
			}
		}
		
		
//		public object CurrentFromPosition(int pos)
//		{
		//		BaseComparer bc = GetComparer(value);
//				Current = baseList[bc.ListIndex];
//				return baseList[((BaseComparer)IndexList[CurrentPosition]).ListIndex];
//		}
		
		
		public int CurrentPosition {
			
			get {
				return IndexList.CurrentPosition;
			}
			set {
				if ((value > -1)|| (value > this.IndexList.Count)){
					this.IndexList.CurrentPosition = value;
				}
				
//				BaseComparer bc = GetComparer(value);
//				Current = baseList[bc.ListIndex];
				
//				current = this.baseList[((BaseComparer)IndexList[value]).ListIndex];
			}
		}
		
		
		public bool MoveNext()
		{
			this.IndexList.CurrentPosition ++;
			return this.IndexList.CurrentPosition<this.IndexList.Count;
		}
		
		
		public IndexList IndexList {get; private set;}
		
		
		public void Bind()
		{
			if (reportSettings.GroupColumnCollection.Any()) {
				this.Group();
			} else {
				this.Sort ();
			}
		}
		
		#region Fill
	/*
		public void Fill(IDataItem item)
		{

//			PropertyInfo pi = itemType.GetProperty(item.ColumnName);
//			var pi1 = pi.GetValue(Current);
			
			var p = listProperties.Find(item.ColumnName,true);
			item.DBValue = p.GetValue(Current).ToString();
			if (String.IsNullOrEmpty(item.DataType)) {
				item.DataType = p.PropertyType.ToString();
			}
		}
		*/
		
		public void Fill(ReportItemCollection collection)
		{
			foreach (IDataItem item in collection)
            {
                FillInternal(item);
            }
		}
		
		void FillInternal (IDataItem item) {
			var p = listProperties.Find(item.ColumnName,true);
			item.DBValue = p.GetValue(Current).ToString();
			if (String.IsNullOrEmpty(item.DataType)) {
				item.DataType = p.PropertyType.ToString();
			}
		}
		
		#endregion
		
		
		#region Grouping
		
		public void Group()
		{
			var unsortedList = this.BuildIndexInternal(reportSettings.GroupColumnCollection);
//			IndexList = BuildGroup(unsortedList,reportSettings.GroupColumnCollection);
			var sorted = unsortedList.OrderBy(a => a.ObjectArray[0]);
			var x = sorted.GroupBy(a => a.ObjectArray[0]);
			
			Console.WriteLine("GroupBy()");
			foreach (var element in x) {
				Console.WriteLine("{0} - {1} ",element.Key.ToString(),element is BaseComparer);
				foreach (var xx in element) {
					Console.WriteLine("...{0}",((BaseComparer)xx).ObjectArray[0].ToString());
				}
			}
			
			Console.WriteLine("---------------	");
			
			var aa = BuildGroup_1(unsortedList,reportSettings.GroupColumnCollection);
//			ShowIndexList(IndexList);
		}
		
		
		private Dictionary<string,IndexList> BuildGroup_1 (IndexList list,GroupColumnCollection groups) {
			var dictionary = new Dictionary<string,IndexList>();
			PropertyDescriptor[] groupProperties = BuildSortProperties (groups);
			foreach (var element in list) {
				string groupValue = ExtractValue (element,groupProperties);
				if (!dictionary.ContainsKey(groupValue)) {
					dictionary[groupValue] = new IndexList();
				}
				
				dictionary[groupValue].Add(element);
			}
			
			Console.WriteLine("Dictonary ");
			foreach (var el in dictionary.Values) {
				Console.WriteLine(el.Count.ToString());
				foreach (var element in el) {
					Console.WriteLine("-- {0}",element.ToString());
				}
			}
			return dictionary;
		}
		

		private IndexList BuildGroup (IndexList source,GroupColumnCollection groups)
		{
			string compareValue = String.Empty;
			var idlist = new IndexList();

			PropertyDescriptor[] groupProperties = BuildSortProperties (groups);

			GroupComparer groupComparer = null;
			
			foreach (BaseComparer element in source) {
				var groupValue = ExtractValue(element,groupProperties);
//				var query2 =  idlist.FirstOrDefault( s => ((GroupComparer)s).ObjectArray[0] == groupValue) as GroupComparer;
//				if (query2 == null) {
//					groupComparer = CreateGroupHeader(element);
//					idlist.Add(groupComparer);
//				} else {
//					Console.WriteLine("xx");
//				}
				if (compareValue != groupValue) {
					groupComparer = CreateGroupHeader(element);
					idlist.Add(groupComparer);
				}
				groupComparer.IndexList.Add(element);
				
				compareValue = groupValue;
			}
			ShowGrouping(ref idlist);
			return idlist;
		}


		void ShowGrouping(ref IndexList idlist)
		{
			Console.WriteLine("----ShowGrouping---");
			foreach (GroupComparer el in idlist) {
				Console.WriteLine("{0}", el.ToString());
				if (el.IndexList.Any()) {
					foreach (var element in el.IndexList) {
						Console.WriteLine("--{0}", element.ToString());
					}
				}
			}
		}
		
		
		string ExtractValue(BaseComparer element,PropertyDescriptor[] groupProperties)
		{
			var rowItem = baseList[element.ListIndex];
			var values = FillComparer(groupProperties, rowItem);
//			return element.ObjectArray[0].ToString();
			return values[0].ToString();
		}
		
		
		static GroupComparer CreateGroupHeader (BaseComparer sc)
		{
			var gc = new GroupComparer(sc.ColumnCollection,sc.ListIndex,sc.ObjectArray);
			gc.IndexList = new IndexList();
			return gc;
		}
		
		#endregion
		
		#region BuildIndexList
		
		IndexList BuildSortIndex(Collection<AbstractColumn> sortColumnsCollection)
		{
			
			IndexList indexList = BuildIndexInternal(sortColumnsCollection);
			
			if (indexList[0].ObjectArray.GetLength(0) == 1) {
				
				IEnumerable<BaseComparer> sortedList = GenericSorter (indexList);
				indexList.Clear();
				indexList.AddRange(sortedList);
			}
			else {
				indexList.Sort();
			}
			return indexList;
		}



		IndexList BuildIndexInternal(Collection<AbstractColumn> sortColumnsCollection)
		{
			var indexList = new IndexList();
			PropertyDescriptor[] sortProperties = BuildSortProperties(sortColumnsCollection);
			for (int rowIndex = 0; rowIndex < this.baseList.Count; rowIndex++) {
				var rowItem = this.baseList[rowIndex];
				var values = FillComparer(sortProperties, rowItem);
				indexList.Add(new SortComparer(sortColumnsCollection, rowIndex, values));
			}
			return indexList;
		}
		
		#endregion
		
		#region Sorting delegates
		
		public void Sort()
		{
			if ((this.reportSettings.SortColumnsCollection != null)) {
				if (this.reportSettings.SortColumnsCollection.Count > 0) {
					IndexList = this.BuildSortIndex (reportSettings.SortColumnsCollection);
				} else {
					IndexList = this.BuildIndexInternal(reportSettings.SortColumnsCollection);
				}
			}
		}
		
		static IEnumerable<BaseComparer> GenericSorter (List<BaseComparer> list)
		{

			List<BaseComparer> sortedList = null;
			ListSortDirection sortDirection = GetSortDirection(list);
			
			sortedList = sortDirection == ListSortDirection.Ascending ? list.AsQueryable().AscendingOrder().ToList() : list.AsQueryable().DescendingOrder().ToList();
			return sortedList;
		}

		
		static ListSortDirection GetSortDirection(List<BaseComparer> list)
		{
			BaseComparer bc = list[0];
			var sortColumn = bc.ColumnCollection[0] as SortColumn;
			ListSortDirection sd = sortColumn.SortDirection;
			return sd;
		}
		
		
		Object[] FillComparer(PropertyDescriptor[] sortProperties,  object rowItem)
		{
			object[] values = new object[sortProperties.Length];
			for (int criteriaIndex = 0; criteriaIndex < sortProperties.Length; criteriaIndex++) {
				object value = sortProperties[criteriaIndex].GetValue(rowItem);
				if (value != null && value != DBNull.Value) {
					if (!(value is IComparable)) {
						throw new InvalidOperationException("ReportDataSource:BuildSortArray - > This type doesn't support IComparable." + value.ToString());
					}
					values[criteriaIndex] = value;
				}
			}
			return values;
		}
		
		private PropertyDescriptor[] BuildSortProperties (Collection<AbstractColumn> sortColumnCollection)
		{
			var sortProperties = new PropertyDescriptor[sortColumnCollection.Count];
			var descriptorCollection = this.baseList.GetItemProperties(null);
			
			for (var criteriaIndex = 0; criteriaIndex < sortColumnCollection.Count; criteriaIndex++){
				var descriptor = descriptorCollection.Find (sortColumnCollection[criteriaIndex].ColumnName,true);
				
				if (descriptor == null){
					throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
					                                                  "Die Liste enthält keine Spalte [{0}].",
					                                                  sortColumnCollection[criteriaIndex].ColumnName));
				}
				sortProperties[criteriaIndex] = descriptor;
			}
			return sortProperties;
		}
		
		
		BaseComparer GetComparer(int position)
		{
			var bc = (BaseComparer)IndexList[position];
			return bc;
		}
		
		#endregion
		
		#region Debug Code

		private static void ShowIndexList (IndexList list)
		{
			foreach (BaseComparer element in list) {
				var groupComparer = element as GroupComparer;
				if (groupComparer == null) continue;
				if (groupComparer.IndexList.Any()) {
					var ss = String.Format("{0} with {1} Children",element.ObjectArray[0],groupComparer.IndexList.Count);
					System.Console.WriteLine(ss);
					foreach (BaseComparer c in groupComparer.IndexList) {
						Console.WriteLine("---- {0}",c.ObjectArray[0]);
					}
				}
			}
		}
		
		static string  WrongColumnName(string propertyName)
		{
			return String.Format(CultureInfo.InvariantCulture, "Error : <{0}> missing!", propertyName);
		}
		
		#endregion
		
		
		public void Reset()
		{
			throw new NotImplementedException();
		}
	}
}
