// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core
{
	
	
	public class ReportModel :IReportModel, IDisposable
	{
		ReportSettings reportSettings ;
		ReportSectionCollection sectionCollection;
		
		
		public static ReportModel Create() 
		{
			ReportModel m = new ReportModel();
			foreach (GlobalEnums.ReportSection sec in Enum.GetValues(typeof(GlobalEnums.ReportSection))) {
				m.SectionCollection.Add (SectionFactory.Create(sec.ToString()));
			}
			return m;
		}
		
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
		
		#region propertys
		
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
		
		
		public GlobalEnums.PushPullModel DataModel
		{
			get {
				return reportSettings.DataModel;
			}
		}
		
		public ReportSectionCollection SectionCollection
		{
			get {
				if (this.sectionCollection == null) {
					this.sectionCollection = new ReportSectionCollection();
				}
				return sectionCollection;
			}
		}
		
		#endregion
		
		#region IDispoable
		public void Dispose()
		{
			this.Dispose(true);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.reportSettings != null) {
					this.reportSettings.Dispose();
					this.reportSettings = null;
				}
				if (this.sectionCollection != null) {
					this.sectionCollection.Clear();
					this.sectionCollection = null;
				}
				
			}
		}
		#endregion
	}
}
