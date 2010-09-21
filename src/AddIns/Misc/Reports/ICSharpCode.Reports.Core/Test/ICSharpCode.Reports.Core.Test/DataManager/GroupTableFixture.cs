// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		public void GroupingCollection_EmptyGrouping_IsGrouped_False()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			
			Assert.That(dataNav.IsGrouped == false);
		}
		
		
		[Test]
		public void AvaiableFields_Should_Be_Set()
		{
			var dataNavigator = PrepareStringGrouping();
			dataNavigator.MoveNext();
			IDataNavigator child = dataNavigator.GetChildNavigator();
			AvailableFieldsCollection availableFieldsCollection = child.AvailableFields;
			Assert.That(availableFieldsCollection,Is.Not.Null);
			Assert.That(availableFieldsCollection.Count,Is.GreaterThan(0));
		}
		
		#region Group by String
		
		[Test]
		public void GroupingCollection_ContainsGrouping_IsGrouped_True()
		{
			var dataNavigator = PrepareStringGrouping();
			Assert.That(dataNavigator.IsGrouped == true);
		}
		
		
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
					var childNavigator = dataNavigator.GetChildNavigator();
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
					var childNavigator = dataNavigator.GetChildNavigator();
					do
					{
						Assert.That(dataNavigator.HasChildren,Is.True);
						DataRow r = dataNavigator.Current as DataRow;
						string v2 = r["last"].ToString() + " GroupVal :" +  r[3].ToString();
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
			while (dataNavigator.MoveNext()) {
				if (dataNavigator.HasChildren)
				{
					var childNavigator = dataNavigator.GetChildNavigator();
					do
					{
						Assert.That(dataNavigator.HasChildren,Is.True);
						DataRow r = dataNavigator.Current as DataRow;
						string v2 = r["last"].ToString() + " GroupVal :" +  r[5].ToString();
						Console.WriteLine(v2);
					}
					while (childNavigator.MoveNext());
				}
			}
		}
		
		
		[Test]
		public void DataTimeCollection_Contains_IsGrouped_False()
		{
			var dataNavigator = PrepareDateTimeGrouping();
			Assert.That(dataNavigator.IsGrouped == true);
		}
		
		
		[Test]
		public void DataTime_Has_Children()
		{
			var dataNav = PrepareDateTimeGrouping();
			while (dataNav.MoveNext()) {
				Assert.That(dataNav.HasChildren,Is.True);
			}
		}
		
		
		#endregion
		
	
		#region Try make recursive with ChildNavigator
	
		
		[Test]
		public void Can_Get_ChildNavigator ()
		{
			Console.WriteLine("Start Recusive Version");
			var dataNavigator = PrepareStringGrouping();
			
			while (dataNavigator.MoveNext()) {
				if (dataNavigator.HasChildren) {
					DataRow r = dataNavigator.Current as DataRow;
					
					
					IDataNavigator child = dataNavigator.GetChildNavigator();
					string v2 = r["last"].ToString() + " GroupVal :" +  r[3].ToString() ;
					Console.WriteLine(v2);
					Assert.That (child,Is.Not.Null);
					RecursiveCall(child);
				}
			}
		}
		
		
		
		private void RecursiveCall (IDataNavigator startNavigator)
		{
			do
			{
				DataRow r = startNavigator.Current as DataRow;
				string v1 = r["last"].ToString() + " :" +  r[3].ToString();
				Console.WriteLine("\t {0}",v1);
				if (startNavigator.HasChildren) {
					IDataNavigator child = startNavigator.GetChildNavigator();
					Console.WriteLine("header {0} - Child_Count:{1}",v1,child.Count);
					RecursiveCall (child);
				}
				
			} while (startNavigator.MoveNext());
		}
	
		
		#endregion
		
		
		
		private IDataNavigator PrepareStringGrouping ()
		{
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			return dm.GetNavigator;
		}
		
		
		private IDataNavigator PrepareDateTimeGrouping ()
		{
			Console.WriteLine("PrepareDateTimeGrouping ()");
			
			GroupColumn gc = new GroupColumn("RandomDate",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			return dm.GetNavigator;
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
