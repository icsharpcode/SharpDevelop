/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 14.01.2010
 * Zeit: 20:08
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;

namespace SharpReportSamples
{
	/// <summary>
	/// Description of ReportViewerMessagesProvider.
	/// </summary>
	public class ReportViewerMessagesProvider:ICSharpCode.Reports.Core.ReportViewer.IReportViewerMessages
	{
		public ReportViewerMessagesProvider(){

		}
		
		public string BackButtonText {
			get {
				return  "Back";
			}
		}
		
		public string NextButtonMessage {
			get {
				return "Forward";
			}
		}
		
		public string PrintButtonMessage {
			get {
				return "Printer";
			}
		}
		
		public string CancelButtonMessage {
			get {
				return "Stop";
			}
		}
		
		public string ZoomMessage {
			get {
				return "Zoom";
			}
		}
		
		
		public string PagesCreatedMessage {
			get{
				return "Pages created";
			}
		}
		
		public string PdfFileMessage {
			get{
				return "Create PdfFile";
			}
		}
		
		public string FirstPageMessage {
			get {
				return  "FirstPage";
			}
		}
		
		public string LastPageMessage {
			get {
				return  "LastPage";
			}
		}
	}
}
