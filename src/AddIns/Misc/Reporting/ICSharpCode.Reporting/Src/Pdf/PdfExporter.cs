// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace ICSharpCode.Reporting.Pdf
{
	/// <summary>
	/// Description of PdfExporter.
	/// </summary>
	public class PdfExporter:BaseExporter{
		
		PdfVisitor visitor;
		PdfDocument pdfDocument;
		
		
		public PdfExporter(Collection<ExportPage> pages):base(pages){
		}
		
		public override void Run()
		{
			pdfDocument = new PdfDocument();
			visitor = new PdfVisitor(pdfDocument);
			
			SetDocumentTitle(Pages[0].PageInfo.ReportName);

			Console.WriteLine();
			Console.WriteLine("Start PdfExporter with {0} Pages ",Pages.Count);
			
			foreach (var page in Pages) {
				IAcceptor acceptor = page as IAcceptor;
				if (acceptor != null) {
					visitor.Visit(page);
				}
				
				Console.WriteLine("-----------PageBreak---------");
			}
			
			const string filename = "HelloWorld.pdf";
			
			pdfDocument.Save(filename);
			
			// ...and start a viewer.
			
			Process.Start(filename);
			Console.WriteLine("Finish WpfVisitor");
			Console.WriteLine();
			 
		}
		
		void SetDocumentTitle(string reportName)
		{
			Console.WriteLine("Set DocumentTitle to {0}",reportName);
			pdfDocument.Info.Title = reportName;
		}
		
		public PdfDocument PdfDocument {
			get { return pdfDocument; }
		}
	}
}
