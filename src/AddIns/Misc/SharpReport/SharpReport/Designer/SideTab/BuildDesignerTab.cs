/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier Helmut
 * Datum: 24.10.2006
 * Zeit: 22:50
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Windows.Forms;

namespace SharpReport
{
	/// <summary>
	/// Description of BuildTabControl.
	/// </summary>
	public class BuildDesignerTab
	{
		public static TabControl BuildTabControl () {
			TabControl tabControl = new TabControl();
			// Designer Tap
			TabPage designerPage = new TabPage();

			//Standart Preview page
			//create only the TabPage, no Controls are added
			TabPage previewPage = new TabPage();
			
			// ReportViewer
			TabPage reportViewerPage = new TabPage();
			
			tabControl.TabPages.Add (designerPage);
			tabControl.TabPages.Add (previewPage);
			tabControl.TabPages.Add(reportViewerPage);
			
			tabControl.Alignment = TabAlignment.Bottom;
			tabControl.Dock = DockStyle.Fill;
			return tabControl;
		}
			
	}
}
