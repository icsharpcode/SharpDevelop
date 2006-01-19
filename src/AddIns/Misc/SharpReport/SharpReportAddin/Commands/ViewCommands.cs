/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 19.01.2006
 * Time: 10:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

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
	
	/// <summary>
	/// Let the <see cref="SharpReportView"></see> update or fill
	/// the <see cref="FieldsExplorer"></see>
	/// </summary>
	
	public class SetFieldsExplorer : AbstractSharpReportCommand{
 		public SetFieldsExplorer() {
 			
 		}
 		public override void Run() {
 			try {
 				base.View.UpdateFieldsExplorer();
 			} catch (Exception) {
 				throw;
 			}
 		}
 		
 	} 
}
