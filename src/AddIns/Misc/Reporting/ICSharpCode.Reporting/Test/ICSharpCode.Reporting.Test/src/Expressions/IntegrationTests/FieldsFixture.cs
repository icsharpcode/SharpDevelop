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

using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions.InterationTests
{
	[TestFixture]
	public class FieldsFixture
	{
		Collection<ExportText> collection;
		ExpressionVisitor visitor;
	
		[Test]
		public void FieldsInContainer() {
			var script = "=Fields!myfield1 + Fields!myfield2";
			collection[0].Text = script;
			collection.Add(new ExportText() 
			               {
			               	Text = "Sharpdevelop",
			               	Name = "myfield1"
			               });
			collection.Add(new ExportText() 
			               {
			               	Text = " is great",
			               	Name = "myfield2"
			               });               

			var exportContainer = new ExportContainer();
			exportContainer.ExportedItems.Add(collection[0]);
			exportContainer.ExportedItems.Add(collection[1]);
			exportContainer.ExportedItems.Add(collection[2]);
			
			visitor.Visit(exportContainer);	
			Assert.That (collection[0].Text,Is.EqualTo("Sharpdevelop is great"));
		}
		
		
		[Test]
		public void FieldNotExist() {
			var script = "=Fields!myfieldNotExist";
			collection[0].Text = script;
			collection.Add(new ExportText() 
			               {
			               	Text = "Sharpdevelop",
			               	Name = "myfield1"
			               });

			var exportContainer = new ExportContainer();
			exportContainer.ExportedItems.Add(collection[0]);
			exportContainer.ExportedItems.Add(collection[1]);
			
			visitor.Visit(exportContainer);	
			Assert.That (collection[0].Text.StartsWith("Missing"));
			Assert.That (collection[0].Text.Contains("myfieldNotExist"));
		}
		
		
		[SetUp]
		public void CreateExportlist() {
			collection = new Collection<ExportText>();
			collection.Add(new ExportText()
			               {
			               	Text = "myExporttextColumn"
			               });
			visitor = new ExpressionVisitor (new ReportSettings());
		}
		
	}
}
