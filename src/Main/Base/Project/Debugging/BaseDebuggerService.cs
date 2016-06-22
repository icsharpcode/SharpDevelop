// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public abstract class BaseDebuggerService : IDebuggerService
	{
		protected BaseDebuggerService()
		{
			SD.ProjectService.SolutionOpened += delegate {
				ClearDebugMessages();
			};
			SD.ProjectService.SolutionClosing += OnSolutionClosing;
		}
		
		public virtual IDebuggerOptions Options {
			get { return DummyDebuggerOptions.Instance; }
		}

		public virtual void Dispose()
		{
			SD.ProjectService.SolutionClosing -= OnSolutionClosing;
		}

		bool debuggerStarted;

		/// <summary>
		/// Gets whether the debugger is currently active.
		/// </summary>
		public bool IsDebuggerStarted {
			get {
				return debuggerStarted;
			}
		}

		public event EventHandler DebugStarted;

		protected virtual void OnDebugStarted(EventArgs e)
		{
			debuggerStarted = true;
			if (DebugStarted != null) {
				DebugStarted(this, e);
			}
		}

		IAnalyticsMonitorTrackedFeature debugFeature;

		public event EventHandler IsProcessRunningChanged;

		protected virtual void OnIsProcessRunningChanged(EventArgs e)
		{
			if (IsProcessRunningChanged != null) {
				IsProcessRunningChanged(this, e);
			}
		}

		public event EventHandler DebugStopped;

		protected virtual void OnDebugStopped(EventArgs e)
		{
			debuggerStarted = false;
			if (debugFeature != null)
				debugFeature.EndTracking();
			RemoveCurrentLineMarker();
			SD.Workbench.CurrentLayoutConfiguration = "Default";
			if (DebugStopped != null) {
				DebugStopped(this, e);
			}
		}

		public event EventHandler DebugStarting;

		protected virtual void OnDebugStarting(EventArgs e)
		{
			SD.Workbench.CurrentLayoutConfiguration = "Debug";
			debugFeature = SD.AnalyticsMonitor.TrackFeature("Debugger");
			ClearDebugMessages();
			if (DebugStarting != null)
				DebugStarting(null, e);
		}

		void OnSolutionClosing(object sender, SolutionClosingEventArgs e)
		{
			if (IsDebugging) {
				if (!e.AllowCancel) {
					Stop();
					return;
				}
				string caption = StringParser.Parse("${res:XML.MainMenu.DebugMenu.Stop}");
				string message = StringParser.Parse("${res:MainWindow.Windows.Debug.StopDebugging.Message}");
				string[] buttonLabels = new string[] {
					StringParser.Parse("${res:Global.Yes}"),
					StringParser.Parse("${res:Global.No}")
				};
				int result = MessageService.ShowCustomDialog(caption, message, 0, // yes
				1, // no
				buttonLabels);
				if (result == 0) {
					Stop();
				} else {
					e.Cancel = true;
				}
			}
		}

		public abstract bool CanDebug(IProject project);

		public abstract bool Supports(DebuggerFeatures feature);

		public abstract void Start(ProcessStartInfo processStartInfo);

		public abstract void StartWithoutDebugging(ProcessStartInfo processStartInfo);

		public abstract void Stop();

		public abstract void Break();

		public abstract void Continue();

		public abstract void StepInto();

		public abstract void StepOver();

		public abstract void StepOut();

		public abstract void ShowAttachDialog();

		public abstract void Attach(Process process);

		public abstract void Detach();

		public abstract bool SetInstructionPointer(string filename, int line, int column, bool dryRun);

		public virtual bool IsDebuggerLoaded {
			get {
				return true;
			}
		}

		public abstract bool IsDebugging {
			get;
		}

		public abstract bool IsProcessRunning {
			get;
		}

		public abstract bool BreakAtBeginning {
			get;
			set;
		}

		public abstract bool IsAttached {
			get;
		}

		public abstract void HandleToolTipRequest(ToolTipRequestEventArgs e);

		static MessageViewCategory debugCategory = null;

		static void EnsureDebugCategory()
		{
			if (debugCategory == null) {
				MessageViewCategory.Create(ref debugCategory, "Debug", "${res:MainWindow.Windows.OutputWindow.DebugCategory}");
			}
		}

		public static void ClearDebugMessages()
		{
			EnsureDebugCategory();
			debugCategory.ClearText();
		}

		public static void PrintDebugMessage(string msg)
		{
			EnsureDebugCategory();
			debugCategory.AppendText(msg);
		}

		public abstract void ToggleBreakpointAt(ITextEditor editor, int lineNumber);

		public abstract void RemoveCurrentLineMarker();

		public virtual void JumpToCurrentLine(string sourceFullFilename, int startLine, int startColumn, int endLine, int endColumn)
		{
			FileService.JumpToFilePosition(sourceFullFilename, startLine, startColumn);
		}
	}
}


