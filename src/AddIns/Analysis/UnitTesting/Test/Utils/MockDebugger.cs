// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Project;

namespace UnitTesting.Tests.Utils
{
	public class MockDebugger : IDebugger
	{
		bool debugging;
		bool stopCalled;
		ProcessStartInfo processStartInfo;
		Exception exceptionToThrowOnStart;
		
		public event EventHandler DebugStarting;
		
		protected virtual void OnDebugStarting(EventArgs e)
		{
			if (DebugStarting != null) {
				DebugStarting(this, e);
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
		
		public void FireDebugStoppedEvent()
		{
			OnDebugStopped(new EventArgs());
		}
		
		protected virtual void OnDebugStopped(EventArgs e)
		{
			if (DebugStopped != null) {
				DebugStopped(this, e);
			}
		}
		
		public bool IsDebugging {
			get { return debugging; }
			set { debugging = value; }
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
			if (exceptionToThrowOnStart != null) {
				throw exceptionToThrowOnStart; 
			}
		}
		
		public ProcessStartInfo ProcessStartInfo {
			get { return processStartInfo; }
		}
		
		public Exception ThrowExceptionOnStart {
			get { return exceptionToThrowOnStart; }
			set { exceptionToThrowOnStart = value; }
		}
		
		public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			throw new NotImplementedException();
		}
		
		public void Stop()
		{
			stopCalled = true;
		}
		
		public bool IsStopCalled {
			get { return stopCalled; }
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
		
		public void Attach(Process process)
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
		
		public bool BreakAtBeginning { get; set; }
		public bool IsAttached { get; set; }
	}
}
