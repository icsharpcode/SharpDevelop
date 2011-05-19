/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.05.2011
 * Time: 21:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.ReportViewer;

namespace ICSharpCode.Reports.Core.WpfReportViewer
{
	
	public interface IWpfReportViewer:IPreviewControl
	{
		IDocumentPaginatorSource Document {set;}
	}
	/// <summary>
	/// Interaction logic for WpfReportViewer.xaml
	/// </summary>
	
	public partial class WpfReportViewer : UserControl,IWpfReportViewer
	{
		public WpfReportViewer()
		{
			InitializeComponent();
			Pages = new PagesCollection();
		}
		
		
		
		public IDocumentPaginatorSource Document {
			set {
				this.DocumentViewer.Document = value;
			}
		}
		
//		public event EventHandler<EventArgs> PreviewLayoutChanged;
		
		public PagesCollection Pages {get;private set;}
			
		
		public IReportViewerMessages Messages {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
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
		
		
		public void RunReport(ReportModel reportModel, System.Data.DataTable dataTable, ReportParameters parameters)
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
			reportCreator.PageCreated += OnPageCreated;
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
			reportCreator.PageCreated += OnPageCreated;
			reportCreator.BuildExportList();
		}
		
		private void RunFormSheet(ReportModel reportModel)
		{
			IReportCreator reportCreator = FormPageBuilder.CreateInstance(reportModel);
//			reportCreator.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
			reportCreator.PageCreated += OnPageCreated;
			reportCreator.BuildExportList();
		}
		
		
		private void OnPageCreated (object sender,PageCreatedEventArgs e)
		{
			Pages.Add(e.SinglePage);
		}
	}
}