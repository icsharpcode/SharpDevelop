// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of LayoutPanel.
	/// </summary>
	public class LayoutPanel: AbstractWizardPanel
	{
		private LayoutPanelControl layoutControl;
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
			reportStructure = (ReportStructure)base.CustomizationObject;
			if (message == DialogMessage.Activated) {
				layoutControl.AvailableFieldsCollection = reportStructure.AvailableFieldsCollection;
			}
			
			else if (message == DialogMessage.Finish)
			{
				reportStructure.ReportLayout = layoutControl.ReportLayout;
				reportStructure.Grouping = layoutControl.GroupName;
			}
			return true;
		}
	}
}
