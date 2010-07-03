/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 01.07.2010
 * Time: 20:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Data;

using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager
{
	[TestFixture]
	public class GroupTableFixture
	{
		
		DataTable table;
		
		[Test]
		public void GroupingCollection_Empty_IsGrouped_False()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			
			Assert.That(dataNav.IsGrouped == false);
		}
		
		
		[Test]
		public void GroupingCollection_Contains_IsGrouped_False()
		{
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			Assert.That(dataNav.IsGrouped == true);
		}
		
		
		[Test]
		public void aaAddGroupToSettings ()
		{
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			
			rs.GroupColumnsCollection.Add(gc);
			Assert.AreEqual(1,rs.GroupColumnsCollection.Count);
		}
			
		
		
		[Test]
		public void aaa()
		{
			
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;

			while (dataNav.MoveNext()) {
				DataRow r = dataNav.Current as DataRow;
				string v2 = r["Groupitem"].ToString();
			}
		}
		
		
		
			                
		[TestFixtureSetUp]
		public void Init()
		{
			ContributorsList contributorsList = new ContributorsList();
			this.table = contributorsList.ContributorTable;
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}
