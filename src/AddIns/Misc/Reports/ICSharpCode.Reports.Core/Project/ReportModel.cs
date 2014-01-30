// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
