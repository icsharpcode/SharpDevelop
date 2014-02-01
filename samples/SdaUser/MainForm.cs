// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Sda;

namespace SdaUser
{
	public partial class MainForm
	{
		#region Application Startup
		// The LoaderOptimization hint is important - without it, loading WPF into the SD AppDomain takes very long
		[STAThread]
		[LoaderOptimization(LoaderOptimization.MultiDomainHost)]
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
		#endregion
		
		#region RunWorkbench
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
				startup.ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ICSharpCode\\HostedSharpDevelop4");
				startup.DataDirectory = Path.Combine(Path.GetDirectoryName(typeof(SharpDevelopHost).Assembly.Location), "../data");
				string sdaAddInDir = Path.Combine(Path.GetDirectoryName(typeof(MainForm).Assembly.Location), "SdaAddIns");
				startup.AddAddInsFromDirectory(sdaAddInDir);
				
				host = new SharpDevelopHost(startup);
				host.InvokeTarget = this;
				host.BeforeRunWorkbench += delegate { groupBox1.Enabled = true; };
				host.WorkbenchClosed += delegate { groupBox1.Enabled = false; };
			}
			
			host.RunWorkbench(wbSettings);
		}
		#endregion
		
		#region MainForm Events
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
		#endregion
		
		void UnloadHostDomainButtonClick(object sender, System.EventArgs e)
		{
			unloadHostDomainButton.Enabled = false;
			host.UnloadDomain();
			host = null;
			
			// disable the group box so no events are fired
			groupBox1.Enabled = false;
			runButton.Enabled = true;
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
