// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Data;

using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager.TableStrategy
{
	
	[TestFixture]
	public class GroupTableFixture
	{
		
		DataTable table;
		
		[Test]
		public void AddGroupColumn ()
		{
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			
			rs.GroupColumnsCollection.Add(gc);
			Assert.AreEqual(1,rs.GroupColumnsCollection.Count);
		}
		
		
		[Test]
		public void AvaiableFieldsShouldSet()
		{
			var dataNavigator = PrepareStringGrouping();
			dataNavigator.MoveNext();
			IDataNavigator child = dataNavigator.GetChildNavigator;
			AvailableFieldsCollection availableFieldsCollection = child.AvailableFields;
			Assert.That(availableFieldsCollection,Is.Not.Null);
			Assert.That(availableFieldsCollection.Count,Is.GreaterThan(0));
		}
		
		
		#region Group by String
		
		[Test]
		public void HasChildren()
		{
			var dataNavigator = PrepareStringGrouping();
			while (dataNavigator.MoveNext()) {
				Assert.That(dataNavigator.HasChildren,Is.True);
			}
		}
		
		[Test]
		public void ReadChildCount ()
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
		public void CeckGrouping()
		{
			var dataNavigator = PrepareStringGrouping();
			string compare = string.Empty;
			
			while (dataNavigator.MoveNext())
			{
				DataRow dr = dataNavigator.Current as DataRow;
				var result = dr[3].ToString();
				Assert.That (compare,Is.LessThan(result));
				compare =  result;
			}
		}
			
		
		[Test]
		public void FillChild()
		{
			var dataNavigator = PrepareStringGrouping();
			string compare = string.Empty;
			
			while (dataNavigator.MoveNext())
			{
				DataRow dr = dataNavigator.Current as DataRow;
				Assert.That (compare,Is.LessThan(dr[3].ToString()));
				if (dataNavigator.HasChildren)
				{
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						Assert.That(dataNavigator.HasChildren,Is.True);
						DataRow r = childNavigator.Current as DataRow;
						Assert.That( r[3].ToString(),Is.Not.Empty);
//						string v2 = r["last"].ToString() + " GroupVal :" +  r[3].ToString();
//						Console.WriteLine(v2);
					}
					while (childNavigator.MoveNext());
				}
				compare =  dr[3].ToString();
			}
		}
		
		
		[Test]
		public void SortChildrenDescending()
		{
			ReportSettings rs = new ReportSettings();
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			rs.GroupColumnsCollection.Add(gc);
			
			SortColumn sc = new SortColumn("Last",ListSortDirection.Descending);
			rs.SortColumnsCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			var dataNavigator = dm.GetNavigator;
			
			string compare = String.Empty;
			
			while (dataNavigator.MoveNext())
			{
				DataRow dr = dataNavigator.Current as DataRow;
				var result = dr[3].ToString();
				Assert.That(compare,Is.LessThan(result));
				if (dataNavigator.HasChildren)
				{
					string compareChild = String.Empty;
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						DataRow childRow = childNavigator.Current as DataRow;
						var childResult = childRow[1].ToString();
						if (!String.IsNullOrEmpty(compareChild)) {
							Assert.LessOrEqual(childResult,compareChild);
						}
//						Console.WriteLine("\t{0}",childResult);
						compareChild = childResult;
					}
					while (childNavigator.MoveNext());
				}
				compare = result;
			}
		}
		
		
		#endregion
		
		#region GroupbyDataTime
		
		[Test]
		public void DateTimeCanFillChild()
		{
			var dataNavigator = PrepareDateTimeGrouping();
			var compare = System.DateTime.MinValue;
			
			while (dataNavigator.MoveNext()) 
			{
				
				DataRow dr = dataNavigator.Current as DataRow;
				var result = Convert.ToDateTime(dr[5]);
				Assert.That (compare,Is.LessThan(result));
				
				if (dataNavigator.HasChildren)
				{
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						Assert.That(dataNavigator.HasChildren,Is.True);
						DataRow r = childNavigator.Current as DataRow;
						Assert.That( r[3].ToString(),Is.Not.Empty);
//						string v2 = r["last"].ToString() + " GroupVal :" +  r[5].ToString();
//						Console.WriteLine(v2);
					}
					while (childNavigator.MoveNext());
				}
				compare =  Convert.ToDateTime(dr[5]);
			}
		}
		
		
		[Test]
		public void DataTimeHasChildren()
		{
			var dataNav = PrepareDateTimeGrouping();
			while (dataNav.MoveNext()) {
				Assert.That(dataNav.HasChildren,Is.True);
			}
		}
		
		#endregion
		
		#region Try make recursive with ChildNavigator
	
		[Test]
		public void GetChildNavigator ()
		{
//			Console.WriteLine("Start Recusive Version");
			var dataNavigator = PrepareStringGrouping();
			
			while (dataNavigator.MoveNext()) {
				if (dataNavigator.HasChildren) {
					DataRow r = dataNavigator.Current as DataRow;
					
					string v2 = r["last"].ToString() + " GroupVal :" +  r[3].ToString() ;
					IDataNavigator child = dataNavigator.GetChildNavigator;
					
//					Console.WriteLine(v2);
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
					IDataNavigator child = startNavigator.GetChildNavigator;
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
			ReportSettings rs = new ReportSettings();
			GroupColumn gc = new GroupColumn("RandomDate",1,ListSortDirection.Ascending);
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
