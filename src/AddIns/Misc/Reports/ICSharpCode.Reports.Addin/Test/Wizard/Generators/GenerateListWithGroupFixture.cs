// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard.Generators
{
	/// <summary>
	/// Description of GenerateListWithGroupFixture.
	/// </summary>
	[TestFixture]
	public class GenerateListWithGroupFixture
	{
		
		private const string reportName = "ListBasedReport";
		private ReportModel reportModel;
		
	
		[Test]
		public void PageDetail_Should_Contains_GroupedHeader()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.DetailSection;
			
			//GroupHeader
			var c =  new Collection<ICSharpCode.Reports.Core.GroupHeader>(section.Items.OfType<ICSharpCode.Reports.Core.GroupHeader>().ToList());
			Assert.That(c.Count,Is.GreaterThanOrEqualTo(1));
		}
		
		
		[Test]
		public void Section_Should_Contain_DataRow()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.DetailSection;
			//DatatRow
			var c =  new Collection<ICSharpCode.Reports.Core.BaseRowItem>(section.Items.OfType<ICSharpCode.Reports.Core.BaseRowItem>().ToList());
			Assert.That(c.Count,Is.GreaterThanOrEqualTo(1));
		}
		
	
		[Test]
		public void DataRow_Should_Contain_GroupFooter()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.DetailSection;
			
			//GroupFooter
			var c =  new Collection<ICSharpCode.Reports.Core.GroupFooter>(section.Items.OfType<ICSharpCode.Reports.Core.GroupFooter>().ToList());
			Assert.That(c.Count,Is.GreaterThanOrEqualTo(1));
		}
		
		
		[Test]
		public void PageDetail_Should_Contain_Three_Items()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.DetailSection;
			Assert.That(section.Items.Count.Equals(3));
		}
		
		
		
		
		#region Setup/Teardown
		
		[TestFixtureSetUp]
		public void Init()
		{
			bool createGrouping = true;
			this.reportModel = ReportGenerationHelper.CreateModel(reportName,createGrouping);
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	
		private static ReportStructure CreateReportStructure (string reportName)
		{
			ReportStructure structure = new ReportStructure();
			structure.ReportName = reportName;
			structure.DataModel = GlobalEnums.PushPullModel.PushData;
			structure.ReportType = GlobalEnums.ReportType.DataReport;
			return structure;
		}
		
		
		#endregion
	}
}
