/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.07.2014
 * Time: 19:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.Reporting.Factories;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.BaseClasses;

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
		const int startLocation = 5;
		public ReportGenerator()
		{
			ReportModel = ReportModelFactory.Create();
		}
		
		
		public void  Generate(ReportWizardContext context) {
			if (context == null)
				throw new ArgumentNullException("context");
			
			
			if (IsDataReport(context)) {
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
			CreatePageHeader(context);
			CreateDetailsSection(context);
			CreatePageFooter (context);
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

		
		
		void CreateDetailsSection(ReportWizardContext context){
			var pushModelContext = (PushModelContext)context.PushModelContext;
			foreach (var element in pushModelContext.Items) {
				var dataItem = new BaseDataItem(){
					Name = element.ColumnName,
					Text = element.ColumnName,
					ColumnName = element.ColumnName,
					DataType = element.DataTypeName
				};
				ReportModel.DetailSection.Items.Add(dataItem);
			}
			AdjustItems(ReportModel.DetailSection.Items,startLocation);
		}
		
		
		void CreateReportHeader(ReportWizardContext context){
			var pageOneContext = (PageOneContext)context.PageOneContext;
			
			var headerText = new BaseTextItem();
			
			headerText.Text = String.IsNullOrEmpty(pageOneContext.ReportName) ? GlobalValues.DefaultReportName : pageOneContext.ReportName;
			
			var xLoc = (ReportModel.ReportSettings.PrintableWidth() - headerText.Size.Width) / 2;
			headerText.Location = new Point(xLoc,4);
			ReportModel.ReportHeader.Items.Add(headerText);
			
			xLoc = ReportModel.ReportSettings.PrintableWidth() - GlobalValues.PreferedSize.Width - 20;
		
			var dateText = new BaseTextItem(){
				Text ="= Today.Today",
				Location = new Point(xLoc ,10)
			};
			
			ReportModel.ReportHeader.Items.Add(dateText);
		}

		
		void CreatePageHeader(ReportWizardContext context)
		{
			var pushModelContext = (PushModelContext)context.PushModelContext;
			foreach (var element in pushModelContext.Items) {
				var dataItem = new BaseTextItem(){
					Name = element.ColumnName,
					Text = element.ColumnName
				};
				ReportModel.PageHeader.Items.Add(dataItem);
			}
			
			AdjustItems(ReportModel.PageHeader.Items,startLocation);
			
			var line = new BaseLineItem(){
				Location = new Point(2,35),
				FromPoint = new Point(1,1),
				ToPoint = new Point(ReportModel.ReportSettings.PrintableWidth() -10,1),
				Size = new Size (ReportModel.ReportSettings.PrintableWidth() -5,5)
			};
			
			ReportModel.PageHeader.Items.Add(line);
			
		}
// 
		void CreatePageFooter(ReportWizardContext context)
		{
			var pageOneContext = (PageOneContext)context.PageOneContext;
			var lineNrField = new BaseTextItem() {
				Text = "='Page : ' + Globals!PageNumber + ' of ' + Globals!Pages + ' Page(s)'",
				Name = "LineNumber",
				Location = new Point(300,10),
				Size = new Size (GlobalValues.PreferedSize.Width * 2,GlobalValues.PreferedSize.Height)
			};
			ReportModel.PageFooter.Items.Add(lineNrField);
		}
		
		
		void AdjustItems (List<IPrintableObject> list,int startValue ) {
		
			var xLocation = startValue;
			foreach (var element in list) {
				element.Location =  new Point(xLocation,6);
				xLocation = xLocation + GlobalValues.PreferedSize.Width + gap;
			}
		}
		
		static bool IsDataReport(ReportWizardContext context)
		{
			var poc = (PageOneContext)context.PageOneContext;
			return poc.ReportType.Equals(ReportType.DataReport);
		}
		
		public IReportModel ReportModel {get;private set;}
	}
}
