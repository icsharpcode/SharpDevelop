//
// SharpDevelop ReportEditor
//
// Copyright (C) 2005 Peter Forstmeier
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Peter Forstmeier (Peter.Forstmeier@t-online.de)

/// <summary>
/// Containerclass for the complete ReportModel
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 09.12.2004 22:11:55
/// </remarks>

namespace SharpReportCore {
	
	using System;
	using System.Drawing;
	using SharpReportCore;
	
	
	public class ReportModel : object,SharpReportCore.IModel,IDisposable{
		
		ReportSettings reportSettings;
		ReportSectionCollection sectionCollection;
	
		public ReportModel() {
			reportSettings = new ReportSettings(new System.Drawing.Printing.PageSettings());
			sectionCollection = new ReportSectionCollection();
			SectionFactory sectionFactory = new SectionFactory();
			foreach (GlobalEnums.enmSection sec in Enum.GetValues(typeof(GlobalEnums.enmSection))) {
				sectionCollection.Add (sectionFactory.Create(sec.ToString()));
			}
			
		}
		
		public ReportModel(GraphicsUnit graphicsUnit):this() {
			reportSettings.GraphicsUnit = graphicsUnit;
		}
		
		
		
		#region SharpReport.DelegatesInterfaces.IModel interface implementation
		public void Accept(IModelVisitor visitor) {
			visitor.Visit (this);
		}
		#endregion
		

		
		#region Sections
		public BaseSection ReportHeader{
			get {
				return (BaseSection)sectionCollection[0];
			}
		}
		
		public BaseSection PageHeader {
			get {
				return (BaseSection)sectionCollection[1];
			}
		}
		
		public BaseSection DetailSection {
			get {
				return (BaseSection)sectionCollection[2];
			}
		}
		
		public BaseSection PageFooter {
			get {
				return (BaseSection)sectionCollection[3];
			}
		}
		
		public BaseSection ReportFooter {
			get {
				return (BaseSection)sectionCollection[4];
			}
		}
		#endregion
		
		#region propertys
		public ReportSettings ReportSettings {
			get {
				return reportSettings;
			}
			set {
				reportSettings = value;
			}
		}
		
		// this Property is only a shortcut,,otherwise we have
		// to use 'ReportModel.reportSettings.DataModel'
		
		public GlobalEnums.enmPushPullModel DataModel {
			get {
				return reportSettings.DataModel;
			}
		}
		
		public ReportSectionCollection SectionCollection {
			get {
				return sectionCollection;
			}
		}
		
		#endregion
		
		#region IDispoable
		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~ReportModel()
		{
			Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing){
			try {
				if (disposing) {
					// Free other state (managed objects).
					if (this.reportSettings != null) {
						this.reportSettings.Dispose();
						this.reportSettings = null;
					}
					if (this.sectionCollection != null) {
						this.sectionCollection.Clear();
						this.sectionCollection = null;
					}
				}
			} finally {
				// Free your own state (unmanaged objects).
				// Set large fields to null.

			}
		}
		#endregion
	}
}
