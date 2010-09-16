/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 09.09.2010
 * Time: 19:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager
{
	[TestFixture]
	public class GroupListFixture
	{
		
		ContributorCollection contributorCollection;
		
		
		[Test]
		public void GroupingCollection_Empty_IsGrouped_False()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			
			Assert.That(dataNav.IsGrouped == false);
		}
		
		
		[Test]
		public void GroupingCollection_Contains_IsGrouped_True()
		{
			var dataNav = PrepareStringGrouping();
			Assert.That(dataNav.IsGrouped,Is.True);
		}
		
		
		#region group by StringValue
		
		[Test]
		public void Has_Children()
		{
			var dataNav = PrepareStringGrouping();
			while (dataNav.MoveNext()) {
				Assert.That(dataNav.HasChildren,Is.True);
			}
		}
		
		
		[Test]
		public void Can_Read_Child_Count ()
		{
			var dataNav = PrepareStringGrouping();
			while (dataNav.MoveNext())
			{
				if (dataNav.HasChildren)
				{
					var childNavigator = dataNav.GetChildNavigator();
					Assert.That(childNavigator.Count,Is.GreaterThan(0));
				}
			}
		}
		
		
		[Test]
		public void Can_FillChild()
		{
			var dataNav = PrepareStringGrouping();
			while (dataNav.MoveNext()) {
				if (dataNav.HasChildren)
				{
					var childNavigator = dataNav.GetChildNavigator();
					do
					{
						Assert.That(dataNav.HasChildren,Is.True);
						
						// we know that current is a 'contributor'
						Contributor c = dataNav.Current as Contributor;
						string v2 = c.Last + " GroupVal :" +  c.GroupItem;
						Console.WriteLine(v2);
					}
					while (childNavigator.MoveNext());
				}
			}
		}
		
		
		#endregion
		
		#region GroupbyDataTime
		
		[Test]
		public void DateTimeCan_FillChild()
		{
			var dataNav = PrepareDateTimeGrouping();
			
			Console.WriteLine("start datetime");
			while (dataNav.MoveNext()) {
				if (dataNav.HasChildren) {
					var childNavigator = dataNav.GetChildNavigator();
					do
					{
						
						Assert.That(dataNav.HasChildren,Is.True);
						// we know that current is a 'contributor'
						Contributor c = dataNav.Current as Contributor;
						string v2 = c.Last + " GroupVal :" +  c.RandomDate;
						Console.WriteLine(v2);
					}
					while (childNavigator.MoveNext());
				}
				
			}
		}
		
		#endregion
		
		private IDataNavigator PrepareDateTimeGrouping ()
		{
			Console.WriteLine("PrepareDateTimeGrouping ()");
			
			GroupColumn gc = new GroupColumn("RandomDate",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			return dm.GetNavigator;
		}
		
		
		private IDataNavigator PrepareStringGrouping ()
		{
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			return dm.GetNavigator;
		}
		
		
		[TestFixtureSetUp]
		public void Init()
		{
			ContributorsList contributorsList = new ContributorsList();
			this.contributorCollection = contributorsList.ContributorCollection;
		}
		
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}
