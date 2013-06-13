// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Dialogs;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Reports.Addin.Commands
{
	/// <summary>
	/// Description of StartView
	/// </summary>
	public class StartViewCommand : AbstractMenuCommand
	{
		
		public override void Run()
		{
			throw new NotImplementedException("StartViewCommand-Run");
//			SD.Workbench.ShowView(SetupDesigner());
		}
		
		/*
		public static ReportDesignerView SetupDesigner ()
		{
			throw new NotImplementedException("SetupDesigner");
			ReportModel model = ReportModel.Create();
			
			var reportStructure = new ReportStructure()
			{
				ReportLayout = GlobalEnums.ReportLayout.ListLayout;
			}
			IReportGenerator generator = new GeneratePlainReport(model,reportStructure);
			generator.GenerateReport();
			
//			OpenedFile file = FileService.CreateUntitledOpenedFile(GlobalValues.PlainFileName,new byte[0]);
//			file.SetData(generator.Generated.ToArray());
//			return SetupDesigner(file);
			return SetupDesigner(null);
		}
		*/
		
		public static ReportDesignerView SetupDesigner (OpenedFile file)
		{
			if (file == null) {
				throw new ArgumentNullException("file");
			}
			IDesignerGenerator generator = new ReportDesignerGenerator();
			return new ReportDesignerView(file, generator);
		}
		
	}
	
	
	
	public class CollectParametersCommand :AbstractCommand
	{
		ReportSettings reportSettings;
		
		public CollectParametersCommand (ReportSettings reportSettings)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("ReportSettings");
			}
			this.reportSettings = reportSettings;
		}
		
		
		public override void Run()
		{
			if (reportSettings.SqlParameters.Count > 0) {
				using (ParameterDialog paramDialog = new ParameterDialog(reportSettings.SqlParameters))
				{
					paramDialog.ShowDialog();
					if (paramDialog.DialogResult == System.Windows.Forms.DialogResult.OK) {
						foreach (SqlParameter bp in paramDialog.SqlParameterCollection)
						{
							var p = reportSettings.SqlParameters.Find (bp.ParameterName);
							p.ParameterValue = bp.ParameterValue;
						}
					}
				}
			}
		}
	}
	
	
	public class DataSetFromXsdCommand:AbstractCommand{
		System.Data.DataSet dataSet;
		public override void Run()
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog()){
				openFileDialog.Filter = GlobalValues.XsdFileFilter;
				openFileDialog.DefaultExt = GlobalValues.XsdExtension;
				openFileDialog.AddExtension    = true;
				if(openFileDialog.ShowDialog() == DialogResult.OK){
					if (openFileDialog.FileName.Length > 0) {
						this.dataSet = new System.Data.DataSet();
						this.dataSet.ReadXml (openFileDialog.FileName);
						this.dataSet.Locale = CultureInfo.InvariantCulture;
					}
				}
			}
		}
		
		public System.Data.DataSet DataSet {
			get { return dataSet; }
		}
		
	}
}
