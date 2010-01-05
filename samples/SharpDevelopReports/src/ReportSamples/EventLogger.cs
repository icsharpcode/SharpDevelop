/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 26.07.2006
 * Time: 08:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;

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
	public class EventLogger:ReportSamples.BaseSample
	{
		ImageList imageList ;
		public EventLogger():base()
		{
		}

		public override void Run() {
			EventLog ev = new EventLog();
			ev.Log = "System";
			ev.MachineName = ".";  // Lokale Maschine
			FillImageList();

			try
			{
				base.Run();
				if (!String.IsNullOrEmpty(base.ReportName)) {
					// EventLog dosn't implement IList, so we have to convert it to the 'cheapest'
					// IList implementaion
					ArrayList ar = new ArrayList();
					ar.AddRange(ev.Entries);
					
					ReportEngine engine = new ReportEngine();
					
					engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
//					engine.SectionRendered += new EventHandler<SectionRenderEventArgs>(PushPrinted);
					engine.PreviewPushDataReport(base.ReportName,ar,null);
				}
				
			}
		
		catch (Exception e){
			MessageBox.Show(e.Message,this.ToString());
		}

		}
		
		private void PushPrinting (object sender,SectionRenderEventArgs e) {

			switch (e.CurrentSection) {
				case GlobalEnums.ReportSection.ReportHeader:
					break;

				case GlobalEnums.ReportSection.ReportPageHeader:
					break;
					
				case GlobalEnums.ReportSection.ReportDetail:
					BaseRowItem ri = e.Section.Items[0] as BaseRowItem;
//					if (ri != null) {
//						BaseDataItem r = (BaseDataItem)ri.Items.Find("unbound1");
//						if (r != null) {
//							System.Console.WriteLine("ubound1");
//				
//						}
//					}
					break;
					
				case GlobalEnums.ReportSection.ReportPageFooter:
					break;
					
				case GlobalEnums.ReportSection.ReportFooter:
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
			
			System.Console.WriteLine("{0}",ns);
			
			Assembly a = Assembly.GetExecutingAssembly();
			string [] resNames = a.GetManifestResourceNames();
			foreach(string s in resNames)
			{
				System.Console.WriteLine("{0}",s);
			}

			this.imageList = new ImageList();

			Stream imgStream =  a.GetManifestResourceStream("ReportSamples.Resources.Error.png");
			this.imageList.Images.Add(Image.FromStream(imgStream));
			
			imgStream =  a.GetManifestResourceStream("ReportSamples.Resources.Info.png");
			this.imageList.Images.Add(Image.FromStream(imgStream));
			
			imgStream =  a.GetManifestResourceStream("ReportSamples.Resources.Warning.png");
			this.imageList.Images.Add(Image.FromStream(imgStream));
			
			System.Console.WriteLine("imagelist contains {0} images",this.imageList.Images.Count);
		}
	}
}



