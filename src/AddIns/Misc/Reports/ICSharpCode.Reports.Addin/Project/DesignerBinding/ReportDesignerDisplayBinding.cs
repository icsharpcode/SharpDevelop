// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Reports.Addin.ReportWizard;
namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportDesignerDisplayBinding.
	/// </summary>
	public class ReportDesignerDisplayBinding:IDisplayBinding
	{
		public ReportDesignerDisplayBinding()
		{
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			return System.IO.Path.GetExtension(fileName).Equals(".srd",StringComparison.OrdinalIgnoreCase) ;
		}
		
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			if (file.IsDirty) {
				ReportWizardCommand cmd = new ReportWizardCommand(file);
				cmd.Run();
				if (cmd.Canceled) {
					return null;
				}
				file.SetData(cmd.GeneratedReport.ToArray());
			}
			ReportDesignerView view = ICSharpCode.Reports.Addin.Commands.StartViewCommand.SetupDesigner(file);
			return view;
		}
	}
}
