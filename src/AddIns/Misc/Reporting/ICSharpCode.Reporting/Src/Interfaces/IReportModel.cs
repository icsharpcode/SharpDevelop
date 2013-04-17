/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2013
 * Time: 17:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Interfaces
{
	/// <summary>
	/// Description of IReportModel.
	/// </summary>
	public interface IReportModel
	{
		ReportSettings ReportSettings {get;set;}
		List<BaseSection> SectionCollection {get;}
		/*
		ISection ReportHeader {get;}
		ISection PageHeader {get;}
		ISection DetailSection {get;}
		ISection PageFooter {get;}
		ISection ReportFooter {get;}
		
		GlobalEnums.PushPullModel DataModel {get;}
		*/
		IReportContainer ReportHeader {get;}
		IReportContainer PageHeader {get;}
		IReportContainer DetailSection {get;}
		IReportContainer PageFooter {get;}
		IReportContainer ReportFooter {get;}
	}
}
