// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of ExpressionVisitor.
	/// </summary>
	internal class ExpressionVisitor: AbstractVisitor
	{
		private readonly Collection<ExportPage> pages;
		
		public ExpressionVisitor(Collection<ExportPage> pages)
		{
			this.pages = pages;
		}
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportColumn exportColumn)
		{
			Console.WriteLine("Visit Page ");
		}
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportText exportColumn)
		{
			Console.WriteLine("ExpressionVisitor <{0}>",exportColumn.Name);
		}
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportContainer exportColumn)
		{
			Console.WriteLine("ExpressionVisitor <{0}>",exportColumn.Name);
		}
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportPage page)
		{
			Console.WriteLine("ExpressionVisitor page <{0}>",page.PageInfo.PageNumber);
		}
	}
}
