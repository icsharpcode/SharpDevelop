/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.03.2013
 * Time: 19:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Items;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Tests
{
	[TestFixture]
	public class ReportSettingsFixture
	{
		ReportSettings reportSettings;
		
		[Test]
		public void DefaultConstructureShouldReturnStandardValues()
		{
			Assert.IsNotNull(reportSettings,"Should not be 'null'");
			Assert.AreEqual(GlobalValues.DefaultReportName,reportSettings.ReportName);
		}
		
		
		[Test]
		public void DefaultPageSize ()
		{
			Assert.AreEqual(GlobalValues.DefaultPageSize,reportSettings.PageSize);
		}
		
		
		[Test]
		public void LandScape_True_Return_PageSize_For_LandScape ()
		{
			reportSettings.Landscape = true;
			var landscapeSize = new Size(Globals.GlobalValues.DefaultPageSize.Height,
			                             Globals.GlobalValues.DefaultPageSize.Width);
			Assert.That(reportSettings.PageSize,Is.EqualTo(landscapeSize));
		}
		
		[SetUp]
		public void Setup () {
			reportSettings = new ReportSettings();
		}
	}
}
