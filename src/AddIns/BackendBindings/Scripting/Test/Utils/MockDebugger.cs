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

//using System;
//using System.Diagnostics;
//using ICSharpCode.NRefactory;
//using ICSharpCode.SharpDevelop.Debugging;
//using ICSharpCode.SharpDevelop.Project;
//
//namespace ICSharpCode.Scripting.Tests.Utils
//{
//	public class MockDebugger : IDebugger
//	{
//		ProcessStartInfo processStartInfo;
//		bool startMethodCalled;
//		bool startWithoutDebuggingMethodCalled;
//		bool stopMethodCalled;
//		
//		public MockDebugger()
//		{
//		}
//
//		/// <summary>
//		/// Gets the ProcessStartInfo passed to the Start or StartWithoutDebugging methods.
//		/// </summary>
//		public ProcessStartInfo ProcessStartInfo {
//			get { return processStartInfo; }
//		}
//		
//		public bool StartMethodCalled {
//			get { return startMethodCalled; }
//		}
//		
//		public bool StartWithoutDebuggingMethodCalled {
//			get { return startWithoutDebuggingMethodCalled; }
//		}
//
//		public bool StopMethodCalled {
//			get { return stopMethodCalled; }
//		}
//		
//		public event EventHandler DebugStarting;
//		public event EventHandler DebugStarted;
//		public event EventHandler IsProcessRunningChanged;
//		public event EventHandler DebugStopped;
//		
//		public bool IsDebugging {
//			get {
//				throw new NotImplementedException();
//			}
//		}
//		
//		public bool IsProcessRunning {
//			get {
//				throw new NotImplementedException();
//			}
//		}
//		
//		public bool CanDebug(IProject project)
//		{
//			throw new NotImplementedException();
//		}
//		
//		public void Start(ProcessStartInfo processStartInfo)
//		{
//			this.processStartInfo = processStartInfo;
//			startMethodCalled = true;
//		}
//		
//		public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
//		{
//			this.processStartInfo = processStartInfo;
//			startWithoutDebuggingMethodCalled = true;
//		}
//		
//		public void Stop()
//		{
//			stopMethodCalled = true;
//		}
//		
//		public void Break()
//		{
//			throw new NotImplementedException();
//		}
//		
//		public void Continue()
//		{
//			throw new NotImplementedException();
//		}
//		
//		public void StepInto()
//		{
//			throw new NotImplementedException();
//		}
//		
//		public void StepOver()
//		{
//			throw new NotImplementedException();
//		}
//		
//		public void StepOut()
//		{
//			throw new NotImplementedException();
//		}
//		
//		public void ShowAttachDialog()
//		{
//			throw new NotImplementedException();
//		}
//		
//		public void Attach(System.Diagnostics.Process process)
//		{
//			throw new NotImplementedException();
//		}
//		
//		public void Detach()
//		{
//			throw new NotImplementedException();
//		}
//		
//		public string GetValueAsString(string variable)
//		{
//			throw new NotImplementedException();
//		}
//		
//		public object GetTooltipControl(TextLocation logicalPosition, string variable)
//		{
//			throw new NotImplementedException();
//		}
//		
//		public bool SetInstructionPointer(string filename, int line, int column, bool dryRun)
//		{
//			throw new NotImplementedException();
//		}
//		
//		public void Dispose()
//		{
//			throw new NotImplementedException();
//		}
//		
//		protected virtual void OnDebugStarting(EventArgs e)
//		{
//			if (DebugStarting != null) {
//				DebugStarting(this, e);
//			}
//		}
//		
//
//		protected virtual void OnDebugStarted(EventArgs e)
//		{
//			if (DebugStarted != null) {
//				DebugStarted(this, e);
//			}
//		}
//		
//		protected virtual void OnIsProcessRunningChanged(EventArgs e)
//		{
//			if (IsProcessRunningChanged != null) {
//				IsProcessRunningChanged(this, e);
//			}
//		}
//
//		protected virtual void OnDebugStopped(EventArgs e)
//		{
//			if (DebugStopped != null) {
//				DebugStopped(this, e);
//			}
//		}		
//		
//		public bool BreakAtBeginning { get; set; }
//		
//		public bool IsAttached { get; set; }
//	}
//}
