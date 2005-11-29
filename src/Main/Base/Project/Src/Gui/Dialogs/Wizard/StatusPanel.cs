// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class StatusPanel : UserControl
	{
		WizardDialog wizard;
		
		Font smallFont;
		Font normalFont;
		Font boldFont;
		
		public StatusPanel(WizardDialog wizard)
		{
			smallFont  = ResourceService.LoadFont("Tahoma",  14, GraphicsUnit.World);
			normalFont = ResourceService.LoadFont("Tahoma", 14, GraphicsUnit.World);
			boldFont   = ResourceService.LoadFont("Tahoma", 14, FontStyle.Bold, GraphicsUnit.World);
			
			this.wizard = wizard;
			this.BackgroundImage = ResourceService.GetBitmap("GeneralWizardBackground");
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
