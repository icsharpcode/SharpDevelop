/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 19.01.2006
 * Time: 10:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using SharpReportCore;

namespace SharpReportAddin.Commands{
	
	/// <summary>
	/// This command is used when starting the <see cref="SharpReportView"></see>
	/// from MainMenu/Extras/ReportGenerator
	/// </summary>
	/// 
	public class RunSharpReport: AbstractMenuCommand{
		
		public override void Run(){
			SharpReportView view = new SharpReportView();
			if (view != null) {
				if (SharpReportCore.GlobalValues.IsValidPrinter()) {
					WorkbenchSingleton.Workbench.ShowView(view);
				} else {
					MessageService.ShowError(ResourceService.GetString("Sharpreport.Error.NoPrinter"));
				}
			}
		}
	}
	
	
	public class DataSetFromXSDCommand:AbstractCommand{
		System.Data.DataSet ds;
		public override void Run()
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog()){
				openFileDialog.Filter = GlobalValues.XsdFileFilter;
				openFileDialog.DefaultExt = GlobalValues.XsdExtension;
				openFileDialog.AddExtension    = true;
				if(openFileDialog.ShowDialog() == DialogResult.OK){
					if (openFileDialog.FileName.Length > 0) {
						this.ds = new System.Data.DataSet();
						this.ds.ReadXml (openFileDialog.FileName);
						this.ds.Locale = CultureInfo.InvariantCulture;
					}
				}
			}
		}
		
		public System.Data.DataSet DataSet {
			get { return ds; }
		}
		
	}
}
