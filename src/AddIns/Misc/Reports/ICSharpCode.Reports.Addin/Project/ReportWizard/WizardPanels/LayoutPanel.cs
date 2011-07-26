// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of LayoutPanel.
	/// </summary>
	public class LayoutPanel: AbstractWizardPanel
	{
		private LayoutPanelControl layoutControl;
		private Properties customizer;
		private ReportStructure reportStructure;
		
		public LayoutPanel()
		{
			base.EnableFinish = true;
			base.EnableCancel = true;
			base.EnableNext = true;
			base.Refresh();
			layoutControl = new LayoutPanelControl();
			layoutControl.Location = new Point (20,20);
			this.Controls.Add(layoutControl);
			
		}
		
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			base.EnableFinish = true;
			base.IsLastPanel = true;
			base.EnablePrevious = true;
			
			if (customizer == null) {
				customizer = (Properties)base.CustomizationObject;
			}
			
			if (message == DialogMessage.Activated) {
				
				this.layoutControl.ReportLayout = (GlobalEnums.ReportLayout)customizer.Get("ReportLayout");
				reportStructure = (ReportStructure)customizer.Get("Generator");
				layoutControl.AvailableFieldsCollection = reportStructure.AvailableFieldsCollection;
			}
			
			else if (message == DialogMessage.Finish)
			{
				customizer.Set ("ReportLayout",this.layoutControl.ReportLayout);
				var reportStructure = (ReportStructure)customizer.Get("Generator");
				reportStructure.Grouping = layoutControl.GroupName;
			}
			return true;
		}
	}
}
