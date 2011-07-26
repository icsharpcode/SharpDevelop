// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Reflection;

	/// <summary>
	/// Global used Enumerations
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 13.12.2004 09:39:14
	/// </remarks>
	/// 
namespace ICSharpCode.Reports.Core.Globals
{
	public sealed class GlobalEnums
	{
		private GlobalEnums() {	
		}
		
		public enum ItemsLayout{
			Left,
			Center,
			Right
		}
		
		public enum ReportLayout{
			ListLayout,
			TableLayout
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
		/// All available ReportItems
		/// </summary>
		public enum ReportItemType {
			ReportTextItem,
			ReportDataItem,
			ReportRowItem,
			ReportRectangleItem,
			ReportImageItem,
			ReportLineItem,
			ReportCircleItem,
			PageNumber,
			TodaysDate,
			ReportTableItem
		}
		/// <summary>
		/// FormSheet means a blank form with Labels, Lines and Checkboxes
		/// DataReport handles all Reports with Data
		/// </summary>
		public enum ReportType {
			FormSheet,
			DataReport,
		}
		
		/// <summary>
		/// Type of Sections used
		/// </summary>
		internal enum ReportSection {
			ReportHeader,
			ReportPageHeader,
			ReportDetail,
			ReportPageFooter,
			ReportFooter
		}

		
		///<summary>
		///Names of the different collections like Sorting,grouping etc
		/// </summary>
		
		public enum ParamCollectionname{
			AvailableColumns,
			SqlParameterCollection,
			Sortings,
			Groupings
		}
		
		public enum ImageSource{
			File,
			Database,
			External
		}
	}
}
