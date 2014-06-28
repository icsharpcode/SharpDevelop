/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.03.2014
 * Time: 17:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Factories
{
	/// <summary>
	/// Description of ReportModelFactory.
	/// </summary>
	public static class ReportModelFactory
	{
		public static ReportModel Create() 
		{
			var reportModel = new ReportModel();
			
			foreach (var section in Enum.GetValues(typeof(GlobalEnums.ReportSection))) {
				reportModel.SectionCollection.Add (SectionFactory.Create(section.ToString()));
			}
			
			foreach (var section in reportModel.SectionCollection) {
					section.Size = new Size(reportModel.ReportSettings.PageSize.Width - reportModel.ReportSettings.LeftMargin - reportModel.ReportSettings.RightMargin,
						GlobalValues.DefaultSectionHeight);
			}
			
			return reportModel;
		}
	}
}
