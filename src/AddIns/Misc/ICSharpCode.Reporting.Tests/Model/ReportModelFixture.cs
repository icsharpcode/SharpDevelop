/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.03.2013
 * Time: 19:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Items;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Tests.Model
{
	[TestFixture]
	public class ReportModelFixture
	{
		private ReportModel model;
		
		[Test]
		public void CanCreateReportModel()
		{
			Assert.That(model,Is.Not.Null);
		}
		
		
		[Test]
		public void ModelInitializeReportSettings()
		{
			Assert.That(model.ReportSettings,Is.Not.Null);
		}
		
		
		[Test]
		public void ModelReturnsPlainReportName()
		{
			Assert.That(model.ReportSettings.ReportName,Is.EqualTo(Globals.GlobalValues.DefaultReportName));
		}
		
		
		[SetUp]
		public void CreateModel()
		{
			model = ReportModel.Create();
		}
			
	}
}
