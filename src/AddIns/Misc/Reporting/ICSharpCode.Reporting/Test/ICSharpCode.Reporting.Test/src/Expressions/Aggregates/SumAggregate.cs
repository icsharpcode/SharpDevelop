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
		ExpressionVisitor visitor;
		
		[Test]
		public void CanSum_Int_WholeCollection()
		{
			var script = "= sum('intValue')";
			var result = Evaluate(script);
			Assert.That (result.Text,Is.EqualTo("406"));
			Assert.That(Convert.ToInt32(result.Text),Is.TypeOf(typeof(int)));
		}
		
		
		[Test]
		public void CanSum_Double_WholeCollection()
		{
			var script = "= sum('doubleValue')";
			var result = Evaluate(script);
			Assert.That (result.Text,Is.EqualTo("408,25"));
			Assert.That(Convert.ToDouble(result.Text),Is.TypeOf(typeof(double)));
		}
		
		
		[Test]
		public void CanSum_Double_With_String_Concat()
		{
			var script = "= 'myText ' + sum('doubleValue')";
			var result = Evaluate(script);
			Assert.That (result.Text,Is.EqualTo("myText 408,25"));
		}
		
		
		ExportText Evaluate (string script) {
			collection[0].Text = script;
			visitor.Visit(collection[0]);
			return collection[0];
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
			dataSource = new CollectionDataSource(aggregateCollection,new ReportSettings());
			dataSource.Bind();
			visitor = new ExpressionVisitor(new ReportSettings());
			visitor.SetCurrentDataSource(dataSource.SortedList);
		}
		
	}
}
