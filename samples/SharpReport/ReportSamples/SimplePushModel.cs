/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 14.08.2006
 * Time: 22:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data;
using System.Windows.Forms;
using SharpReportCore;

namespace ReportSamples
{
	/// <summary>
	/// Description of SimplePushModel.
	/// </summary>
	public class SimplePushModel:BaseSample
	{
		public SimplePushModel()
		{
		
		}
		
		public override void Run(){
			try {
				base.Run();
				DataTable table = SelectData();
				
				if (table != null) {
					base.Engine.PreviewPushDataReport(base.ReportName,table);
				}
				
			} catch (Exception e) {
				MessageBox.Show(e.Message,this.ToString());
			}
			
		}
		
	}
}
