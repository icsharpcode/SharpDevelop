/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 23.08.2006
 * Time: 22:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;

using SharpReportCore;

namespace ReportSamples
{
	/// <summary>
	/// Description of CustOrdersDetail.
	/// </summary>
	public class CustOrdersOrdersDetail:BaseSample
	{
		string paramName;
		
		public CustOrdersOrdersDetail()
		{
		}
		
		public override void Run()
		{
			try {
				base.Run();
				
				if (!String.IsNullOrEmpty(base.ReportName)) {
					ConnectionObject con = new ConnectionObject(base.MSDEConnection);
					ReportParameters par = base.Engine.LoadParameters(base.ReportName);
					par.ConnectionObject = con;
				
					using (ParameterDialog dialog = new ParameterDialog(par.SqlParameters)){
						dialog.ShowDialog();
						if (dialog.DialogResult == DialogResult.OK) {
							SqlParameter p = par.SqlParameters.Find("@CustomerID");
							this.paramName = (string)p.ParameterValue;
							base.Engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(CustOrdersOrdersPrinting);
							base.Engine.PreviewStandartReport(base.ReportName,par);
						}
					}
					
					
					
				}
			} catch (Exception e) {
				MessageBox.Show(e.Message,this.ToString());
			}
			
		}
		private void CustOrdersOrdersPrinting (object sender,SectionRenderEventArgs e) {
		
			switch (e.CurrentSection) {
				case GlobalEnums.enmSection.ReportHeader:
					break;

				case GlobalEnums.enmSection.ReportPageHeader:
					BaseDataItem bdi = e.Section.Items.Find("CustomerID") as BaseDataItem;
					bdi.DbValue = "[" + this.paramName + "]";
					break;
					
				case GlobalEnums.enmSection.ReportDetail:
					break;
					
				case GlobalEnums.enmSection.ReportPageFooter:
	
					break;
					
				case GlobalEnums.enmSection.ReportFooter:
					break;
					
				default:
					break;
			}
		}
	}
}
