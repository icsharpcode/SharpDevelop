// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.ComponentModel;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using NUnit.Framework;

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
		public void GroupbyOneColumn () {
			var rs = new ReportSettings();
			rs.GroupColumnsCollection.Add( new GroupColumn("GroupItem",1,ListSortDirection.Ascending));
			var collectionSource = new CollectionDataSource	(list,rs);
			collectionSource.Bind();
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
			collectionSource.Fill(ric);
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
			do {
				collectionSource.Fill(ric);
				Console.WriteLine("first : <{0}> Last <{1}> ",((BaseDataItem)ric[0]).DBValue,
				                  ((BaseDataItem)ric[1]).DBValue);
				Assert.That(((BaseDataItem)ric[0]).DBValue,Is.GreaterThanOrEqualTo(compare));
				compare = ((BaseDataItem)ric[0]).DBValue;
				                 
				i ++;
			}while (collectionSource.MoveNext());
			
			Assert.That(i,Is.EqualTo(collectionSource.Count));
		}
		
		
		[Test]
		public void GroupbyOneColumnAndFill () {
			var dataItemsCollection = CreateDataItems();
			var repiortsettings = new ReportSettings();
			repiortsettings.GroupColumnsCollection.Add( new GroupColumn("GroupItem",1,ListSortDirection.Ascending));
			repiortsettings.GroupColumnsCollection.Add( new GroupColumn("RandomInt",1,ListSortDirection.Ascending));
			
			var collectionSource = new CollectionDataSource (list,repiortsettings);
			collectionSource.Bind();
			int i = 0;
			do {
				collectionSource.Fill(dataItemsCollection);
				Console.WriteLine("first : <{0}> Last <{1}> Group <{2}> Randomint <{3}>",((BaseDataItem)dataItemsCollection[0]).DBValue,
				                  ((BaseDataItem)dataItemsCollection[1]).DBValue,
				                  ((BaseDataItem)dataItemsCollection[2]).DBValue,
				                  ((BaseDataItem)dataItemsCollection[3]).DBValue);
				i ++;
			}while (collectionSource.MoveNext());
			
			Assert.That(i,Is.EqualTo(collectionSource.Count));
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
			do {
				collectionSource.Fill(row);
				var r = (BaseRowItem)row[0];
				foreach (var element in r.Items) {
					Assert.That(((BaseDataItem)element).DBValue,Is.Not.Empty);
				}
				i ++;
			}while (collectionSource.MoveNext());
			
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
			do {
				collectionSource.Fill(row);
				var res = (BaseDataItem)row.Find(c => ((BaseDataItem)c).ColumnName == "GroupItem");
				Assert.That(res.DBValue,Is.Not.Empty);
				i ++;
			}while (collectionSource.MoveNext());
			
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
