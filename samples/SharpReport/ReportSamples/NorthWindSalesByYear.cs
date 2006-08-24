/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 16.08.2006
 * Time: 11:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;

using SharpReportCore;
namespace ReportSamples
{
	/// <summary>
	/// Description of StoredProcedure.
	/// </summary>
	public class NorthWindSalesByYear:BaseSample
	{
//		string conString = @"Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Northwind";
		public NorthWindSalesByYear():base()
		{
		}
		
		public override void Run(){
			try {
				base.Run();
				if (!String.IsNullOrEmpty(base.ReportName)) {
					ConnectionObject con = new ConnectionObject(base.MSDEConnection);
					ReportParameters par = base.Engine.LoadParameters(base.ReportName);
					
					par.ConnectionObject = con;
					par.SqlParameters.Clear();
					par.SqlParameters.Add(new SqlParameter("@Beginning_Date",
					                                       System.Data.DbType.DateTime,
					                                       "01/01/1997"));
					par.SqlParameters.Add(new SqlParameter("@Ending_Date",
					                                       System.Data.DbType.DateTime,
					                                       "31.01.1997"));
					base.Engine.PreviewStandartReport(base.ReportName,par);
				}
			} catch (Exception e) {
				MessageBox.Show(e.Message,this.ToString());
			}
		}
		
	}
}
