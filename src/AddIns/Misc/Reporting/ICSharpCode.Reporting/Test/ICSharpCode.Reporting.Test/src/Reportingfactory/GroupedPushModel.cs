// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Reflection;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.PageBuilder;
using ICSharpCode.Reporting.Test.DataSource;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Reportingfactory
{
	[TestFixture]
	public class GroupedPushModel
	{
		IReportCreator reportCreator;
		
		[Test]
		public void TestMethod()
		{
			reportCreator.BuildExportList();
			// Only a test value
			Assert.That(reportCreator.Pages.Count,Is.EqualTo(2));
		}
		
		
		[SetUp]
		public void LoadFromStream()
		{
			var contributorList = new ContributorsList();
			var list = contributorList.ContributorCollection;
			
			var asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.GroupedList);

			var reportingFactory  = new ReportingFactory();
			var model =  reportingFactory.LoadReportModel (stream);
			reportCreator = new DataPageBuilder(model,list);
		}	
	}
}
