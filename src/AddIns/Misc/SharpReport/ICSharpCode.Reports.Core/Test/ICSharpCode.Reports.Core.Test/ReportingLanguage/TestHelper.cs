/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 12.06.2009
 * Zeit: 20:09
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage
{
	/// <summary>
	/// Description of TestHelper.
	/// </summary>
	public class TestHelper
	{
		
		public static SinglePage CreateSinglePage ()
		{
			SectionBounds sb = new SectionBounds(new ReportSettings(),false);
			SinglePage p = new SinglePage(sb,15);
		
			p.TotalPages = 25;
			p.ReportName = "SharpTestReport.srd";
			p.ReportFileName =@"c:\testreports\SharpTestReport.srd";
			p.ExecutionTime = new DateTime(2009,12,24,23,59,59);
		
			return p;
		}
		
	}
}
