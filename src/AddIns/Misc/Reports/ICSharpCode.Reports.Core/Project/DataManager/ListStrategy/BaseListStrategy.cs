// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

		private IndexList indexList;
		private AvailableFieldsCollection availableFields;
		

		#region Constructor
		
		protected BaseListStrategy(ReportSettings reportSettings)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			this.ReportSettings = reportSettings;
			this.indexList = new IndexList("IndexList");
		}
		
		#endregion
		
		
		public IndexList IndexList
		{
			get {
				return indexList;
			}
			set {
				this.indexList = value;
			}
		}
		
		
		protected ReportSettings ReportSettings {get;private set;}
	
		
		#region Sorting delegates
		
		protected static List<BaseComparer>  GenericSorter (List<BaseComparer> list)
		{
			BaseComparer bc = list[0];
			SortColumn  scc = bc.ColumnCollection[0] as SortColumn;
			ListSortDirection sd = scc.SortDirection;
			List<BaseComparer> lbc = null;
			if (sd == ListSortDirection.Ascending) {
				lbc = list.AsQueryable().AscendingOrder().ToList();
			} else {

				lbc = list.AsQueryable().DescendingOrder().ToList();
			}
			return lbc;
		}
		
		#endregion
		
		
		#region Grouping
		
		protected void BuildGroup (IndexList list)
		{
			string compVal = String.Empty;
			IndexList.Clear();
			IndexList childList = null;
			foreach (BaseComparer element in list)
			{
				string v = element.ObjectArray[0].ToString();
				if (compVal != v) {
					childList = new IndexList();
					GroupComparer gc = CreateGroupHeader(element);
					gc.IndexList = childList;
					CreateGroupeChildren(childList,element);
				} else {
					CreateGroupeChildren(childList,element);
				}
				compVal = v;
			}
//			ShowIndexList(IndexList);
		}
		
		
		protected GroupComparer CreateGroupHeader (BaseComparer sc)
		{
			GroupComparer gc = new GroupComparer(sc.ColumnCollection,sc.ListIndex,sc.ObjectArray);
			IndexList.Add(gc);
			return gc;
		}
		
		
		protected static void CreateGroupeChildren(IndexList list,BaseComparer sc)
		{
			list.Add(sc);
		}
		
		#endregion
		
		#region Debug Code
		/*
		protected  static void ShowIndexList (IndexList list)
		{
			
			foreach (BaseComparer element in list) {
				string s = String.Format("{0} ",element.ObjectArray[0]);
				GroupComparer gc = element as GroupComparer;
				
				if ( gc != null) {
					s = s + "GroupHeader";
					if (gc.IndexList != null) {
						
						s = s + String.Format(" <{0}> Childs",gc.IndexList.Count);
					}
					System.Console.WriteLine(s);
					foreach (BaseComparer c in gc.IndexList) {
							Console.WriteLine("---- {0}",c.ObjectArray[0]);
					}
				}
			}
		}
		*/
		#endregion
		
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
		
		
		public virtual bool IsSorted {get;set;}
		
		
		
		public bool IsGrouped {get;set;}
		
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
		
		
		public virtual void Group()
		{
			this.indexList.Clear();
			this.IsGrouped = true;
			
		}
		
		public virtual void Bind() 
		{
			
		}
		
		
		public  virtual void Fill(IDataItem item)
		{
		}
		
		
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
