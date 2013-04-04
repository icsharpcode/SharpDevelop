/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reporting.Test
{
	/// <summary>
	/// Description of TestHelper.
	/// </summary>
	public class TestHelper
	{
		private const string nameSpace = "ICSharpCode.Reporting.Test.src.TestReports.";
		private const string plainReportName = "PlainModel.srd";
		
		public TestHelper()
		{
		}
		
		public static string PlainReportFileName{
			get{return nameSpace + plainReportName;}
		}
	}
}
