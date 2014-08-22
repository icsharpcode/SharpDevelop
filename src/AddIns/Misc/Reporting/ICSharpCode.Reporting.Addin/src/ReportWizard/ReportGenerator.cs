/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.07.2014
 * Time: 19:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Factories;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;


using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.Addin.ReportWizard.ViewModels;

namespace ICSharpCode.Reporting.Addin.ReportWizard
{
	/// <summary>
	/// Description of ReportGenerator.
	/// </summary>
	public class ReportGenerator
	{
		const int gap = 10;

		public ReportGenerator()
		{
			ReportModel = ReportModelFactory.Create();
		}
		
		
		public void  Generate(ReportWizardContext context) {
			if (context == null)
				throw new ArgumentNullException("context");
			
			var poc = (PageOneContext)context.PageOneContext;
			if (IsDataReport(poc)) {
				CreateDataReport (context);
			} else {
				CreateFormSheetReport(context);
			}
		}

		void CreateFormSheetReport(ReportWizardContext context)
		{
			GenerateBaseSettings(context);
			CreateReportHeader(context);
		}
		
		void CreateDataReport(ReportWizardContext context)
		{
			GenerateBaseSettings(context);
			CreateReportHeader(context);
			GeneratePushModel(context);
		}
		
		void GenerateBaseSettings (ReportWizardContext context)	{
			var pageOneContext = (PageOneContext)context.PageOneContext;
			var reportSettings = ReportModel.ReportSettings;
			reportSettings.DataModel = pageOneContext.DataModel;
			reportSettings.FileName = pageOneContext.FileName;
			reportSettings.Landscape = !pageOneContext.Legal;
			reportSettings.ReportName = pageOneContext.ReportName;
			reportSettings.ReportType = pageOneContext.ReportType;
		}

		
		
		void GeneratePushModel(ReportWizardContext context){
			var pushModelContext = (PushModelContext)context.PushModelContext;
			var xLocation = 5;
			foreach (var element in pushModelContext.Items) {
				var dataItem = new BaseDataItem(){
					Name = element.ColumnName,
					Text = element.ColumnName,
					ColumnName = element.ColumnName,
					DataType = element.DataTypeName
				};
				
				var location = new Point(xLocation,4);
				dataItem.Location = location;
				dataItem.Size = GlobalValues.PreferedSize;
				xLocation = xLocation + GlobalValues.PreferedSize.Width + gap;
				ReportModel.DetailSection.Items.Add(dataItem);
			}
		}
		
		
		void CreateReportHeader(ReportWizardContext context){
			var pageOneContext = (PageOneContext)context.PageOneContext;
			var headerText = new BaseTextItem();
			
			if (String.IsNullOrEmpty(pageOneContext.ReportName)) {
				headerText.Text = GlobalValues.DefaultReportName;
			} else {
				headerText.Text = pageOneContext.ReportName;
			}
			
			headerText.Size = GlobalValues.PreferedSize;
			var printableWith = ReportModel.ReportSettings.PageSize.Width - ReportModel.ReportSettings.LeftMargin - ReportModel.ReportSettings.RightMargin;
			
			var x = (int)(printableWith - headerText.Size.Width) / 2;
			headerText.Location = new Point(x,4);
			ReportModel.ReportHeader.Items.Add(headerText);
			
		}

		static bool IsDataReport(PageOneContext poc)
		{
			return poc.ReportType.Equals(ReportType.DataReport);
		}
		
		public IReportModel ReportModel {get;private set;}
	}
}
