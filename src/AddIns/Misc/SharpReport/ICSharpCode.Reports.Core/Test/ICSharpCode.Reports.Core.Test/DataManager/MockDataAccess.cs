/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 10.12.2008
 * Zeit: 20:06
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Data;
using System.Data.Common;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Project.Interfaces;
using ICSharpCode.Reports.Core.Test.TestHelpers;

namespace ICSharpCode.Reports.Core.Test.DataManager
{
	/// <summary>
	/// Description of MockDataAccess.
	/// </summary>
	public class MockDataAccessStrategy:IDataAccessStrategy
	{
		ReportSettings reportSettings;
		
		public MockDataAccessStrategy(ReportSettings reportSettings)
		{
			this.reportSettings = reportSettings;
		}
		
		
		
		public bool OpenConnection ()
		{
			
			if (String.IsNullOrEmpty(reportSettings.ConnectionString)) {
				throw new ArgumentNullException("ConnectionString");
			}
			
			if (reportSettings.ConnectionString == "bad") {
				throw new ArgumentException();
			}
			return true;
		}
		
		public System.Data.DataSet ReadData()
		{
			ContributorsList contributorsList = new ContributorsList();
			DataSet ds = new DataSet();
			ds.Tables.Add(contributorsList.ContributorTable);
			return ds;
		}
		
	}
}
