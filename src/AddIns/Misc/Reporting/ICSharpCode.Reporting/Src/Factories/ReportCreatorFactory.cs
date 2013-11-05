/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 01.06.2013
 * Time: 18:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder;

namespace ICSharpCode.Reporting.Factories
{
	/// <summary>
	/// Description of ReportCreatorFactory.
	/// </summary>
	internal  class ReportCreatorFactory {
		
		public static IReportCreator ExporterFactory(IReportModel reportModel)
		{
			IReportCreator reportCreator = null;
			switch (reportModel.ReportSettings.DataModel) {
				case GlobalEnums.PushPullModel.FormSheet:
					{
						reportCreator = new FormPageBuilder(reportModel);
						break;
					}

				case GlobalEnums.PushPullModel.PullData:
					{
						break;
					}

				case GlobalEnums.PushPullModel.PushData:
					{
						break;
					}

			}
			return reportCreator;
		}
	}
}
