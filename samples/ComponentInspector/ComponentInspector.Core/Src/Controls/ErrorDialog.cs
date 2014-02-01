// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using NoGoop.Util;

namespace NoGoop.Controls
{
	public class ErrorDialog : Form
	{
		protected const int         MIN_WIDTH = 300;
		protected bool              _calculated;
		protected int               _maxWidth;
		protected RichTextBox       _text;
		protected Panel             _messagePanel;
		protected Label             _imageLabel;
		protected Button            _moreInfo;
		protected String            _title;
		protected String            _firstLine;
		protected String            _summary;
		protected String            _details;
		protected Exception         _exception;
		protected bool              _detailIsDisplayed;
		
		// Title is the title of the dialog
		// First line is the top line of the text; the title is used
		//  if firstLine is not present
		// summary is the text to be displayed with "less information"
		// details is the text to be displayed with "more information"
		//
		internal ErrorDialog(String firstLine,
							String summary,
							String details,
							String title,
							MessageBoxIcon iconType)
		{
			DockPadding.All = 15;
			StartPosition = FormStartPosition.CenterParent;
			FormBorderStyle = FormBorderStyle.Sizable;
			MinimizeBox = false;
			MaximizeBox = false;
			ControlBox = false;
			Icon = null;
			Text = title;
			_title = title;
			if (firstLine == null)
				_firstLine = title;
			else
				_firstLine = firstLine;
			_summary = summary;
			_details = details;
			_messagePanel = new Panel();
			_messagePanel.Dock = DockStyle.Fill;
			if (iconType != MessageBoxIcon.None) {
				_imageLabel = new Label();
				_imageLabel.Dock = DockStyle.Left;
				switch (iconType) {
					case MessageBoxIcon.Error:
						_imageLabel.Image = SystemIcons.Error.ToBitmap();
						break;
					case MessageBoxIcon.Warning:
						_imageLabel.Image = SystemIcons.Warning.ToBitmap();
						break;
					case MessageBoxIcon.Information:
						_imageLabel.Image = SystemIcons.Information.ToBitmap();
						break;
					default:
						throw new Exception("Invalid icon type");
				}
				_imageLabel.Height = 32;
				_imageLabel.Width = 42;
				_imageLabel.ImageAlign = ContentAlignment.TopLeft;
			}
			_detailIsDisplayed = false;
			SetText();
			Panel buttonPanel = new Panel();
			buttonPanel.Dock = DockStyle.Bottom;
			Button ok = Utils.MakeButton(StringParser.Parse("${res:Global.OKButtonText}"));
			ok.Dock = DockStyle.Right;
			ok.DialogResult = DialogResult.OK;
			buttonPanel.Height = Utils.BUTTON_HEIGHT;
			buttonPanel.Controls.Add(ok);
			if (_details != null) {
				_moreInfo = Utils.MakeButton(StringParser.Parse("${res:ComponentInspector.ErrorDialog.MoreInformationButton}"));
				_moreInfo.Dock = DockStyle.Right;
				_moreInfo.Click += new EventHandler(MoreInfoClickedHandler);
				buttonPanel.Height = Utils.BUTTON_HEIGHT;
				buttonPanel.Controls.Add(_moreInfo);
			}
			Button bug = Utils.MakeButton(StringParser.Parse("${res:ComponentInspector.ErrorDialog.ReportAsBugButton}"));
			bug.Dock = DockStyle.Right;
			bug.Click += new EventHandler(BugClickedHandler);
			buttonPanel.Height = Utils.BUTTON_HEIGHT;
			buttonPanel.Controls.Add(bug);
			AcceptButton = ok;
			Label pad = new Label();
			pad.Dock = DockStyle.Bottom;
			pad.Height = 5;
			Controls.Add(pad);
			Controls.Add(buttonPanel);
			Controls.Add(_messagePanel);
		}
		
		protected void SetText()
		{
			_messagePanel.Controls.Clear();
			_maxWidth = MIN_WIDTH;
			// FIXME - probably factor this with some of the code
			// from the DetailPanel
			_text = new RichTextBox();
			_text.Dock = DockStyle.Fill;
			_text.Text = _firstLine;
			if (_detailIsDisplayed)
				_text.Text += "\n\n" + _summary + "\n\n" + _details;
			else
				_text.Text += "\n\n" + _summary;
			_text.BorderStyle = BorderStyle.None;
			_text.BackColor = BackColor;
			_text.ReadOnly = true;
			_text.Multiline = true;
			_text.WordWrap = true;
			_text.LinkClicked += new LinkClickedEventHandler(LinkClicked);
			// Set dialog to be as long as the longest line, up to a limit
			foreach (String line in _text.Lines) {
				_maxWidth = Utils.SetMaxWidth(_text, line, _maxWidth);
			}
			if (_maxWidth > 630)
				_maxWidth = 630;
			Width = _maxWidth + 80;
			_calculated = false;
			_text.Layout += new LayoutEventHandler(TextLayoutHandler);
			_messagePanel.Controls.Add(_text);
			_messagePanel.Controls.Add(_imageLabel);
		}
		
