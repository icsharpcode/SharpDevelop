/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 14.08.2006
 * Time: 22:28
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
	/// Description of BaseSample.
	/// </summary>
	public class BaseSample{
		string msdeConnection = @"Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Northwind";
		
		SharpReportEngine engine = new SharpReportCore.SharpReportEngine();
		string reportName;
		
		public BaseSample(){
			engine = new SharpReportCore.SharpReportEngine();
		}
		
		public virtual void Run() {
			try
			{
				OpenFileDialog dg = new OpenFileDialog();
				dg.Filter = "SharpReport files|*.srd";
				dg.Title = "Select a report file: ";
				
				if (dg.ShowDialog() == DialogResult.OK){
					this.reportName = dg.FileName;
				}
			}
			catch(Exception er)
			{
				MessageBox.Show(er.ToString(),"MainForm");
			}
		}
		
		protected DataTable SelectData()
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
		
		protected SharpReportEngine Engine {
			get {
				return engine;
			}
			
		}
		
		protected string ReportName {
			get {
				return reportName;
			}
			
		}
		protected string MSDEConnection {
			get {
				return msdeConnection;
			}
		
		}
		
	}
}
