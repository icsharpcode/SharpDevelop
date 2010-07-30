// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using System.Drawing;

using ICSharpCode.Reports.Core;

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
		
		private ParameterCollection queryParameters;
		
		
		public ReportStructure()
		{
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
			CheckGrouping(model.ReportSettings);
			
			return model;
		}
		
		private void CheckGrouping(ReportSettings settings)
		{
			if (!String.IsNullOrEmpty(this.Grouping)){
				GroupColumn g = new GroupColumn(Grouping,1,System.ComponentModel.ListSortDirection.Ascending);
				
				settings.GroupColumnsCollection.Add(g);
			}
		}
		
		#region BaseSettingsPanel property's
		
		public GraphicsUnit GraphicsUnit {get;set;}
			
		public string ReportName {get;set;}
		
	
		public ICSharpCode.Reports.Core.GlobalEnums.ReportType ReportType {get;set;}
		
		public string FileName {get;set;}
		
		
		public string Path {get;set;}
		
		public bool Landscape {get;set;}
		
		#endregion
		
		#region DatabasePanel
		
		public string ConnectionString {get;set;}
		
		public string SqlString {get;set;}
		
		public CommandType CommandType {get;set;}
	
		public ICSharpCode.Reports.Core.GlobalEnums.PushPullModel DataModel {get;set;}
			

		public ParameterCollection SqlQueryParameters {
			get {
				if (this.queryParameters == null) {
					this.queryParameters = new ParameterCollection();
				}
				return queryParameters;
			}
		}
		
		
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
				if (this.queryParameters != null) {
					this.queryParameters.Clear();
					this.queryParameters = null;
					
				}
				
//				if (this.SharpQueryProcedure != null) {
//					this.SharpQueryProcedure = null;
//				}
				
			}
		
			// Release unmanaged resources.
			// Set large fields to null.
			// Call Dispose on your base class.
		}
		#endregion
	}
}
