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
			GenerateBaseSettings(context);
			GeneratePushModel(context);
			CreateReportHeader(context);
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
				Console.WriteLine("Create dataItem  with  {0} - {1}items",dataItem.Location,dataItem.Size);
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
			Console.WriteLine("");
			Console.WriteLine("Createreportheader Size {0}",ReportModel.ReportHeader.Size);
		}
		
		
		public IReportModel ReportModel {get;private set;}
	}
}
