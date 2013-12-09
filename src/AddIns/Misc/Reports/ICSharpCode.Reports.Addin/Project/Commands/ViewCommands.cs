// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Dialogs;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Reports.Addin.Commands
{
	/// <summary>
	/// Description of StartView
	/// </summary>
	public class CreateDesignViewCommand : AbstractMenuCommand
	{
		readonly OpenedFile openedFile;
		
		public CreateDesignViewCommand (OpenedFile openedFile) {
			if (openedFile == null)
				throw new ArgumentNullException("openedFile");
			this.openedFile = openedFile;
		}
		
		public override void Run(){
			var generator = new ReportDesignerGenerator();
			DesignerView =  new ReportDesignerView(openedFile, generator);
		}
		
		public ReportDesignerView DesignerView {get; private set;}
	}
	
	
	
	public class CollectParametersCommand :AbstractCommand
	{
		readonly ReportSettings reportSettings;
		
		public CollectParametersCommand (ReportSettings reportSettings)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			this.reportSettings = reportSettings;
		}
		
		
		public override void Run()
		{
			if (reportSettings.SqlParameters.Count > 0) {
				using (var paramDialog = new ParameterDialog(reportSettings.SqlParameters))
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
