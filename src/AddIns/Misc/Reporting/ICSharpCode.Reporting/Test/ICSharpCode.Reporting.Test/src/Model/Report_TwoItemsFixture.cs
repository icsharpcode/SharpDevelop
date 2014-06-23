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
using System.Drawing;
using System.Reflection;

using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Model
{
	[TestFixture]
	public class ReportTwoItemsFixture
	{

		IReportModel model;
		
		[Test]
		public void LoadModelWithItems()
		{
			Assert.That(model,Is.Not.Null);
		}
		
		
		[Test]
		public void ReportHeaderOneItem () {
			var section = model.ReportHeader;
			Assert.That(section.Items.Count,Is.EqualTo(1));
		}
		
		
		[Test]
		public void PageHeaderOneItem () {
			var section = model.ReportHeader;
			Assert.That(section.Items.Count,Is.EqualTo(1));
		}
		
		
		[Test]
		public void ItemIsTextItem() {
			var item = model.ReportHeader.Items[0];
			Assert.That(item,Is.AssignableFrom(typeof(BaseTextItem)));
		}
		

		[Test]
		public void IsLocationSet() {
			var item = model.ReportHeader.Items[0];
			Assert.That(item.Location,Is.Not.EqualTo(Point.Empty));                                   
		}
		
		
		[SetUp]
		public void LoadModelFromStream()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
			var rf = new ReportingFactory();
			model = rf.LoadReportModel(stream);
		}	
	}
}
