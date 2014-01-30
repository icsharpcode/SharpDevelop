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
using System.Windows.Forms;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin.Test
{
	[TestFixture]
	public class MeasurementServiceFixture
	{
		Graphics graphics;
		BaseSection section;
		
		[Test]
		public void Constructor()
		{
			MeasurementService s = new MeasurementService(this.graphics);
			Assert.IsNotNull(s);
		}
		
		[Test]
		public void ReportItemFitInSection ()
		{
			MeasurementService s = new MeasurementService(this.graphics);
			Rectangle r = new Rectangle(section.Location,section.Size);
			this.section.Items.Clear();
			BaseReportItem item = new BaseReportItem();
			item.Location = new Point(10,10);
			item.Size = new Size(50,50);
			this.section.Items.Add(item);
				s.FitSectionToItems(this.section);
			Assert.AreEqual(r.Location,s.Rectangle.Location);
		}
		
		[Test]
		public void CheckPlainSection ()
		{
			MeasurementService s = new MeasurementService(this.graphics);
			Rectangle r = new Rectangle(section.Location,section.Size);
			s.FitSectionToItems(this.section);
			Assert.AreEqual(r.Location,s.Rectangle.Location);
		}
	
		
		[TestFixtureSetUp]
		public void Init()
		{
			Label l = new Label();
			this.graphics = l.CreateGraphics();
			section = new BaseSection();
			section.Location = new Point (50,50);
			section.Size = new Size(400,200);
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}
