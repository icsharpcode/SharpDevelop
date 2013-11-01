// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Pdf
{
	/// <summary>
	/// Description of PdfVisitor.
	/// </summary>
	class PdfVisitor: AbstractVisitor
	{
		public PdfVisitor()
		{
		}
		
		public override void Visit(ExportPage page)
		{
			Console.WriteLine("Pdf_Visitor page <{0}>",page.PageInfo.PageNumber);
			base.Visit(page);
		}
		
		public override void Visit(ExportContainer exportColumn)
		{
			
			Console.WriteLine("\tPdf_Visitor <{0}>",exportColumn.Name);
			foreach (var element in exportColumn.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
			Console.WriteLine("\t\tPdf_Visitor <{0}>",exportColumn.Name);
		}
	}
}
