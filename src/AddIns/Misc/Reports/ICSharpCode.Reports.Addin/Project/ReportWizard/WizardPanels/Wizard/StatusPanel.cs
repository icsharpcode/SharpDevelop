// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	public class StatusPanel : UserControl
	{
		WizardDialog wizard;
		
		Font smallFont;
		Font normalFont;
		Font boldFont;
		
		public StatusPanel(WizardDialog wizard)
		{
			smallFont  = WinFormsResourceService.LoadFont("Tahoma",  14, GraphicsUnit.World);
			normalFont = WinFormsResourceService.LoadFont("Tahoma", 14, GraphicsUnit.World);
			boldFont   = WinFormsResourceService.LoadFont("Tahoma", 14, FontStyle.Bold, GraphicsUnit.World);
			
			this.wizard = wizard;
			this.BackgroundImage = WinFormsResourceService.GetBitmap("GeneralWizardBackground");
			Size = new Size(198, 400);
			ResizeRedraw  = false;
			
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			//    		base.OnPaint(pe);
			Graphics g = pe.Graphics;
			
			g.DrawString(ResourceService.GetString("SharpDevelop.Gui.Dialogs.WizardDialog.StepsLabel"),
			             smallFont,
			             SystemBrushes.WindowText,
			             10,
			             24 - smallFont.Height);
			
			g.DrawLine(SystemPens.WindowText, 10, 24, Width - 10, 24);
			
			int curNumber = 0;
			for (int i = 0; i < wizard.WizardPanels.Count; i = wizard.GetSuccessorNumber(i)) {
				Font curFont = wizard.ActivePanelNumber == i ? boldFont : normalFont;
				IDialogPanelDescriptor descriptor = ((IDialogPanelDescriptor)wizard.WizardPanels[i]);
				g.DrawString((1 + curNumber) + ". " + descriptor.Label, curFont, SystemBrushes.WindowText, 10, 40 + curNumber * curFont.Height);
				++curNumber;
			}
		}
	}
}
