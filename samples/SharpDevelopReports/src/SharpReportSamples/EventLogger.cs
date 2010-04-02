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
using System.Reflection;
using System.Windows.Forms;

namespace SharpReportSamples
{
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
			ArrayList ar = new ArrayList();
			foreach (System.Diagnostics.EventLogEntry entry in ev.Entries)
			{
				if (entry.TimeWritten > DateTime.Now.AddDays(-1))
				{
					ar.Add(entry);
				}
			}
			this.EventLog = ar;
		}
		
		
		
//		using (var provider = ProfilingDataSQLiteProvider.FromFile("ProfilingSession.sdps"))
//		var functions = provider.GetFunctions(0, provider.DataSets.Count - 1);
//		foreach (CallTreeNode n in functions) Console.WriteLine("{0}: {1} calls, {2:f2}ms", n.Name, n.CallCount, n.TimeSpent);
		
		
	
		private void FillImageList() {
			string ns = this.GetType().Namespace;
			
//			System.Console.WriteLine("{0}",ns);
			
			Assembly a = Assembly.GetExecutingAssembly();
			string [] resNames = a.GetManifestResourceNames();
//			foreach(string s in resNames)
//			{
//				System.Console.WriteLine("{0}",s);
//			}

			this.imageList = new ImageList();
Stream imgStream =  a.GetManifestResourceStream("SharpReportSamples.Resources.error.ico");
//			Stream imgStream =  a.GetManifestResourceStream("SharpReportSamples.Resources.Error.png");
			this.imageList.Images.Add(Image.FromStream(imgStream));
			
			imgStream =  a.GetManifestResourceStream("SharpReportSamples.Resources.Info.png");
			this.imageList.Images.Add(Image.FromStream(imgStream));
			imgStream =  a.GetManifestResourceStream("SharpReportSamples.Resources.warning.ico");
//			imgStream =  a.GetManifestResourceStream("SharpReportSamples.Resources.Warning.png");
			this.imageList.Images.Add(Image.FromStream(imgStream));
			
//			System.Console.WriteLine("imagelist contains {0} images",this.imageList.Images.Count);
		}
		
		
		public ArrayList EventLog {get;set;}
		
		
		public ImageList Images
		{
			get {return this.imageList;}
		}
	}
}

