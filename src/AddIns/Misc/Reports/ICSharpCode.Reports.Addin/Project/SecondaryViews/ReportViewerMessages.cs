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
using ICSharpCode.Core;
using ICSharpCode.Reports.Core.ReportViewer;

namespace ICSharpCode.Reports.Addin.SecondaryViews
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
