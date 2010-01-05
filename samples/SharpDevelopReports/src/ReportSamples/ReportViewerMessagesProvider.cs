// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
namespace ReportSamples
{
	/// <summary>
	/// This class is used to localise ReportViewer
	/// </summary>
	internal class ReportViewerMessagesProvider:ICSharpCode.Reports.Core.ReportViewer.IReportViewerMessages
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

