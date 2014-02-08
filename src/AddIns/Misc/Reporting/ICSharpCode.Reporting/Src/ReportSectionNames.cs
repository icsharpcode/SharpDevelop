/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.02.2014
 * Time: 18:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reporting
{
	/// <summary>
	/// Description of ReportSectionNames.
	/// </summary>
	/// 
	public enum ReportSection {
			ReportHeader,
			ReportPageHeader,
			ReportDetail,
			ReportPageFooter,
			ReportFooter
	}
	
	public static class ReportSectionNames
	{
		public static string ReportHeader
		{
			get{ return ReportSection.ReportHeader.ToString();}
		}
		
		public static string ReportPageHeader
		{
			get{ return ReportSection.ReportPageHeader.ToString();}
		}
		
		
		public static string ReportDetail
		{
			get { return ReportSection.ReportDetail.ToString();}
		}
		
		public static string ReportPageFooter
		{
			get { return ReportSection.ReportPageFooter.ToString();}
		}
		
		public static string ReportFooter
		{
			get{ return ReportSection.ReportFooter.ToString();}
		}
	}
}
