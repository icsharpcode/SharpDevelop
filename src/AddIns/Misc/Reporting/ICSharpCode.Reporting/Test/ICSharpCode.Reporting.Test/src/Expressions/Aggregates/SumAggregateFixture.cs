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
