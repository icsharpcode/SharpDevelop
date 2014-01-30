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
using NUnit.Framework;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Test.Expressions.IntegrationTests
{
	
	[TestFixture]
	public class DateTimeFixture
	{
		Collection<ExportText> collection;
		ExpressionVisitor expressionVisitor;
		
		[Test]
		public void CanHandleToday(){
			
			var script = "= Today.ToShortDateString()";
			var resultColumn = Evaluate(script);
			Assert.That(resultColumn.Text,Is.EqualTo(System.DateTime.Now.ToShortDateString()));
		}
		
		[Test]
		public void CanHandleDateAdd() {
			var expected = DateTime.Today.AddDays(2).ToShortDateString();
			var script = "=Today.AddDays(2).ToShortDateString()";
			var resultColumn = Evaluate(script);
			Assert.That(resultColumn.Text,Is.EqualTo(expected));
		}

		
			
		
		[Test]
		public void CanHandleDayOfWeek () {
			var expected = DateTime.Today.Day.ToString();
			var script ="=Today.Day";
			var resultColumn = Evaluate(script);
			Assert.That(resultColumn.Text,Is.EqualTo(expected));
		}
		
		
		ExportText Evaluate(string script)
		{
			collection[0].Text = script;
			var exportContainer = new ExportContainer();
			exportContainer.ExportedItems.Add(collection[0]);
			expressionVisitor.Visit(exportContainer);
			var resultColumn = (ExportText)exportContainer.ExportedItems[0];
			return resultColumn;
		}	
		
		
		[SetUp]
		public void CreateSut() {
			collection = new Collection<ExportText>();
			collection.Add(new ExportText()
			               {
			               	Text = String.Empty
			               });
			expressionVisitor = new ExpressionVisitor(new ReportSettings());
		}
	}
}
