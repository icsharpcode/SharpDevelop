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
using SharpQuery.SchemaClass;
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
		//BaseSettingsPanel
		
		private string reportName;
		private string fileName;
		private string path;
		
		private GlobalEnums.ReportType reportType;
		private GraphicsUnit graphicsUnit;
		
		//Database
		private GlobalEnums.PushPullModel dataModel;

		private string connectionString;
		private string sqlString;
		
		private CommandType commandType;
		private SharpQueryProcedure sharpQueryProcedure;
		private ReportItemCollection reportItemCollection;
		private AvailableFieldsCollection availableFieldsCollection;
		
		private ParameterCollection queryParameters;
		
		
		public ReportStructure()
		{
		}
		
		public ReportModel CreateAndFillReportModel ()
		{
			ReportModel model = ReportModel.Create();
			
			model.ReportSettings.ReportName = this.reportName;
			model.ReportSettings.FileName = this.path + this.fileName;
			
			model.ReportSettings.GraphicsUnit = this.graphicsUnit;
			model.ReportSettings.ReportType = this.reportType;

			model.ReportSettings.ConnectionString = this.connectionString;
			model.ReportSettings.CommandText = this.sqlString;
			model.ReportSettings.CommandType = this.commandType;
			model.ReportSettings.DataModel = this.dataModel;
			return model;
		}
		
		#region BaseSettingsPanel property's
		
		public GraphicsUnit GraphicsUnit {
			get {
				return graphicsUnit;
			}
			set {
				graphicsUnit = value;
			}
		}
		public string ReportName {
			get {
				return reportName;
			}
			set {
				reportName = value;
			}
		}
		public ICSharpCode.Reports.Core.GlobalEnums.ReportType ReportType {
			get {
				return reportType;
			}
			set {
				reportType = value;
			}
		}
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public string Path {
			get {
				return path;
			}
			set {
				path = value;
			}
		}
		
		#endregion
		
		#region DatabasePanel
		
		public string ConnectionString {
			get {
				return connectionString;
			}
			set {
				connectionString = value;
			}
		}
		
		/*
		public string CatalogName {
			get {
				return catalogName;
		}
			set {
				catalogName = value;
			}
		}
		*/
		
		public string SqlString {
			get {
				return sqlString;
			}
			set {
				sqlString = value;
			}
		}
		
		
		public CommandType CommandType 
		{
			get {
				return commandType;
			}
			set {
				commandType = value;
			}
		}
		
		
		public ICSharpCode.Reports.Core.GlobalEnums.PushPullModel DataModel {
			get {
				return dataModel;
			}
			set {
				dataModel = value;
			}
		}
		

		public ParameterCollection SqlQueryParameters {
			get {
				if (this.queryParameters == null) {
					this.queryParameters = new ParameterCollection();
				}
				return queryParameters;
			}
		}
		
		/// <summary>
		/// This Property is only usefull for ResultPanel
		/// </summary>
		public SharpQueryProcedure SharpQueryProcedure {
			get {
				return sharpQueryProcedure;
			}
			set {
				sharpQueryProcedure = value;
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
				
				if (this.sharpQueryProcedure != null) {
					this.sharpQueryProcedure = null;
				}
				
			}
		
			// Release unmanaged resources.
			// Set large fields to null.
			// Call Dispose on your base class.
		}
		#endregion
	}
}
