// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using ICSharpCode.Reports.Core;

namespace ReportSamples
{
	/// <summary>
	/// Description of EmployeesPushModel.
	/// </summary>
	public class StandartPushModel:BaseSample
	{
		DataTable dataTable;
		public StandartPushModel(){
			try {
				base.Run();
				this.dataTable = base.SelectData();
			} catch (Exception e) {
				throw e;
			}

		}
		
		public ReportModel ReportModel
		{
			get {
				return ReportEngine.LoadReportModel(base.ReportName);
			}
		}
		
		public DataTable DataTable {
			get { return dataTable; }
		}
		
	}
}

