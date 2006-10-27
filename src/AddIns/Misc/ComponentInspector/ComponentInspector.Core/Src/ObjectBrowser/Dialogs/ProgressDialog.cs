// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using NoGoop.Util;

namespace NoGoop.ObjBrowser.Dialogs
{
	internal class ProgressDialog : Dialog
	{
		protected ProgressBar           _progress;
		protected RichTextBox           _textBox;
		protected Label                 _label;
		protected bool                  _done;

		// Parameters
		protected String                _title;
		protected String                _bodyText;
		protected int                   _maxValue;
		protected bool                  _hasProgressText;
		protected bool                  _final;

		protected Cursor                _saveCursor;

		// Indicates this should be considered finished when the
		// end is reached on this progress bar.  If this is false,
		// the progress dialog is still kept up because it expects
		// more work to do.
		internal const bool              FINAL = true;

		internal const bool              HAS_PROGRESS_TEXT = true;

		// Used for the maxValue
		internal const int               NO_PROGRESS_BAR = -1;

		public ProgressDialog() : base(!INCLUDE_BUTTONS)
		{
			Height = 100;
			Width = 450;

			// Can't close me
			ControlBox = false;
			
			StartPosition = FormStartPosition.Manual;

			/****
				This does not seem to work
			TopLevel = false;
			TopMost = true;
			Parent = ObjectBrowser.ObjBrowser;
			StartPosition = FormStartPosition.CenterParent;
			****/

			// Have to manually compute location based on parent's location
			if (ObjectBrowserForm.Instance != null) {
				Size parentSize = ObjectBrowserForm.Instance.Size;
				Point loc = ObjectBrowserForm.Instance.DesktopLocation;
				
				Point ourLoc = new Point();
				ourLoc.Y = loc.Y + (int)((parentSize.Height - Size.Height) / 2);
				ourLoc.X = loc.X + (int)((parentSize.Width - Size.Width) / 2);
	
				Location = ourLoc;
				DesktopLocation = ourLoc;
			}
		}

		public void Setup(String title,
						  String bodyText,
						  int maxValue,
						  bool hasProgressText,
						  bool final)
		{
			TraceUtil.WriteLineInfo(this, "Progress Setup " 
									   + bodyText + " max: " + maxValue);

			// When transitioning from phase to phase, if this is the 
			// last phase, the maxValue might be zero
			if (maxValue == 0 && final)
			{
				TraceUtil.WriteLineInfo(this, "Setup final close");
				Finished();
				return;
			}

			_title = title;
			_bodyText = bodyText;
			_maxValue = maxValue;
			_hasProgressText = hasProgressText;
			_final = final;

			// We are resetting, use the existing controls
			if (Controls.Count > 0)
			{
				_done = false;
				_textBox.Text = _bodyText;
				_progress.Value = 0;
				_progress.Maximum = _maxValue;
				if (_label != null)
					_label.Text = "";
				TraceUtil.WriteLineInfo(this, "Using existing controls");
				return;
			}

			Text = _title;

			if (_hasProgressText)
			{
				_label = new Label();
				_label.Dock = DockStyle.Bottom;
				_label.Text = "";
				// Give space for two lines incase it wraps
				_label.Height = 30;
				Controls.Add(_label);
			}

			if (_maxValue != NO_PROGRESS_BAR)
			{
				_progress = new ProgressBar();
				_progress.Maximum = _maxValue;
				_progress.Dock = DockStyle.Top;
				Controls.Add(_progress);
				Height += 80;
			}

			_textBox = Utils.MakeDescText(_bodyText, this);
			_textBox.Dock = DockStyle.Top;
			Controls.Add(_textBox);

		}
		
		internal void UpdateProgress(int increment)
		{
			_progress.Value += increment;
			
			TraceUtil.WriteLineVerbose(this, 
									   Thread.CurrentThread.Name 
									   + " Update progress: "
									   + increment 
									   + " current: " 
									   + _progress.Value);
			if (_progress.Value >= _progress.Maximum &&
				_final)
			{
				TraceUtil.WriteLineInfo(this, Thread.CurrentThread.Name 
										+ " Update progress done");
				Finished();
			}
		}

		internal void UpdateProgressText(String text)
		{
			_label.Text = text;
		}


		internal void Finished()
		{
			lock (this)
			{
				TraceUtil.WriteLineInfo(this, "Finished - enter");
				_done = true;
				Cursor.Current = _saveCursor;
			}

			TraceUtil.WriteLineInfo
				(this, "Finished - doing close");

			try
			{
				Close();
			}
			catch (Exception ex)
			{
				TraceUtil.WriteLineWarning
					(this, "Finished - exception on close " + ex);
			}
			TraceUtil.WriteLineInfo
				(this, "Finished - after close");

		}

		internal void ShowIfNotDone()
		{
			TraceUtil.WriteLineInfo(this, "Progress - Show if not done");
			lock (this) {
				if (_done)
					return;
				TraceUtil.WriteLineInfo(this, "Progress - Showing");

				// Set hourglass cursor
				_saveCursor = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;
			}

			// Setup and add the controls on the same thread
			// as the one we show it
			TraceUtil.WriteLineInfo(this, "Progress ShowThread - setup complete");
			
			Show();

			// Make sure text and all associated with the dialog 
			// is present
			Application.DoEvents();
		}
	}
}
