// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Reflection;

using ICSharpCode.Reporting.Items;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Model
{
	[TestFixture]
	public class Report_FromListFixture
	{
		private ReportModel model;
		
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
		public void DetailContainsOneDataItem() {
			var section = model.DetailSection;
			Assert.That(section.Items.Count,Is.EqualTo(1));
		}
		[SetUp]
		public void LoadModelFromStream()
		{
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.ReportFromList);
			var rf = new ReportingFactory();
			model = rf.LoadReportModel(stream);
		}	
	}
}
