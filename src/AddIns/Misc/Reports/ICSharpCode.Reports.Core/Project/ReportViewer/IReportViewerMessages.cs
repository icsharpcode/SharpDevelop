// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
