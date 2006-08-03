/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 08.06.2005
 * Time: 22:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

/// <summary>
/// Description of AbstractExplorerCommand
/// </summary>
namespace SharpReportAddin.Commands{	
	public abstract class AbstractExplorerCommand : AbstractMenuCommand{

		ReportExplorer reportExplorer = null;
		protected AbstractExplorerCommand(){
			
//			Type type = typeof(ReportExplorer);
//			this.reportExplorer = (SharpReportAddin.ReportExplorer)WorkbenchSingleton.Workbench.GetPad(type).PadContent;
			this.reportExplorer = (SharpReportAddin.ReportExplorer)WorkbenchSingleton.Workbench.GetPad(typeof(ReportExplorer)).PadContent;
			if (reportExplorer == null) {
				throw new NullReferenceException ("AbstractExplorerCommand : No FieldExplorer Pad  available");
			}
		}
			/// <summary>
		/// Enabled or disabled the command
		/// <remarks> /remarks>
		/// </summary>				
		public override bool IsEnabled {
			get{
				if (reportExplorer != null) {
					return true;
				}
				return false;
			}			
			set{}
		}
		
		public ReportExplorer ReportExplorer {
			get {
				return reportExplorer;
			}
		}
	}
}
