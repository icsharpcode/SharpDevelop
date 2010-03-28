/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 24.03.2010
 * Zeit: 19:45
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;

//using System.Collections.Generic;

namespace SharpReportSamples
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
		string fileName;
		
		public EventLogger(string fileName)
		{
			this.fileName = fileName;
			this.Run();
		}

		private  void Run() {
			
			EventLog ev = new EventLog();
			ev.Log = "System";
			ev.MachineName = ".";  // Lokale Maschine
			
			FillImageList();
			
			// EventLog dosn't implement IList, so we have to convert it to the 'cheapest'
			// IList implementaion
//
			
			ArrayList ar = new ArrayList();
			
			foreach (System.Diagnostics.EventLogEntry entry in ev.Entries)
			{
				if (entry.TimeWritten > DateTime.Now.AddDays(-1))
				{
//					Console.WriteLine ("{0}  {1}",entry.TimeWritten,entry.Message);
					ar.Add(entry);
				}
			}
			this.EventLog = ar;
	//			ReportEngine engine = new ReportEngine();
				
	//					engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
	//					engine.SectionRendered += new EventHandler<SectionRenderEventArgs>(PushPrinted);
	//			engine.PreviewPushDataReport(fileName,ar,null);
			
		}
		
		
		
//		using (var provider = ProfilingDataSQLiteProvider.FromFile("ProfilingSession.sdps"))
//		var functions = provider.GetFunctions(0, provider.DataSets.Count - 1);
//		foreach (CallTreeNode n in functions) Console.WriteLine("{0}: {1} calls, {2:f2}ms", n.Name, n.CallCount, n.TimeSpent);
		private void filter (EventLog e)
		{
			/*
			int i = 0;
			foreach (System.Diagnostics.EventLogEntry entry in e.Entries)
			{
				if (entry.TimeWritten > DateTime.Now.AddDays(-1))
				{
					Console.WriteLine ("{0}  {1}",entry.TimeWritten,entry.Message);
					i++;
				}
			}
			*/
			
			//http://blog-mstechnology.blogspot.com/2009/08/filter-eventlog-entries-thru-c-code.html
/*
			string SomeDateTime = "20100324000000.000000+000";
			string Query = String.Format("SELECT * FROM Win32_NTLogEvent WHERE Logfile = 'Application' AND TimeGenerated > '{0}'", SomeDateTime);

			ManagementObjectSearcher mos = new ManagementObjectSearcher(Query);
			object o;
			foreach (ManagementObject mo in mos.Get())
			{
				foreach (PropertyData pd in mo.Properties)
				{
					o = mo[pd.Name];
					if (o != null)
					{
//						listBox1.Items.Add(String.Format("{0}: {1}", pd.Name,mo[pd.Name].ToString()));
					}
				}

			}
			*/
		}
		
		private void FillImageList() {
			string ns = this.GetType().Namespace;
			
			System.Console.WriteLine("{0}",ns);
			
			Assembly a = Assembly.GetExecutingAssembly();
			string [] resNames = a.GetManifestResourceNames();
			foreach(string s in resNames)
			{
				System.Console.WriteLine("{0}",s);
			}

			this.imageList = new ImageList();

			Stream imgStream =  a.GetManifestResourceStream("SharpReportSamples.Resources.Error.png");
			this.imageList.Images.Add(Image.FromStream(imgStream));
			
			imgStream =  a.GetManifestResourceStream("SharpReportSamples.Resources.Info.png");
			this.imageList.Images.Add(Image.FromStream(imgStream));
			
			imgStream =  a.GetManifestResourceStream("SharpReportSamples.Resources.Warning.png");
			this.imageList.Images.Add(Image.FromStream(imgStream));
			
			System.Console.WriteLine("imagelist contains {0} images",this.imageList.Images.Count);
		}
		
		public ArrayList EventLog {get;set;}
		
		public ImageList Images
		{
			get {return this.imageList;}
		}
	}
}

