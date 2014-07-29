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
		ReportWizardContext context;
	
		
		public ReportGenerator(ReportWizardContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
			ReportModel = ReportModelFactory.Create();
		}
		
		
		public void  Generate() {
			PageOneGenerate();
			CreateReportHeader();
		}

		void PageOneGenerate()
		{
			var pageOne = (PageOneContext)context.PageOneContext;
			var reportSettings = ReportModel.ReportSettings;
			reportSettings.DataModel = pageOne.DataModel;
			reportSettings.FileName = pageOne.FileName;
			if (pageOne.Legal) {
				reportSettings.Landscape = false;
			} else {
				reportSettings.Landscape = true;
			}
			reportSettings.ReportName = pageOne.ReportName;
			reportSettings.ReportType = pageOne.ReportType;
		}

		
		void CreateReportHeader()
		{
			var headerText = new BaseTextItem();
			headerText.Text = "Header";
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
