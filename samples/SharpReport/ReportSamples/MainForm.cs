/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 08.02.2006
 * Time: 15:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Windows.Forms;

using SharpReportCore;

namespace ReportSamples
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
	
		
		private void MissingConnection () {
			try
			{
				string connectionString="";
				OpenFileDialog dg = new OpenFileDialog();
				dg.Filter = "SharpReport files|*.srd";
				dg.Title = "Select a report file: ";
				if (dg.ShowDialog() == DialogResult.OK)
				{
					SharpReportCore.SharpReportEngine mn = new SharpReportCore.SharpReportEngine();
					SharpReportCore.ReportParameters pn = new SharpReportCore.ReportParameters();

//					sql = "Provider=SQLOLEDB.1;Password=xxx;Persist Security Info=True;User ID=xxx;Initial Catalog=Northwind;Data Source=WS161";
					connectionString = this.ConnectionStringFromMsDataLink();
		

//					pn.ConnectionObject = new ConnectionObject(sql);
					
					pn.ConnectionObject = this.ConnectionFromConnectionBuilder(connectionString);

//					pn.SqlParameters.Clear();
//					pn.SqlParameters.Add(new SharpReportCore.SqlParameter("@OrderID",System.Data.DbType.Int32,10248));
//					pn.SortColumnCollection.Add(new SortColumn("UnitPrice",System.ComponentModel.ListSortDirection.Descending));
					mn.PreviewStandartReport(dg.FileName.ToString(),pn);
//					mn.PrintStandartReport(dg.FileName.ToString(),pn);
				}
			}
			catch(Exception er)
			{
				MessageBox.Show(er.ToString(),": MainForm");
			}
		}
		
		private void OpenPull()
		{
			try
			{
				OpenFileDialog dg = new OpenFileDialog();
				dg.Filter = "SharpReport files|*.srd";
				dg.Title = "Select a report file: ";
				if (dg.ShowDialog() == DialogResult.OK){
					SharpReportCore.SharpReportEngine mn = new SharpReportCore.SharpReportEngine();
					mn.PreviewStandartReport(dg.FileName.ToString());
//					mn.PrintStandartReport(dg.FileName.ToString());
					
				}
			}
			catch(Exception er)
			{
				MessageBox.Show(er.ToString(),"MainForm");
			}
		}
		
		#region unbound
		private void OpenUnbound() {
			try{
				OpenFileDialog dg = new OpenFileDialog();
				dg.Filter = "SharpReport files|*.srd";
				dg.Title = "Select a report file: ";
				if (dg.ShowDialog() == DialogResult.OK){
					SharpReportCore.SharpReportEngine mn = new SharpReportCore.SharpReportEngine();
					mn.SectionRendering += new EventHandler<SectionRenderEventArgs>(UnboundPrinting);
					mn.SectionRendered += new EventHandler<SectionRenderEventArgs>(UnboundPrinted);
					mn.PreviewStandartReport(dg.FileName.ToString());
					
				}
			}
			catch(Exception er){
				MessageBox.Show(er.ToString(),"MainForm");
			}
		}
		
		private void UnboundPrinting (object sender,SectionRenderEventArgs e) {
//			System.Console.WriteLine("");
//			System.Console.WriteLine("--------------");
//			System.Console.WriteLine("MainForm:OnTestPrinting <{0}> for PageNr <{1}>",e.CurrentSection,e.PageNumber);
//			System.Console.WriteLine("\t <{0}> Items",e.Section.Items.Count);
//
//		
			switch (e.CurrentSection) {
				case GlobalEnums.enmSection.ReportHeader:
					System.Console.WriteLine("\tI found the ReportHeader");
					break;
				
				
				case GlobalEnums.enmSection.ReportPageHeader:
					
					BaseTextItem t = (BaseTextItem)e.Section.Items.Find("reportTextItem1");
					if (t != null) {
						t.Location = new Point(80,5);
						t.Text = "Label";
					}
					
					
					BaseDataItem bb = (BaseDataItem)e.Section.Items.Find("reportDbTextItem1");
					if (bb != null) {
						bb.DrawBorder = true;
						bb.Location = new Point(200,5);
						bb.DbValue = "Hello World";
					}
					
					System.Console.WriteLine("\tI found the Pageheader");
					break;
				
				case GlobalEnums.enmSection.ReportDetail:
					System.Console.WriteLine("\tI found the ReportDetail");
					BaseDataItem bdi = (BaseDataItem)e.Section.Items.Find("reportDbTextItem1");
					if (bdi != null) {
						bdi.BackColor = Color.LightGray;
						bdi.Location = new Point(200,5);
						bdi.DbValue = "Unbound Field in DetailSection";
					}
					
					
					break;
				
				case GlobalEnums.enmSection.ReportPageFooter:
					System.Console.WriteLine("\tI found the PageFooter");
					BaseReportItem b = (BaseReportItem)e.Section.Items.Find("pageNumber1");
					if (b != null) {
						b.BackColor = Color.AliceBlue;
					} else {
						string s = String.Format ("<{0}> not found");
						MessageBox.Show (s);
					}
					break;
				
				case GlobalEnums.enmSection.ReportFooter:
					System.Console.WriteLine("\tI found the ReportFooter");
					break;
				
				default:
					break;
			}
			System.Console.WriteLine("");
		}
		
		private void UnboundPrinted (object sender,SectionRenderEventArgs e) {
//			System.Console.WriteLine("MainForm:Rendering done for  <{0}>",e.CurrentSection);
			System.Console.WriteLine("----------");
		}
		
		#endregion
		
		
		
		///<summary>Preferd Method to initialise the <see cref="SharpReportCore.ConnectionObject"></see>
		/// hav a look to
		/// <http://msdn2.microsoft.com/en-us/library/system.data.oledb.oledbconnectionstringbuilder(VS.80).aspx>
		/// <example><code>
		/// public void OleDbConnectionBuilderForSqlServer () {
		///			OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
		///			builder["Provider"] = "SQLOLEDB.1";
		///			builder["Integrated Security"] = "SSPI";
		///			builder["Initial Catalog"] = "NorthWind";
		///			System.Console.WriteLine("{0}",builder.ConnectionString);
		///			ConnectionObject obj = new ConnectionObject(builder);
		///			Assert.IsTrue(obj.Connection.State == ConnectionState.Closed,"Connection should be closed");
		///			obj.Connection.Open();
		///			Assert.IsTrue(obj.Connection.State == ConnectionState.Open,"Connection should be opend");
		///}
		/// </code></example>
		/// </summary>
		/// 
	
		private SharpReportCore.ConnectionObject ConnectionFromConnectionBuilder (string sql) {
			System.Data.Common.DbConnectionStringBuilder builder = new System.Data.OleDb.OleDbConnectionStringBuilder(sql);
			return new SharpReportCore.ConnectionObject (builder.ConnectionString);
		}
	
		
		private string ConnectionStringFromMsDataLink() {
			ADODB._Connection AdoConnection;
			MSDASC.DataLinks dataLink = new MSDASC.DataLinks();
			
			AdoConnection = null;
			AdoConnection = (ADODB._Connection) dataLink.PromptNew();
			return AdoConnection.ConnectionString;
		}
		
		///<summary>This Report is send directly to the Printer, PrintDialog is
		///  showing when UseStandartPrinter to 'false'</summary>
		
		private void OpenPushModell()
		{
			string reportFileName;
			try
			{
				OpenFileDialog dg = new OpenFileDialog();
				dg.Filter = "SharpReport files|*.srd";
				dg.Title = "Select a report file: ";
				if (dg.ShowDialog() == DialogResult.OK){
					SharpReportCore.SharpReportEngine mn = new SharpReportCore.SharpReportEngine();
					reportFileName = dg.FileName.ToString();
					DataTable table = SelectData();
					if (table != null) {
						mn.PreviewPushDataReport(reportFileName,table);
//						mn.PrintPushDataReport(reportFileName,table);
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
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void SimpleFormsSheetClick(object sender, System.EventArgs e)
		{
			OpenPull();
		}
		
		void CustomersClick(object sender, System.EventArgs e)
		{
			OpenPull();
		}
		
		void EmployeeClick(object sender, System.EventArgs e)
		{
			OpenPull();
		}
		
		void MissingConnectionClick(object sender, System.EventArgs e)
		{
			MissingConnection();
		}
		
		void EmployeesPushClick(object sender, System.EventArgs e)
		{
			OpenPushModell();
		}
		
		void UnboundToolStripMenuItem1Click(object sender, System.EventArgs e)
		{
			this.OpenUnbound();
		}
		
		void UnboundPullModelToolStripMenuItemClick(object sender, System.EventArgs e){
			SimpleUnboundPullModel sm = new SimpleUnboundPullModel();
			sm.Run();
		}
		
		void MultipageUnboundPullModelToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			MultiPageUnboundPullModel mp = new MultiPageUnboundPullModel();
			mp.Run();
		}
		
		void UnboundPushModelToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			UnboundPushModel u = new UnboundPushModel();
			u.Run();
		}
		
		void UnboundFormSheetToolStripMenuItemClick(object sender, System.EventArgs e){
			UnboundFormSheet unboundFormSheet = new UnboundFormSheet();
			unboundFormSheet.Run();
		}
		
		void ListDatasourceToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			ContributersList r = new ContributersList();
			r.Run();
		}
		
		void EventLoggerToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			EventLogger el = new EventLogger();
			el.Run();
		}
	}
}
