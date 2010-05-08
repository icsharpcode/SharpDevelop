// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

/// <summary>
/// BaseClass for all Datahandling Strategies
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 13.11.2005 15:26:02
/// </remarks>

namespace ICSharpCode.Reports.Core {	
	
	
	public static class SortExtension
	{
		// template
		/*
		public static IOrderedQueryable<User> MyOrder(this IQueryable<User> source )
		 {      
		 	
		 	return source.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.Email);
		 }
		*/
		
		public static IOrderedQueryable<BaseComparer> AscendingOrder(this IQueryable<BaseComparer> source )
		{  
			
			return source.OrderBy(x => x.ObjectArray[0]);
		}
		
		public static IOrderedQueryable<BaseComparer> DescendingOrder(this IQueryable<BaseComparer> source )
		{  
			
			return source.OrderByDescending(x => x.ObjectArray[0]);
		}
	}
		
	
	internal  abstract class BaseListStrategy :IDataViewStrategy,IEnumerator {
		private bool isSorted;
//		private bool isFiltered;
//		private bool isGrouped;
		
		//Index to plain Datat
		private IndexList indexList;
		private ReportSettings reportSettings;

		private AvailableFieldsCollection availableFields;
		
//	private ListChangedEventArgs resetList = new ListChangedEventArgs(ListChangedType.Reset,-1,-1);
		
//		public event EventHandler <ListChangedEventArgs> ListChanged;
//		public event EventHandler <GroupChangedEventArgs> GroupChanged;
		
		#region Constructor
		
		protected BaseListStrategy(ReportSettings reportSettings)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			this.reportSettings = reportSettings;
			this.indexList = new IndexList("IndexList");
		}
		
		#endregion
		
		#region Event's
		
		/*
		protected void NotifyResetList()
		{
			if (this.ListChanged != null) {
				this.ListChanged (this,this.resetList);
			}
		}
		*/
		
		/*
		protected void NotifyGroupChanging (object source,GroupSeparator groupSeperator)
		{
			if (this.GroupChanged != null) {
				this.GroupChanged (source,new GroupChangedEventArgs(groupSeperator));
			}
		}
		*/
		
		#endregion
		
		
		protected Collection<AbstractColumn> AbstractCollection
		{
			get {
				Collection<AbstractColumn> abstrCol = new Collection<AbstractColumn>();
				foreach(SortColumn sc in ReportSettings.SortColumnCollection)
				{
					abstrCol.Add(sc);
				}
				return abstrCol;
			}
		}

		public IndexList IndexList
		{
			get {
				return indexList;
			}
			set {
				this.indexList = value;
			}
		}
		
		
		public ReportSettings ReportSettings 
		{
			get {
				return reportSettings;
			}
		}
		
		
		#region Building Groups
		/*
		private static void WriteToIndexFile (IndexList destination,
		                                      BaseComparer comparer) 
		{
			destination.Add(comparer);
		}
		*/
		/*
		private static GroupSeparator BuildGroupSeperator (BaseComparer newGroup,int groupLevel)
		{
			
			GroupSeparator seperator = new GroupSeparator (newGroup.ColumnCollection,
			                                               newGroup.ListIndex,
			                                               newGroup.ObjectArray,
			                                               groupLevel);
			
			return seperator;
		}
		
		
		protected void CreateGroupedIndexList (IndexList sourceList) 
		{
			if (sourceList == null) {
				throw new ArgumentNullException("sourceList");
			}
			int level = 0;
			this.indexList.Clear();
			
			
			SortComparer compareComparer = null;
			GroupSeparator parent = null;
			for (int i = 0;i < sourceList.Count ;i++ ) {
				SortComparer currentComparer = (SortComparer)sourceList[i];
				
				if (compareComparer != null) {
					string str1,str2;
					str1 = currentComparer.ObjectArray[0].ToString();
					str2 = compareComparer.ObjectArray[0].ToString();
					int compareVal = str1.CompareTo(str2);
					
					if (compareVal != 0) {
						BaseListStrategy.WriteToIndexFile(parent.GetChildren,compareComparer);
						parent = BaseListStrategy.BuildGroupSeperator (currentComparer,level);
						this.indexList.Add(parent);
						BaseListStrategy.WriteToIndexFile(parent.GetChildren,currentComparer);
					} else {
						BaseListStrategy.WriteToIndexFile(parent.GetChildren,compareComparer);
						
					}
				}
				else {
					parent = BaseListStrategy.BuildGroupSeperator (currentComparer,level);
				this.indexList.Add(parent);
				}		
				compareComparer = (SortComparer)sourceList[i];
			}
			BaseListStrategy.WriteToIndexFile(parent.GetChildren,compareComparer);
		}
		*/
		#endregion
		
		
		#region Sorting delegates
		
