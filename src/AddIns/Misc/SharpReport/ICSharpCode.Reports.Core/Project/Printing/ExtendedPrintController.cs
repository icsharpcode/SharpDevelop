/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 02.03.2009
 * Zeit: 20:07
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Drawing.Printing;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of ExtendedPrintController.
	/// </summary>

	
	public class ExtendedPrintController:PrintController
	{
		PrintController controller;
		
		public ExtendedPrintController(PrintController controller):base()
		{
			if (controller == null) {
				throw new ArgumentNullException("controller");
			}
			this.controller = controller;
		}
		
		public override System.Drawing.Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("ExtPrintcontroller OnStartPage");
			return this.controller.OnStartPage (document, e);
		}
		
		
		public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("ExtPrintcontroller OnStartPrint");
			this.controller.OnStartPrint(document, e);
		}
		
		
		public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("ExtPrintcontroller OnEndPage");
			this.controller.OnEndPage (document, e);
		}
		
		public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("ExtPrintcontroller OnEndprint");
			PreviewPrintController c = this.controller as PreviewPrintController;
			if (c != null) {
				PreviewPageInfo[] ppia = c.GetPreviewPageInfo();
			}
			this.controller.OnEndPrint(document,e);
		}
	}
}
