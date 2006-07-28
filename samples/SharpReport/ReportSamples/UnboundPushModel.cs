/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 10.07.2006
 * Time: 13:15
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
	/// Description of UnboundPushModel.
	/// </summary>
	public class UnboundPushModel
	{
		int rowNr;
		
		public UnboundPushModel()
		{
		}
		
		public void Run() {
			string reportFileName;
			try
			{
				OpenFileDialog dg = new OpenFileDialog();
				dg.Filter = "SharpReport files|*.srd";
				dg.Title = "Select a report file: ";
				if (dg.ShowDialog() == DialogResult.OK){
					SharpReportCore.SharpReportEngine engine = new SharpReportCore.SharpReportEngine();
					reportFileName = dg.FileName.ToString();
					DataTable table = SelectData();
					
					if (table != null) {
						engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
						engine.SectionRendered += new EventHandler<SectionRenderEventArgs>(PushPrinted);
//						engine.PreviewPushDataReport(reportFileName,table);
						engine.PrintPushDataReport(reportFileName,table);
					}
				}
			}
			catch (Exception){
			}
		}
		
		
		
		private DataTable SelectData()
		{
			OpenFileDialog dg = new OpenFileDialog();
			dg.Filter = "SharpReport files|*.xsd";
			dg.Title = "Select a '.xsdfile: ";
			if (dg.ShowDialog() == DialogResult.OK){
				DataSet ds = new DataSet();
				ds.ReadXml(dg.FileName);
				return ds.Tables[0];
			}
			return null;
		}
		
		private void PushPrinting (object sender,SectionRenderEventArgs e) {
			System.Console.WriteLine("UnboundPushModel");
			CheckItems(e.Section.Items);
			switch (e.CurrentSection) {
				case GlobalEnums.enmSection.ReportHeader:
					System.Console.WriteLine("\tReportHeader");
					break;

				case GlobalEnums.enmSection.ReportPageHeader:
					
					System.Console.WriteLine("\tPageheader");
					System.Console.WriteLine("");
					break;
					
				case GlobalEnums.enmSection.ReportDetail:
				
					this.rowNr ++;
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
					break;
					
				case GlobalEnums.enmSection.ReportFooter:
					System.Console.WriteLine("\tReportFooter");
					BaseDataItem b = e.Section.Items.Find("ReportDbTextItem")as BaseDataItem;
					if (b != null) {
						b.DbValue = this.rowNr.ToString();
					}
					
					break;
					
				default:
					break;
			}
		}
		
		private void PushPrinted (object sender,SectionRenderEventArgs e) {
//			System.Console.WriteLine("---Rendering done <{0}>-----",e.CurrentSection);
		}
		
		private void CheckItems (ReportItemCollection items) {
			foreach (BaseReportItem i in items) {
//				System.Console.WriteLine("\tItem {0}",i.Name);
				IContainerItem container = i as IContainerItem;
				if (container != null) {
//					System.Console.WriteLine("\t\tContainer found");
					CheckItems (container.Items);
				}
			}
		}
	}
}
