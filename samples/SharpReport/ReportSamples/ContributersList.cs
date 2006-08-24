/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 24.07.2006
 * Time: 11:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpReportCore;

namespace ReportSamples{
	/// <summary>
	/// Description of ReportFromCollection.
	/// </summary>
	public class ContributersList:BaseSample
	{
		public ContributersList(){
		}
		
		public override void Run () {
			
			try
			{
				base.Run();
				if (!String.IsNullOrEmpty(base.ReportName)) {
					TestList list = CreateTestList();
					base.Engine.PreviewPushDataReport(base.ReportName,list);
				}
				
			}
			
			catch (Exception e){
				MessageBox.Show(e.Message,this.ToString());
			}
		}
			
		
		
		private TestList CreateTestList () {
			TestList list = new TestList();
			
			list.Add(new LastFirst("Bernhard","Spuida","Core"));
			list.Add(new LastFirst("Daniel","Grunwald","Core"));
			list.Add(new LastFirst("Christoph","Wille","Core"));
			
			list.Add(new LastFirst("Markus","Palme","Prg."));
			list.Add(new LastFirst("Georg","Brandl","Prg."));
			list.Add(new LastFirst("David","Srbecky","Debugger"));
			list.Add(new LastFirst("Dickon","Field","DBTools"));
			list.Add(new LastFirst("Matt","Ward","NUnit"));
			list.Add(new LastFirst("Troy","Simpson","Prg."));
			list.Add(new LastFirst("Peter","Forstmeier","SharpReport"));
			list.Add(new LastFirst("David","Alpert","Prg."));
			return list;
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
