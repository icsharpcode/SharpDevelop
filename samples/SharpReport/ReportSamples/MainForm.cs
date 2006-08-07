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
			SimplePullModel simplePull = new SimplePullModel();
			simplePull.Run();
		}
		
		
		
		void PullModelClick(object sender, System.EventArgs e)
		{
			SimplePullModel simplePull = new SimplePullModel();
			simplePull.Run();
		}
	
		
		void MissingConnectionClick(object sender, System.EventArgs e)
		{
			MissingConnection missingConnection = new MissingConnection();
			missingConnection.Run();
		}
		
		void SimplePushClick(object sender, System.EventArgs e)
		{
			SimplePushModel simplePush = new SimplePushModel();
			simplePush.Run();
		}
		
		void UnboundToolStripMenuItem1Click(object sender, System.EventArgs e)
		{
			SimpleUnbound simpleUnbound = new SimpleUnbound();
			simpleUnbound.Run();
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
