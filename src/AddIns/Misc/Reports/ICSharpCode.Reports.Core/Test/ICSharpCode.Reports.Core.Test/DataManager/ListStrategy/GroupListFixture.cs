// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager.ListStrategy
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
			var dataNavigator = PrepareStringGrouping();
			Assert.That(dataNavigator.IsGrouped,Is.True);
		}
		
		
		[Test]
		public void AvaiableFields_Should_Be_Set()
		{
			var dataNavigator = PrepareStringGrouping();
			dataNavigator.MoveNext();
			IDataNavigator child = dataNavigator.GetChildNavigator;
			AvailableFieldsCollection availableFieldsCollection = child.AvailableFields;
			Assert.That(availableFieldsCollection,Is.Not.Null);
			Assert.That(availableFieldsCollection.Count,Is.GreaterThan(0));
		}
		
		#region group by StringValue
		
		[Test]
		public void Has_Children()
		{
			var dataNavigator = PrepareStringGrouping();
			while (dataNavigator.MoveNext()) {
				Assert.That(dataNavigator.HasChildren,Is.True);
			}
		}
		
		
		[Test]
		public void Can_Read_Child_Count ()
		{
			var dataNavigator = PrepareStringGrouping();
			while (dataNavigator.MoveNext())
			{
				if (dataNavigator.HasChildren)
				{
					var childNavigator = dataNavigator.GetChildNavigator;
					Assert.That(childNavigator.Count,Is.GreaterThan(0));
				}
			}
		}
		
		
		[Test]
		public void Can_FillChild()
		{
			var dataNavigator = PrepareStringGrouping();
			while (dataNavigator.MoveNext()) {
				if (dataNavigator.HasChildren)
				{
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						Assert.That(dataNavigator.HasChildren,Is.True);
						
						// we know that current is a 'contributor'
						Contributor c = dataNavigator.Current as Contributor;
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
			var dataNavigator = PrepareDateTimeGrouping();
			
			Console.WriteLine("start datetime");
			while (dataNavigator.MoveNext()) {
				if (dataNavigator.HasChildren) {
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						
						Assert.That(dataNavigator.HasChildren,Is.True);
						// we know that current is a 'contributor'
						Contributor c = dataNavigator.Current as Contributor;
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
