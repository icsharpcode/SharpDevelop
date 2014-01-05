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
		ExpressionVisitor visitor;
		ReportSettings reportSettings;
		
		[Test]
		public void TestMethod()
		{
			visitor = new ExpressionVisitor(reportSettings,null);
			var script = "= sum('doubleValue')";
			collection[0].Text = script;
			foreach (IGrouping<object, object> grouping in dataSource.GroupedList) {
				Console.WriteLine("key {0}",grouping.Key);
				//Childs
				foreach (var child in grouping) {
					var c = (Contributor)child;
					Console.WriteLine("\tContributor {0}, {1}",c.Firstname,c.Lastname);
				}
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
