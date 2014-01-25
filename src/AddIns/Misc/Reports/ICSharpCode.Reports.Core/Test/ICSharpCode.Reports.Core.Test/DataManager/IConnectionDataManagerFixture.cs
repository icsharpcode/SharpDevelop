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
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.DataAccess;
using ICSharpCode.Reports.Core.Project.Interfaces;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager
{
	[TestFixture]
	public class IConnectionDataManagerFixture
	{
		ContributorsList contributorsList;
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorNullDataAccessStrategy ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(new ReportSettings(),
			                                                                      (SqlDataAccessStrategy)null);
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorNullReportSettings ()
		{
			IDataAccessStrategy da = new MockDataAccessStrategy (new ReportSettings());
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(null,da);                                                               
		}
		
		
		[Test]		
		public void DefaultConstructor ()
		{
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);			                                                                     
			Assert.IsNotNull (dm,"IDataManager should not be 'null");
		}
		
		
		[Test]
		public void TableCountEqualListCount ()
		{
			
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			Assert.AreEqual(contributorsList.ContributorCollection.Count,
			                dm.GetNavigator.Count,
			                "TableCount should be equal listCount");
		}
		
		
		[Test]
		public void DataNavigatorNotNull()
		{
			
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			IDataNavigator dataNav = dm.GetNavigator;
			Assert.IsNotNull(dataNav,"Navigator should not be 'null'");
		}
		
		
		[Test]
		public void DataNavigatorCountEqualListCount ()
		{
		
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			IDataNavigator dataNav = dm.GetNavigator;
			Assert.AreEqual(this.contributorsList.ContributorCollection.Count,
			                dataNav.Count,
			                "Count of elements in Navigator should be equal listCount");
		}
		
		
		[Test]
		public void DataNavigatorCorrectPosition ()
		{
			
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			IDataNavigator dataNav = dm.GetNavigator;
			Assert.AreEqual(-1,dataNav.CurrentRow);
		}
		
		
//		[Test]
//		[ExpectedException(typeof(ArgumentException))]
//		public void ConstructorBadConnectionString ()
//		{
//			
//			ReportSettings rs = new ReportSettings();
//			rs.ConnectionString = "bad";
//			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
//			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
//		}
		
		[Test]
		public void CheckDataMember()
		{
			
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			ICSharpCode.Reports.Core.DataManager dm = (ICSharpCode.Reports.Core.DataManager)ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			Assert.AreEqual("ContributorTable",dm.DataMember);
		}
		
		
		[Test]
		public void CheckDataSource()
		{
			
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			ICSharpCode.Reports.Core.DataManager dm = (ICSharpCode.Reports.Core.DataManager)ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			Assert.IsAssignableFrom(typeof(System.Data.DataTable),dm.DataSource);
		}
		
		
		#region Setup/TearDown
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.contributorsList = new ContributorsList();
		}
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			if (this.contributorsList == null) {
				this.contributorsList = null;
			}
			
//			if (this.testTable != null) {
//				this.testTable = null;
//				this.testTable.Dispose();
//			}
		}
		
		#endregion
		
	}
}