		protected static String ExceptionSummary(Exception exception, out String details)
		{
			StringBuilder ret = new StringBuilder();
			StringBuilder outDetails = new StringBuilder();
			while (true) {
				outDetails.Append(exception.Message);
				outDetails.Append("\n");
				outDetails.Append("  ");
				outDetails.Append(exception.GetType().ToString());
				outDetails.Append("\n");
				if (exception is ExternalException) {
					ExternalException ee = (ExternalException)exception;
					if (ee.ErrorCode != 0) {
						outDetails.Append("  0x");
						outDetails.Append(ee.ErrorCode.ToString("X"));
						outDetails.Append("\n");
					}
				}
				if (exception is Win32Exception) {
					Win32Exception ee = (Win32Exception)exception;
					if (ee.NativeErrorCode != 0) {
						outDetails.Append("  Win32: 0x");
						outDetails.Append(ee.NativeErrorCode.ToString("X"));
						outDetails.Append("\n");
					}
				}
				if (exception.HelpLink != null) {
					outDetails.Append("  ");
					outDetails.Append(exception.HelpLink);
					outDetails.Append("\n");
				}
				if (exception.Source != null) {
					outDetails.Append("  ");
					outDetails.Append(exception.Source);
					outDetails.Append("\n");
				}
				if (exception.TargetSite != null) {
					outDetails.Append("  ");
					outDetails.Append(exception.TargetSite);
					outDetails.Append("\n");
				}
				if (exception.StackTrace != null) {
					outDetails.Append(exception.StackTrace);
					outDetails.Append("\n");
				}
				ret.Append(exception.Message);
				exception = exception.InnerException;
				if (exception == null)
					break;
				outDetails.Append("\n");
				ret.Append("\n\n");
			}
			details = outDetails.ToString();
			return ret.ToString();
		}
		
		public static void Show(Exception exception)
		{
			Show(exception, null, MessageBoxIcon.None);
		}
		
		public static void Show(Exception exception, String title)
		{
			Show(exception, title, MessageBoxIcon.None);
		}
		
		public static void Show(Exception exception, String title, MessageBoxIcon iconType) 
		{
			Show(exception, null, title, iconType);
		}
		
		public static void Show(String errorText, String title)
		{
			Show(errorText, title, MessageBoxIcon.None);
		}
		
		public static void Show(String errorText)
		{
			Show(errorText, String.Empty);
		}
		
		public static void Show(Exception exception, String firstLine, String title, MessageBoxIcon iconType) 
		{
			String details;
			String summary = ExceptionSummary(exception, out details);
			using (ErrorDialog errorDialog = new ErrorDialog(firstLine, summary, details, title, iconType)) {
				errorDialog._exception = exception;
				errorDialog.ShowDialog();
			}
		}

		public static void Show(String errorText, String title, MessageBoxIcon iconType) 
		{
			using (ErrorDialog errorDialog = new ErrorDialog(null, errorText, null, title, iconType)) {
				errorDialog.ShowDialog();
			}
		}
		
		protected void TextLayoutHandler(object sender, LayoutEventArgs e)
		{
			// Use this to calculate only the initial height
			if (!_calculated) {
				RichTextBox tb = (RichTextBox)sender;
				tb.Width = _maxWidth;
				// Get number of actual lines the text box needs 
				// given the current width
				int lines = (1 + tb.GetLineFromCharIndex(tb.TextLength));
				tb.Height = tb.PreferredHeight * lines;
				Height = tb.Height + 150;
				if (Height > 800)
					Height = 800;
				_calculated = true;
			}
		}
		
		protected void MoreInfoClickedHandler(object sender, EventArgs e)
		{
			_detailIsDisplayed = !_detailIsDisplayed;
			if (_detailIsDisplayed)
				_moreInfo.Text = StringParser.Parse("${res:ComponentInspector.ErrorDialog.LessInformationButton}");
			else
				_moreInfo.Text = StringParser.Parse("${res:ComponentInspector.ErrorDialog.MoreInformationButton}");
			SetText();
		}
		
		protected void BugClickedHandler(object sender, EventArgs e)
		{
			try {
				CopyInfoToClipboard();
				string uriText = "http://www.icsharpcode.net/OpenSource/SD/BugReporting.aspx";
				Uri uri = new Uri(uriText);
				Utils.InvokeBrowser(uri, !Utils.SHOW_WINDOW);
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning(this, "Error sending bug report: " + ex);
			}
		}
		
		protected void LinkClicked(object sender, LinkClickedEventArgs e)
		{
			Utils.InvokeBrowser(e.LinkText);
		}
		
		void CopyInfoToClipboard()
		{
			SD.Clipboard.SetText(GetClipboardString());
		}
		
		string GetClipboardString()
		{
			string str = String.Empty;
			str += ".NET Version : " + Environment.Version.ToString() + Environment.NewLine;
			str += "OS Version : " + Environment.OSVersion.ToString() + Environment.NewLine;
			System.Version v = Assembly.GetEntryAssembly().GetName().Version;
			str += "Component Inspector Version : " + v.Major + "." + v.Minor + "." + v.Build + "." + v.Revision + Environment.NewLine;
			str += Environment.NewLine;
			
			if (_exception != null) {
				str += _exception + Environment.NewLine;
			} else {
				str += _summary + Environment.NewLine;
			}
			return str;
		}
	}
}
