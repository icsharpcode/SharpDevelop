/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 14.11.2004
 * Time: 17:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;

//using SharpReport;


//using SharpReportAddin.Commands;
///<summary>Main Command to Run Sharpreport Addin
///</summary>
namespace SharpReportAddin {
	
	public class RunSharpReport: AbstractMenuCommand
	{
		public override void Run()
		{
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
}

