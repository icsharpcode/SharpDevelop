// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Factories;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;

/// <summary>
/// This Class contains the basic Functions to handle reports
/// </summary>

namespace ICSharpCode.Reports.Core {
	public class ReportEngine : IDisposable
	{
		private PreviewControl previewControl;
		
		/// <summary>
		/// This event is fired before a Section is Rendered, you can use
		/// it to modify items to be printed
		/// </summary>
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		/// <summary>
		/// This event is fired after a section's output is done
		/// </summary>
		public event EventHandler<SectionRenderEventArgs> SectionRendered;
		
		
		#region Constructor
		
		public ReportEngine()
		{
		}
		
		#endregion
		
		#region create Connection handle Sql Parameter's
		/*
		public static ConnectionObject PrepareConnectionFromParameters (ReportSettings settings,
		                                                                ReportParameters reportParameters)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			ConnectionObject conObj = null;
			if (reportParameters != null) {
				if (reportParameters.ConnectionObject != null) {
					conObj = reportParameters.ConnectionObject;
				}
			}
			
			if (!String.IsNullOrEmpty(settings.ConnectionString)) {
				conObj = ConnectionObjectFactory.BuildConnectionObject(settings);
			}
			return conObj;
		}
		 */
		#endregion
		
		
		#region Events
		
		private void OnSectionPrinting (object sender,SectionRenderEventArgs e) {
			if (this.SectionRendering != null) {
				this.SectionRendering(this,e);
			}
		}
		
		private void OnSectionPrinted (object sender,SectionRenderEventArgs e) {
			if (this.SectionRendered != null) {
				this.SectionRendered (this,e);
			}
		}
		
		#endregion
		
		
		#region Setup for print/preview
		
		private static IReportModel ValidatePushModel (string fileName) {
			IReportModel model = LoadReportModel (fileName);
			if (model.ReportSettings.DataModel != GlobalEnums.PushPullModel.PushData) {
				throw new InvalidReportModelException();
			}
			return model;
		}
		
		
		internal static void CheckForParameters (IReportModel model,ReportParameters reportParameters)
		{
			if (reportParameters != null) {
				
				if (reportParameters.SortColumnCollection.Count > 0) {
					model.ReportSettings.SortColumnsCollection.AddRange(reportParameters.SortColumnCollection);
				}
				
				if (reportParameters.Parameters.Count > 0)
				{
					reportParameters.Parameters.ForEach(item => SetReportParam(model,item));
				}
				
				if (reportParameters.SqlParameters.Count > 0)
				{
					reportParameters.SqlParameters.ForEach(item => SetSqlParam(model,item));
				}
			}
		}
		
		
		private static void SetReportParam (IReportModel model,BasicParameter param)
		{
			var p = model.ReportSettings.ParameterCollection.Find(param.ParameterName);
			if (p != null) {
				p.ParameterValue = param.ParameterValue;
			}
		}
		
		
		private static void SetSqlParam (IReportModel model,SqlParameter param)
		{
			var p = model.ReportSettings.SqlParameters.Find(param.ParameterName);
			if (p != null) {
				p.ParameterValue = param.ParameterValue;
			}
		}
		

		/*
		protected static ColumnCollection CollectFieldsFromModel(ReportModel model){
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			ColumnCollection col = new ColumnCollection();
			
			foreach (BaseSection section in model.SectionCollection){
				for (int i = 0;i < section.Items.Count ;i ++ ) {
					IReportItem item = section.Items[i];
					BaseDataItem baseItem = item as BaseDataItem;
					if (baseItem != null) {
						col.Add(new AbstractColumn(baseItem.ColumnName));
					}
				}
			}
			return col;
		}
		 */
		
