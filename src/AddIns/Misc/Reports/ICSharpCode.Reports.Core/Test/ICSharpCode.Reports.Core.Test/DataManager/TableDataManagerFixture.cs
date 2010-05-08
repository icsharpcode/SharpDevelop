/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 27.11.2008
 * Zeit: 19:31
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections;
using System.Data;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager
{
	[TestFixture]
	public class TableDataManagerFixture
	{

		DataTable table;
		[Test]
		public void DefaultConstructor()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			Assert.IsNotNull(dm,"IDataManager should not be 'null'");
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructoreWithNullTable()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance((System.Data.DataTable)null,new ReportSettings());
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorWithNullSettings()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,null);
			Assert.IsNotNull( dm);
		}
		
		
		[Test]
		public void DataNavigatorNotNull ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			Assert.IsNotNull( dm);
			DataNavigator dataNav = dm.GetNavigator;
			Assert.IsNotNull (dataNav,"Navigator should not be 'null'");
		}
		
		[Test]
		public void DataNavigatorOnCorrectPosition ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			Assert.AreEqual(-1,dataNav.CurrentRow);
		}
		
		
		[Test]
		public void DataNavigatorHasMoreDataReturnsTrue ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			Assert.IsTrue(dataNav.HasMoreData);
		}
		
		[Test]
		public void DataNavigatorHasMoreDataReturnsFalse ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			while (dataNav.MoveNext()) {
//				DataRow r = dataNav.Current as DataRow;
//				string v2 = r["last"].ToString();
			}
			Assert.IsFalse(dataNav.HasMoreData);
		}
		
		
		[Test]
		public void DataNavigatorSortedEqualsFalse ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			Assert.IsFalse(dataNav.IsSorted);
			
		}
		[Test]
		public void DataNavigatorCountEqualListCount ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			Assert.IsNotNull(dm);
			IDataNavigator n = dm.GetNavigator;
			Assert.AreEqual(this.table.Rows.Count,n.Count,"Count of ListItems should be equal");
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ColumnNotExistThrow()
		{
			SortColumn sc = new SortColumn("notexist",System.ComponentModel.ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
		}
		
		
		#region Sorting
		
		[Test]
		public void SortAscendingByOneColumn()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			string v1 = String.Empty;
			while (dataNav.MoveNext()) {
				DataRow r = dataNav.Current as DataRow;
				string v2 = r["last"].ToString();
				Assert.LessOrEqual(v1,v2);
				v1 = v2;
			}
			Assert.IsTrue(dataNav.IsSorted);
		}
		
		/*
		[Test]
		[Ignore("Sort of integer not working")]
		public void SortAscendingByInteger()
		{
			SortColumn sc = new SortColumn("RandomInt",System.ComponentModel.ListSortDirection.Ascending,
			                               typeof(System.Int16),false);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			string v1 = String.Empty;
			while (dataNav.MoveNext()) {
				DataRow r = dataNav.Current as DataRow;
				string v2 = r["randomInt"].ToString();
//				string ss = String.Format("< {0} > <{1}>",v1,v2);
				//Console.WriteLine(v2);
				Assert.LessOrEqual(v1,v2);
				v1 = v2;
			}
			Assert.IsTrue(dataNav.IsSorted);
		}
		*/
		
		[Test]
		public void SortAscendingByDateTime()
		{
			SortColumn sc = new SortColumn("RandomDate",System.ComponentModel.ListSortDirection.Ascending,
			                               typeof(System.Int16),false);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			DateTime d1 = new DateTime(1,1,1);
			while (dataNav.MoveNext()) {
				DataRow r = dataNav.Current as DataRow;
				DateTime d2 = Convert.ToDateTime(r["randomDate"]);
//				string ss = String.Format("<{0}>",d2);
//				Console.WriteLine(ss);
				Assert.LessOrEqual(d1,d2);
				d1 = d2;
			}
			Assert.IsTrue(dataNav.IsSorted);
		}
		
		
		
		[Test]
		public void SortDescendingByDateTime()
		{
			SortColumn sc = new SortColumn("RandomDate",System.ComponentModel.ListSortDirection.Descending,
			                               typeof(System.Int16),false);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			DateTime d1 = new DateTime(2099,12,30);
			while (dataNav.MoveNext()) {
				DataRow r = dataNav.Current as DataRow;
				DateTime d2 = Convert.ToDateTime(r["randomDate"]);
//				string ss = String.Format("<{0}>",d2);
//				Console.WriteLine(ss);
				Assert.GreaterOrEqual(d1,d2);
				d1 = d2;
			}
			Assert.IsTrue(dataNav.IsSorted);
		}
		
		
		[Test]
		public void SortAscendingByTwoColumns()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Ascending);
			SortColumn sc1 = new SortColumn("RandomInt",System.ComponentModel.ListSortDirection.Ascending);
			
			ReportSettings rs = new ReportSettings();
			rs.SortColumnCollection.Add(sc);
			rs.SortColumnCollection.Add(sc1);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			string v1 = String.Empty;
			while (dataNav.MoveNext()) {
				DataRow r = dataNav.Current as DataRow;
				string v2 = r["last"].ToString() + "-" + r["RandomInt"].ToString();
				//string ss = String.Format("< {0} > <{1}>",v1,v2);
				//Console.WriteLine(ss);
				//Console.WriteLine(v2);
				Assert.LessOrEqual(v1,v2);
				v1 = v2;
			}
			Assert.IsTrue(dataNav.IsSorted);
		}
		
		[Test]
		public void SortDescendingByOneColumn()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Descending);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			string compareTo = "z";
			while (dataNav.MoveNext()) {
				DataRow r = dataNav.Current as DataRow;
				string actual = r["last"].ToString();
				Assert.GreaterOrEqual(compareTo,actual);
//				string ss = String.Format("< {0} > <{1}>",compareTo,actual);
//				Console.WriteLine(ss);
				compareTo = actual;
			}
			Assert.IsTrue(dataNav.IsSorted);
		}
		
		
		#endregion
		
		
		#region Standart Enumerator
		
		[Test]
		public void IEnumerableStartFromBegin ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,
			                                                                      new ReportSettings());
			IDataNavigator dn = dm.GetNavigator;
			IEnumerable ie = dn as IEnumerable;
			IEnumerator en = ie.GetEnumerator();
			int start = 0;
			en.MoveNext();
			do {
				DataRow v = dn.Current as DataRow;
				start ++;
			}
			while (dn.MoveNext());
			Assert.AreEqual(this.table.Rows.Count,start);
			Assert.IsFalse(dn.HasMoreData);
		}
		
		
		[Test]
		public void EnumeratorStartFromBegin ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,
			                                                                      new ReportSettings());
			IDataNavigator dn = dm.GetNavigator;
			dn.MoveNext();
			int start = 0;
			do {
				DataRow v = dn.Current as DataRow;
				start ++;
			}
			while (dn.MoveNext());
			Assert.AreEqual(this.table.Rows.Count,start);
		}
		
		
		#endregion
		
		#region RangeEnumerator
		
		[Test]
		public void RangeEnumeratorStartFromBegin ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,
			                                                                      new ReportSettings());
			IDataNavigator dn = dm.GetNavigator;
			int start = 0;
			int end = 10;
			System.Collections.IEnumerator en = dn.RangeEnumerator(start,end);
			
			while (en.MoveNext()) {
				DataRow row = en.Current as DataRow;
				start++;
			}
			Assert.AreEqual(start -1,dn.CurrentRow);
		}
		
		
		[Test]
		public void RangeEnumeratorStartFrom5To10 ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,
			                                                                      new ReportSettings());
		
			IDataNavigator dn = dm.GetNavigator;
			int start = 5;
			int end = 10;
			System.Collections.IEnumerator en = dn.RangeEnumerator(start,end);
			
			while (en.MoveNext()) {
				DataRow row = en.Current as DataRow;
				start++;
			}
			Assert.AreEqual(start -1,dn.CurrentRow);
		}
		
		
		[Test]
		[ExpectedExceptionAttribute(typeof(ArgumentException))]
		public void RangeEnumeratorThrowException ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,
			                                                                      new ReportSettings());
		
			IDataNavigator dn = dm.GetNavigator;
			int start = 10;
			int end = 5;
			System.Collections.IEnumerator en = dn.RangeEnumerator(start,end);
			
			while (en.MoveNext()) {
				DataRow row = en.Current as DataRow;
				start++;
			}
			Assert.AreEqual(start -1,dn.CurrentRow);
		}
		#endregion	
		
		
		#region Available Fields
		
		[Test]
		public void AvailableFields ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			Assert.AreEqual(this.table.Columns.Count,dataNav.AvailableFields.Count);
		}
		
		#endregion
		
		#region DataRow
		
		[Test]
		public void CreatePlainDataRow ()
		{
			ReportSettings rs = new ReportSettings();
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			while (dataNav.MoveNext()) {
				CurrentItemsCollection c = dataNav.GetDataRow();
				Assert.AreEqual(typeof(string),c[0].DataType);
				Assert.AreEqual(typeof(string),c[1].DataType);
				Assert.AreEqual(typeof(string),c[2].DataType);
				Assert.AreEqual(typeof(int),c[3].DataType);
				Assert.AreEqual(typeof(DateTime),c[4].DataType);
			}
		}
		
		
		[Test]
		public void DataRowCountEqualListCount ()
		{
			ReportSettings rs = new ReportSettings();
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			int count = 0;
			while (dataNav.MoveNext()) {
				CurrentItemsCollection c = dataNav.GetDataRow();
				if ( c != null) {
					count ++;
				}
			}
			Assert.AreEqual(this.table.Rows.Count,count);
		}
		
		#endregion
		
		#region Setup/TearDown
		
		[TestFixtureSetUp]
		public void Init()
		{
			ContributorsList contributorsList = new ContributorsList();
			this.table = contributorsList.ContributorTable;
		}
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			
			if (this.table != null) {	
				this.table = null;
				this.table.Dispose();
			}
		}
		
		#endregion
	}
}
