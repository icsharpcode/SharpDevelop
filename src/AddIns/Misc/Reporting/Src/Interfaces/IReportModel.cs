/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2013
 * Time: 17:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Interfaces
{
	/// <summary>
	/// Description of IReportModel.
	/// </summary>
	public interface IReportModel
	{
		ReportSettings ReportSettings {get;set;}
		/*
		BaseSection ReportHeader {get;}
		BaseSection PageHeader {get;}
		BaseSection DetailSection {get;}
		BaseSection PageFooter {get;}
		BaseSection ReportFooter {get;}
		
		GlobalEnums.PushPullModel DataModel {get;}
		ReportSectionCollection SectionCollection {get;}
		*/
	}
}