		protected static List<BaseComparer>  GenericSorter (List<BaseComparer> list)
		{
			BaseComparer bc = list[0];
			SortColumn  scc = bc.ColumnCollection[0] as SortColumn;
			ListSortDirection sd = scc.SortDirection;
			List<BaseComparer> lbc = null;
			if (sd == ListSortDirection.Ascending) {
//				lbc = list.OrderBy(i => i.ObjectArray[0]).ToList();
				lbc = list.AsQueryable().AscendingOrder().ToList();
			} else {
//				lbc = list.OrderByDescending(i => i.ObjectArray[0]).ToList();
				lbc = list.AsQueryable().DescendingOrder().ToList();
			}
			return lbc;
		}
		
		
		#endregion
		
		/*
		protected static void CheckSortArray (ExtendedIndexCollection arr,string text)
		{
			if (arr != null) {
				int row = 0;
				foreach (BaseComparer bc in arr) {
					GroupSeparator sep = bc as GroupSeparator;
					if (sep != null) {

						
					} else {
						object [] oarr = bc.ObjectArray;
						for (int i = 0;i < oarr.Length ;i++ ) {
							string str = oarr[i].ToString();
						}
						row ++;
					}
					
				}
			}
			System.Console.WriteLine("-----End of <CheckSortArray>-----------");
		}
		*/
		
		public  virtual void Reset()
		{
			this.indexList.CurrentPosition = -1;
		}
		
		public virtual object Current 
		{
			get {
				throw new NotImplementedException();
			}
		}
		
		
		public virtual bool MoveNext()
		{
			this.indexList.CurrentPosition ++;
			return this.indexList.CurrentPosition<this.indexList.Count;
		}
		
		#region test
		
		public virtual CurrentItemsCollection FillDataRow()
		{
			return new CurrentItemsCollection();
		}
		
		#endregion
		
		#region SharpReportCore.IDataViewStrategy interface implementation
		
		public virtual AvailableFieldsCollection AvailableFields 
		{
			get {
				if (this.availableFields == null) {
					this.availableFields = new AvailableFieldsCollection();
				}
				return this.availableFields;
			}
		}
		
		
		public virtual int Count 
		{
			get {
				return 0;
			}
		}
	
		
		public virtual int CurrentPosition
		{
			get {
				return this.indexList.CurrentPosition;
			}
			set {
				if ((value > -1)|| (value > this.indexList.Count)){
					this.indexList.CurrentPosition = value;
				}
			}
		}
		
		
		public bool HasMoreData
		{
			get {
				return true;
			}
		}
		
		
		public virtual bool IsSorted
		{
			get {
				return this.isSorted;
			}
			set {
				this.isSorted = value;
			}
		}
		
		/*
		public bool IsFiltered
		{
			get {
				return this.isFiltered;
			} set {
				this.isFiltered = value;
			}
		}
		*/
		/*
		public bool IsGrouped 
		{
			get {
				return this.isGrouped;
			}
		}
		*/
		
		/*
		protected virtual void Group() 
		{
			if (this.indexList != null) {
				this.isGrouped = true;
				this.isSorted = true;
			} else {
				throw new ReportException ("BaseListStrategy:Group Sorry, no IndexList");
			}
		
		}
		*/
		
		
		public virtual void Sort() 
		{
			this.indexList.Clear();
		}
		
		
		public virtual void Bind() 
		{
			
		}
		
		public  virtual void Fill(IReportItem item)
		{
		
		}
		
		/*
		public  bool HasChildren 
		{
			get {
				if (this.IsGrouped == true) {
					GroupSeparator gs = (GroupSeparator)this.indexList[this.CurrentRow] as GroupSeparator;
					if (gs != null) {
						return (gs.GetChildren.Count > 0);
					} else {
						return false;
					}
				} else {
					return false;
				}
			}
		}
		
		*/
		/*
		public IndexList ChildRows 
		{
			get {
				if (this.IsGrouped == true) {
					GroupSeparator gs = (GroupSeparator)this.indexList[this.CurrentRow] as GroupSeparator;
					if (gs != null) {
						return (gs.GetChildren);
					} else {
						return null;
					}
				} else {
					return null;
				}
			}
		}
		*/
		
		public virtual void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~BaseListStrategy(){
			Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing) {
				// Free other state (managed objects).
				if (this.indexList != null) {
					this.indexList.Clear();
					this.indexList = null;
				}
			}
			
			// Release unmanaged resources.
			// Set large fields to null.
			// Call Dispose on your base class.
		}
		#endregion
		
	}
}
