/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.05.2010
 * Time: 20:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of PageCreatedEventArgs.
	/// </summary>
	public class NewPageCreatedEventArgs:System.EventArgs
	{
		PageDescription			singlePage;
		
		public NewPageCreatedEventArgs(PageDescription page)
		{
			this.singlePage = page;
		}
		
		
		public PageDescription SinglePage {
			get { return singlePage; }
		}
	}
}
