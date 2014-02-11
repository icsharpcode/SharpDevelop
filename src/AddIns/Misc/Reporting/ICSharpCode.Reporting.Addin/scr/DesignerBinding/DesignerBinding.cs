/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.02.2014
 * Time: 20:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;
namespace ICSharpCode.Reporting.Addin.DesignerBinding {
	
	
	public class ReportDesignerBinding:IDisplayBinding {
		
		
		#region IDisplayBinding implementation
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return true;
		}
		
		public bool CanCreateContentForFile(FileName fileName)
		{
			return Path.GetExtension(fileName).Equals(".srd", StringComparison.OrdinalIgnoreCase);
		}
		
		public double AutoDetectFileContent(FileName fileName, System.IO.Stream fileContent, string detectedMimeType)
		{
			throw new System.NotImplementedException();
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			MessageService.ShowMessage("ReportDesigner not available at the Moment","New ReportDesigner");
			return null;
			/*
			if (file.IsDirty) {
				var cmd = new ReportWizardCommand(file);
				cmd.Run();
				if (cmd.Canceled) {
					return null;
				}
				file.SetData(cmd.GeneratedReport.ToArray());
			}
			var viewCmd = new CreateDesignViewCommand(file);
			viewCmd.Run();
			return viewCmd.DesignerView;
			*/
			
		}
		
		#endregion
	}
}