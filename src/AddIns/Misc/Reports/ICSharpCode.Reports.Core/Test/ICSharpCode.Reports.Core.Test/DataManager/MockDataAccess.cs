// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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