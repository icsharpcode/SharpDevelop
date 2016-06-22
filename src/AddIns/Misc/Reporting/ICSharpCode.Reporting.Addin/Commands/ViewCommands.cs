/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.02.2014
 * Time: 20:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Reporting.Addin.Commands
{
	/// <summary>
	/// Description of ViewCommands.
	/// </summary>
	public class CreateDesignerCommand : AbstractMenuCommand
	{
		readonly OpenedFile openedFile;
		
		public CreateDesignerCommand (OpenedFile openedFile) {
			if (openedFile == null)
				throw new ArgumentNullException("openedFile");
			this.openedFile = openedFile;
		}
		
		public override void Run(){
//			var generator = new ReportDesignerGenerator();
//			DesignerView =  new ReportDesignerView(openedFile, generator);
			MessageService.ShowMessage("ReportDesigner not available at the Moment","New ReportDesigner");
		}
		
//		public ReportDesignerView DesignerView {get; private set;}
	}
}
