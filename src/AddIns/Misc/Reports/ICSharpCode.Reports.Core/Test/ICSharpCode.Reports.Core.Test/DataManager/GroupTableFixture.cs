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
			var dataNav = PrepareStandardGrouping();
			Assert.That(dataNav.IsGrouped == true);
		}
		
		
		[Test]
		public void Has_Children()
		{
			var dataNav = PrepareStandardGrouping();
			while (dataNav.MoveNext()) {
				Assert.That(dataNav.HasChildren,Is.True);
			}
		}
		
		[Test]
		public void Can_Read_Child_Count ()
		{
			var dataNav = PrepareStandardGrouping();
			while (dataNav.MoveNext()) 
			{
				Assert.That(dataNav.ChildListCount,Is.GreaterThan(0));
			}
		}
		
		
		[Test]
		public void Can_FillChild()
		{
			var dataNav = PrepareStandardGrouping();
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
		
		
		void FillChildList (IDataNavigator nav)
		{
			BaseDataItem first= new BaseDataItem("First");
			BaseDataItem last  = new BaseDataItem("Last");
			ReportItemCollection ric = new ReportItemCollection();
			
			ric.Add(first);
			ric.Add(last);
			
			nav.SwitchGroup();
			do {
				nav.FillChild(ric);
				foreach (BaseDataItem element in ric) {
					Console.WriteLine("\t{0} - {1}",element.ColumnName,element.DBValue);
				}
			}
			while ( nav.ChildMoveNext());
		}
		
		
		[Test]
		public void Can_Read_Grouped_List()
		{
			var dataNav = PrepareStandardGrouping();
			while (dataNav.MoveNext()) {
				if (dataNav.HasChildren) {
					Assert.That(dataNav.HasChildren,Is.True);
					DataRow r = dataNav.Current as DataRow;
					string v2 = r["last"].ToString() + " GroupVal :" +  r[3].ToString();
					Console.WriteLine(v2);
					ReadChildList(dataNav);
				}
				
			}
		}
		
		
		void ReadChildList (IDataNavigator nav)
		{
			nav.SwitchGroup();
			do {
				var o = nav.ReadChild() as DataRow;
				string v = o.ItemArray[3].ToString();
				Console.WriteLine("\t {0}",v);
			}
			while ( nav.ChildMoveNext());
		}
		
		
		
		/*
	
		[Test]
		public void ChildNavigator_Can_Init()
		{
			var dataNav = PrepareStandardGrouping();
			Console.WriteLine("start list");
			while (dataNav.MoveNext())
			{
				DataRow r = dataNav.Current as DataRow;
				string v2 = r["Groupitem"].ToString();
				Console.WriteLine(" - {0}  ",v2);
				ChildNavigator childNav = dataNav.GetChildNavigator();
				Assert.That(childNav != null);
			}
		}
		
		*/
		
		
		/*
		
		[Test]
		public void ChildNavigator_All_Childs_Contains_Elements()
		{
			var dataNav = PrepareStandardGrouping();
			Console.WriteLine("start list");
			while (dataNav.MoveNext())
			{
				ChildNavigator childNav = dataNav.GetChildNavigator();
				Assert.That(childNav.IndexList.Count > 0);
			}
		}
		*/
				
		private IDataNavigator PrepareStandardGrouping ()
		{
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
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
