/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 14.05.2010
 * Time: 19:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Project.Exporter
{
	/// <summary>
	/// Description of BaseReportCreator.
	/// </summary>
	public class AbstractReportCreator:IReportCreator
	{
		public AbstractReportCreator(IReportModel reportModel)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("ReportModel");
			}
			
			ReportModel = reportModel;
		}
		
		
		protected IReportModel ReportModel {get; private set;}
		
		#region IReportCreator
		
		public event EventHandler<ICSharpCode.Reports.Core.old_Exporter.PageCreatedEventArgs> PageCreated;
		
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		
		public PagesCollection Pages {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void BuildExportList()
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}
