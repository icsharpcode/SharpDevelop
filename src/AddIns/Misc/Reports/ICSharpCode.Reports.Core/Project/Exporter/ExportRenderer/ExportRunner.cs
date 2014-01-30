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
