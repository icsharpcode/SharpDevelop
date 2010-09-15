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
		public void GroupingCollection_EmptyGrouping_IsGrouped_False()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			
			Assert.That(dataNav.IsGrouped == false);
		}
		
		
		#region Group by String
		
		[Test]
		public void GroupingCollection_ContainsGrouping_IsGrouped_True()
		{
			var dataNav = PrepareStringGrouping();
			Assert.That(dataNav.IsGrouped == true);
		}
		
		
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
				Assert.That(dataNav.ChildListCount,Is.GreaterThan(0));
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
					Assert.That(dataNav.HasChildren,Is.True);
					DataRow r = dataNav.Current as DataRow;
					string v2 = r["last"].ToString() + " GroupVal :" +  r[5].ToString();
					Console.WriteLine(v2);
					DateTimeChildList(dataNav);
				}
				
			}
		}
		
		
		[Test]
		public void DataTimeCollection_Contains_IsGrouped_False()
		{
			var dataNav = PrepareDateTimeGrouping();
			Assert.That(dataNav.IsGrouped == true);
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
		
		
		
		#region Read-Fill Child List
		
		[Test]
		public void Can_FillChild()
		{
			var dataNav = PrepareStringGrouping();
			while (dataNav.MoveNext()) {
				
				if (dataNav.HasChildren)
				{
					var n = dataNav.GetChildNavigator();
					do
					{
						Assert.That(dataNav.HasChildren,Is.True);
						DataRow r = dataNav.Current as DataRow;
						string v2 = r["last"].ToString() + " GroupVal :" +  r[3].ToString();
						Console.WriteLine(v2);
					}
					while (n.MoveNext());
				}
			}
		}
		
		
		
		private void DateTimeChildList (IDataNavigator nav)
		{
			BaseDataItem first= new BaseDataItem("First");
			BaseDataItem last  = new BaseDataItem("Last");
			BaseDataItem datetime = new BaseDataItem("RandomDate");
			
			ReportItemCollection ric = new ReportItemCollection();
			
			ric.Add(first);
			ric.Add(last);
			ric.Add(datetime);
			/*
			nav.SwitchGroup();
			do {
				nav.FillChild(ric);
				foreach (BaseDataItem element in ric) {
					Console.WriteLine("\t{0} - {1} ",element.ColumnName,element.DBValue);
				}
			}
			while ( nav.ChildMoveNext());
			*/
		}
		
		
		private void FillChildList (IDataNavigator nav)
		{
			BaseDataItem first= new BaseDataItem("First");
			BaseDataItem last  = new BaseDataItem("Last");
			
			ReportItemCollection ric = new ReportItemCollection();
			
			ric.Add(first);
			ric.Add(last);
			/*
			nav.SwitchGroup();
			do {
				nav.FillChild(ric);
				foreach (BaseDataItem element in ric) {
					Console.WriteLine("\t{0} - {1} ",element.ColumnName,element.DBValue);
				}
			}
			while ( nav.ChildMoveNext());
			*/
		}
		
		#endregion
		
		
		#region Try make recursive with ChildNavigator
		/*
		public void Can_FillChild()
		{
			var dataNav = PrepareStringGrouping();
			while (dataNav.MoveNext()) {
				if (dataNav.HasChildren) {
					Assert.That(dataNav.HasChildren,Is.True);
					DataRow r = dataNav.Current as DataRow;
					string v2 = r["last"].ToString() + " GroupVal :" +  r[3].ToString();
					Console.WriteLine(v2);
					FillChildList(dataNav);
				}
				
			}
		}
		*/
		
		[Test]
		public void Can_Get_ChildNavigator ()
		{
			Console.WriteLine("Start Recusive Version");
			var dataNav = PrepareStringGrouping();
			
			while (dataNav.MoveNext()) {
				if (dataNav.HasChildren) {
					DataRow r = dataNav.Current as DataRow;
					
					
					IDataNavigator child = dataNav.GetChildNavigator();
					string v2 = r["last"].ToString() + " GroupVal :" +  r[3].ToString() ;
					Console.WriteLine(v2);
					Assert.That (child,Is.Not.Null);
					reccall(child);
				}
			}
			Console.WriteLine("End Recusive Version");
		}
		
		
		[Test]
		public void RecursiveCall_Childs ()
		{
			var dataNav = PrepareStringGrouping();
			dataNav.MoveNext();
			Console.WriteLine("--------------start rec ------------");
			reccall (dataNav);
			Console.WriteLine("--------------end rec ------------");
		}
		
		
		private void reccall (IDataNavigator startNavigator)
		{
			do
			{
				DataRow r = startNavigator.Current as DataRow;
				string v1 = r["last"].ToString() + " :" +  r[3].ToString();
				Console.WriteLine("\t {0}",v1);
				if (startNavigator.HasChildren) {
					IDataNavigator child = startNavigator.GetChildNavigator();
					Console.WriteLine("header {0} - Child_Count:{1}",v1,child.Count);
					reccall (child);
				}
				
			} while (startNavigator.MoveNext());
		}
	
		
		
		private void reccall_1 (IDataNavigator startNavigator)
		{
			Console.WriteLine("start rec ");
			do
			{
				DataRow r = startNavigator.Current as DataRow;
				string v1 = r["last"].ToString() + " :" +  r[3].ToString();
				Console.WriteLine(v1);
				if (startNavigator.HasChildren) {
					IDataNavigator child = startNavigator.GetChildNavigator();
					
					if (child.HasChildren) {
						do {
							Console.WriteLine ("children");
							//reccall (child);
						}
						while (child.MoveNext()) ;
					}
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
