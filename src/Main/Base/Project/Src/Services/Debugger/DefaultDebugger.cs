using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core 
{
	public class DefaultDebugger : IDebugger
	{
		System.Diagnostics.Process attachedProcess = null;
		
		public bool IsDebugging {
			get {
				return IsProcessRunning;
			}
		}
		
		public bool IsProcessRunning {
			get {
				return attachedProcess != null;
			}
		}
		
		public bool SupportsStartStop {
			get {
				return true;
			}
		}
		
		public bool SupportsExecutionControl {
			get {
				return false;
			}
		}
		
		public bool SupportsStepping {
			get {
				return false;
			}
		}
		
		public bool CanDebug(IProject project)
		{
			return true;
		}
		
		public void Start(string fileName, string workingDirectory, string arguments)
		{
			if (attachedProcess != null) {
				return;
			}

			System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
			psi.FileName = fileName;
			psi.WorkingDirectory = workingDirectory;
			psi.Arguments = arguments;

			try {
				attachedProcess = new System.Diagnostics.Process();
				attachedProcess.StartInfo = psi;
				attachedProcess.Exited += new EventHandler(AttachedProcessExited);
				attachedProcess.EnableRaisingEvents = true;
				attachedProcess.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + psi.FileName + "\"\n");
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
		
		#region System.IDisposable interface implementation
		public void Dispose() 
		{
			if (attachedProcess != null) {
				attachedProcess.Exited -= new EventHandler(AttachedProcessExited);
				attachedProcess.Kill();
				attachedProcess.Close();
				attachedProcess.Dispose();
				attachedProcess = null;
			}
		}
		#endregion
		
		// ExecutionControl:
		
		public void Break()
		{
			throw new NotSupportedException();
		}
		
		public void Continue()
		{
			throw new NotSupportedException();
		}

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
		
		void AttachedProcessExited(object sender, EventArgs e)
		{
			attachedProcess.Exited -= new EventHandler(AttachedProcessExited);
			attachedProcess.Dispose();
			attachedProcess = null;	
			OnDebugStopped(EventArgs.Empty);
		}
		
		protected virtual void OnDebugStopped(EventArgs e) 
		{
			if (DebugStopped != null) {
				DebugStopped(this, e);
			}
		}
		
		public event EventHandler DebugStopped;
		
		/// <summary>
		/// Gets the current value of the variable as string that can be displayed in tooltips.
		/// </summary>
		public string GetValueAsString(string variable)
		{
			return null;
		}
	}
}
