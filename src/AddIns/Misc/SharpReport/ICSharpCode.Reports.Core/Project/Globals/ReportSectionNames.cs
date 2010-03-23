/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 23.03.2010
 * Zeit: 20:08
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of ReportSectionNames.
	/// </summary>
	public static class ReportSectionNames
	{
		public static string ReportHeader
		{
			get{ return GlobalEnums.ReportSection.ReportHeader.ToString();}
		}
		
		public static string ReportPageHeader
		{
			get{ return GlobalEnums.ReportSection.ReportPageHeader.ToString();}
		}
		
		
		public static string ReportDetail
		{
			get { return GlobalEnums.ReportSection.ReportDetail.ToString();}
		}
		
		public static string ReportPageFooter
		{
			get { return GlobalEnums.ReportSection.ReportPageFooter.ToString();}
		}
		
		public static string ReportFooter
		{
			get{ return GlobalEnums.ReportSection.ReportFooter.ToString();}
		}
	}
}
