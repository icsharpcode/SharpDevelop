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
	internal class ReportModel :IReportModel
	{
		
		public ReportModel() {
			SectionCollection = new List<BaseSection>();
//			foreach (GlobalEnums.ReportSection sec in Enum.GetValues(typeof(GlobalEnums.ReportSection))) {
//				SectionCollection.Add (SectionFactory.Create(sec.ToString()));
//			}
		}
			
	
		#region Sections
		
		public ISection ReportHeader
		{
			get {
				return (BaseSection)SectionCollection[0];
			}
		}
		
		
		public ISection PageHeader
		{
			get {
				return (BaseSection)SectionCollection[1];
			}
		}
		
		
		public ISection DetailSection
		{
			get {
				return (BaseSection)SectionCollection[2];
			}
		}
		
		
		public ISection PageFooter
		{
			get {
				return (BaseSection)SectionCollection[3];
			}
		}
		
		public ISection ReportFooter
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
