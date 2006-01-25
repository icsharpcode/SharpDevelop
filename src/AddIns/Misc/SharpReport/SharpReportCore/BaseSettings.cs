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
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Xml.Serialization;

	/// <summary>
	/// BaseClass for all ...Settings
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 07.10.2005 22:50:43
	/// </remarks>
namespace SharpReportCore {	
	public class BaseSettings : object {
		private const string defaultReportName = "SharpReport1";

		
		private string reportName;
		private string fileName;
		private PageSettings pageSettings;
		
		private bool useStandartPrinter;
		private bool includeSettings;
		
		//if file is read, supress events
		private bool initDone;
		
		private GraphicsUnit graphicsUnit;
		private Margins defaultMargins = new Margins (50,50,50,50);
		
		public event EventHandler PropertyChanged;
		public event EventHandler FileNameChanged;
		
		#region Constructor
		public BaseSettings():this(new PageSettings(),"","") {
			BaseValues();
		}
		
		public BaseSettings(PageSettings pageSettings , string reportName,string fileName){
			if (pageSettings == null) {
				throw new ArgumentException("BaseSettings:PageSettings");
			}
			
			if (String.IsNullOrEmpty(reportName)) {
				this.reportName = GlobalValues.SharpReportStandartFileName;
			} else {
				this.reportName = reportName;
			}
			
			if (String.IsNullOrEmpty(fileName)) {
				this.fileName = GlobalValues.SharpReportPlainFileName;
			} else {
				this.fileName = MakePoperFilename(fileName);
			}
			
			
			this.pageSettings = pageSettings;
			
			
			BaseValues();
		}
		void BaseValues() {
			this.useStandartPrinter = true;
			this.includeSettings = false;
			this.graphicsUnit = GraphicsUnit.Millimeter;
		}
		
		private string MakePoperFilename (string file) {

			if (file.EndsWith (GlobalValues.SharpReportExtension)) {
				return file;
			} else {
				if (file.IndexOf('.') > 0) {
					string [] s1 = file.Split('.');
					return s1[0] + GlobalValues.SharpReportExtension;
				} else {
					return file + GlobalValues.SharpReportExtension;
				}
				
			}
		}
		#endregion
		
		protected void NotifyPropertyChanged() {
			if (this.initDone) {
				if (PropertyChanged != null) {
					PropertyChanged (this,new EventArgs());
				}
			}
		}
		
		#region Properties
		protected string DefaultReportName {
			get {
				return defaultReportName;
			}
		}
		
		protected string DefaultFileName {
			get {
				return GlobalValues.SharpReportPlainFileName;
			}
		}
		
		
		protected bool InitDone {
			get {
				return initDone;
			}
			set {
				initDone = value;
			}
		}
		
		///<summary>
		/// printout ReportSettings at ReportHeaderPage
		/// </summary>
		[Category("Base Settings")]
		[DefaultValueAttribute (false)]
		public bool IncludeSettings {
			get {
				return includeSettings;
			}
			set {
				if (includeSettings != value) {
					includeSettings = value;
					this.NotifyPropertyChanged();
				}
			}
		}
		
		[Category("Base Settings")]
		[DefaultValueAttribute ("")]
		public string ReportName {
			get {
				if (reportName.Length == 0) {
					reportName = defaultReportName;
				}
				return reportName;
			}
			set {
				if (reportName != value) {
					reportName = value;
					this.NotifyPropertyChanged();
				}
			}
		}
		
		[Category("Base Settings")]
		[DefaultValueAttribute ("")]
		[XmlIgnoreAttribute]
		public string FileName {
			get {
				if (fileName.Length == 0) {
					fileName = GlobalValues.SharpReportPlainFileName;
				}
				return fileName;
			}
			set {
				if (fileName != value) {
					fileName = value;
					this.NotifyPropertyChanged();
					if (FileNameChanged != null ){
						FileNameChanged (this,EventArgs.Empty);
					}
				}
			}
		}
		
		[Category("Output Settings")]
		[DefaultValueAttribute (true)]
		public bool UseStandartPrinter {
			get {
				return useStandartPrinter;
			}
			set {
				if (useStandartPrinter != value) {
					useStandartPrinter = value;
					this.NotifyPropertyChanged();
				}
			}
		}
		
		
		[Category("Output Settings")]
		[XmlIgnoreAttribute]
		public System.Drawing.Printing.PageSettings PageSettings {
			get {
				return pageSettings;
			}
			set {
				this.pageSettings = value;
				this.NotifyPropertyChanged();
			}
		}
		
		[Category("Output Settings")]
		public System.Drawing.Printing.Margins DefaultMargins {
			get {
				return defaultMargins;
				
			}
			set {
				if (defaultMargins != value) {
					defaultMargins = value;
					PageSettings.Margins = defaultMargins;
					this.NotifyPropertyChanged();
				}
				
			}
		}
		[Category("Designer Settings")]
		[DefaultValueAttribute (System.Drawing.GraphicsUnit.Millimeter)]
		public System.Drawing.GraphicsUnit GraphicsUnit {
			get {
				return graphicsUnit;
			}
			set {
				if (graphicsUnit != value) {
					graphicsUnit = value;
					this.NotifyPropertyChanged();
				}
			}
		}
		#endregion
	}
}
