// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of IReportCreator.
	/// </summary>
	public interface IReportCreator
	{
		void BuildExportList ();
		PagesCollection Pages{get;}
		event EventHandler<PageCreatedEventArgs> PageCreated;
		event EventHandler<SectionRenderEventArgs> SectionRendering;
		event EventHandler<GroupHeaderEventArgs> GroupHeaderRendering;
		event EventHandler<GroupFooterEventArgs> GroupFooterRendering;
		event EventHandler<RowRenderEventArgs> RowRendering;
	}
}
