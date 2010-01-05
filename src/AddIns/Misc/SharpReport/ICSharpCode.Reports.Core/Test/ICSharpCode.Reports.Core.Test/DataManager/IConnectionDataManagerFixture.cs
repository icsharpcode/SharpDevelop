/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 27.11.2008
 * Zeit: 19:32
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Reports.Core;
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
		
		
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ConstructorBadConnectionString ()
		{
			
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "bad";
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
		}
		
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
		
		
		[Test]
		public void CheckIsSorted()
		{
			
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			ICSharpCode.Reports.Core.DataManager dm = (ICSharpCode.Reports.Core.DataManager)ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			Assert.IsFalse(dm.IsSorted,"IsSorted should be 'false'");
		}
		
		/*
		[Test]
		public void CheckIsGrouped()
		{
			IDataAccessStrategy da = new MockDataAccessStrategy ();
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			ICSharpCode.Reports.Core.DataManager dm = (ICSharpCode.Reports.Core.DataManager)ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			Assert.IsFalse(dm.IsGrouped,"IsGrouped should be 'false'");
		}
		
		
		[Test]
		public void CheckIsFitered()
		{
			IDataAccessStrategy da = new MockDataAccessStrategy ();
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			ICSharpCode.Reports.Core.DataManager dm = (ICSharpCode.Reports.Core.DataManager)ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			Assert.IsFalse(dm.IsFiltered,"IsFiltered should be 'false'");
		}
		*/
		
		
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
