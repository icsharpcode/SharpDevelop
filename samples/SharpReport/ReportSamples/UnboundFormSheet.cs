/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 18.07.2006
 * Time: 22:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;

using SharpReportCore;

namespace ReportSamples
{
	/// <summary>
	/// Description of UnboundFormSheet.
	/// </summary>
	public class UnboundFormSheet:BaseSample
	{
		public UnboundFormSheet():base()
		{
		
		}
		
		public override void Run()
		{
			try {
				
				base.Run();
				base.Engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(UnboundPrinting);
				base.Engine.SectionRendered += new EventHandler<SectionRenderEventArgs>(UnboundPrinted);
				base.Engine.PreviewStandartReport(base.ReportName);
			} catch (Exception e) {
				MessageBox.Show(e.Message,this.ToString());
			}
		}
		
		
		private void UnboundPrinting (object sender,SectionRenderEventArgs e) {
			System.Console.WriteLine("UnboundFormSheet");
			
			switch (e.CurrentSection) {
				case GlobalEnums.enmSection.ReportHeader:
					System.Console.WriteLine("\tReportHeader");
					BaseDataItem a = e.Section.Items.Find("reportHeaderLabel")as BaseDataItem;
					System.Console.WriteLine("\t{0}",a.Font.ToString());
					if (a != null) {
						a.DbValue = "The Report starts here";
					}
					break;

				case GlobalEnums.enmSection.ReportPageHeader:
					
					System.Console.WriteLine("\tPageheader");
					System.Console.WriteLine("");
					BaseDataItem b = e.Section.Items.Find("pageHeaderLabel")as BaseDataItem;
					if (b != null) {
						b.DbValue = "This is the Pageheader";
					}
					break;
					
				case GlobalEnums.enmSection.ReportDetail:
				
					
					System.Console.WriteLine("\tReportDetail");
					BaseDataItem c = e.Section.Items.Find("detailLabel")as BaseDataItem;
					if (c != null) {
						c.DbValue = "The Detail Section";
					}
					break;
					
				case GlobalEnums.enmSection.ReportPageFooter:
					System.Console.WriteLine("\tPageFooter");
					BaseDataItem d = e.Section.Items.Find("pageFooterLabel")as BaseDataItem;
					if (d != null) {
						d.DbValue = "Page Footer";
					}
					break;
					
				case GlobalEnums.enmSection.ReportFooter:
					System.Console.WriteLine("\tReportFooter");
					BaseDataItem ee = e.Section.Items.Find("reportFooterLabel")as BaseDataItem;
					if (ee != null) {
						ee.DbValue = "Report Footer is printed before PageFooter";
					}
					break;
					
				default:
					break;
			}
		}
		
		private void UnboundPrinted (object sender,SectionRenderEventArgs e) {
//			System.Console.WriteLine("---Rendering done <{0}>-----",e.CurrentSection);
		}
	}
}
