// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

// project created on 2/6/2003 at 11:10 AM
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;

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
			MessageService.CustomErrorReporter = ShowErrorBox;
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
		
		static void ShowErrorBox(Exception exception, string message)
		{
			ShowErrorBox(exception, message, false);
		}
		
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		static void ShowErrorBox(Exception exception, string message, bool mustTerminate)
		{
			try {
				using (ExceptionBox box = new ExceptionBox(exception, message, mustTerminate)) {
					try {
						box.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
					} catch (InvalidOperationException) {
						box.ShowDialog();
					}
				}
			} catch (Exception ex) {
				LoggingService.Warn("Error showing ExceptionBox", ex);
				MessageBox.Show(exception.ToString(), message, MessageBoxButtons.OK,
				                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
			}
		}
		
		/// <summary>
		/// Creates a new ExceptionBox instance.
		/// </summary>
		/// <param name="e">The exception to display</param>
		/// <param name="message">An additional message to display</param>
		/// <param name="mustTerminate">If <paramref name="mustTerminate"/> is true, the
		/// continue button is not available.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public ExceptionBox(Exception e, string message, bool mustTerminate)
		{
			this.exceptionThrown = e;
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
				this.pictureBox.Image = ResourceService.GetBitmap("ErrorReport");
			} catch {}
		}
		
		void Translate(Control ctl)
		{
			ctl.Text = StringParser.Parse(ctl.Text);
			foreach (Control child in ctl.Controls) {
				Translate(child);
			}
		}
		
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		string getClipboardString()
		{
			string str = "";
			Version v = typeof(ExceptionBox).Assembly.GetName().Version;
			str += "SharpDevelop Version : " + v.ToString() + Environment.NewLine;
			str += ".NET Version         : " + Environment.Version.ToString() + Environment.NewLine;
			str += "OS Version           : " + Environment.OSVersion.ToString() + Environment.NewLine;
			string cultureName = null;
			try {
				cultureName = CultureInfo.CurrentCulture.Name;
				str += "Current culture      : " + CultureInfo.CurrentCulture.EnglishName + " (" + cultureName + ")" + Environment.NewLine;
			} catch {}
			try {
				if (cultureName == null || !cultureName.StartsWith(ResourceService.Language)) {
					str += "Current UI language  : " + ResourceService.Language + Environment.NewLine;
				}
			} catch {}
			try {
				if (IntPtr.Size != 4) {
					str += "Running as " + (IntPtr.Size * 8) + " bit process" + Environment.NewLine;
				}
				if (SystemInformation.TerminalServerSession) {
					str += "Terminal Server Session" + Environment.NewLine;
				}
				if (SystemInformation.BootMode != BootMode.Normal) {
					str += "Boot Mode            : " + SystemInformation.BootMode + Environment.NewLine;
				}
			} catch {}
			str += "Working Set Memory   : " + (Environment.WorkingSet / 1024) + "kb" + Environment.NewLine;
			str += "GC Heap Memory       : " + (GC.GetTotalMemory(false) / 1024) + "kb" + Environment.NewLine;
			
			str += Environment.NewLine;
			
			if (message != null) {
				str += message + Environment.NewLine;
			}
			str += "Exception thrown: " + Environment.NewLine;
			str += exceptionThrown.ToString();
			return str;
		}
		
		void CopyInfoToClipboard()
		{
			if (copyErrorCheckBox.Checked) {
				if (Application.OleRequired() == ApartmentState.STA) {
					ClipboardWrapper.SetText(getClipboardString());
				} else {
					Thread th = new Thread((ThreadStart)delegate {
					                       	ClipboardWrapper.SetText(getClipboardString());
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
			
			StartUrl("http://www.icsharpcode.net/OpenSource/SD/BugReporting.aspx?version=" + RevisionClass.Version + "." + RevisionClass.Revision);
			
			/*
			string text = "This version of SharpDevelop is an internal build, " +
				"not a released version.\n" +
				"Please report problems in the internal builds to the " +
				"SVN-SharpDevelop-Users mailing list.";
			
			int result = MessageService.ShowCustomDialog("SharpDevelop", text,
			                                             "Join the list", "Write mail", "Cancel");
			if (result == 0) {
				StartUrl("http://www.glengamoi.com/mailman/listinfo/icsharpcode.svn-sharpdevelop-users");
			} else if (result == 1) {
				// clipboard text is too long to be inserted into the mail-url
				string exceptionTitle = "";
				Exception ex = exceptionThrown;
				if (ex != null) {
					try {
						while (ex.InnerException != null) ex = ex.InnerException;
						exceptionTitle = " (" + ex.GetType().Name + ")";
					} catch {}
				}
				string url = "mailto:icsharpcode.svn-sharpdevelop-users@glengamoi.com?subject=Bug Report"
					+ Uri.EscapeDataString(exceptionTitle)
					+ "&body="
					+ Uri.EscapeDataString("Write an English description on how to reproduce the error and paste the exception text.");
				StartUrl(url);
			}
			 */
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
				Application.Exit();
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
