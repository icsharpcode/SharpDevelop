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
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using NUnit.Framework;
using System.Linq;

namespace ICSharpCode.Reporting.Test.DataSource
{
	[TestFixture]
	public class CollectionDataSourceFixture
	{
		 ContributorCollection list;
		
		[Test]
		public void CollectionCountIsEqualToListCount() {
			var collectionSource = new CollectionDataSource	(list,new ReportSettings());
			Assert.That(collectionSource.Count,Is.EqualTo(list.Count));
		}
		
		
		[Test]
		public void AvailableFieldsEqualContibutorsPropertyCount() {
			var collectionSource = new CollectionDataSource	(list,new ReportSettings());
			Assert.That(collectionSource.AvailableFields.Count,Is.EqualTo(6));
		}
		
		
		[Test]
		public void GroupbyOneColumnSortSubList () {
			var rs = new ReportSettings();
			var gc = new GroupColumn() {
				ColumnName ="GroupItem",
				SortDirection = ListSortDirection.Ascending,
				GroupSortColumn = new SortColumn() {
					ColumnName = "Randomint",
					SortDirection = ListSortDirection.Ascending
				}
			};
			rs.GroupColumnsCollection.Add(gc);
			var collectionSource = new CollectionDataSource	(list,rs);
			collectionSource.Bind();
			var testKey = String.Empty;
			var testSubKey = -1;
			var groupedList = collectionSource.GroupedList;
			foreach (var element in groupedList) {
				Assert.That(element.Key,Is.GreaterThan(testKey));
				testKey = element.Key.ToString();
				foreach (Contributor sub in element) {
					Assert.That(sub.RandomInt,Is.GreaterThanOrEqualTo(testSubKey));
					testSubKey = sub.RandomInt;
				}
				testSubKey = -1;
			}
		}
		
		
		[Test]
		public void GroupbyOneColumn () {
			var rs = new ReportSettings();
			rs.GroupColumnsCollection.Add( new GroupColumn("GroupItem",1,ListSortDirection.Ascending));
			var collectionSource = new CollectionDataSource	(list,rs);
			collectionSource.Bind();
			var testKey = String.Empty;
			var groupedList = collectionSource.GroupedList;
			foreach (var element in groupedList) {
				Assert.That(element.Key,Is.GreaterThan(testKey));
				testKey = element.Key.ToString();
			}
		}
		
		
		[Test]
		public void TypeOfReportItemIsString () {
			var ric = new System.Collections.Generic.List<IPrintableObject>(){
				new BaseDataItem(){
					ColumnName = "Lastname"
						
				},
				new BaseDataItem(){
					ColumnName = "Firstname"
				}
			};
			var collectionSource = new CollectionDataSource	(list,new ReportSettings());
			collectionSource.Bind();
			var result = collectionSource.SortedList.FirstOrDefault();
			collectionSource.Fill(ric,result);
			foreach (BaseDataItem element in ric) {
				Assert.That(element.DataType,Is.EqualTo("System.String"));
			}
		}
		
		
		[Test]
		public void SortOneColumnAscending() {
			var ric = new System.Collections.Generic.List<IPrintableObject>(){
				new BaseDataItem(){
					ColumnName = "Lastname"
				},
				new BaseDataItem(){
					ColumnName = "Firstname"
				}
			};
			
			var rs = new ReportSettings();
			rs.SortColumnsCollection.Add(new SortColumn("Lastname",ListSortDirection.Ascending));
			var collectionSource = new CollectionDataSource	(list,rs);
			collectionSource.Bind();
			string compare = String.Empty;
			int i = 0;
			foreach (var element in collectionSource.SortedList) {
				collectionSource.Fill(ric,element);
//				Console.WriteLine("first : <{0}> Last <{1}> ",((BaseDataItem)ric[0]).DBValue,((BaseDataItem)ric[1]).DBValue);
				                  
				Assert.That(((BaseDataItem)ric[0]).DBValue,Is.GreaterThanOrEqualTo(compare));
				
				compare = ((BaseDataItem)ric[0]).DBValue;
				i++;
			}
			/*
			do {
				collectionSource.Fill(ric);
				Console.WriteLine("first : <{0}> Last <{1}> ",((BaseDataItem)ric[0]).DBValue,
				                  ((BaseDataItem)ric[1]).DBValue);
				Assert.That(((BaseDataItem)ric[0]).DBValue,Is.GreaterThanOrEqualTo(compare));
				compare = ((BaseDataItem)ric[0]).DBValue;
				                 
				i ++;
			}while (collectionSource.MoveNext());
			
			 */
			Assert.That(i,Is.EqualTo(collectionSource.Count));
		}
		
		
		[Test]
		public void GroupbyOneColumnAndFill () {
			var dataItemsCollection = CreateDataItems();
			var reportsettings = new ReportSettings();
			reportsettings.GroupColumnsCollection.Add( new GroupColumn("GroupItem",1,ListSortDirection.Ascending));
		
			var collectionSource = new CollectionDataSource (list,reportsettings);
			collectionSource.Bind();
			int i = 0;
			
			foreach (var element in collectionSource.GroupedList) {
				Console.WriteLine("Key {0} ",element.Key);
				foreach (var l in element) {
					collectionSource.Fill(dataItemsCollection,l);
//					Console.WriteLine("first : <{0}> Last <{1}> Group <{2}> Randomint <{3}>",((BaseDataItem)dataItemsCollection[0]).DBValue,
//					                  ((BaseDataItem)dataItemsCollection[1]).DBValue,
//					                  ((BaseDataItem)dataItemsCollection[2]).DBValue,
//					                  ((BaseDataItem)dataItemsCollection[3]).DBValue);
					i++;
				}
			}
			/*
			do {
				collectionSource.Fill(dataItemsCollection);
				Console.WriteLine("first : <{0}> Last <{1}> Group <{2}> Randomint <{3}>",((BaseDataItem)dataItemsCollection[0]).DBValue,
				                  ((BaseDataItem)dataItemsCollection[1]).DBValue,
				                  ((BaseDataItem)dataItemsCollection[2]).DBValue,
				                  ((BaseDataItem)dataItemsCollection[3]).DBValue);
				i ++;
			}while (collectionSource.MoveNext());
			*/
			Assert.That(i,Is.EqualTo(collectionSource.Count));
		}
	
