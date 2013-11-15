// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions.Aggregates
{
	[TestFixture]
	public class SumAggregate
	{
		
		Collection<ExportText> collection;
		AggregateCollection aggregateCollection;
		AggregateFuctionHelper helper;
		CollectionDataSource dataSource;
		
		
		[Test]
		public void CanSum_Int_WholeCollection()
		{
			var reportSettings = new ReportSettings();
			var visitor = new ExpressionVisitor(reportSettings,dataSource);
			var script = "= sum('intValue')";
			collection[0].Text = script;
			visitor.Visit(collection[0]);
			Assert.That (collection[0].Text,Is.EqualTo("406"));
			Assert.That(Convert.ToInt32(collection[0].Text),Is.TypeOf(typeof(int)));
		}
		
		
		[Test]
		public void CanSum_Double_WholeCollection()
		{
			var reportSettings = new ReportSettings();
			var visitor = new ExpressionVisitor(reportSettings,dataSource);
			var script = "= sum('doubleValue')";
			collection[0].Text = script;
			visitor.Visit(collection[0]);
			Assert.That (collection[0].Text,Is.EqualTo("408,25"));
			Assert.That(Convert.ToDouble(collection[0].Text),Is.TypeOf(typeof(double)));
		}
		
		
		
		[SetUp]
		public void CreateExportlist() {
			collection = new Collection<ExportText>();
			collection.Add(new ExportText()
			               {
			               	Text = String.Empty
			               });
			
			helper = new AggregateFuctionHelper();
			aggregateCollection = helper.AggregateCollection;
			dataSource = new CollectionDataSource(aggregateCollection,typeof(Aggregate),new ReportSettings());
			dataSource.Bind();
		}
		
	}
}
