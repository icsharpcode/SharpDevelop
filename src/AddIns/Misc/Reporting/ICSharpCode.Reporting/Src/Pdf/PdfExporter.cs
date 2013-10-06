// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Pdf
{
	/// <summary>
	/// Description of PdfExporter.
	/// </summary>
	public class PdfExporter:BaseExporter{
		
		readonly PdfVisitor visitor;
		
		public PdfExporter(Collection<ExportPage> pages):base(pages){
			visitor = new PdfVisitor();
		}
		
		public override void Run()
		{
			
			Console.WriteLine();
			Console.WriteLine("Start WpfExporter with {0} Pages ",Pages.Count);
			
			foreach (var page in Pages) {
				IAcceptor acceptor = page as IAcceptor;
				if (acceptor != null) {
					visitor.Visit(page);
				}
				
				Console.WriteLine("-----------PageBreak---------");
			}
		
			Console.WriteLine("Finish WpfVisitor");
			Console.WriteLine();
			
		}
	}
}
