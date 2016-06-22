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
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.IO;
using PdfSharp.Pdf;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Pdf
{
	/// <summary>
	/// Description of PdfExporter.
	/// </summary>
	public class PdfExporter:BaseExporter{
		
		PdfDocument pdfDocument;
		
		public PdfExporter(Collection<ExportPage> pages):base(pages){
		}
		
		
		public void Run (string fileName,bool show) {
			if (String.IsNullOrEmpty(fileName)) {
				fileName = Pages[0].PageInfo.ReportName + ".pdf";
			}
			pdfDocument = new PdfDocument();
			ConvertPagesToPdf();
			pdfDocument.Save(fileName);
			if (show) {
				Process.Start(fileName);	
			}	
			
		}
		
	
		public void Run (Stream stream) {
			if (stream == null)
				throw new ArgumentNullException("stream");
			pdfDocument = new PdfDocument(stream);
			ConvertPagesToPdf();
			pdfDocument.Save(stream,false);
			stream.Seek(0, SeekOrigin.Begin);
		}
		
	
		public override void Run()
		{
			var fileName = Pages[0].PageInfo.ReportName + ".pdf";
			Run(fileName, false);
			Process.Start(fileName);
		}

		
		void ConvertPagesToPdf()
		{
			var visitor = new PdfVisitor(pdfDocument);
			SetDocumentTitle(Pages[0].PageInfo.ReportName);
			foreach (var page in Pages) {
				var acceptor = page as IAcceptor;
				if (acceptor != null) {
					visitor.Visit(page);
				}
			}
		}
		
		
		void SetDocumentTitle(string reportName)
		{
			pdfDocument.Info.Title = reportName;
		}
		
		
		public PdfDocument PdfDocument {
			get { return pdfDocument; }
		}
	}
}
