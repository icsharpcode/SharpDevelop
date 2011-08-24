// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter
{
	public interface IExportContainer:IBaseExportColumn
	{
		ExporterCollection Items { get; }
	}
}
