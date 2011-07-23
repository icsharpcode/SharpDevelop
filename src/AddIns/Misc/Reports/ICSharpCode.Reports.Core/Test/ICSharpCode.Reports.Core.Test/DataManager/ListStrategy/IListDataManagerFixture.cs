// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager.ListStrategy
{
	[TestFixture]
	
	public class IListDataManagerFixture
	{
		
		ContributorCollection contributorCollection;
		
		[Test]
		public void DefaultConstructor()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection as System.Collections.IList,new ReportSettings());
			Assert.IsNotNull(dm,"IDataManager should not be 'null'");
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
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,new ReportSettings());
			IDataNavigator dataNav = dm.GetNavigator;
			BaseDataItem item = new BaseDataItem();
			item.ColumnName = "ColumnNotExist";
			var items = new ReportItemCollection();
			items.Add(item);
			dataNav.Reset();
			dataNav.MoveNext();
			dataNav.Fill(items);
//			string str = "<" + item.ColumnName +">";
			Assert.That(item.DBValue.StartsWith("Error"));
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
		
		
		
		[Test]
		public void NullValue_In_Property_Should_Return_EmptyString ()
		{
			ContributorsList contributorsList = new ContributorsList();
			var contColl = contributorsList.ContributorCollection;
			
			foreach (Contributor element in contColl) {
				element.GroupItem = null;
			}
			
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(contColl, new ReportSettings());
			IDataNavigator dataNav = dm.GetNavigator;
			
			
			ReportItemCollection searchCol = new ReportItemCollection();
			searchCol.Add(new BaseDataItem ()
			             {
			             	Name ="GroupItem",
			             	ColumnName ="GroupItem"
			             		
			             }
			            );
			dataNav.Reset();
			dataNav.MoveNext();
			do 
			{
				dataNav.Fill(searchCol);
				BaseDataItem resultItem = searchCol[0] as BaseDataItem;
				
				Assert.That (resultItem.Name,Is.EqualTo("GroupItem"));
				Assert.That(resultItem.DBValue,Is.EqualTo(String.Empty));
				
			}
			while (dataNav.MoveNext());
		}
			
		
		#endregion
		
		#region Available Fields
		
		[Test]
		public void AvailableFields ()
		{
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,new ReportSettings());
			DataNavigator dataNav = dm.GetNavigator;
			Assert.AreEqual(7,dataNav.AvailableFields.Count);
		}
		
		
		#endregion
		
		#region Sorting
		
		[Test]
		public void SortAscendingByTwoColumns()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Ascending);
			SortColumn sc1 = new SortColumn("RandomInt",System.ComponentModel.ListSortDirection.Ascending);
			
			ReportSettings rs = new ReportSettings();
			rs.SortColumnsCollection.Add(sc);
			rs.SortColumnsCollection.Add(sc1);
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
		}
		
		
		[Test]
		public void SortDescendingByOneColumn()
		{
			SortColumn sc = new SortColumn("Last",System.ComponentModel.ListSortDirection.Descending);
			ReportSettings rs = new ReportSettings();
			rs.SortColumnsCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			DataNavigator dataNav = dm.GetNavigator;
			string compareTo = "z";
			while (dataNav.MoveNext()) {
				Contributor view = dataNav.Current as Contributor;
				string actual = view.Last;
				Assert.GreaterOrEqual(compareTo,actual);
				compareTo = actual;
			}
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
				CurrentItemsCollection c = dataNav.GetDataRow;
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
				CurrentItemsCollection c = dataNav.GetDataRow;
				if ( c != null) {
					count ++;
				}
			}
			Assert.AreEqual(this.contributorCollection.Count,count);
		}
		
		#endregion
		
		
		#region get included class
			
		ContributorCollection ModifyCollection ()
		{
			var newcol = this.contributorCollection;
			MyDummyClass dummy;
			int start = 0;
			foreach (var element in newcol) 
			{
				dummy = new MyDummyClass();
				dummy.DummyString = "dummy" + start.ToString();
				dummy.DummyInt = start;
				element.DummyClass = dummy;
				start ++;
			}
			return newcol;
		}
		
		
		[Test]
		public void Collection_Contains_Subclass ()
		{
			var modifyedCollection = this.ModifyCollection();
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(modifyedCollection,new ReportSettings());
			IDataNavigator dn = dm.GetNavigator;
			
			ReportItemCollection searchCol = new ReportItemCollection();
			searchCol.Add(new BaseDataItem ()
			              {
			              	ColumnName ="DummyClass.DummyString"			     
			              }
			             );
			
				searchCol.Add(new BaseDataItem ()
			              {
			              	Name ="GroupItem",			           
			              	ColumnName ="GroupItem"	
			              }
			             );
			
			dn.Reset();
			dn.MoveNext();

			while (dn.MoveNext()) {
				dn.Fill(searchCol);
				var a = (BaseDataItem)searchCol[0];
				var b = (BaseDataItem)searchCol[1];
				var c = modifyedCollection[dn.CurrentRow];
				Assert.AreEqual(a.DBValue,c.DummyClass.DummyString);
				Assert.AreEqual(b.DBValue,c.GroupItem);
			}
		}
		
		
		[Test]
		public void SubClassName_Is_Wrong ()
		{
			var modifyedCollection = this.ModifyCollection();
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(modifyedCollection,new ReportSettings());
			IDataNavigator dn = dm.GetNavigator;
			ReportItemCollection searchCol = new ReportItemCollection();
			searchCol.Add(new BaseDataItem ()
			              {
			              	ColumnName ="wrong.DummyString",
							DataType = "System.Int32"		              	
			              }
			             );
			
			dn.Reset();
			dn.MoveNext();

			while (dn.MoveNext()) {
				dn.Fill(searchCol);
				var a = (BaseDataItem)searchCol[0];
				Assert.That(a.DBValue.StartsWith("Error"));
			}
		}
		
		[Test]
		public void SubPropertyName_Is_Wrong ()
		{
			var modifyedCollection = this.ModifyCollection();
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(modifyedCollection,new ReportSettings());
			IDataNavigator dn = dm.GetNavigator;
			ReportItemCollection searchCol = new ReportItemCollection();
			searchCol.Add(new BaseDataItem ()
			              {
			              	Name ="GroupItem",
			              	ColumnName ="DummyClass.Wrong",
							DataType = "System.Int32"		              	
			              }
			             );
			dn.Reset();
			dn.MoveNext();
			while (dn.MoveNext()) {
				dn.Fill(searchCol);
				var a = (BaseDataItem)searchCol[0];
				Assert.That(a.DBValue.StartsWith("Error"));
			}
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
