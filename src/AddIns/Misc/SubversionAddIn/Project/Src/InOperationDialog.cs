/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 22.11.2004
 * Time: 20:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of InOperationDialog.
	/// </summary>
	public class InOperationDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label operationNameLabel;
		
		Thread operation;
		public Thread Operation {
			get {
				return operation;
			}
			set {
				operation = value;
			}
		}
		
		System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
		
		public InOperationDialog(string operationName, Thread operation)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.operation = operation;
			
			operationNameLabel.Text = operationName;
			
			timer.Tick += new EventHandler(TimerTick);
			timer.Interval = 100;
			timer.Start();
			
		}
		
		void TimerTick(object myObject, EventArgs e)
		{
			if (progressBar1.Value + 1 < progressBar1.Maximum) {
				progressBar1.Value++;
			} else {
				progressBar1.Value = progressBar1.Minimum;
			}
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.operationNameLabel = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// operationNameLabel
			// 
			this.operationNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.operationNameLabel.Location = new System.Drawing.Point(8, 40);
			this.operationNameLabel.Name = "operationNameLabel";
			this.operationNameLabel.Size = new System.Drawing.Size(312, 23);
			this.operationNameLabel.TabIndex = 1;
			this.operationNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(248, 72);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.Location = new System.Drawing.Point(8, 8);
			this.progressBar1.Maximum = 20;
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(312, 23);
			this.progressBar1.TabIndex = 0;
			// 
			// InOperationDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(330, 104);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.operationNameLabel);
			this.Controls.Add(this.progressBar1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "InOperationDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Please wait...";
			this.ResumeLayout(false);
		}
		#endregion
		
		void CancelButtonClick(object sender, System.EventArgs e)
		{
			Close();
		}
		
		protected override void OnClosed(System.EventArgs ea)
		{
			base.OnClosed(ea);
			try {
				timer.Stop();
			} catch (Exception e) {
				Console.WriteLine(e);
			}
			
			try {
				if (operation != null && operation.IsAlive) {
					operation.Abort();
				}
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}
	}
}
