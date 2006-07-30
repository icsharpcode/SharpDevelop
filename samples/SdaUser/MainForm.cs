/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 26.07.2006
 * Time: 19:49
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Sda;

namespace SdaUser
{
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
		
		void RunButtonClick(object sender, EventArgs e)
		{
			Run();
		}
		
		SharpDevelopHost host;
		
		void Run()
		{
			runButton.Enabled = false;
			unloadHostDomainButton.Enabled = true;
			System.Threading.ThreadPool.QueueUserWorkItem(ThreadedRun);
		}
		
		void ThreadedRun(object state)
		{
			RunWorkbench(new WorkbenchSettings());
		}
		
		void RunWorkbench(WorkbenchSettings wbSettings)
		{
			if (host == null) {
				StartupSettings startup = new StartupSettings();
				startup.ApplicationName = "HostedSharpDevelop";
				startup.DataDirectory = Path.Combine(Path.GetDirectoryName(typeof(SharpDevelopHost).Assembly.Location), "../data");
				string sdaDir = Path.Combine(Path.GetDirectoryName(typeof(MainForm).Assembly.Location), "SdaAddIns");
				startup.AddAddInFile(Path.Combine(sdaDir, "SdaBase.addin"));
				
				host = new SharpDevelopHost(startup);
				host.InvokeTarget = this;
				host.BeforeRunWorkbench += delegate { groupBox1.Enabled = true; };
				host.WorkbenchClosed += delegate { groupBox1.Enabled = false; runButton.Enabled = true; };
			}
			
			host.RunWorkbench(wbSettings);
		}
		
		void VisibleCheckBoxCheckedChanged(object sender, System.EventArgs e)
		{
			host.WorkbenchVisible = visibleCheckBox.Checked;
		}
		
		void CloseButtonClick(object sender, System.EventArgs e)
		{
			if (!host.CloseWorkbench(false)) {
				if (DialogResult.Yes == MessageBox.Show("Force close?", "Force", MessageBoxButtons.YesNo)) {
					host.CloseWorkbench(true);
				}
			}
		}
		
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (host != null && host.WorkbenchVisible == false) {
				if (!host.CloseWorkbench(false)) {
					host.WorkbenchVisible = true;
				}
			}
		}
		
		void UnloadHostDomainButtonClick(object sender, System.EventArgs e)
		{
			unloadHostDomainButton.Enabled = false;
			host.UnloadDomain();
			host = null;
		}
		
		void OpenFileButtonClick(object sender, System.EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				if (dlg.ShowDialog() == DialogResult.OK) {
					if (runButton.Enabled) {
						// create host with InitialFile set
						runButton.Enabled = false;
						unloadHostDomainButton.Enabled = true;
						
						WorkbenchSettings wbSettings = new WorkbenchSettings();
						wbSettings.InitialFileList.Add(dlg.FileName);
						RunWorkbench(wbSettings);
					} else if (host != null) {
						if (host.IsSolutionOrProject(dlg.FileName)) {
							// won't occur because no project types are defined
							// in our reduces .addin file.
							host.OpenProject(dlg.FileName);
						} else {
							host.OpenDocument(dlg.FileName);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// This demonstrates how to access SharpDevelop's internals from the host application.
		/// </summary>
		void MakeTransparentButtonClick(object sender, System.EventArgs e)
		{
			// We need to use a wrapper class to cross the AppDomain barrier.
			// The assembly containing the wrapper class will be loaded to both AppDomains.
			// Therefore we don't use our possibly large main application, but a special
			// assembly just for the interaction.
			
			SharpDevelopInteraction.InteractionClass obj;
			obj = host.CreateInstanceInTargetDomain<SharpDevelopInteraction.InteractionClass>();
			
			obj.MakeTransparent();
		}
	}
}
