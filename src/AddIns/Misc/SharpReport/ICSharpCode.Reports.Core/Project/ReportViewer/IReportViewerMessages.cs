// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Reports.Core.ReportViewer{
	
	public interface IReportViewerMessages {
		
		string BackButtonText {get;}
		
		string NextButtonMessage {get;}
		
		string PrintButtonMessage {get;}
		
		string PagesCreatedMessage {get;}
		
		string FirstPageMessage {get;}
		
		string LastPageMessage {get;}
		
		string PdfFileMessage {get;}
		
		string ZoomMessage {get;}
	}
}
