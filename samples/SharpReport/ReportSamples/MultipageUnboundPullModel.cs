/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 29.06.2006
 * Time: 13:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

using System.Windows.Forms;

using SharpReportCore;

namespace ReportSamples{
	/// <summary>
	/// Description of MultipageUnboundPullModel.
	/// </summary>
	public class MultiPageUnboundPullModel{
		int rowNr;
		int rowsPerPage;
		System.DateTime startTime;
		System.DateTime endTime;
		
		public MultiPageUnboundPullModel(){
		}
		
		public void Run() {
			try{
				OpenFileDialog dg = new OpenFileDialog();
				dg.Filter = "SharpReport files|*.srd";
				dg.Title = "Select a report file: ";
				if (dg.ShowDialog() == DialogResult.OK){
					SharpReportCore.SharpReportEngine mn = new SharpReportCore.SharpReportEngine();
					mn.SectionRendering += new EventHandler<SectionRenderEventArgs>(MultipagePrinting);
					mn.SectionRendered += new EventHandler<SectionRenderEventArgs>(MultipagePrinted);
					this.startTime = System.DateTime.Now;
					
					mn.PreviewStandartReport(dg.FileName.ToString());
				}
			}
			catch(Exception er){
				MessageBox.Show(er.ToString(),"MainForm");
			}
		}
		
		
		private void MultipagePrinting (object sender,SectionRenderEventArgs e) {
			System.Console.WriteLine("UnboundPullPrinting");
			CheckItems(e.Section.Items);
			switch (e.CurrentSection) {
				case GlobalEnums.enmSection.ReportHeader:
					System.Console.WriteLine("\tReportHeader");
					break;

				case GlobalEnums.enmSection.ReportPageHeader:
					
					System.Console.WriteLine("\tPageheader");
					System.Console.WriteLine("");
					this.rowsPerPage = 0;
					break;
					
				case GlobalEnums.enmSection.ReportDetail:
				
					this.rowNr ++;
					this.rowsPerPage ++;
					System.Console.WriteLine("\tReportDetail");
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
					System.Console.WriteLine("\tPageFooter");
					BaseDataItem bdi = e.Section.Items.Find("ItemsPerPage") as BaseDataItem;
					if (bdi != null) {
						bdi.DbValue = this.rowsPerPage.ToString();
					}
					break;
					
				case GlobalEnums.enmSection.ReportFooter:
					System.Console.WriteLine("\tReportFooter");
					this.endTime = System.DateTime.Now;
					
					BaseDataItem b = e.Section.Items.Find("reportDbTextItem1")as BaseDataItem;
					if (b != null) {
						b.FormatString = "t";
						b.DbValue = (this.endTime - this.startTime).ToString();
					}
					
					break;
					
				default:
					break;
			}
		}
		
		private void MultipagePrinted (object sender,SectionRenderEventArgs e) {
//			System.Console.WriteLine("---Rendering done <{0}>-----",e.CurrentSection);
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
