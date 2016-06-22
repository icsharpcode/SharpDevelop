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
using System.Linq;
using System.Reflection;

using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class PageLayoutFixture
	{
		private IReportCreator reportCreator;
		
		[Test]
		public void FormsReportContains_4_Sections()
		{
			reportCreator.BuildExportList();
			var x = reportCreator.Pages[0].ExportedItems;
			var y = from s in x 
				where s.GetType() == typeof(ExportContainer)
				select s;
			Assert.That(y.ToList().Count,Is.EqualTo(4));
			Console.WriteLine("-------PageLayoutFixture:ShowDebug---------");
			var ex = new DebugExporter(reportCreator.Pages);
			ex.Run();
		}
		
		
		[Test]
		public void SectionsInPageDoNotOverlap()
		{
			Point referencePoint = Point.Empty;
			var referenceRect = Rectangle.Empty;
			
			reportCreator.BuildExportList();
			foreach (var item in reportCreator.Pages[0].ExportedItems) {
				var p2 = new Point(item.Location.X,item.Location.Y);
				if (item.Name != "ReportFooter") {
					Assert.That(p2.Y,Is.GreaterThan(referencePoint.Y),item.Name);
					var t = referenceRect.IntersectsWith(item.DisplayRectangle);
					Assert.That(referenceRect.IntersectsWith(item.DisplayRectangle),Is.False);
				}
				referencePoint = new Point(item.Location.X,item.Location.Y + item.Size.Height);
				referenceRect = item.DisplayRectangle;
			}
		}
		
		
		[SetUp]
		public void LoadFromStream()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
			var reportingFactory = new ReportingFactory();
			reportCreator = reportingFactory.ReportCreator(stream);
		}
	}
}