		/// <summary>
		/// Creates an <see cref="AbstractRenderer"></see>
		/// any Class deriving from this can be
		/// used to get a <see cref="System.Drawing.Printing.PrintDocument"></see>
		/// </summary>
		/// <param name="model"><see cref="ReportModel"></see></param>
		/// <returns></returns>
		
		
		protected AbstractRenderer SetupStandardRenderer (IReportModel model,ReportParameters parameters) {

			AbstractRenderer abstr = null;
			try {
				switch (model.ReportSettings.ReportType) {
						//FormSheets reports
					case GlobalEnums.ReportType.FormSheet:
						abstr = RendererFactory.Create (model,null);
						break;
						//Databased reports
					case GlobalEnums.ReportType.DataReport :
						IDataManager dataMan  = DataManagerFactory.CreateDataManager(model,parameters);
						abstr = RendererFactory.Create (model,dataMan);
						break;
					default:
						throw new ReportException ("SharpReportmanager:SetupRenderer -> Unknown Reporttype");
				}
				
				abstr.Rendering += new EventHandler<SectionRenderEventArgs>(OnSectionPrinting);
				abstr.SectionRendered +=new EventHandler<SectionRenderEventArgs>(OnSectionPrinted);
				return abstr;
			} catch (Exception) {
				throw;
			}
		}
		
		
		protected AbstractRenderer SetupPushDataRenderer (IReportModel model,
		                                                  IList list) {
			
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			if (model.ReportSettings.ReportType != GlobalEnums.ReportType.DataReport) {
				throw new ArgumentException("SetupPushDataRenderer <No valid ReportModel>");
			}
			if (model.ReportSettings.DataModel != GlobalEnums.PushPullModel.PushData) {
				throw new ArgumentException("SetupPushDataRenderer <No valid ReportType>");
			}
			
			AbstractRenderer abstr = null;
			
			IDataManager dataMan  = DataManager.CreateInstance(list,model.ReportSettings);
			abstr = RendererFactory.Create (model,dataMan);

			abstr.Rendering += new EventHandler<SectionRenderEventArgs>(OnSectionPrinting);
			abstr.SectionRendered +=new EventHandler<SectionRenderEventArgs>(OnSectionPrinted);
			return abstr;
		}
		
		
		protected AbstractRenderer SetupPushDataRenderer (IReportModel model,
		                                                  DataTable dataTable) {
			
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			if (model.ReportSettings.ReportType != GlobalEnums.ReportType.DataReport) {
				throw new ArgumentException("SetupPushDataRenderer <No valid ReportModel>");
			}
			if (model.ReportSettings.DataModel != GlobalEnums.PushPullModel.PushData) {
				throw new ArgumentException("SetupPushDataRenderer <No valid ReportType>");
			}
			AbstractRenderer abstr = null;
			IDataManager dataMan  = DataManager.CreateInstance(dataTable,model.ReportSettings);
			if (dataMan != null) {
				abstr = RendererFactory.Create (model,dataMan);
				abstr.Rendering += new EventHandler<SectionRenderEventArgs>(OnSectionPrinting);
				abstr.SectionRendered +=new EventHandler<SectionRenderEventArgs>(OnSectionPrinted);
				
				return abstr;
			}
			return null;
		}
		
		
		#endregion
		
		
		#region Parameter Handling
		
		///<summary>
		/// Loads the report, and initialise <see cref="ReportParameters"></see>
		/// Fill the <see cref="AbstractParametersCollection"></see>
		///  with <see cref="SqlParameters">
		/// this is an easy way to ask the report for desired paramaters</see></summary>
		public static ReportParameters LoadParameters (string fileName) {
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			ReportModel model = null;
			try {
				model = LoadReportModel (fileName);
				ReportParameters pars = new ReportParameters();
				pars.Parameters.AddRange (model.ReportSettings.ParameterCollection);
				pars.SqlParameters.AddRange(model.ReportSettings.SqlParameters);
				return pars;
			} catch (Exception) {
				throw;
			}
		}
		
		#endregion
		
		#region LoadModule
		
		/// <summary>
		/// Load's a <see cref="ReportModel"></see> from File using the
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns><see cref="ReportModel"></see></returns>
		public static ReportModel LoadReportModel (string fileName) {
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			var doc = new XmlDocument();
			doc.Load(fileName);
			ReportModel model = LoadModel(doc);
			
			model.ReportSettings.FileName = fileName;
			FilePathConverter.AdjustReportName(model);
			return model;
		}

		
		public static ReportModel LoadReportModel (Stream stream) {
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			var doc = new XmlDocument();
			doc.Load(stream);
			return LoadModel(doc);
		}
		
		
		static ReportModel LoadModel(XmlDocument doc)
		{
			BaseItemLoader loader = new BaseItemLoader();
			object root = loader.Load(doc.DocumentElement);

			var model = root as ReportModel;
			if (model == null) {
				throw new IllegalFileFormatException("ReportModel");
			}
			return model;
		}
		
		
		#endregion
		
