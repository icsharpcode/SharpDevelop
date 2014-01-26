/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 26.01.2014
 * Time: 18:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
