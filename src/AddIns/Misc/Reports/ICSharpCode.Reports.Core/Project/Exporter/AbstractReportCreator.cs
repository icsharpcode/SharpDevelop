/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 14.05.2010
 * Time: 19:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of BaseReportCreator.
	/// </summary>
	public class AbstractReportCreator:IReportCreator_2
	{
		
		
		public AbstractReportCreator(IReportModel reportModel)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("ReportModel");
			}
			
			this.ReportModel = reportModel;
		}
		
		
		#region Convertion
		
		/*
		protected virtual void BuildReportHeader ()
		{
		}
		
		protected virtual void BuildPageHeader ()
		{
		}
		
		protected virtual void BuildDetailInternal (BaseSection section)
		{
		}
		
		protected virtual void BuildPageFooter ()
		{
		}
		
		protected virtual void BuildReportFooter ()
		{
		}
		
		*/
		
		public virtual void BuildExportList ()
		{
			this.Pages.Clear();
			AbstractExportListBuilder.WritePages();
		}
		
		protected virtual void AddPage ()
		{
		}
		
		#endregion
		
	
		
		protected IReportModel ReportModel {get;  set;}
		
		protected AbstractExportListBuilder AbstractExportListBuilder {get;set;}
		
		#region IReportCreator
		
		public event EventHandler<ICSharpCode.Reports.Core.old_Exporter.PageCreatedEventArgs> PageCreated;
		
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		
			
		public virtual PageDescriptions Pages {
			get {
				return AbstractExportListBuilder.Pages;
					
			}
		}
	
		
		#endregion
	}
}
