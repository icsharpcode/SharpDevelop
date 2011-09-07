// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Data;
using NUnit.Framework;
using ICSharpCode.Reports.Core.Test.TestHelpers;

namespace ICSharpCode.Reports.Core.Test.DataManager.TableStrategy
{

	[TestFixture]
	public class TableStrategyFixture
	{
		
		DataTable table;
		
		[Test]
		public void TableStrategy_CanInit()
		{
			var ts = new ICSharpCode.Reports.Core.ListStrategy.TableStrategy(this.table,new ReportSettings());
			Assert.That(ts != null);
		}
		
	
		#region Grouping
		
		[Test]
		public void CanGroup_All_Elements_are_GroupComparer ()
		{
			GroupColumn groupComparer = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ICSharpCode.Reports.Core.ListStrategy.TableStrategy tableStrategy = GroupTableStrategyFactory (groupComparer);
			tableStrategy.Bind();
			foreach (BaseComparer element in tableStrategy.IndexList) 
			{
				GroupComparer checkComparer = element as GroupComparer;
				Assert.IsNotNull(checkComparer);
			}
		}
		
			
		[Test]
		public void CanGroup_All_Elements_Contains_Children ()
		{
			GroupColumn groupComparer = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			var  tableStrategy =GroupTableStrategyFactory (groupComparer);
			tableStrategy.Bind();
			foreach (BaseComparer element in tableStrategy.IndexList) 
			{
				GroupComparer checkComparer = element as GroupComparer;
				Assert.That(checkComparer.IndexList.Count > 0);
			}
		}
		
		#endregion
		
		
		#region Sorting
		
		[Test]
		public void CanSort_String_Ascending()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Ascending);
			var tableStrategy = SortTableStrategyFactory(sc);
			tableStrategy.Bind();
			string v1 = String.Empty;
			foreach (BaseComparer element in tableStrategy.IndexList) {
				string v2 = element.ObjectArray[0].ToString();
				Assert.LessOrEqual(v1,v2);
				v1 = v2;
			}
		}
		
		
		[Test]
		public void CanSort_DateTime_Ascending()
		{
			SortColumn sc = new SortColumn("RandomDate",System.ComponentModel.ListSortDirection.Ascending,
			                               typeof(System.Int16),false);

			var tableStrategy =SortTableStrategyFactory (sc);
		
			DateTime d1 = new DateTime(1,1,1);
			
			foreach (BaseComparer element in tableStrategy.IndexList) {

				DateTime d2 = Convert.ToDateTime(element.ObjectArray[0].ToString());
				Assert.LessOrEqual(d1,d2);
				d1 = d2;
			}
		}
		
		
		[Test]
		public void CanSort_DateTime_Descending()
		{
			SortColumn sc = new SortColumn("RandomDate",System.ComponentModel.ListSortDirection.Descending,
			                               typeof(System.Int16),false);

			var tableStrategy = SortTableStrategyFactory(sc);
			DateTime d1 = new DateTime(1,1,1);
			
			foreach (BaseComparer element in tableStrategy.IndexList) {

				DateTime d2 = Convert.ToDateTime(element.ObjectArray[0].ToString());
				Assert.GreaterOrEqual(d1,d2);
				d1 = d2;
			}
		}
		
		
		[Test]
		public void CanSort_Ascending_By_TwoColumns()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Ascending);
			SortColumn sc1 = new SortColumn("RandomInt",System.ComponentModel.ListSortDirection.Ascending);
			
			ReportSettings reportSettings = new ReportSettings();
			reportSettings.SortColumnsCollection.Add(sc);
			reportSettings.SortColumnsCollection.Add(sc1);
			
			var tableStrategy = new ICSharpCode.Reports.Core.ListStrategy.TableStrategy(this.table,reportSettings);
			string v1 = String.Empty;
			
			foreach (BaseComparer element in tableStrategy.IndexList) {
				string v2 = element.ObjectArray[0].ToString();
				Assert.LessOrEqual(v1,v2);
				v1 = v2;
			}
		}
		
		
		#endregion
		
		
		#region Setup/TearDown
		
		private ICSharpCode.Reports.Core.ListStrategy.TableStrategy SortTableStrategyFactory (SortColumn sortColumn)
		{
			var reportSettings = new ReportSettings();
			reportSettings.SortColumnsCollection.Add(sortColumn);
			var tableStrategy = new ICSharpCode.Reports.Core.ListStrategy.TableStrategy(this.table,reportSettings);
			return tableStrategy;
		}
		
		
		private ICSharpCode.Reports.Core.ListStrategy.TableStrategy GroupTableStrategyFactory (GroupColumn sortColumn)
		{
			var reportSettings = new ReportSettings();
			reportSettings.GroupColumnsCollection.Add(sortColumn);
			var tableStrategy = new ICSharpCode.Reports.Core.ListStrategy.TableStrategy(this.table,reportSettings);
			return tableStrategy;
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
		
		#endregion
	}
}
