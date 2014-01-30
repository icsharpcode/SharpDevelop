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
