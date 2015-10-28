/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2014
 * Time: 20:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel.Design;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.Reporting.Addin.Views;

namespace ICSharpCode.Reporting.Addin.Commands
{
	/// <summary>
	/// Description of FormsCommands.
	/// </summary>
	
	public abstract class AbstractFormsDesignerCommand : AbstractMenuCommand{
	
		public abstract CommandID CommandID {
			get;
		}
		
		protected virtual bool CanExecuteCommand(IDesignerHost host)
		{return true;}
			
		
		
		protected static DesignerView ReportDesigner {
			get {
				var window = SD.Workbench;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as DesignerView;
			}
		}
		
		
		public override void Run(){
			var formDesigner = ReportDesigner;
			if (formDesigner != null && CanExecuteCommand(formDesigner.Host)) {
				var menuCommandService = (IMenuCommandService)formDesigner.Host.GetService(typeof(IMenuCommandService));
				menuCommandService.GlobalInvoke(CommandID);
			}
		}

		internal virtual void CommandCallBack(object sender, EventArgs e){
			Run();
		}
	}
	
	
	
	public class ViewCode : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {return StandardCommands.ViewCode;}
		}
		
		public override void Run(){
			var window = SD.Workbench;
			if (window == null) {
				return;
			}
			
			var formDesigner = AbstractFormsDesignerCommand.ReportDesigner;
			if (formDesigner != null) {
				formDesigner.ShowSourceCode();
			}
		}
	}
}
