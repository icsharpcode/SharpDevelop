/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.07.2010
 * Time: 18:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Data;
using NUnit.Framework;
using ICSharpCode.Reports.Core.Test.TestHelpers;

namespace ICSharpCode.Reports.Core.Test.DataManager.Strategy
{

	[TestFixture]
	public class TableStrategyFixture
	{
		
		DataTable table;
		
		[Test]
		public void TableStrategy_CanInit()
		{
			TableStrategy ts = new TableStrategy(this.table,new ReportSettings());
			Assert.That(ts != null);
		}
		
		
		[Test]
		public void TableStrategy_Set_IsSorted()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnCollection.Add(sc);
			TableStrategy ts = new TableStrategy(table,rs);
			ts.Bind();
			Assert.That(ts.IsSorted == true);
		}
		
		
		[Test]
		public void CanSort_String_Ascending()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnCollection.Add(sc);

			TableStrategy ts = new TableStrategy(table,rs);
			ts.Bind();
			string v1 = String.Empty;
			foreach (BaseComparer element in ts.IndexList) {
				string v2 = element.ObjectArray[0].ToString();
				Assert.LessOrEqual(v1,v2);
				v1 = v2;
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
