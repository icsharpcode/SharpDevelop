// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using ICSharpCode.Reporting.Test.DataSource;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions.IntegrationTests
{
	[TestFixture]
	public class AggregatesGroupesFixture
	{
		Collection<ExportText> collection;
		CollectionDataSource dataSource;
		ContributorCollection list;
		ReportSettings reportSettings;
		
		[Test]
		public void SumGroupedList()
		{
			var visitor = new ExpressionVisitor (reportSettings);
			visitor.SetCurrentDataSource(dataSource.GroupedList);
			var script = "= sum('randomint')";
			collection[0].Text = script;
			visitor.Visit(collection[0]);
			var result = list.Sum(x => x.RandomInt);
			Assert.That(Convert.ToDouble(collection[0].Text),Is.EqualTo(result));
		}
		
		
		[Test]
		public void SumOneGroup () {
			
			var container = new ExportContainer();
			
			var script = "= sum('randomint')";
			collection[0].Text = script;
			container.ExportedItems.AddRange(collection);
			
			var visitor = new ExpressionVisitor (reportSettings);
			visitor.SetCurrentDataSource(dataSource.GroupedList);
			var group = dataSource.GroupedList.FirstOrDefault();
			
			visitor.SetCurrentDataSource(group);
			visitor.Visit(container);
			
			var result = list.Where(k => k.GroupItem == group.Key.ToString()).Sum(x => x.RandomInt);
			Assert.That(Convert.ToDouble(collection[0].Text),Is.EqualTo(result));
		}
		
		
		[Test]
		public void SumAllGroups () {
			var container = new ExportContainer();
			container.ExportedItems.AddRange(collection);

			var visitor = new ExpressionVisitor (reportSettings);
			visitor.SetCurrentDataSource(dataSource.GroupedList);
			foreach (var group in dataSource.GroupedList) {
				var script = "= sum('randomint')";
				collection[0].Text = script;
				visitor.SetCurrentDataSource(group);
				visitor.Visit(container);
				
				var result = list.Where(k => k.GroupItem == group.Key.ToString()).Sum(x => x.RandomInt);
				Assert.That(Convert.ToDouble(collection[0].Text),Is.EqualTo(result));
			}
		}
		
		
		[SetUp]
		public void CreateExportlist() {
			collection = new Collection<ExportText>();
			collection.Add(new ExportText()
			               {
			               	Text = String.Empty
			               });
			
			var contributorList = new ContributorsList();
			list = contributorList.ContributorCollection;
			reportSettings = new ReportSettings();
			reportSettings.GroupColumnsCollection.Add(
				
				new GroupColumn("groupItem",1,ListSortDirection.Ascending )
			);
			
			dataSource = new CollectionDataSource(list,reportSettings);
			dataSource.Bind();
		}
	}
}
