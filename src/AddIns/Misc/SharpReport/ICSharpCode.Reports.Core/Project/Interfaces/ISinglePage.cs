/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 02.02.2009
 * Zeit: 13:27
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Core.Interfaces
{
	public interface ISinglePage
	{
		void CalculatePageBounds(IReportModel reportModel);
		
		
		SectionBounds SectionBounds {get;set;}
		
		int StartRow {get;set;}
		int EndRow {get;set;}
		int PageNumber {get;set;}
		int TotalPages {get;set;}
		string ReportName {get;set;}
		string ReportFileName {get;set;}
		string ReportFolder {get;}
		DateTime ExecutionTime {get;set;}
		System.Collections.Hashtable ParameterHash {get;set;}
		IDataNavigator IDataNavigator {get;set;}
	}
}
