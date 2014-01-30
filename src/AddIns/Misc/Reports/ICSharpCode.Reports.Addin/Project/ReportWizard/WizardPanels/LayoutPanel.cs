// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
