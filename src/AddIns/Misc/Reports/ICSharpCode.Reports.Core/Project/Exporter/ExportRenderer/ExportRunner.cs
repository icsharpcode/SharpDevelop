/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.05.2011
 * Time: 20:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer
{
	/// <summary>
	/// Description of WpfReportViewModel.
	/// </summary>
	public class ExportRunner:IExportRunner
	{
		
		public event EventHandler<PageCreatedEventArgs> PageCreated;
		
		public ExportRunner()
		{
			Pages = new PagesCollection();
		}
		
		
		public void RunReport(string fileName, ReportParameters parameters)
		{
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			ReportModel model = ReportEngine.LoadReportModel(fileName);
			this.RunReport(model, parameters);
		}
		
		
		public void RunReport(ReportModel reportModel, ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			Pages.Clear();
			if (reportModel.DataModel == GlobalEnums.PushPullModel.FormSheet)
			{
				RunFormSheet(reportModel);
			} else {
				ReportEngine.CheckForParameters(reportModel, parameters);
				var dataManager = DataManagerFactory.CreateDataManager(reportModel, parameters);
				RunReport(reportModel, dataManager);
			}
		}
		
		public void RunReport(ReportModel reportModel, DataTable dataTable, ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			ReportEngine.CheckForParameters(reportModel, parameters);
			IDataManager dataManager = DataManagerFactory.CreateDataManager(reportModel, dataTable);
			IReportCreator reportCreator = DataPageBuilder.CreateInstance(reportModel, dataManager);
//			reportCreator.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
//			reportCreator.GroupHeaderRendering += new EventHandler<GroupHeaderEventArgs>(GroupHeaderRendering);
//			reportCreator.GroupFooterRendering += GroupFooterRendering;
//
//			reportCreator.RowRendering += new EventHandler<RowRenderEventArgs>(RowRendering);
			reportCreator.BuildExportList();
		}
		
		public void RunReport(ReportModel reportModel, System.Collections.IList dataSource, ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataSource == null) {
				throw new ArgumentNullException("dataSource");
			}
			ReportEngine.CheckForParameters(reportModel, parameters);
			IDataManager dataManager = DataManagerFactory.CreateDataManager(reportModel, dataSource);
			RunReport(reportModel, dataManager);
		}
		
		
		public void RunReport(ReportModel reportModel, IDataManager dataManager)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
//			ReportEngine.CheckForParameters(reportModel, parameters);
			IReportCreator reportCreator = DataPageBuilder.CreateInstance(reportModel, dataManager);
			BuildExportList(reportCreator);
		}
		
		
		private void RunFormSheet(ReportModel reportModel)
		{
			IReportCreator reportCreator = FormPageBuilder.CreateInstance(reportModel);
//			reportCreator.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
			BuildExportList(reportCreator);
		}
		
		
		void BuildExportList(IReportCreator reportCreator)
		{
			Pages.Clear();
			reportCreator.PageCreated += OnPageCreated;
			reportCreator.BuildExportList();
			reportCreator.PageCreated -= OnPageCreated;
		}
		
	
		public PagesCollection Pages {get;private set;}
			
			
		private void OnPageCreated (object sender,PageCreatedEventArgs e)
		{
			Pages.Add(e.SinglePage);
			if (PageCreated != null) {
				PageCreated (this,e);
			}
		}
	}
}
