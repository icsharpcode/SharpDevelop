/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 29.06.2006
 * Time: 09:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

using SharpReportCore;
namespace ReportSamples
{
	/// <summary>
	/// Description of SimpleUnboundPullModel.
	/// </summary>
	public class SimpleUnboundPullModel{
		int rowNr;
		
		public SimpleUnboundPullModel(){
				
		}
		
		public void Run() {
			try{
				OpenFileDialog dg = new OpenFileDialog();
				dg.Filter = "SharpReport files|*.srd";
				dg.Title = "Select a report file: ";
				if (dg.ShowDialog() == DialogResult.OK){
					SharpReportCore.SharpReportEngine mn = new SharpReportCore.SharpReportEngine();
					mn.SectionRendering += new EventHandler<SectionRenderEventArgs>(SimplePullPrinting);
					mn.SectionRendered += new EventHandler<SectionRenderEventArgs>(SimplePullPrinted);
					mn.PreviewStandartReport(dg.FileName.ToString());
					
				}
			}
			catch(Exception er){
				MessageBox.Show(er.ToString(),this.ToString());
			}
		}
		private void SimplePullPrinting (object sender,SectionRenderEventArgs e) {
//			System.Console.WriteLine("SimpleUnboundPullPrinting");
			CheckItems(e.Section.Items);
			switch (e.CurrentSection) {
				case GlobalEnums.enmSection.ReportHeader:
//					System.Console.WriteLine("\tI found the ReportHeader");
					break;

				case GlobalEnums.enmSection.ReportPageHeader:
					
//					System.Console.WriteLine("\tI found the Pageheader");
					break;
					
				case GlobalEnums.enmSection.ReportDetail:
//					System.Console.WriteLine("\tI found the ReportDetail");
					this.rowNr ++;
					RowItem ri = e.Section.Items[0] as RowItem;
					if (ri != null) {
						if (this.rowNr %2 == 0) {
							ri.DrawBorder = true;
						} else {
							ri.DrawBorder = false;
						}
					}
					break;
					
				case GlobalEnums.enmSection.ReportPageFooter:
//					System.Console.WriteLine("\tI found the PageFooter");
					break;
					
				case GlobalEnums.enmSection.ReportFooter:
//					System.Console.WriteLine("\tI found the ReportFooter");
					break;
					
				default:
					break;
			}
		}
		
		private void SimplePullPrinted (object sender,SectionRenderEventArgs e) {
////			System.Console.WriteLine("MainForm:Rendering done for  <{0}>",e.CurrentSection);
//			System.Console.WriteLine("----------");
		}
		
		private void CheckItems (ReportItemCollection items) {
//			System.Console.WriteLine("\t<{0}> Items",items.Count );
			foreach (BaseReportItem i in items) {
				IContainerItem container = i as IContainerItem;
				if (container != null) {
//					System.Console.WriteLine("\t\tContainer found");
					CheckItems (container.Items);
				}
			}
		}
	}
}
