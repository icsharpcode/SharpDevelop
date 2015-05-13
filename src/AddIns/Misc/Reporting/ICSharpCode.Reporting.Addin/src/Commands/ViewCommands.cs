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
using ICSharpCode.Reporting.Addin.DesignerBinding;
using ICSharpCode.Reporting.Addin.Views;

namespace ICSharpCode.Reporting.Addin.Commands
{
	/// <summary>
	/// Description of ViewCommands.
	/// </summary>
	public class CreateDesignerCommand : AbstractMenuCommand{
	
		readonly OpenedFile openedFile;
		
		public CreateDesignerCommand (OpenedFile openedFile) {
			if (openedFile == null)
				throw new ArgumentNullException("openedFile");
			this.openedFile = openedFile;
		}
		
		public override void Run(){
			var generator = new DesignerGenerator();
			DesignerView =  new DesignerView(openedFile, generator);
		}
		
		public DesignerView DesignerView {get; private set;}
	}
}
