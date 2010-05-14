/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 13.05.2010
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Events;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Core.Project.Exporter;

namespace ICSharpCode.Reports.Core.old_Exporter
{
	/// <summary>
	/// Description of DataReportCreator.
	/// </summary>
	public class DataReportCreator:AbstractReportCreator
	{
		
		
		#region Constructor
		
		
		public static IReportCreator CreateInstance(IReportModel reportModel, IDataManager dataManager,ILayouter layouter)
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
//			this.dataManager = dataManager;
		}
		
		#endregion
		
		
		
	}
}
