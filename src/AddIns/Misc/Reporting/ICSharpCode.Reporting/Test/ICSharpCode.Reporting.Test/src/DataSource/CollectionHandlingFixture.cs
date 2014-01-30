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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.DataSource;
using ICSharpCode.Reporting.Items;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.DataSource
{
	[TestFixture]
	public class CollectionHandlingFixture
	{
	
		private ContributorCollection list;

		[Test]
		public void CanInitDataCollection()
		{
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			Assert.That(collectionSource,Is.Not.Null);
		}
		
		[Test]
		public void CurrentpositionShouldZeroAfterBind () {
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			collectionSource.Bind();
			Assert.That(collectionSource.CurrentPosition,Is.EqualTo(0));
		}
		
		[Test]
		public void CurrentPositionIsTwo () {
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			collectionSource.Bind();
			collectionSource.MoveNext();
			collectionSource.MoveNext();
			Assert.That(collectionSource.CurrentPosition,Is.EqualTo(2));
		}
		
		[Test]
		public void CollectionCountIsEqualToListCount() {
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			Assert.That(collectionSource.Count,Is.EqualTo(list.Count));
		}
		
		
		[Test]
		public void AvailableFieldsEqualContibutorsPropertyCount() {
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			Assert.That(collectionSource.AvailableFields.Count,Is.EqualTo(6));
		}
		
		#region Fill
		
		[Test]
		public void TypeOfReportItemIsString () {
			var ric = new ReportItemCollection(){
				new BaseDataItem(){
					ColumnName = "Lastname"
						
				},
				new BaseDataItem(){
					ColumnName = "Firstname"
				}
			};
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			collectionSource.Bind();
			collectionSource.Fill(ric);
			foreach (BaseDataItem element in ric) {
				Assert.That(element.DataType,Is.EqualTo("System.String"));
			}
		}
		
		
		[Test]
		public void FillReportItemCollection () {
			var ric = new ReportItemCollection(){
				new BaseDataItem(){
					ColumnName = "Lastname"
						
				},
				new BaseDataItem(){
					ColumnName = "Firstname"
				}
			};
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			collectionSource.Bind();
			collectionSource.Fill(ric);
			foreach (BaseDataItem element in ric) {
				Assert.That(element.DBValue,Is.Not.EqualTo(String.Empty));
			}
		}
		
		#endregion
		
		
		#region Grouping
		
		[Test]
		public void GroupbyOneColumn () {
			var rs = new ReportSettings();
			rs.GroupColumnCollection.Add( new GroupColumn("GroupItem",1,ListSortDirection.Ascending));
			var collectionSource = new CollectionSource	(list,rs);
			collectionSource.Bind();
		}
		
		
		[Test]
		public void bla () {
			var s = list.OrderBy(a => a.Lastname);
			var x = s.GroupBy(y => y.GroupItem);
			foreach (var group in x) {
				Console.WriteLine("{0} - {1}",group.Key,group.GetType().ToString());
				foreach (var element in group) {
					Console.WriteLine(element.Firstname);
				}
			}
		}
		#endregion
		
		#region Sort

			
		[Test]
		public void CreateUnsortedIndex() {
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			collectionSource.Bind();
			Assert.That(collectionSource.IndexList.Count,Is.EqualTo(collectionSource.Count));
			Assert.That(collectionSource.IndexList.CurrentPosition,Is.EqualTo(0));
		}
		
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void SortColumnNotExist() {
			var rs = new ReportSettings();
			rs.SortColumnsCollection.Add(new SortColumn("aa",ListSortDirection.Ascending));
			var collectionSource = new CollectionSource	(list,rs);
			collectionSource.Bind();
			Assert.That(collectionSource.IndexList,Is.Not.Null);
			Assert.That(collectionSource.IndexList.Count,Is.EqualTo(0));
		}
		
		
		[Test]
		public void SortOneColumnAscending() {
			var rs = new ReportSettings();
			rs.SortColumnsCollection.Add(new SortColumn("Lastname",ListSortDirection.Ascending));
			var collectionSource = new CollectionSource	(list,rs);
			collectionSource.Bind();
			string compare = collectionSource.IndexList[0].ObjectArray[0].ToString();
			foreach (var element in collectionSource.IndexList) {
				string result = String.Format("{0} - {1}",element.ListIndex,element.ObjectArray[0]);
				Console.WriteLine(result);
				Assert.That(compare,Is.LessThanOrEqualTo(element.ObjectArray[0].ToString()));
				compare = element.ObjectArray[0].ToString();
			}
		}
		
		
		[Test]
		public void SortTwoColumnsAscending() {
			var rs = new ReportSettings();
			rs.SortColumnsCollection.Add(new SortColumn("Lastname",ListSortDirection.Ascending));
			rs.SortColumnsCollection.Add(new SortColumn("RandomInt",ListSortDirection.Ascending));
			var collectionSource = new CollectionSource	(list,rs);
			collectionSource.Bind();
			string compare = collectionSource.IndexList[0].ObjectArray[0].ToString();
			foreach (var element in collectionSource.IndexList) {
				string result = String.Format("{0} - {1} - {2}",element.ListIndex,element.ObjectArray[0],element.ObjectArray[1].ToString());
				Console.WriteLine(result);
				Assert.That(compare,Is.LessThanOrEqualTo(element.ObjectArray[0].ToString()));
				compare = element.ObjectArray[0].ToString();
			}
		}
		
		#endregion
		
		
		[SetUp]
		public void CreateList() {
			var contributorList = new ContributorsList();
			list = contributorList.ContributorCollection;
		}	
	}
}
