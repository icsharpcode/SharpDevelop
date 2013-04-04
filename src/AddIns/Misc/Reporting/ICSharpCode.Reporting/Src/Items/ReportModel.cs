/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2013
 * Time: 17:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of ReportModel.
	/// </summary>
	internal class ReportModel :IReportModel
	{
		
//		ReportSectionCollection sectionCollection;
		
		
		public static ReportModel Create() 
		{
			var model = new ReportModel();
//			foreach (GlobalEnums.ReportSection sec in Enum.GetValues(typeof(GlobalEnums.ReportSection))) {
//				m.SectionCollection.Add (SectionFactory.Create(sec.ToString()));
//			}
			return model;
		}
		
		/*
		public static ReportModel Create(GraphicsUnit graphicsUnit) 
		{
			ReportModel m = Create();
			m.ReportSettings.GraphicsUnit = graphicsUnit;
			return m;
		}
		
		
		#region Sections
		
		public BaseSection ReportHeader
		{
			get {
				return (BaseSection)sectionCollection[0];
			}
		}
		
		
		public BaseSection PageHeader
		{
			get {
				return (BaseSection)sectionCollection[1];
			}
		}
		
		
		public BaseSection DetailSection
		{
			get {
				return (BaseSection)sectionCollection[2];
			}
		}
		
		
		public BaseSection PageFooter
		{
			get {
				return (BaseSection)sectionCollection[3];
			}
		}
		
		public BaseSection ReportFooter
		{
			get {
				return (BaseSection)sectionCollection[4];
			}
		}
		
		#endregion
		*/
		
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
	}
}
