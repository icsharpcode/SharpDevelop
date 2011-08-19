// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using ICSharpCode.Reports.Expressions.ReportingLanguage;

/// <summary>
/// BaseClass for all Datahandling Strategies
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 13.11.2005 15:26:02
/// </remarks>

namespace ICSharpCode.Reports.Core.ListStrategy
{
	
	internal static class SortExtension
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
	
	
	internal  abstract class BaseListStrategy :IDataViewStrategy,IEnumerator
	{

		private AvailableFieldsCollection availableFields;
		

		#region Constructor
		
		protected BaseListStrategy(ReportSettings reportSettings)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			this.ReportSettings = reportSettings;
			this.IndexList = new IndexList("IndexList");
			ExpressionEvaluator = new ExpressionEvaluatorFacade (null);
		}
		
		#endregion
		
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
				string groupValue = ExtractValue (element);
				
				if (compVal != groupValue) {
					childList = new IndexList();
					GroupComparer gc = CreateGroupHeader(element);
					gc.IndexList = childList;
				}
				CreateGroupedChildren(childList,element);
				compVal = groupValue;
			}			
//			ShowIndexList(IndexList);
		}
		
		
		static string ExtractValue(BaseComparer element)
		{
			string val = String.Empty;
			GroupColumn gc = element.ColumnCollection[0] as GroupColumn;
			if (gc !=  null) {
				val = element.ObjectArray[0].ToString();
			}
			return val;
		}
		
		
		protected GroupComparer CreateGroupHeader (BaseComparer sc)
		{
			GroupComparer gc = new GroupComparer(sc.ColumnCollection,sc.ListIndex,sc.ObjectArray);
			IndexList.Add(gc);
			return gc;
		}
		
		
		protected static void CreateGroupedChildren(IndexList list,BaseComparer sc)
		{
			list.Add(sc);
		}
		
		#endregion
		
		#region Debug Code
		
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
		
		
		#endregion
		
		public  virtual void Reset()
		{
			this.IndexList.CurrentPosition = -1;
		}
		
		
		public virtual object Current 
		{
			get {
				throw new NotImplementedException();
			}
		}
		
		
		public virtual bool MoveNext()
		{
			this.IndexList.CurrentPosition ++;
			return this.IndexList.CurrentPosition<this.IndexList.Count;
		}
		
		#region test
		
		public virtual CurrentItemsCollection FillDataRow()
		{
			return new CurrentItemsCollection();
		}

		
		public virtual object CurrentFromPosition (int pos)
		{
			throw new NotImplementedException();
		}
		

        public virtual CurrentItemsCollection FillDataRow(int pos)
        {
            return FillDataRow();
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
				return this.IndexList.CurrentPosition;
			}
			set {
				if ((value > -1)|| (value > this.IndexList.Count)){
					this.IndexList.CurrentPosition = value;
				}
			}
		}
		
		
		public virtual void Sort() 
		{
			this.IndexList.Clear();
		}
		
		
		public virtual void Group()
		{
			this.IndexList.Clear();
			if (ReportSettings.SortColumnsCollection.Count > 0) {
				ReportSettings.GroupColumnsCollection.AddRange(ReportSettings.SortColumnsCollection);
			}
		}
		
		public virtual void Bind() 
		{
			
		}
		
		
		public virtual  void Fill(int position,ReportItemCollection collection)
		{
			throw new NotImplementedException();
		}
		
		
		public  virtual void Fill(IDataItem item)
		{
		}
		
		
		protected ReportSettings ReportSettings {get;set;}
		
		public IndexList IndexList {get; set;}
		
		public IExpressionEvaluatorFacade ExpressionEvaluator {get;private set;}
		
		#endregion
		
		#region IDisposeable
		
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
				if (this.IndexList != null) {
					this.IndexList.Clear();
					this.IndexList = null;
				}
			}
			
			// Release unmanaged resources.
			// Set large fields to null.
			// Call Dispose on your base class.
		}
		
		#endregion
	
	}
}
