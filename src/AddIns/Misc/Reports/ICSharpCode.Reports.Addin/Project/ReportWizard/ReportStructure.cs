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
using System.Data;
using System.Drawing;

using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;

/// <summary>
/// This class creates settings for a report
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 28.01.2005 10:31:01
/// </remarks>

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	public class ReportStructure : IDisposable
	{
		
		private ReportItemCollection reportItemCollection;
		private AvailableFieldsCollection availableFieldsCollection;
		
		
		public ReportStructure()
		{
			SqlQueryParameters = new SqlParameterCollection();
		}
		
		public ReportModel CreateAndFillReportModel ()
		{
			ReportModel model = ReportModel.Create();
			
			model.ReportSettings.ReportName = this.ReportName;
			model.ReportSettings.FileName = this.Path + this.FileName;
			
			model.ReportSettings.GraphicsUnit = this.GraphicsUnit;
			model.ReportSettings.ReportType = this.ReportType;

			model.ReportSettings.ConnectionString = this.ConnectionString;
			model.ReportSettings.CommandText = this.SqlString;
			model.ReportSettings.CommandType = this.CommandType;
			model.ReportSettings.DataModel = this.DataModel;
			
			model.ReportSettings.Landscape = this.Landscape;
			
			if (Landscape) {
				model.ReportSettings.PageSize = new Size(GlobalValues.DefaultPageSize.Height,GlobalValues.DefaultPageSize.Width);
			} else {
				model.ReportSettings.PageSize = GlobalValues.DefaultPageSize;
			}
			
			CreateGrouping(model.ReportSettings);
			
			return model;
		}
		
		
		private void CreateGrouping(ReportSettings settings)
		{
			if (!String.IsNullOrEmpty(this.Grouping))
			{
				string s = "=[" + this.Grouping + "]";
				GroupColumn g = new GroupColumn(s,1,System.ComponentModel.ListSortDirection.Ascending);
				settings.GroupColumnsCollection.Add(g);
			}
		}
		
		
		#region BaseSettingsPanel property's
		
		public GraphicsUnit GraphicsUnit {get;set;}
			
		public string ReportName {get;set;}
		
	
		public ICSharpCode.Reports.Core.Globals.GlobalEnums.ReportType ReportType {get;set;}
		
		public string FileName {get;set;}
		
		
		public string Path {get;set;}
		
		public bool Landscape {get;set;}
		
		#endregion
		
		#region DatabasePanel
		
		public string ConnectionString {get;set;}
		
		public string SqlString {get;set;}
		
		public CommandType CommandType {get;set;}
	
		public ICSharpCode.Reports.Core.Globals.GlobalEnums.PushPullModel DataModel {get;set;}
			
		public IDatabaseObjectBase IDatabaseObjectBase {get;set;}
		

		public SqlParameterCollection SqlQueryParameters {get;set;}
		
		#endregion
		
		#region PushModelPanel
		
		public AvailableFieldsCollection AvailableFieldsCollection {
			get {
				if (this.availableFieldsCollection == null) {
					this.availableFieldsCollection = new AvailableFieldsCollection();
				}
				return availableFieldsCollection;
			}
		}
		
		
		public ReportItemCollection ReportItemCollection {
			get {
				if (this.reportItemCollection== null) {
					this.reportItemCollection = new ReportItemCollection();
				}
				return reportItemCollection;
			}
		}
		
		#endregion
		
		public GlobalEnums.ReportLayout ReportLayout {get;set;}
		
		#region Grouping
		
		public string Grouping {get;set;}
		
		#endregion
		
		
		#region IDisposable
		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~ReportStructure(){
			Dispose(false);
		}
		
		protected  virtual void Dispose(bool disposing){
			if (disposing) {
				if (this.reportItemCollection != null) {
					this.reportItemCollection.Clear();
					this.reportItemCollection = null;
				}
				if (this.availableFieldsCollection != null) {
					this.availableFieldsCollection.Clear();
					this.availableFieldsCollection = null;
				}
				if (this.SqlQueryParameters != null) {
					this.SqlQueryParameters.Clear();
					this.SqlQueryParameters = null;
					
				}
			}
		
			// Release unmanaged resources.
			// Set large fields to null.
			// Call Dispose on your base class.
		}
		#endregion
	}
}
