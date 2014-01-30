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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public class DefaultDebugger : IDebugger
	{
		Process attachedProcess = null;
		
		public bool IsDebugging {
			get {
				return attachedProcess != null;
			}
		}
		
		public bool IsProcessRunning {
			get {
				return IsDebugging;
			}
		}
		
		/// <inheritdoc/>
		public bool BreakAtBeginning {
			get; set; 
		}
		
		public bool CanDebug(IProject project)
		{
			return true;
		}
		
		public void Start(ProcessStartInfo processStartInfo)
		{
			if (attachedProcess != null) {
				return;
			}
			
			OnDebugStarting(EventArgs.Empty);
			try {
				attachedProcess = new Process();
				attachedProcess.StartInfo = processStartInfo;
				attachedProcess.Exited += new EventHandler(AttachedProcessExited);
				attachedProcess.EnableRaisingEvents = true;
				attachedProcess.Start();
				OnDebugStarted(EventArgs.Empty);
			} catch (Exception) {
				OnDebugStopped(EventArgs.Empty);
				throw new ApplicationException("Can't execute \"" + processStartInfo.FileName + "\"\n");
			}
		}
		
		public void ShowAttachDialog()
		{
		}
		
		public void Attach(Process process)
		{
		}
		
		public void Detach()
		{
		}
		
		void AttachedProcessExited(object sender, EventArgs e)
		{
			attachedProcess.Exited -= new EventHandler(AttachedProcessExited);
			attachedProcess.Dispose();
			attachedProcess = null;
			SD.MainThread.InvokeAsyncAndForget(() => new Action<EventArgs>(OnDebugStopped)(EventArgs.Empty));
		}
		
		public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			Process.Start(processStartInfo);
		}
		
		public void Stop()
		{
			if (attachedProcess != null) {
				attachedProcess.Exited -= new EventHandler(AttachedProcessExited);
				attachedProcess.Kill();
				attachedProcess.Close();
				attachedProcess.Dispose();
				attachedProcess = null;
			}
		}
		
		// ExecutionControl:
		
		public void Break()
		{
			throw new NotSupportedException();
		}
		
		public void Continue()
		{
			throw new NotSupportedException();
		}
		// Stepping:
		
		public void StepInto()
		{
			throw new NotSupportedException();
		}
		
		public void StepOver()
		{
			throw new NotSupportedException();
		}
		
		public void StepOut()
		{
			throw new NotSupportedException();
		}
		
		public void HandleToolTipRequest(ToolTipRequestEventArgs e)
		{
		}
		
		public bool SetInstructionPointer(string filename, int line, int column, bool dryRun)
		{
			return false;
		}
		
		
		public event EventHandler DebugStarted;
		
		protected virtual void OnDebugStarted(EventArgs e)
		{
			if (DebugStarted != null) {
				DebugStarted(this, e);
			}
		}


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
			if (DebugStopped != null) {
				DebugStopped(this, e);
			}
		}
		
		public event EventHandler DebugStarting;
		
		protected virtual void OnDebugStarting(EventArgs e)
		{
			if (DebugStarting != null) {
				DebugStarting(this, e);
			}
		}
		
		public void Dispose()
		{
			Stop();
		}
		
		public bool IsAttached {
			get {
				return false;
			}
		}
	}
}
