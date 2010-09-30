// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of PageCreatedEventArgs.
	/// </summary>
	public class PageCreatedEventArgs:System.EventArgs
	{
		ExporterPage singlePage;
		
		public PageCreatedEventArgs(ExporterPage page)
		{
			this.singlePage = page;
		}
		
		
		public ExporterPage SinglePage {
			get { return singlePage; }
		}
	}
}
