/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 24.07.2006
 * Time: 11:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ReportSamples{
	/// <summary>
	/// Description of ReportFromCollection.
	/// </summary>
	public class ContributersList
	{
		public ContributersList(){
		}
		
		public void Run () {
			string reportFileName;
			try
			{
				OpenFileDialog dg = new OpenFileDialog();
				dg.Filter = "SharpReport files|*.srd";
				dg.Title = "Select a report file: ";
				if (dg.ShowDialog() == DialogResult.OK){
					SharpReportCore.SharpReportEngine engine = new SharpReportCore.SharpReportEngine();
					reportFileName = dg.FileName.ToString();

					TestList list = new TestList();
					
					list.Add(new LastFirst("Bernhard","Spuida","Core"));
					list.Add(new LastFirst("Daniel","Grünwald","Core"));
					list.Add(new LastFirst("Cristoph","Wille","Core"));
					
					list.Add(new LastFirst("Markus","Palme","Prg."));
					list.Add(new LastFirst("Georg","Brandl","Prg."));
					list.Add(new LastFirst("David","Srbecky","Debugger"));
					list.Add(new LastFirst("Dickon","Field","DBTools"));
					list.Add(new LastFirst("Matt","Ward","NUnit"));
					list.Add(new LastFirst("Troy","Simson","Prg."));
					list.Add(new LastFirst("Peter","Forstmeier","SharpReport"));
					list.Add(new LastFirst("David","Albert","Prg."));
					
//					list.Add(new LastFirst("Sylvana","Schmid"));
					

//						engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
//						engine.SectionRendered += new EventHandler<SectionRenderEventArgs>(PushPrinted);
					engine.PreviewPushDataReport(reportFileName,list);

//					}
				}
			}
			catch (Exception){
			}
		}
	}
	
	public class LastFirst {
		string last;
		string first;
		string job;
		
		public LastFirst(string last, string first,string job)
		{
			this.last = last;
			this.first = first;
			this.job = job;
		}
		
		public string Last {
			get {
				return last;
			}
		
		}
		
		public string First {
			get {
				return first;
			}
			
		}
		
		public string Job {
			get {
				return job;
			}
		}
		
	}
	
	public class TestList: List<LastFirst>{
	}
	
}
