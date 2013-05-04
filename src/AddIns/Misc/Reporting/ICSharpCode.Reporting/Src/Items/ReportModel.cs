/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2013
 * Time: 17:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.Factories;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of ReportModel.
	/// </summary>
	public class ReportModel :IReportModel
	{
		
		public ReportModel() {
			SectionCollection = new List<BaseSection>();
		}
			
	
		#region Sections
		
		public IReportContainer ReportHeader
		{
			get {
				return (BaseSection)SectionCollection[0];
			}
		}
		
		
		public IReportContainer PageHeader
		{
			get {
				return (BaseSection)SectionCollection[1];
			}
		}
		
		
		public IReportContainer DetailSection
		{
			get {
				return (BaseSection)SectionCollection[2];
			}
		}
		
		
		public IReportContainer PageFooter
		{
			get {
				return (BaseSection)SectionCollection[3];
			}
		}
		
		public IReportContainer ReportFooter
		{
			get {
				return (BaseSection)SectionCollection[4];
			}
		}
		
		#endregion
		
		
		ReportSettings reportSettings ;
		
		public ReportSettings ReportSettings
		{
			get {
				if (this.reportSettings == null) {
					this.reportSettings = new ReportSettings();
				}
				return reportSettings;
			}
			set {
				reportSettings = value;
			}
		}
		
		public List<BaseSection> SectionCollection {get; private set;}
	}
}
