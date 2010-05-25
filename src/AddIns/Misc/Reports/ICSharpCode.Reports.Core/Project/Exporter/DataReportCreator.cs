/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 13.05.2010
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using ICSharpCode.Reports.Core.Interfaces;


namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of DataReportCreator.
	/// </summary>
	public class DataReportCreator:AbstractReportCreator
	{
		
		IReportModel reportModel;
		IDataManager dataManager;
		ILayouter layouter;
		
		#region Constructor
		
		
		
		public static AbstractReportCreator CreateInstance(IReportModel reportModel, IDataManager dataManager,ILayouter layouter)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
			if (layouter == null) {
				throw new ArgumentNullException("layouter");
			}
			var instance = new DataReportCreator(reportModel,dataManager,layouter);
			
			return instance;
		}
		
		
		private DataReportCreator (IReportModel reportModel,IDataManager dataManager,ILayouter layouter):base(reportModel) 
		{
			this.reportModel = reportModel;
			this.dataManager = dataManager;
			this.layouter = layouter;
			
			base.AbstractExportListBuilder = new DataExportListBuilder(reportModel,dataManager);
		}
		
		#endregion
		
		public override void BuildExportList()
		{
			base.AbstractExportListBuilder.WritePages();
			
		}
		
	}
}
