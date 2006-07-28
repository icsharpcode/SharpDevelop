/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 26.07.2006
 * Time: 08:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

using SharpReportCore;
using System.Reflection;
//using System.Collections.Generic;

namespace ReportSamples
{
//	public class EventList: List<EventLogEntry>{
//	}
//	
//	
	/// <summary>
	/// Description of EventLogger.
	/// </summary>
	public class EventLogger
	{
		ImageList imageList ;
		public EventLogger()
		{
		}

		public void Run() {
			EventLog ev = new EventLog();
			ev.Log = "System";
			ev.MachineName = ".";  // Lokale Maschine
			
			FillImageList();

			string reportFileName;
	
			try
			{
				OpenFileDialog dg = new OpenFileDialog();
				dg.Filter = "SharpReport files|*.srd";
				dg.Title = "Select a report file: ";
				if (dg.ShowDialog() == DialogResult.OK){
					SharpReportCore.SharpReportEngine engine = new SharpReportCore.SharpReportEngine();
					reportFileName = dg.FileName.ToString();
// EventLog dosn#t implement IList, so we have to convert it to the 'cheapest'
// IList implementaion
					ArrayList ar = new ArrayList();
					ar.AddRange(ev.Entries);
					
					engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
					engine.SectionRendered += new EventHandler<SectionRenderEventArgs>(PushPrinted);
					engine.PreviewPushDataReport(reportFileName,ar);
					
				}
			}
			catch (Exception){
			}

		}
		
		private void PushPrinting (object sender,SectionRenderEventArgs e) {
//			System.Console.WriteLine("SimpleUnboundPullPrinting");
//			CheckItems(e.Section.Items);
			switch (e.CurrentSection) {
				case GlobalEnums.enmSection.ReportHeader:
//					System.Console.WriteLine("\tI found the ReportHeader");
					break;

				case GlobalEnums.enmSection.ReportPageHeader:
					
//					System.Console.WriteLine("\tI found the Pageheader");
					break;
					
				case GlobalEnums.enmSection.ReportDetail:
//					System.Console.WriteLine("\tI found the ReportDetail");
//					this.rowNr ++;
					RowItem ri = e.Section.Items[0] as RowItem;
					if (ri != null) {
						BaseDataItem r = (BaseDataItem)ri.Items.Find("reportDbTextItem1");
						
						if (r != null) {
							BaseImageItem image = (BaseImageItem)ri.Items.Find("Image");
							switch (r.DbValue) {
								case "Information":
									image.Image = this.imageList.Images["Info"];
									break;
								case "Error":
									image.Image = this.imageList.Images["Error"];
									break;
								case "Warning" :
									image.Image = this.imageList.Images["Warning"];
									break;
							}
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
		
		private void PushPrinted (object sender,SectionRenderEventArgs e) {
////			System.Console.WriteLine("MainForm:Rendering done for  <{0}>",e.CurrentSection);
//			System.Console.WriteLine("----------");
		}
		
		void FillImageList() {
			string ns = this.GetType().Namespace;
			System.Console.WriteLine("\t{0}",ns);
			ResourceManager resMan = new ResourceManager(ns +
			                                             ".ImageResource", this.GetType().Assembly);


			this.imageList = new ImageList();
			Image i = (Image)resMan.GetObject("Error");
			this.imageList.Images.Add("Error",i);
			
			
			i = (Image)resMan.GetObject("Info");
			this.imageList.Images.Add("Info",i);
			
			i = (Image)resMan.GetObject("Warning");
			this.imageList.Images.Add("Warning",i);
		}
	}
}