		#region Preview to Windows PreviewDialog
		
		public void PreviewStandardReport (string fileName,ReportParameters reportParameters)
		{
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			
			AbstractRenderer renderer = null;
			try {
				ReportModel model = LoadReportModel (fileName);
				CheckForParameters(model,reportParameters);
				renderer = SetupStandardRenderer (model,reportParameters);
				if (renderer != null) {
					PreviewControl.ShowPreview(renderer,1.5,false);
				}
			} catch (Exception) {
				throw;
			}
		}
		

		public void PreviewPushDataReport (string fileName,DataTable dataTable,ReportParameters reportParameters)
		{
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			AbstractRenderer renderer = null;
			try {
				IReportModel model = ValidatePushModel (fileName);
				CheckForParameters(model,reportParameters);
				renderer = this.SetupPushDataRenderer (model,dataTable);
				if (renderer != null) {
					PreviewControl.ShowPreview(renderer,1.5,false);
				}
			} catch (Exception) {
				throw;
			}
		}
		
		
		public void PreviewPushDataReport (string fileName,IList list,ReportParameters reportParameters)
		{
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			if (list == null) {
				throw new ArgumentNullException("list");
			}
			AbstractRenderer renderer = null;
			try {
				IReportModel model = ValidatePushModel(fileName);
				CheckForParameters(model,reportParameters);
				renderer = this.SetupPushDataRenderer (model,list);
				
				if (renderer != null) {
					PreviewControl.ShowPreview(renderer,1.5,false);
				}
				
			} catch (Exception) {
				throw;
			}
		}
		
		#endregion
		
		#region PageBuilder
		
