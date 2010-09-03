// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// project created on 2/6/2003 at 11:10 AM
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Sda
{
	/// <summary>
	/// Form used to display display unhandled errors in SharpDevelop.
	/// </summary>
	public class ExceptionBox : Form
	{
		private System.Windows.Forms.TextBox exceptionTextBox;
		private System.Windows.Forms.CheckBox copyErrorCheckBox;
		//private System.Windows.Forms.CheckBox includeSysInfoCheckBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Button continueButton;
		private System.Windows.Forms.Button reportButton;
		private System.Windows.Forms.PictureBox pictureBox;
		Exception exceptionThrown;
		string message;
		
		internal static void RegisterExceptionBoxForUnhandledExceptions()
		{
			Application.ThreadException += ShowErrorBox;
			AppDomain.CurrentDomain.UnhandledException += ShowErrorBox;
			System.Windows.Threading.Dispatcher.CurrentDispatcher.UnhandledException += Dispatcher_UnhandledException;
		}
		
		static void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			ShowErrorBox(e.Exception, "Unhandled WPF exception", false);
			e.Handled = true;
		}
		
		static void ShowErrorBox(object sender, ThreadExceptionEventArgs e)
		{
			LoggingService.Error("ThreadException caught", e.Exception);
			ShowErrorBox(e.Exception, null);
		}
		
		[SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
		static void ShowErrorBox(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			LoggingService.Fatal("UnhandledException caught", ex);
			if (e.IsTerminating)
				LoggingService.Fatal("Runtime is terminating because of unhandled exception.");
			ShowErrorBox(ex, "Unhandled exception", e.IsTerminating);
		}
		
		/// <summary>
		/// Displays the exception box.
		/// </summary>
		public static void ShowErrorBox(Exception exception, string message)
		{
			ShowErrorBox(exception, message, false);
		}
		
		[ThreadStatic]
		static bool showingBox;
		
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		static void ShowErrorBox(Exception exception, string message, bool mustTerminate)
		{
			if (exception == null)
				throw new ArgumentNullException("exception");
			
			// ignore reentrant calls (e.g. when there's an exception in OnRender)
			if (showingBox)
				return;
			showingBox = true;
			try {
				try {
					AnalyticsMonitorService.TrackException(exception);
				} catch (Exception ex) {
					LoggingService.Warn("Error tracking exception", ex);
				}
				using (ExceptionBox box = new ExceptionBox(exception, message, mustTerminate)) {
					if (ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.InvokeRequired)
						box.ShowDialog();
					else
						box.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window);
				}
			} catch (Exception ex) {
				LoggingService.Warn("Error showing ExceptionBox", ex);
				MessageBox.Show(exception.ToString(), message, MessageBoxButtons.OK,
				                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
			} finally {
				showingBox = false;
			}
		}
		
		/// <summary>
		/// Creates a new ExceptionBox instance.
		/// </summary>
		/// <param name="exception">The exception to display</param>
		/// <param name="message">An additional message to display</param>
		/// <param name="mustTerminate">If <paramref name="mustTerminate"/> is true, the
		/// continue button is not available.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public ExceptionBox(Exception exception, string message, bool mustTerminate)
		{
			this.exceptionThrown = exception;
			this.message = message;
			InitializeComponent();
			if (mustTerminate) {
				closeButton.Visible = false;
				continueButton.Text = closeButton.Text;
				continueButton.Left -= closeButton.Width - continueButton.Width;
				continueButton.Width = closeButton.Width;
			}
			
			try {
				Translate(this);
			} catch {}
			
			exceptionTextBox.Text = getClipboardString();
			
			try {
				this.pictureBox.Image = WinFormsResourceService.GetBitmap("ErrorReport");
			} catch {}
		}
		
		void Translate(Control ctl)
		{
			ctl.Text = StringParser.Parse(ctl.Text);
			foreach (Control child in ctl.Controls) {
				Translate(child);
			}
		}
		
		string getClipboardString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.Append(Gui.AboutSharpDevelopTabPage.GetVersionInformationString());
			
			sb.AppendLine();
			
			if (message != null) {
				sb.AppendLine(message);
			}
			sb.AppendLine("Exception thrown:");
			sb.AppendLine(exceptionThrown.ToString());
			sb.AppendLine();
			sb.AppendLine("---- Recent log messages:");
			try {
				LogMessageRecorder.AppendRecentLogMessages(sb, log4net.LogManager.GetLogger(typeof(log4netLoggingService)));
			} catch (Exception ex) {
				sb.AppendLine("Failed to append recent log messages.");
				sb.AppendLine(ex.ToString());
			}
			sb.AppendLine();
			sb.AppendLine("---- Post-error application state information:");
			try {
				ApplicationStateInfoService.AppendFormatted(sb);
			} catch (Exception ex) {
				sb.AppendLine("Failed to append application state information.");
				sb.AppendLine(ex.ToString());
			}
			return sb.ToString();
		}
		
		void CopyInfoToClipboard()
		{
			if (copyErrorCheckBox.Checked) {
				string exceptionText = exceptionTextBox.Text;
				if (Application.OleRequired() == ApartmentState.STA) {
					ClipboardWrapper.SetText(exceptionText);
				} else {
					Thread th = new Thread((ThreadStart)delegate {
					                       	ClipboardWrapper.SetText(exceptionText);
					                       });
					th.Name = "CopyInfoToClipboard";
					th.SetApartmentState(ApartmentState.STA);
					th.Start();
				}
			}
		}
		
		void buttonClick(object sender, System.EventArgs e)
		{
			CopyInfoToClipboard();
			
			StartUrl("http://www.icsharpcode.net/OpenSource/SD/BugReporting.aspx?version=" + RevisionClass.FullVersion);
		}
		
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		static void StartUrl(string url)
		{
			try {
				Process.Start(url);
			} catch (Exception e) {
				LoggingService.Warn("Cannot start " + url, e);
			}
		}
		
		void continueButtonClick(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Ignore;
			Close();
		}
		
		void CloseButtonClick(object sender, EventArgs e)
		{
			if (MessageBox.Show(StringParser.Parse("${res:ICSharpCode.SharpDevelop.ExceptionBox.QuitWarning}"), MessageService.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly)
			    == DialogResult.Yes)
			{
				Process.GetCurrentProcess().Kill();
			}
		}
		
		[SuppressMessage("Microsoft.Globalization", "CA1303")]
		void InitializeComponent()
		{
			this.closeButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label = new System.Windows.Forms.Label();
			this.continueButton = new System.Windows.Forms.Button();
			this.reportButton = new System.Windows.Forms.Button();
			this.copyErrorCheckBox = new System.Windows.Forms.CheckBox();
			this.exceptionTextBox = new System.Windows.Forms.TextBox();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// closeButton
			// 
			this.closeButton.Location = new System.Drawing.Point(445, 424);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(141, 23);
			this.closeButton.TabIndex = 5;
			this.closeButton.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.ExitSharpDevelop}";
			this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(230, 159);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(448, 23);
			this.label3.TabIndex = 9;
			this.label3.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.ThankYouMsg}";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(232, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(448, 95);
			this.label2.TabIndex = 8;
			this.label2.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.HelpText2}";
			// 
			// label
			// 
			this.label.Location = new System.Drawing.Point(232, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(448, 48);
			this.label.TabIndex = 6;
			this.label.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.HelpText1}";
			// 
			// continueButton
			// 
			this.continueButton.Location = new System.Drawing.Point(592, 424);
			this.continueButton.Name = "continueButton";
			this.continueButton.Size = new System.Drawing.Size(88, 23);
			this.continueButton.TabIndex = 6;
			this.continueButton.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.Continue}";
			this.continueButton.Click += new System.EventHandler(this.continueButtonClick);
			// 
			// reportButton
			// 
			this.reportButton.Location = new System.Drawing.Point(230, 424);
			this.reportButton.Name = "reportButton";
			this.reportButton.Size = new System.Drawing.Size(209, 23);
			this.reportButton.TabIndex = 4;
			this.reportButton.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.ReportError}";
			this.reportButton.Click += new System.EventHandler(this.buttonClick);
			// 
			// copyErrorCheckBox
			// 
			this.copyErrorCheckBox.Checked = true;
			this.copyErrorCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.copyErrorCheckBox.Location = new System.Drawing.Point(230, 373);
			this.copyErrorCheckBox.Name = "copyErrorCheckBox";
			this.copyErrorCheckBox.Size = new System.Drawing.Size(440, 24);
			this.copyErrorCheckBox.TabIndex = 2;
			this.copyErrorCheckBox.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.CopyToClipboard}";
			// 
			// exceptionTextBox
			// 
			this.exceptionTextBox.Location = new System.Drawing.Point(230, 183);
			this.exceptionTextBox.Multiline = true;
			this.exceptionTextBox.Name = "exceptionTextBox";
			this.exceptionTextBox.ReadOnly = true;
			this.exceptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.exceptionTextBox.Size = new System.Drawing.Size(448, 184);
			this.exceptionTextBox.TabIndex = 1;
			this.exceptionTextBox.Text = "textBoxExceptionText";
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(224, 464);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// ExceptionBox
			// 
			this.ClientSize = new System.Drawing.Size(688, 453);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label);
			this.Controls.Add(this.continueButton);
			this.Controls.Add(this.reportButton);
			this.Controls.Add(this.copyErrorCheckBox);
			this.Controls.Add(this.exceptionTextBox);
			this.Controls.Add(this.pictureBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExceptionBox";
			this.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.Title}";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button closeButton;
	}
}
