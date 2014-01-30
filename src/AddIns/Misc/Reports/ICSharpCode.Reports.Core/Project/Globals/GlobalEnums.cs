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
