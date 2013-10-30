// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of FormatVisitor.
	/// </summary>
	public class FormatVisitor: AbstractVisitor
	{
		readonly Collection<ExportPage> pages;
		public FormatVisitor(Collection<ExportPage> pages)
		{
			if (pages == null)
				throw new ArgumentNullException("pages");
			this.pages = pages;
			Console.WriteLine("Start FormatVisitor");
		}
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportPage page)
		{
			foreach (var element in page.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportContainer exportColumn)
		{
			foreach (var element in exportColumn.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportText exportColumn)
		{
			if (!String.IsNullOrEmpty(exportColumn.FormatString)) {
				Console.WriteLine("Format {0} - {1}",exportColumn.Name,exportColumn.Text);
				exportColumn.Text = StandardFormatter.FormatOutput(exportColumn.Text,
				                                                   exportColumn.FormatString,
				                                                   "System.Int16",
				                                                   "no format");
			}
		}
	}
}
