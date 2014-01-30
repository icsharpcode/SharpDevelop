// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager.ListStrategy
{
	[TestFixture]
	public class GroupListFixture
	{
		
		ContributorCollection contributorCollection;
		
		
		#region standard test's
		
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
		
		#endregion
		
		#region Group by StringValue
		
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
		public void FillCollection()
		{
			var dataNavigator = PrepareStringGrouping();
			ReportItemCollection searchCol = new ReportItemCollection();
			
			searchCol.Add(new BaseDataItem ()
			              {
			              	Name ="Last",
			              	ColumnName ="Last"
			              }
			             );
			searchCol.Add(new BaseDataItem ()
			              {
			              
			              	ColumnName ="GroupItem"
			              }
			             );
			dataNavigator.Reset();
			dataNavigator.MoveNext();

			do
			{
				if (dataNavigator.HasChildren)
				{
					dataNavigator.Fill(searchCol);
					var b = (BaseDataItem)searchCol[1];
//					Console.WriteLine("-- <{0}>-",b.DBValue);
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						childNavigator.Fill(searchCol);
						var filledItem = (BaseDataItem)searchCol[0];
						Assert.That(filledItem.DBValue,Is.Not.Empty);
					}
					while (childNavigator.MoveNext());
				}
			}
			while (dataNavigator.MoveNext());
		}
		
		
		[Test]
		public void CollectionContainsSubclass ()
		{
			var modifyedCollection = this.ModifyCollection();
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(modifyedCollection,rs);
			IDataNavigator dataNavigator = dm.GetNavigator;
			
			ReportItemCollection searchCol = new ReportItemCollection();
			searchCol.Add(new BaseDataItem ()
			              {
			              	ColumnName ="DummyClass.DummyString"			     
			              }
			             );
			searchCol.Add(new BaseDataItem ()
			              {
			              	Name ="Last",			           
			              	ColumnName ="Last"	
			              }
			             );
			
			searchCol.Add(new BaseDataItem ()
			              {
			             			           
			              	ColumnName ="GroupItem"	
			              }
			             );
			
			string compare = string.Empty;

			while (dataNavigator.MoveNext())
			{
				dataNavigator.Fill(searchCol);
				var b1 = (BaseDataItem)searchCol[2];
				var result = b1.DBValue;				
				Assert.That (compare,Is.LessThan(result));
				
				if (dataNavigator.HasChildren)
				{
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						childNavigator.Fill(searchCol);
						var itemDummy = (BaseDataItem)searchCol[0];
						var itemLast = (BaseDataItem)searchCol[1];
						var itemGroup = (BaseDataItem)searchCol[2];
						Console.WriteLine ("\t{0} - {1} - {2}",itemDummy.DBValue,itemLast.DBValue,itemGroup.DBValue);
						Assert.That(itemDummy.DBValue,Is.Not.Empty);
						Assert.That(itemLast.DBValue,Is.Not.Empty);
						Assert.That(itemGroup.DBValue,Is.Not.Empty);
					}
					while (childNavigator.MoveNext());
				}
				compare = result;
			}
		}
		
		#endregion
		
		[Test]
		public void SortChildrenDescending()
		{
			ReportSettings rs = new ReportSettings();
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			rs.GroupColumnsCollection.Add(gc);
			
			SortColumn sc = new SortColumn("Last",ListSortDirection.Descending);
			rs.SortColumnsCollection.Add(sc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			var dataNavigator = dm.GetNavigator;
			
			ReportItemCollection searchCol = new ReportItemCollection();
			searchCol.Add(new BaseDataItem ()
			              {
			              	Name ="Last",
			              	ColumnName ="Last"
			              }
			             );
			
			searchCol.Add(new BaseDataItem ()
			              {
			              	
			              	ColumnName ="GroupItem"
			              }
			             );
			
			string compare = String.Empty;

			while (dataNavigator.MoveNext())
			{
				dataNavigator.Fill(searchCol);
				var column = (BaseDataItem)searchCol[1];
				var result = column.DBValue.ToString();
				
				Assert.That (compare,Is.LessThan(result));
				if (dataNavigator.HasChildren) {
					string compareChild = String.Empty;
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						childNavigator.Fill(searchCol);
						var childColumn = (BaseDataItem)searchCol[0];
						var childResult = childColumn.DBValue.ToString();
//						Console.WriteLine("\t{0}",childResult);
						if (!String.IsNullOrEmpty(compareChild)) {
							Assert.LessOrEqual(childResult,compareChild);
						}
						compareChild = childResult;
					}
					while (childNavigator.MoveNext());
				}
			}
		}
		
		
		#region Group by DataTime
		
		[Test]
		public void DateTimeCan_FillChild()
		{
			var dataNavigator = PrepareDateTimeGrouping();
			ReportItemCollection searchCol = new ReportItemCollection();
			searchCol.Add(new BaseDataItem ()
			              {
			              	Name ="RandomDate",
			              	ColumnName ="Last"
			              }
			             );
			
			var compare = System.DateTime.MinValue;

			while (dataNavigator.MoveNext())
			{
				Contributor groupResult = dataNavigator.Current as Contributor;
				Assert.LessOrEqual(compare,groupResult.RandomDate);
				if (dataNavigator.HasChildren) {
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						Assert.That(dataNavigator.HasChildren,Is.True);
						// we know that current is a 'contributor'
						Contributor c = dataNavigator.Current as Contributor;
						Assert.IsNotNull(c);
//						string v2 = c.Last + " GroupVal :" +  c.RandomDate;
//						Console.WriteLine(v2);
					}
					while (childNavigator.MoveNext());
				}
				
			}
		}
		
		#endregion
		
		private ContributorCollection ModifyCollection ()
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
		
		
		private IDataNavigator PrepareDateTimeGrouping ()
		{
			GroupColumn gc = new GroupColumn("RandomDate",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			return dm.GetNavigator;
		}
		
		
		private IDataNavigator PrepareStringGrouping ()
		{
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			return dm.GetNavigator;
		}
		
		
		[TestFixtureSetUp]
		public void Init()
		{
			ContributorsList contributorsList = new ContributorsList();
			this.contributorCollection = contributorsList.ContributorCollection;
		}
		
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}
