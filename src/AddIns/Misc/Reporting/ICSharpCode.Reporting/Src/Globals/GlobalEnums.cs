/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2013
 * Time: 17:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reporting.Globals
{
	/// <summary>
	/// Description of GlobalEnums.
	/// </summary>
	public  class GlobalEnums
	{
		public enum ReportSection {
			ReportHeader,
			ReportPageHeader,
			ReportDetail,
			ReportPageFooter,
			ReportFooter
		}
		
		///<summary>Technics to get the data
		/// Push : report get's a ready filld dataset or something tah implements IList
		/// Pull : report has to fill data by themself
		/// FormSheet : FormSheet report, just labels and images are allowed
		/// </summary>
		/// 
		public enum PushPullModel {
			PushData,
			PullData,
			FormSheet
		}
		
		
		/// <summary>
		/// FormSheet means a blank form with Labels, Lines and Checkboxes
		/// DataReport handles all Reports with Data
		/// </summary>
		public enum ReportType {
			FormSheet,
			DataReport,
		}
		
		
	}
}
