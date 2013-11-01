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
	class FormatVisitor: AbstractVisitor
	{

		
		public override void Visit(ExportText exportColumn)
		{
			if (!String.IsNullOrEmpty(exportColumn.FormatString)) {
				StandardFormatter.FormatOutput(exportColumn);
			}
		}
	}
}