		[Test]
		public void DataItemWithNoColumnNameHasErrorMessageInDbValue() {
			var baseRow = new BaseRowItem();
			var dataItem = new BaseDataItem() {
				
			};
				
			baseRow.Items.Add(dataItem);
			
			var row = new System.Collections.Generic.List<IPrintableObject>();
			row.Add(baseRow);
			var reportSettings = new ReportSettings();
			var collectionSource = new CollectionDataSource (list,reportSettings);
			collectionSource.Bind();
			foreach (var element in collectionSource.SortedList) {
				collectionSource.Fill(row,element);
				var r = (BaseRowItem)row[0];
				foreach (var result in r.Items) {
					Assert.That(((BaseDataItem)result).DBValue.StartsWith("Missing"),Is.EqualTo(true));
				}
			}
		}
			
			
		[Test]
		public void FillDataIncludedInRow() {
			
			var baseRow = new BaseRowItem();
			var dataItemsCollection = CreateDataItems();
			baseRow.Items.AddRange(dataItemsCollection);
			
			var row = new System.Collections.Generic.List<IPrintableObject>();
			row.Add(baseRow);
			var reportSettings = new ReportSettings();
			var collectionSource = new CollectionDataSource (list,reportSettings);
			collectionSource.Bind();
			int i = 0;
			foreach (var element in collectionSource.SortedList) {
				collectionSource.Fill(row,element);
				var r = (BaseRowItem)row[0];
				foreach (var result in r.Items) {
					Assert.That(((BaseDataItem)result).DBValue,Is.Not.Empty);
				}
				i ++;
			}
			Assert.That(i,Is.EqualTo(collectionSource.Count));
		}
		
		
		[Test]
		public void RowContainsRowAndItem() {
			var row = new System.Collections.Generic.List<IPrintableObject>();
				var gItem = 	new BaseDataItem(){
					ColumnName = "GroupItem"
				};
			row.Add(gItem);
			
			var baseRow = new BaseRowItem();
			
			var ric = new System.Collections.Generic.List<IPrintableObject>(){
				new BaseDataItem(){
					ColumnName = "Lastname"
						
				},
				new BaseDataItem(){
					ColumnName = "Firstname"
				}
			};
			baseRow.Items.AddRange(ric);
			row.Add(baseRow);
			var rs = new ReportSettings();
			var collectionSource = new CollectionDataSource (list,rs);
			collectionSource.Bind();
			int i = 0;
			foreach (var element in collectionSource.SortedList) {
				collectionSource.Fill(row,element);
				var res = (BaseDataItem)row.Find(c => ((BaseDataItem)c).ColumnName == "GroupItem");
				Assert.That(res.DBValue,Is.Not.Empty);
				i ++;
			}
			Assert.That(i,Is.EqualTo(collectionSource.Count));
		}
		
		
		System.Collections.Generic.List<IPrintableObject> CreateDataItems() {
			var ric = new System.Collections.Generic.List<IPrintableObject>(){
				new BaseDataItem(){
					ColumnName = "Lastname"
						
				},
				new BaseDataItem(){
					ColumnName = "Firstname"
				},
				
				new BaseDataItem(){
					ColumnName = "GroupItem"
				},
				new BaseDataItem(){
					ColumnName = "RandomInt"
				}
			};
			return ric;
		}
		
		
		[SetUp]
		public void CreateList() {
			var contributorList = new ContributorsList();
			list = contributorList.ContributorCollection;
		}	
	}
}
