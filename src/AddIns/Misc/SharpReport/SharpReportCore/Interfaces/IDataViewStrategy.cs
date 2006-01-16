

using System;
using System.Collections;
using System.ComponentModel;

namespace SharpReportCore{
	public interface IDataViewStrategy {
		
		/// <summary>
		/// Sort the DataSource
		/// </summary>
		void Sort ();
		/// <summary>
		/// Reset Datasource to first position
		/// </summary>
		void Reset();
		/// <summary>
		/// Establish Databinding
		/// Allway'S call this method before any other operations
		/// </summary>
		void Bind();
		/// <summary>
		/// Fill's a <see cref="IItemRenderer"></see> with Data from the CurrentRow
		/// </summary>
		void Fill (IItemRenderer item);
		
		/// <summary>
		/// List of Available (Data)Fields
		/// </summary>
		
		ColumnCollection AvailableFields {
			get;
		}
		
		/// <summary>
		/// Build a Hierarchical List of grouped DataRows
		/// </summary>
		/// <param name="sourceList"></param>
		/// <returns></returns>

		IHierarchicalEnumerable  IHierarchicalEnumerable{
			get;
		}
		
		int Count {
			get;
		}
		
		/// <summary>
 		/// Get the Position in List
 		/// </summary>
 		int CurrentRow {
 			get;set;
 		}
 		/// <summary>
 		/// Returns true when there are more Data to Read, false if we are on the end
 		/// or the list is empty
 		/// </summary>
 		bool HasMoreData  {
 			get;
 		}
 		/// <summary>
 		/// SharpReportCore is sorted
 		/// </summary>
 		bool IsSorted {
 			get;
 		}
 		/// <summary>
 		/// SharpReportCore is filtered
 		/// </summary>
 		bool IsFiltered{
 			get;
 		}
 		/// <summary>
 		/// SharpReportCore is grouped
 		/// </summary>
 		bool IsGrouped {
 			get;
 		}
 		event ListChangedEventHandler ListChanged;
 		
 		/// <summary>
 		/// Fired each tim the grouping will change, this means theGroupLevel changes up or down
 		/// </summary>
 		event GroupChangedEventHandler GroupChanged;
	}
}