		/// <summary>
		/// Use this method for pullData and FormSheet
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="reportParameters">null if no parameters are used</param>
		/// <returns><see cref="BasePager"</returns>
		public static IReportCreator CreatePageBuilder (string fileName,
		                                                ReportParameters reportParameters)
		{
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			
			IReportModel reportModel = LoadReportModel (fileName);
			IReportCreator builder = null;
			if (reportModel.DataModel == GlobalEnums.PushPullModel.FormSheet) {
				builder = FormPageBuilder.CreateInstance(reportModel);
			} else {
				CheckForParameters(reportModel,reportParameters);
				IDataManager dataMan  = DataManagerFactory.CreateDataManager(reportModel,reportParameters);
				builder = DataPageBuilder.CreateInstance(reportModel, dataMan);
			}
			return builder;
		}
		
		
		/// <summary>
		/// For internal use only
		/// </summary>
		/// <param name="reportModel"></param>
		/// <returns></returns>
		/*
		internal static IReportCreator CreatePageBuilder (IReportModel reportModel)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			IDataManager dataMan  = DataManagerFactory.CreateDataManager(reportModel,(ReportParameters)null);
			IReportCreator builder = DataPageBuilder.CreateInstance(reportModel, dataMan);
			return builder;
		}
		 */

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reportModel"></param>
		/// <param name="dataSet"></param>
		/// <param name="reportParameter"></param>
		/// <returns></returns>
		public static IReportCreator CreatePageBuilder (IReportModel reportModel,
		                                                DataSet dataSet,
		                                                ReportParameters reportParameter)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataSet == null) {
				throw new ArgumentNullException("dataSet");
			}
			return CreatePageBuilder (reportModel,dataSet.Tables[0],reportParameter);
		}
		
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reportModel"></param>
		/// <param name="dataTable"></param>
		/// <param name="reportParameters"></param>
		/// <returns></returns>
		
		public static IReportCreator CreatePageBuilder (IReportModel reportModel,
		                                                DataTable dataTable,
		                                                ReportParameters reportParameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			ReportEngine.CheckForParameters(reportModel,reportParameters);
			IDataManager dataMan  = DataManager.CreateInstance(dataTable,reportModel.ReportSettings);
			if (dataMan != null)
			{
				return DataPageBuilder.CreateInstance(reportModel, dataMan);
			} else {
				throw new MissingDataManagerException();
			}
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reportModel"></param>
		/// <param name="list"></param>
		/// <param name="reportParameters"></param>
		/// <returns></returns>
		public static  IReportCreator CreatePageBuilder (IReportModel reportModel,
		                                                 IList list,
		                                                 ReportParameters reportParameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (list == null) {
				throw new ArgumentNullException("list");
			}
			ReportEngine.CheckForParameters(reportModel,reportParameters);
			IDataManager dataMan  = DataManager.CreateInstance(list,reportModel.ReportSettings);
			if (dataMan != null) {
				return DataPageBuilder.CreateInstance(reportModel,dataMan);
			} else {
				throw new MissingDataManagerException();
			}
		}
		
		
		#endregion
		
		
		#region Printing
		
		private static void ReportToPrinter (AbstractRenderer renderer,IReportModel model) {
			if (renderer == null) {
				throw new ArgumentNullException("renderer");
			}
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			PrintDocument  doc = null;
			if (renderer.Cancel == false) {
				doc = renderer.ReportDocument;
				using (PrintDialog dlg = new PrintDialog()) {
					dlg.Document = doc;
					if (model.ReportSettings.UseStandardPrinter == true) {
						dlg.Document.Print();
					} else {
						DialogResult result = dlg.ShowDialog();
						// If the result is OK then print the document.
						if (result==DialogResult.OK){
							dlg.Document.Print();
						}
					}
				}
			}
		}
		
		/// <summary>
		/// send a Standart (PullModel/FormSheet) Report to printer
		/// </summary>
		/// <param name="fileName">Path to ReportFile</param>
		/// <param name="renderTo">Type of renderer currently only "ToText" is implemented</param>
//		[Obsolete("use PrintStandardReport (fileName,null)")]
//		public void PrintStandardReport (string fileName) {
//			if (String.IsNullOrEmpty(fileName)) {
//				throw new ArgumentNullException("fileName");
//			}
//			PrintStandardReport (fileName,null);
//
//		}
//
		
		public void PrintStandardReport (string fileName,ReportParameters reportParameters) {
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			
			AbstractRenderer renderer = null;
			try {
				IReportModel model = LoadReportModel (fileName);
//				if (this.connectionObject == null) {
//					this.connectionObject = ReportEngine.PrepareConnectionFromParameters (model.ReportSettings,reportParameters);
//				}
//
				ReportEngine.CheckForParameters(model,reportParameters);
				renderer = SetupStandardRenderer (model,reportParameters);
				ReportEngine.ReportToPrinter (renderer,model);
				
			} catch (Exception) {
				throw;
			}
		}
		
		/// <summary>
		/// Print a PushModel Report, if <see cref="UseStandartPrinter"></see> in
		/// <see cref="ReportSettings"></see> is true, the Report is send directly to the printer,
		///  otherwise, we show a <see cref="PrintDialog"></see>
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="dataTable"></param>
		
		public void PrintPushDataReport (string fileName,DataTable dataTable) {
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			this.PrintPushDataReport(fileName,dataTable,null);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="dataTable"></param>
		/// <param name="reportModel"></param>
		public void PrintPushDataReport (string fileName,DataTable dataTable,
		                                 ReportParameters reportParameters)
		{
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			AbstractRenderer renderer = null;
			try {
				IReportModel model = ReportEngine.ValidatePushModel(fileName);
				CheckForParameters(model,reportParameters);
				renderer = SetupPushDataRenderer (model,dataTable);
				ReportEngine.ReportToPrinter(renderer,model);
				
			} catch (Exception) {
				throw;
			}
		}
		
		#endregion
		
		
		#region Properties
		///<summary>
		/// Use this Dialog for previewing a Report
		/// This Control is also used by the Designer
		/// </summary>
		
		public PreviewControl  PreviewControl  {
			get {
				if (this.previewControl == null) {
					previewControl = new PreviewControl();
				}
				return this.previewControl;
			}
		}
		
		#endregion
		
		
		#region IDisposable
		
		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~ReportEngine(){
			Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing){
			try {
				if (disposing) {
					if (this.previewControl != null) {
						this.previewControl.Dispose();
					}
				}
			} finally {
				// Release unmanaged resources.
				// Set large fields to null.
				// Call Dispose on your base class.
			}
		}
		#endregion
	}
}
