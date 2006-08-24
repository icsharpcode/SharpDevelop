/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 14.08.2006
 * Time: 22:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using SharpReportCore;
namespace ReportSamples
{
	/// <summary>
	/// Description of MissingConnection.
	/// </summary>
	public class MissingConnection:BaseSample
	{
		string conString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\CSharp\samples\Nwind.mdb;Persist Security Info=False";
		public MissingConnection(){
			
		}
		
		public override void Run(){
			base.Run();
			ReportParameters p =  base.Engine.LoadParameters(base.ReportName);
			
			/* this snippet shows a different method to get a valid ConnectionObject
			
			ConnectionObject con = new ConnectionObject(this.conString);
			p.ConnectionObject = con;
			 */
			
			try {
				System.Data.OleDb.OleDbConnectionStringBuilder b = new OleDbConnectionStringBuilder(this.conString);				p.ConnectionObject = new ConnectionObject(b);
				base.Engine.PreviewStandartReport(base.ReportName,p);
				
			} catch (Exception e) {
				MessageBox.Show(e.Message,this.ToString());
			}
			
		}
		
	}
}
