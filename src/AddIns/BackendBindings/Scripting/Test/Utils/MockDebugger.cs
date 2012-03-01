// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockDebugger : IDebugger
	{
		ProcessStartInfo processStartInfo;
		bool startMethodCalled;
		bool startWithoutDebuggingMethodCalled;
		bool stopMethodCalled;
		
		public MockDebugger()
		{
		}

		/// <summary>
		/// Gets the ProcessStartInfo passed to the Start or StartWithoutDebugging methods.
		/// </summary>
		public ProcessStartInfo ProcessStartInfo {
			get { return processStartInfo; }
		}
		
		public bool StartMethodCalled {
			get { return startMethodCalled; }
		}
		
		public bool StartWithoutDebuggingMethodCalled {
			get { return startWithoutDebuggingMethodCalled; }
		}

		public bool StopMethodCalled {
			get { return stopMethodCalled; }
		}
		
		public event EventHandler DebugStarting;
		public event EventHandler DebugStarted;
		public event EventHandler IsProcessRunningChanged;
		public event EventHandler DebugStopped;
		
		public bool IsDebugging {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsProcessRunning {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool CanDebug(IProject project)
		{
			throw new NotImplementedException();
		}
		
		public void Start(ProcessStartInfo processStartInfo)
		{
			this.processStartInfo = processStartInfo;
			startMethodCalled = true;
		}
		
		public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			this.processStartInfo = processStartInfo;
			startWithoutDebuggingMethodCalled = true;
		}
		
		public void Stop()
		{
			stopMethodCalled = true;
		}
		
		public void Break()
		{
			throw new NotImplementedException();
		}
		
		public void Continue()
		{
			throw new NotImplementedException();
		}
		
		public void StepInto()
		{
			throw new NotImplementedException();
		}
		
		public void StepOver()
		{
			throw new NotImplementedException();
		}
		
		public void StepOut()
		{
			throw new NotImplementedException();
		}
		
		public void ShowAttachDialog()
		{
			throw new NotImplementedException();
		}
		
		public void Attach(System.Diagnostics.Process process)
		{
			throw new NotImplementedException();
		}
		
		public void Detach()
		{
			throw new NotImplementedException();
		}
		
		public string GetValueAsString(string variable)
		{
			throw new NotImplementedException();
		}
		
		public object GetTooltipControl(Location logicalPosition, string variable)
		{
			throw new NotImplementedException();
		}
		
		public bool CanSetInstructionPointer(string filename, int line, int column)
		{
			throw new NotImplementedException();
		}
		
		public bool SetInstructionPointer(string filename, int line, int column)
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
		
		protected virtual void OnDebugStarting(EventArgs e)
		{
			if (DebugStarting != null) {
				DebugStarting(this, e);
			}
		}
		

		protected virtual void OnDebugStarted(EventArgs e)
		{
			if (DebugStarted != null) {
				DebugStarted(this, e);
			}
		}
		
		protected virtual void OnIsProcessRunningChanged(EventArgs e)
		{
			if (IsProcessRunningChanged != null) {
				IsProcessRunningChanged(this, e);
			}
		}

		protected virtual void OnDebugStopped(EventArgs e)
		{
			if (DebugStopped != null) {
				DebugStopped(this, e);
			}
		}		
		
		public bool BreakAtBeginning { get; set; }
		
		public bool IsAttached { get; set; }
	}
}
