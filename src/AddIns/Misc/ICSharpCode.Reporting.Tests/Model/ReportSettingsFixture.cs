/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.03.2013
 * Time: 19:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Items;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Tests
{
	[TestFixture]
	public class ReportSettingsFixture
	{
		[Test]
		public void DefaultConstructureShouldReturnStandardValues()
		{
			ReportSettings rs = new ReportSettings();
			Assert.IsNotNull(rs,"Should not be 'null'");
			Assert.AreEqual(GlobalValues.DefaultReportName,rs.ReportName);
		}
		
		
		[Test]
		public void DefaultPageSize ()
		{
			ReportSettings rs = new ReportSettings();
			Assert.AreEqual(GlobalValues.DefaultPageSize,rs.PageSize);
		}
	}
}
