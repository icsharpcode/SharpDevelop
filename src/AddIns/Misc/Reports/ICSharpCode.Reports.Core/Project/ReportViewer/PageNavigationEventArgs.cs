// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core.ReportViewer
{
	/// <summary>
	/// Description of PageNavigationEventArgs.
	/// </summary>
	public class PageNavigationEventArgs: EventArgs
	{ 
		private ExporterPage page;
		private int pageNumber;
		
		public PageNavigationEventArgs()
		{
		}
		
		public PageNavigationEventArgs(int pageNumber):this(null,pageNumber)
		{
			
		}
		
		
		public PageNavigationEventArgs(ExporterPage page, int pageNumber)
		{
			this.page = page;
			this.pageNumber = pageNumber;
		}
		
		public ExporterPage Page {
			get { return page; }
		}
		
		public int PageNumber {
			get { return pageNumber; }
		}
		
	}
}
