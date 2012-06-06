// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager.TableStrategy
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
			rs.SortColumnsCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
		}
	
		
		#region Sorting

		[Test]
		public void SortDescendingByDateTime()
		{
			SortColumn sc = new SortColumn("RandomDate",System.ComponentModel.ListSortDirection.Descending,
			                               typeof(System.Int16),false);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnsCollection.Add(sc);
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
		}
		
		
		[Test]
		public void SortDescendingByOneColumn()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Descending);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnsCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			string compareTo = "z";
			while (dataNav.MoveNext()) {
				DataRow r = dataNav.Current as DataRow;
				string actual = r["last"].ToString();
				Assert.GreaterOrEqual(compareTo,actual);
				compareTo = actual;
			}
		}
		
		
		#endregion
		
		#region	Substring
		
		[Test]
		public void Expression_In_Text_Evaluate()
		{
			ReportSettings rs = new ReportSettings();
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.table,rs);
			DataNavigator dataNav = dm.GetNavigator;
			BaseDataItem bdi = new BaseDataItem(){
				Name ="MyDataItem",
				ColumnName = "last",
				Text ="=Substring(Fields!last,0,3)",
				Expression ="=Substring(Fields!last,0,3)"	
			};
			var ri = new ReportItemCollection();
			ri.Add(bdi);
			
			while (dataNav.MoveNext())
			{
				dataNav.Fill(ri);
				DataRow r = dataNav.Current as DataRow;
				string actual = r["last"].ToString();
				Assert.That(actual.Substring(0,3), Is.EqualTo(bdi.DBValue));
			}
		}
		
		
		#endregion
		
		#region Standart Enumerator
	
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
		/*
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
		*/
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
				CurrentItemsCollection c = dataNav.GetDataRow;
				Assert.AreEqual(typeof(string),c[0].DataType);
				Assert.AreEqual(typeof(string),c[1].DataType);
				Assert.AreEqual(typeof(string),c[2].DataType);
				Assert.AreEqual(typeof(int),c[4].DataType);
				Assert.AreEqual(typeof(DateTime),c[5].DataType);
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
				CurrentItemsCollection c = dataNav.GetDataRow;
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
				this.table.Dispose();
				this.table = null;
			}
		}
		
		#endregion
	}
}
