/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 27.11.2008
 * Zeit: 19:30
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager
{
	[TestFixture]
	
	public class IListDataManagerFixture
	{
		
		ContributorCollection contributorCollection;
		
		[Test]
		public void DefaultConstructor()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection as System.Collections.IList,new ReportSettings());
			Assert.IsNotNull(dm,"IDataanager should not be 'null'");
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorWithNullIList()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance((System.Collections.IList)null,new ReportSettings());
			Assert.IsNotNull( dm);
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorWithNullSettings()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,null);
			Assert.IsNotNull( dm);
		}
		
		
		[Test]
		public void DataNavigatorNotNull ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection as System.Collections.IList,new ReportSettings());
			Assert.IsNotNull( dm);
			DataNavigator n = dm.GetNavigator;
			Assert.IsNotNull (n,"Navigator should not be 'null'");
		}
		
		
		[Test]
		public void DataNavigatorCountEqualListCount ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection as System.Collections.IList,new ReportSettings());
			Assert.IsNotNull( dm);
			IDataNavigator dataNav = dm.GetNavigator;
			Assert.AreEqual(this.contributorCollection.Count,
			                dataNav.Count,
			                "Count of LstItems should be equal");
		}
		
		
		[Test]
		public void DataNavigatorCorrectPosition ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection as System.Collections.IList,new ReportSettings());
			IDataNavigator dataNav = dm.GetNavigator;
			Assert.AreEqual(-1,
			                dataNav.CurrentRow,
			                "CurrentRow should be -1");
		}
		
		
		[Test]
		public void DataNavigator_Return_ErrMessage_If_ColumnName_NotExist ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection as System.Collections.IList,new ReportSettings());
			IDataNavigator dataNav = dm.GetNavigator;
			BaseDataItem item = new BaseDataItem();
			item.ColumnName = "ColumnNotExist";
			var items = new ReportItemCollection();
			items.Add(item);
			dataNav.Fill(items);
			string str = "<" + item.ColumnName +">";
			Assert.That(item.DBValue.Contains(str));
		}
		
		
		#region Standart Enumerator
		[Test]
		public void EnumeratorStartFromBegin ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection, new ReportSettings());
			                                                                     
			IDataNavigator dataNav = dm.GetNavigator;
			dataNav.MoveNext();
			int start = 0;
			do {
				Contributor view = dataNav.Current as Contributor;
				start ++;
			}
			while (dataNav.MoveNext());
			Assert.AreEqual(this.contributorCollection.Count,start);
		}
		
		#endregion
		
		#region Available Fields
		
		[Test]
		public void AvailableFields ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			Assert.AreEqual(5,dataNav.AvailableFields.Count);
		}
		
		
		#endregion
		
		#region Sorting
		
		[Test]
		public void SortAscendingByOneColumn()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			DataNavigator dataNav = dm.GetNavigator;
			string v1 = String.Empty;
			while (dataNav.MoveNext()) {
				Contributor view = dataNav.Current as Contributor;
				string v2 = view.Last;
				string ss = String.Format("< {0} > <{1}>",v1,v2);
				Console.WriteLine(ss);
//				Assert.LessOrEqual(v1,v2);
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
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			DataNavigator dataNav = dm.GetNavigator;
			string v1 = String.Empty;
			while (dataNav.MoveNext()) {
				Contributor  view= dataNav.Current as Contributor;
				string v2 = view.RandomInt.ToString();
				int i2 = view.RandomInt;
//				string ss = String.Format("< {0} > <{1}>",v1,v2);
				Console.WriteLine(v2);
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
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			DataNavigator dataNav = dm.GetNavigator;
			DateTime d1 = new DateTime(1,1,1);
			while (dataNav.MoveNext()) {
				Contributor view = dataNav.Current as Contributor;
				Assert.LessOrEqual(d1,view.RandomDate);
				d1 = view.RandomDate;
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
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			DataNavigator dataNav = dm.GetNavigator;
			DateTime d1 = new DateTime(2099,12,30);
			while (dataNav.MoveNext()) {
				Contributor view = dataNav.Current as Contributor;
				Assert.GreaterOrEqual(d1,view.RandomDate);
				d1 = view.RandomDate;
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
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			DataNavigator dataNav = dm.GetNavigator;
			string v1 = String.Empty;
			while (dataNav.MoveNext()) {
				Contributor view = dataNav.Current as Contributor;
				string v2 = view.Last + "-" + view.RandomInt;
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
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			DataNavigator dataNav = dm.GetNavigator;
			string compareTo = "z";
			while (dataNav.MoveNext()) {
				Contributor view = dataNav.Current as Contributor;
				string actual = view.Last;
				Assert.GreaterOrEqual(compareTo,actual);
//				string ss = String.Format("< {0} > <{1}>",compareTo,actual);
//				Console.WriteLine(ss);
				compareTo = actual;
			}
			Assert.IsTrue(dataNav.IsSorted);
		}
		
		#endregion
		
		#region DataRow
		
		[Test]
		public void CreatePlainDataRow ()
		{
			ReportSettings rs = new ReportSettings();
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
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
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			DataNavigator dataNav = dm.GetNavigator;
			int count = 0;
			while (dataNav.MoveNext()) {
				CurrentItemsCollection c = dataNav.GetDataRow();
				if ( c != null) {
					count ++;
				}
			}
			Assert.AreEqual(this.contributorCollection.Count,count);
		}
		
		#endregion
		
		#region RangeEnumerator
		
		[Test]
		public void RangeEnumeratorFromBeginToEnd()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,new ReportSettings());
			IDataNavigator dn = dm.GetNavigator;
			int start = 0;
			int end = 10;
			System.Collections.IEnumerator en = dn.RangeEnumerator(start,end);
			
			while (en.MoveNext()) {
				object o = en.Current;
				Contributor view = en.Current as Contributor;
				start++;
			}
			Assert.AreEqual(start -1,dn.CurrentRow);
		}
		
		[Test]
		public void RangeEnumeratFrom5To10 ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,
			                                                                      new ReportSettings());
		
			IDataNavigator dn = dm.GetNavigator;
			int start = 5;
			int end = 10;
			System.Collections.IEnumerator en = dn.RangeEnumerator(start,end);
			
			while (en.MoveNext()) {
				object o = en.Current;
				//Contributor view = en.Current as Contributor;
				start++;
			}
			Assert.AreEqual(start -1,dn.CurrentRow);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void RangeEnumeratorThrowExceptionIfStartGreateEnd ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,
			                                                                      new ReportSettings());
			IDataNavigator dn = dm.GetNavigator;
			int start = 10;
			int end = 5;
			System.Collections.IEnumerator en = dn.RangeEnumerator(start,end);
			while (en.MoveNext()) {
				object o = en.Current;
				Contributor view = en.Current as Contributor;
				start++;
			}
			Assert.AreEqual(start -1,dn.CurrentRow);
		}
		
		#endregion
		
		#region Setup/TearDown
		[TestFixtureSetUp]
		
		public void Init()
		{
			ContributorsList contributorsList = new ContributorsList();
			this.contributorCollection = contributorsList.ContributorCollection;
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			if (this.contributorCollection == null) {
				this.contributorCollection = null;
			}
		}
		
		#endregion
		
	}
}
