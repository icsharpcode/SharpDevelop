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
			
		}
		
		#region FormsSheet
		
		void SimpleFormsSheet(object sender, System.EventArgs e)
		{
			SimplePullModel simplePull = new SimplePullModel();
			simplePull.Run();
		}
		
		void UnboundFormSheet(object sender, System.EventArgs e){
			UnboundFormSheet unboundFormSheet = new UnboundFormSheet();
			unboundFormSheet.Run();
		}
		
		
		#endregion
		
		#region PullModel
		
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
		
		#endregion
		
		#region PushModel
		
		void SimplePushClick(object sender, System.EventArgs e)
		{
			SimplePushModel simplePush = new SimplePushModel();
			simplePush.Run();
		}
		
		#endregion
		
		#region Unbound

		
		void UnboundPullModelClick(object sender, System.EventArgs e){
			SimpleUnboundPullModel sm = new SimpleUnboundPullModel();
			sm.Run();
		}
		
		void MultiPageUnboundPullModelClick(object sender, System.EventArgs e)
		{
			MultiPageUnboundPullModel mp = new MultiPageUnboundPullModel();
			mp.Run();
		}
		
		void UnboundPushModelClick(object sender, System.EventArgs e)
		{
			UnboundPushModel u = new UnboundPushModel();
			u.Run();
		}
		
		#endregion
		
		#region List as DataSource
		void ContributersListClick(object sender, System.EventArgs e)
		{
			ContributersList r = new ContributersList();
			r.Run();
		}
		
		void EventLoggerClick(object sender, System.EventArgs e)
		{
			EventLogger el = new EventLogger();
			el.Run();
		}
		
		#endregion
		
		#region MSDE
		
		void NorthWindSalesByYearClick(object sender, System.EventArgs e)
		{
			NorthWindSalesByYear northWindSalesByYear = new NorthWindSalesByYear();
			northWindSalesByYear.Run();
		}
		
		void CustOrdersDetailClick(object sender, System.EventArgs e)
		{
			CustOrdersOrdersDetail custOrderDetail = new CustOrdersOrdersDetail();
			custOrderDetail.Run();
		}
		#endregion
		
		
	}
}
