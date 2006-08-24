/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 14.08.2006
 * Time: 22:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;
using SharpReportCore;

namespace ReportSamples
{
	/// <summary>
	/// Description of SimplePullModel.
	/// </summary>
	public class SimplePullModel:BaseSample
	{
		public SimplePullModel(){
			
		}
		
		public override void Run(){
			try {
				base.Run();
				base.Engine.PreviewStandartReport(base.ReportName);
			} catch (Exception e) {
				MessageBox.Show(e.ToString(),this.ToString());
			}
			
		}
		
	}
}
