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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core{
	
	public class CollectionChangedEventArgs<T> : EventArgs
	{
		T item;
		
		public T Item {
			get {
				return item;
			}
		}
		
		public CollectionChangedEventArgs(T item)
		{
			this.item = item;
		}
	}
	///<summary>
	/// Comparer to Sort the <see cref="ReportItemCollection"></see>
	/// by System.Drawing.Location.Y, so we have <see cref="BaseReportItem"></see> in the Order we use them
	/// (Line by Line)
	/// </summary>
	internal class LocationSorter : IComparer<BaseReportItem>  {
		public int Compare(BaseReportItem lhs, BaseReportItem rhs){
			if (lhs == null){
				if (rhs == null){
					return 0;
				}
				return -1;
			}
			if (rhs == null){
				return 1;
			}
			
			if (lhs.Location.Y == rhs.Location.Y){
				return lhs.Location.X - rhs.Location.X;
			}
			return lhs.Location.Y - rhs.Location.Y;
		}
	}
	

	///<summary>
	///  A collection that holds <see cref='IItemRenderer'/> objects.
	///</summary>
	public class ReportItemCollection : Collection<BaseReportItem>
	{
		
		// Trick to get the inner list as List<T> (InnerList always has that type because we only use
		// the parameterless constructor on Collection<T>)
		
		private List<BaseReportItem> InnerList {
			get { return (List<BaseReportItem>)base.Items; }
		}
		
		private void Sort(IComparer<BaseReportItem> comparer)
		{
			InnerList.Sort(comparer);
		}
		
		public void ForEach (Action <BaseReportItem> action)
		{
			this.InnerList.ForEach (action);
		}
		
		
		public void AddRange(IEnumerable<BaseReportItem> items)
		{
			foreach (BaseReportItem item in items) Add(item);
		}
		
		
		public void SortByLocation () {
			if (this.Count > 1) {
				this.Sort(new LocationSorter());
			}
		}
		
		
		public bool Exist (string itemName)
		{
			if (String.IsNullOrEmpty(itemName)) {
				throw new ArgumentNullException("itemName");
			}
			
			if (InnerFind(itemName) == null) {
				return false;
			}
			else {
				return true;
			}	
		}
		
	
		private BaseReportItem InnerFind (string name)
		{
			return this.FirstOrDefault(x => 0 == String.Compare(x.Name, name,true,CultureInfo.InvariantCulture));
		}
		
		
		public BaseReportItem Find (string itemName)
		{
			if (String.IsNullOrEmpty(itemName)) {
				throw new ArgumentNullException("itemName");
			}
			return this.InnerFind (itemName);
		}
		
		
		public BaseReportItem FindHighestElement()
		{
			if (this.InnerList.Count == 0) {
				return null;
			}
			BaseReportItem heighest = this.InnerList[0];
			foreach (BaseReportItem item in this.InnerList)
			{
				if (item.Size.Height > heighest.Size.Height) {
					heighest = item;
				}
			}
			return heighest;
		}
		
		
		protected override void InsertItem(int index, BaseReportItem item)
		{
			base.InsertItem(index, item);
		}
		
		
		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
		}
	
		#region Grouphandling

        public  Collection<GroupHeader> FindGroupHeader()
        {
            return new Collection<GroupHeader>(this.Items.OfType<GroupHeader>().ToList());
        }


        public  Collection<GroupFooter> FindGroupFooter()
        {
            return new Collection<GroupFooter>(this.Items.OfType<GroupFooter>().ToList());
        }

      
		private Collection<BaseDataItem> CreateGroupedList ()
		{
			Collection<BaseDataItem> inheritedReportItems = null;
			foreach (BaseReportItem element in this) {
				ISimpleContainer container = element as ISimpleContainer;
				if (container == null) {
					inheritedReportItems = new Collection<BaseDataItem>(this.OfType<BaseDataItem>().ToList());
					break;
				} else {
					inheritedReportItems = new Collection<BaseDataItem>(container.Items.OfType<BaseDataItem>().ToList());
					break;
				}
			}
			return inheritedReportItems;
		}
		
		
		public ReportItemCollection ExtractGroupedColumns ()
		{
			Collection<BaseDataItem> inheritedReportItems = CreateGroupedList();
			ReportItemCollection r = new ReportItemCollection();
			r.AddRange(inheritedReportItems);
			return r;
		}
		
		#endregion
	}

	/// <summary>
	/// This class holds all the available Sections of an Report
	/// </summary>
	[Serializable()]
	public sealed class ReportSectionCollection: Collection<BaseSection>
	{
	}
	
	
	
	[Serializable()]
	public class AvailableFieldsCollection: Collection<AbstractColumn>{
		
		public AvailableFieldsCollection(){
		}
		
		public AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			
			 return this.FirstOrDefault(x => 0 == String.Compare(x.ColumnName,columnName,true,CultureInfo.InvariantCulture));
		}
	}
	
	
	
	
	[Serializable()]
	public class SortColumnCollection: ColumnCollection
	{
		public SortColumnCollection()
		{
		}
		
		public new AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			
			return this.FirstOrDefault(x => 0 == String.Compare(x.ColumnName,columnName,true,CultureInfo.InvariantCulture));
		}
	
		
		public void AddRange (IEnumerable<SortColumn> items)
		{
			foreach (SortColumn item in items){
				this.Add(item);
			}
		}
		
		
		/// <summary>
		/// The Culture is used for direct String Comparison
		/// </summary>
		
		public new static CultureInfo Culture
		{
			get { return CultureInfo.CurrentCulture;}
		}
	}
	
	
	[Serializable()]
	public class GroupColumnCollection: SortColumnCollection
	{
		public GroupColumnCollection()
		{
		}
		
		public new AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			
			return this.FirstOrDefault(x => 0 == String.Compare(x.ColumnName,columnName,true,CultureInfo.InvariantCulture));
		}
	}
	
	
	
	[Serializable()]
	public class ColumnCollection: Collection<AbstractColumn>{
		
		public ColumnCollection()
		{
		}
		
		public AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			
			return this.FirstOrDefault(x => 0 == String.Compare(x.ColumnName,columnName,true,CultureInfo.InvariantCulture));
		}
	
		
		public void AddRange (IEnumerable<AbstractColumn> items)
		{
			foreach (AbstractColumn item in items){
				this.Add(item);
			}
		}
		
		
		/// <summary>
		/// The Culture is used for direct String Comparison
		/// </summary>
		
		public static CultureInfo Culture
		{
			get { return CultureInfo.CurrentCulture;}
		}
	}
	

	public class SqlParameterCollection : Collection<SqlParameter>
	{
		public SqlParameterCollection()
		{
		}
		
		public SqlParameter Find (string parameterName)
		{
			if (String.IsNullOrEmpty(parameterName)) {
				throw new ArgumentNullException("parameterName");
			}
			return this.FirstOrDefault(x => 0 == String.Compare(x.ParameterName,parameterName,true,CultureInfo.InvariantCulture));
		}
		
		
		public void AddRange (IEnumerable<SqlParameter> items)
		{
			foreach (SqlParameter item in items){
				this.Add(item);
			}
		}
		
		public static CultureInfo Culture
		{
			get { return System.Globalization.CultureInfo.CurrentCulture; }
		}
		
	}
	
	public class ParameterCollection: Collection<BasicParameter>{
		
		public ParameterCollection()
		{			
		}
		
		
		public BasicParameter Find (string parameterName)
		{
			if (String.IsNullOrEmpty(parameterName)) {
				throw new ArgumentNullException("parameterName");
			}
			return this.FirstOrDefault(x => 0 == String.Compare(x.ParameterName,parameterName,true,CultureInfo.InvariantCulture));
		}
		
		
		public  System.Collections.Hashtable CreateHash ()
		{
			System.Collections.Hashtable ht = new System.Collections.Hashtable();
			foreach(BasicParameter bt in this){
					ht.Add(bt.ParameterName,bt.ParameterValue);
			}
			return ht;
		}
		
		public static CultureInfo Culture
		{
			get { return System.Globalization.CultureInfo.CurrentCulture; }
		}
		
		
		public void AddRange (IEnumerable<BasicParameter> items)
		{
			foreach (BasicParameter item in items){
				this.Add(item);
			}
		}
	}
	
	#region ExporterCollection
	
	public class ExporterCollection : Collection<BaseExportColumn>
	{
		
		public void AddRange (IEnumerable <BaseExportColumn> items){
			foreach (var item in items) {
				this.Add (item);
			}
		}
	}
	
	
	public class PagesCollection  :Collection<ExporterPage>
	{
		
	}
	
	
	#endregion
	
	public class  CurrentItemsCollection:Collection<CurrentItem>
	{
		public CurrentItem Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			
			return this.FirstOrDefault(x => 0 == String.Compare(x.ColumnName,columnName,true,CultureInfo.InvariantCulture));
		}
	}
}
