/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 03.10.2008
 * Zeit: 17:41
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using SharpQuery.Collections;
using SharpQuery.SchemaClass;


namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of LayoutPanel.
	/// </summary>
	public class LayoutPanel: AbstractWizardPanel
	{
		private LayoutPanelControl lpc;
		private Properties customizer;
		
		public LayoutPanel()
		{
			base.EnableFinish = true;
			base.EnableCancel = true;
			base.EnableNext = true;
			base.Refresh();
			lpc = new LayoutPanelControl();
			lpc.Location = new Point (20,20);
			this.Controls.Add(lpc);
		}
		
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (customizer == null) {
				customizer = (Properties)base.CustomizationObject;
			}
			
			if (message == DialogMessage.Activated) {
				base.EnableFinish = true;
				this.lpc.ReportLayout = (GlobalEnums.ReportLayout)customizer.Get("ReportLayout");
			}
			
			else if (message == DialogMessage.Finish) {
				customizer.Set ("ReportLayout",this.lpc.ReportLayout);
			}
			return true;
		}
	}
}
