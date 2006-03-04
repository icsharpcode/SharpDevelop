/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 08.10.2005
 * Time: 17:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace SharpReportCore {
 	public interface IDataContainer {
 		/// <summary>
 		/// Setup the Databinding, return true if databinding was ok
 		/// </summary>
 		bool DataBind();
 		/// <summary>
 		/// Move to next row
 		/// </summary>
 		/// <returns></returns>
 		void Skip();
 		/// <summary>
 		/// reste Datasource,move to position 0
 		/// </summary>
 		void Reset ();
 		/// <summary>
 		/// Reads one row of data and fill the
 		/// <see cref="ReportItemCollection"></see>
 		void FetchData (ReportItemCollection collection);

 		int Count {
 			get;
 		}
 		/// <summary>
 		/// Get the Position in List
 		/// </summary>
 		int CurrentRow {
 			get;
 		}
 		/// <summary>
 		/// Returns true when there are more Data to Read, false if we are on the end
 		/// or the list is empty
 		/// </summary>
 		bool HasMoreData  {
 			get;
 		}
 		
 		/// <summary>
 		/// Set/read a valid FilterString,  <see cref="System.Datat.DataView"></see>
 		/// </summary>
 		string Filter {
 			get;set;
 		}
 		
 	}
}
