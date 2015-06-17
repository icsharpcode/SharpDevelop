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
		Collection<ExportText> reportItemCollection;
		CollectionDataSource dataSource;
		ContributorCollection list;
		ReportSettings reportSettings;
		
		[Test]
		public void SumGroupedList()
		{
			var visitor = new ExpressionVisitor (reportSettings);
			visitor.SetCurrentDataSource(dataSource.GroupedList);
			var script = "= sum('randomint')";
			reportItemCollection[0].Text = script;
			visitor.Visit(reportItemCollection[0]);
			var result = list.Sum(x => x.RandomInt);
			Assert.That(Convert.ToDouble(reportItemCollection[0].Text),Is.EqualTo(result));
		}
		
		
		[Test]
		public void SumOneGroup () {
			
			var container = new ExportContainer();
			
			var script = "= sum('randomint')";
			reportItemCollection[0].Text = script;
			container.ExportedItems.AddRange(reportItemCollection);
			
			var visitor = new ExpressionVisitor (reportSettings);
			visitor.SetCurrentDataSource(dataSource.GroupedList);
			var group = dataSource.GroupedList.FirstOrDefault();
			
			visitor.SetCurrentDataSource(group);
			visitor.Visit(container);
			
			var result = list.Where(k => k.GroupItem == group.Key.ToString()).Sum(x => x.RandomInt);
			Assert.That(Convert.ToDouble(reportItemCollection[0].Text),Is.EqualTo(result));
		}
		
		
		[Test]
		public void SumAllGroups () {
			var container = new ExportContainer();
			container.ExportedItems.AddRange(reportItemCollection);

			var visitor = new ExpressionVisitor (reportSettings);
			visitor.SetCurrentDataSource(dataSource.GroupedList);
			foreach (var group in dataSource.GroupedList) {
				var script = "= sum('randomint')";
				reportItemCollection[0].Text = script;
				visitor.SetCurrentDataSource(group);
				visitor.Visit(container);
				
				var result = list.Where(k => k.GroupItem == group.Key.ToString()).Sum(x => x.RandomInt);
				Assert.That(Convert.ToDouble(reportItemCollection[0].Text),Is.EqualTo(result));
			}
		}
		
		
		[SetUp]
		public void CreateExportlist() {
			reportItemCollection = new Collection<ExportText>();
			reportItemCollection.Add(new ExportText()
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
