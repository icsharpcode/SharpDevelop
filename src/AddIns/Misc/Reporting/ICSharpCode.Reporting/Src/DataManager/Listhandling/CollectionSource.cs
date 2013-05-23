/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2013
 * Time: 20:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		private object current;
		
		public CollectionSource(IList list,ReportSettings reportSettings)
		{
			
			if (list.Count > 0) {
				var itemType = list[0].GetType();
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
//				base.AvailableFields.Clear();
				var availableFields = new Collection<AbstractColumn>();
				foreach (PropertyDescriptor p in this.listProperties){
					availableFields.Add (new AbstractColumn(p.Name,p.PropertyType));
				}
				return availableFields;
			}
		}
		
		public object Current {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void Bind()
		{
			Sort();
			Reset();
		}
		
		public void Sort()
		{
			if ((this.reportSettings.SortColumnsCollection != null)) {
				if (this.reportSettings.SortColumnsCollection.Count > 0) {
					IndexList = this.BuildSortIndex (reportSettings.SortColumnsCollection);
				} else {
					IndexList = this.UnsortedIndexList(reportSettings.SortColumnsCollection);
				}
			}
		}
		
		
		public bool MoveNext()
		{
			throw new NotImplementedException();
		}
		
		public void Reset()
		{
			this.CurrentPosition = 0;
			this.IndexList.CurrentPosition = -1;
		}
		
		public IndexList IndexList {get; private set;}
		
		
		IndexList BuildSortIndex(SortColumnCollection sortColumnsCollection)
		{
			var indexList = new IndexList();
			PropertyDescriptor[] sortProperties = BuildSortProperties (sortColumnsCollection);
			for (int rowIndex = 0; rowIndex < this.baseList.Count; rowIndex++){
				object rowItem = this.baseList[rowIndex];
				object[] values = new object[sortColumnsCollection.Count];
				
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
				indexList.Add(new SortComparer(sortColumnsCollection, rowIndex, values));
			}
			
			if (indexList[0].ObjectArray.GetLength(0) == 1) {
				
				List<BaseComparer> sortedList = GenericSorter (indexList);
				indexList.Clear();
				indexList.AddRange(sortedList);
			}
			else {
				indexList.Sort();
			}
			return indexList;
		}
		
		#region Sorting delegates
		
		static List<BaseComparer>  GenericSorter (List<BaseComparer> list)
		{

			List<BaseComparer> sortedList = null;
			ListSortDirection sortDirection = GetSortDirection(list);
			
			if (sortDirection == ListSortDirection.Ascending) {
				sortedList = list.AsQueryable().AscendingOrder().ToList();
			} else {

				sortedList = list.AsQueryable().DescendingOrder().ToList();
			}
			return sortedList;
		}

		static ListSortDirection GetSortDirection(List<BaseComparer> list)
		{
			BaseComparer bc = list[0];
			SortColumn sortColumn = bc.ColumnCollection[0] as SortColumn;
			ListSortDirection sd = sortColumn.SortDirection;
			return sd;
		}
		
		#endregion
		private PropertyDescriptor[] BuildSortProperties (SortColumnCollection sortColumnCollection)
		{
			PropertyDescriptor[] sortProperties = new PropertyDescriptor[sortColumnCollection.Count];
			PropertyDescriptorCollection descriptorCollection = this.baseList.GetItemProperties(null);
			
			for (int criteriaIndex = 0; criteriaIndex < sortColumnCollection.Count; criteriaIndex++){
				PropertyDescriptor descriptor = descriptorCollection.Find (sortColumnCollection[criteriaIndex].ColumnName,true);
				
				if (descriptor == null){
					throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
					                                                  "Die Liste enthält keine Spalte [{0}].",
					                                                  sortColumnCollection[criteriaIndex].ColumnName));
				}
				sortProperties[criteriaIndex] = descriptor;
			}
			return sortProperties;
		}
		
		
		IndexList UnsortedIndexList(SortColumnCollection sortColumnsCollection)
		{
			IndexList arrayList = new IndexList();
			for (int rowIndex = 0; rowIndex < this.baseList.Count; rowIndex++){
				object[] values = new object[1];
				arrayList.Add(new SortComparer(sortColumnsCollection, rowIndex, values));
			}
			return arrayList;
		}
		
		
		public int CurrentPosition {
			
			get {
				return IndexList.CurrentPosition;
			}
			set {
				if ((value > -1)|| (value > this.IndexList.Count)){
					this.IndexList.CurrentPosition = value;
				}
//				var a = this.baseList[((BaseComparer)IndexList[value])];
				
				BaseComparer bc = GetComparer(value);
//					var bc = (BaseComparer)IndexList[value];
//			var i = bc.ListIndex;
				
				var myCurrent = baseList[bc.ListIndex];
				current = baseList[bc.ListIndex];
				
//				current = this.baseList[((BaseComparer)IndexList[value]).ListIndex];
			}
		}

		
		BaseComparer GetComparer(int value)
		{
			var bc = (BaseComparer)IndexList[value];
//			var i = bc.ListIndex;
			return bc;
		}
	}
}
