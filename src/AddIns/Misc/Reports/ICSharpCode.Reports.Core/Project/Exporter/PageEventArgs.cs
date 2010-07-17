// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

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
