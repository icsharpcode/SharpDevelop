/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 09.07.2009
 * Zeit: 19:57
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of IReportModel.
	/// </summary>
	public interface IReportModel
	{
		BaseSection ReportHeader {get;}
		BaseSection PageHeader {get;}
		BaseSection DetailSection {get;}
		BaseSection PageFooter {get;}
		BaseSection ReportFooter {get;}
		ReportSettings ReportSettings {get;set;}
		GlobalEnums.PushPullModel DataModel {get;}
		ReportSectionCollection SectionCollection {get;}
	}
}
