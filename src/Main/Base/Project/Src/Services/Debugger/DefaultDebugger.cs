// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using System.Diagnostics;

namespace ICSharpCode.Core 
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
		
		public bool CanDebug(IProject project)
		{
			return true;
		}
		
		public void Start(ProcessStartInfo processStartInfo)
		{
			if (attachedProcess != null) {
				return;
			}

			try {
				attachedProcess = new Process();
				attachedProcess.StartInfo = processStartInfo;
				attachedProcess.Exited += new EventHandler(AttachedProcessExited);
				attachedProcess.EnableRaisingEvents = true;
				attachedProcess.Start();
				OnDebugStarted(EventArgs.Empty);
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + processStartInfo.FileName + "\"\n");
			}
		}

		void AttachedProcessExited(object sender, EventArgs e)
		{
			attachedProcess.Exited -= new EventHandler(AttachedProcessExited);
			attachedProcess.Dispose();
			attachedProcess = null;	
			OnDebugStopped(EventArgs.Empty);
		}

		public bool SupportsStart {
			get {
				return true;
			}
		}

		public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			Process process;
			process = new Process();
			process.StartInfo = processStartInfo;
			process.Start();
		}

		public bool SupportsStartWithoutDebugging {
			get {
				return false;
			}
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

		public bool SupportsStop {
			get {
				return true;
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

		public bool SupportsExecutionControl {
			get {
				return false;
			}
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

		public bool SupportsStepping {
			get {
				return false;
			}
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
		
		/// <summary>
		/// Gets the current value of the variable as string that can be displayed in tooltips.
		/// </summary>
		public string GetValueAsString(string variable)
		{
			return null;
		}

		public void Dispose() 
		{
			Stop();
		}
	}
}
