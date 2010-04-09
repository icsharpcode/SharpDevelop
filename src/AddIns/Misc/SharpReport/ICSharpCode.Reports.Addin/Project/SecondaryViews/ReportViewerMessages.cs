/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 07.04.2010
 * Zeit: 20:09
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core.ReportViewer;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportViewerMessages.
	/// </summary>
	public class ReportViewerMessages:IReportViewerMessages
	{
		public ReportViewerMessages()
		{
		}
		
		
		public string BackButtonText {
			get {
				return ResourceService.GetString("SharpReport.ReportViewer.PreviousPage");
			}
		}
		
		public string NextButtonMessage {
			get {
				return ResourceService.GetString("SharpReport.ReportViewer.NextPage");
			}
		}
		
		public string PrintButtonMessage {
			get {
				return ResourceService.GetString("SharpReport.DesignView.Toolbar.Printer");
			}
		}
		
		
		public string PagesCreatedMessage {
			get {
				return ResourceService.GetString("SharpReport.ReportViewer.PagesCreatedMessage");
			}
		}
		
		
		public string FirstPageMessage {
			get {
				return ResourceService.GetString("SharpReport.ReportViewer.FirstPageMessage");
			}
		}
		
		
		public string LastPageMessage {
			get {
				return ResourceService.GetString("SharpReport.ReportViewer.LastPageMessage");
			}
		}
		
		public string PdfFileMessage {
			get {
				return ResourceService.GetString("SharpReport.ReportViewer.PdfFileMessage");
			}
		}
		
		public string ZoomMessage {
			get {
				return ResourceService.GetString("SharpReport.ReportViewer.ZoomMessage");
			}
		}
	}
}
