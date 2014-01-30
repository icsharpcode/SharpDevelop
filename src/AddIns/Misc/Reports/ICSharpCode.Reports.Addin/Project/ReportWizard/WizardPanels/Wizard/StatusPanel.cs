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
