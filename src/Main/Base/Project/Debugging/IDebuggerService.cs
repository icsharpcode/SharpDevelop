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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Debugging
{
	[SDService("SD.Debugger", FallbackImplementation = typeof(DebuggerServiceFallback))]
	public interface IDebuggerService : IDisposable, ITextAreaToolTipProvider
	{
		/// <summary>
		/// Allows (read-only) access to the currently used debugger options from other AddIns.
		/// </summary>
		IDebuggerOptions Options {
			get;
		}
		
		/// <summary>
		/// Returns true if debugger is loaded.
		/// </summary>
		bool IsDebuggerLoaded {
			get;
		}
		
		bool IsDebuggerStarted {
			get;
		}
		
		/// <summary>
		/// Returns true if debugger is attached to a process
		/// </summary>
		bool IsDebugging {
			get;
		}
		
		/// <summary>
		/// Returns true if process is running
		/// Returns false if breakpoint is hit, program is breaked, program is stepped, etc...
		/// </summary>
		bool IsProcessRunning {
			get;
		}
		
		/// <summary>
		/// Gets or sets whether the debugger should break at the first line of execution.
		/// </summary>
		bool BreakAtBeginning {
			get; set;
		}
		
		bool IsAttached {
			get;
		}
		
		bool CanDebug(IProject project);
		
		bool Supports(DebuggerFeatures feature);
		
		/// <summary>
		/// Starts process and attaches debugger
		/// </summary>
		void Start(ProcessStartInfo processStartInfo);
		
		void StartWithoutDebugging(ProcessStartInfo processStartInfo);
		
		/// <summary>
		/// Stops/terminates attached process
		/// </summary>
		void Stop();
		
		// ExecutionControl:
		
		void Break();
		
		void Continue();
		
		// Stepping:
		
		void StepInto();
		
		void StepOver();
		
		void StepOut();
		
		/// <summary>
		/// Shows a dialog so the user can attach to a process.
		/// </summary>
		void ShowAttachDialog();
		
		/// <summary>
		/// Used to attach to an existing process.
		/// </summary>
		void Attach(Process process);
		
		void Detach();
		
		/// <summary>
		/// Set the instruction pointer to a given position.
		/// </summary>
		/// <returns>True if successful. False otherwise</returns>
		bool SetInstructionPointer(string filename, int line, int column, bool dryRun);
		
		void ToggleBreakpointAt(ITextEditor editor, int lineNumber);
		
		void RemoveCurrentLineMarker();
		
		void JumpToCurrentLine(string sourceFullFilename, int startLine, int startColumn, int endLine, int endColumn);
		
		/// <summary>
		/// Ocurrs when the debugger is starting.
		/// </summary>
		event EventHandler DebugStarting;
		
		/// <summary>
		/// Ocurrs after the debugger has started.
		/// </summary>
		event EventHandler DebugStarted;
		
		/// <summary>
		/// Ocurrs when the value of IsProcessRunning changes.
		/// </summary>
		event EventHandler IsProcessRunningChanged;
		
		/// <summary>
		/// Ocurrs after the debugging of program is finished.
		/// </summary>
		event EventHandler DebugStopped;
	}
	
	/// <summary>
	/// Abstraction of some debugger options.
	/// Allows (read-only) access to the currently used debugger options from other AddIns.
	/// </summary>
	public interface IDebuggerOptions
	{
		bool EnableJustMyCode { get; }
		bool SuppressJITOptimization { get; }
		bool SuppressNGENOptimization { get; }
		bool StepOverDebuggerAttributes { get; }
		bool StepOverAllProperties { get; }
		bool StepOverFieldAccessProperties { get; }
		IEnumerable<string> SymbolsSearchPaths { get; }
		bool PauseOnHandledExceptions { get; }
	}
	
	public enum DebuggerFeatures
	{
		Start,
		StartWithoutDebugging,
		Stop,
		ExecutionControl,
		Stepping,
		Attaching,
		Detaching
	}
	
	/// <summary>
	/// Provides the default debugger tooltips on the text area.
	/// </summary>
	/// <remarks>
	/// This class must be public because it is accessed via the AddInTree.
	/// </remarks>
	public class DebuggerTextAreaToolTipProvider : ITextAreaToolTipProvider
	{
		public void HandleToolTipRequest(ToolTipRequestEventArgs e)
		{
			if (SD.Debugger.IsDebuggerLoaded)
				SD.Debugger.HandleToolTipRequest(e);
		}
	}
	
	sealed class DummyDebuggerOptions : IDebuggerOptions
	{
		DummyDebuggerOptions() {}
		
		public bool EnableJustMyCode {
			get { return true; }
		}
		public bool SuppressJITOptimization {
			get { return false; }
		}
		public bool SuppressNGENOptimization {
			get { return false; }
		}
		public bool StepOverDebuggerAttributes {
			get { return false; }
		}
		public bool StepOverAllProperties {
			get { return false; }
		}
		public bool StepOverFieldAccessProperties {
			get { return false; }
		}
		public IEnumerable<string> SymbolsSearchPaths {
			get { return EmptyList<string>.Instance; }
		}
		public bool PauseOnHandledExceptions {
			get { return false; }
		}
		public IEnumerable<string> ExceptionFilterList {
			get { return EmptyList<string>.Instance; }
		}
		
		public static readonly DummyDebuggerOptions Instance = new DummyDebuggerOptions();
	}
	
	class DebuggerServiceFallback : BaseDebuggerService
	{
		Process attachedProcess = null;
		
		public override bool IsDebugging {
			get {
				return attachedProcess != null;
			}
		}
		
		public override bool IsProcessRunning {
			get {
				return IsDebugging;
			}
		}
		
		/// <inheritdoc/>
		public override bool BreakAtBeginning {
			get; set;
		}
		
		public override bool CanDebug(IProject project)
		{
			return true;
		}
		
		public override bool Supports(DebuggerFeatures feature)
		{
			switch (feature) {
				case DebuggerFeatures.Start:
				case DebuggerFeatures.StartWithoutDebugging:
				case DebuggerFeatures.Stop:
					return true;
				case DebuggerFeatures.ExecutionControl:
				case DebuggerFeatures.Stepping:
				case DebuggerFeatures.Attaching:
				case DebuggerFeatures.Detaching:
					return false;
				default:
					throw new ArgumentOutOfRangeException();
		}
		}
		
		public override void Start(ProcessStartInfo processStartInfo)
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
		
		public override void ShowAttachDialog()
		{
		}
		
		public override void Attach(Process process)
		{
		}
		
		public override void Detach()
		{
		}
		
		void AttachedProcessExited(object sender, EventArgs e)
		{
			attachedProcess.Exited -= AttachedProcessExited;
			attachedProcess.Dispose();
			attachedProcess = null;
			SD.MainThread.InvokeAsyncAndForget(() => new Action<EventArgs>(OnDebugStopped)(EventArgs.Empty));
		}
		
		public override void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			Process.Start(processStartInfo);
		}
		
		public override void Stop()
		{
			if (attachedProcess != null) {
				attachedProcess.Exited -= AttachedProcessExited;
				attachedProcess.Kill();
				attachedProcess.Close();
				attachedProcess.Dispose();
				attachedProcess = null;
			}
		}
		
		// ExecutionControl:
		
		public override void Break()
		{
			throw new NotSupportedException();
		}
		
		public override void Continue()
		{
			throw new NotSupportedException();
		}
		// Stepping:
		
		public override void StepInto()
		{
			throw new NotSupportedException();
		}
		
		public override void StepOver()
		{
			throw new NotSupportedException();
		}
		
		public override void StepOut()
		{
			throw new NotSupportedException();
		}
		
		public override void HandleToolTipRequest(ToolTipRequestEventArgs e)
		{
		}
		
		public override bool SetInstructionPointer(string filename, int line, int column, bool dryRun)
		{
			return false;
		}
		
		public override void Dispose()
		{
			Stop();
			base.Dispose();
		}
		
		public override bool IsAttached {
			get {
				return false;
			}
		}
		
		public override void RemoveCurrentLineMarker()
		{
		}
		
		public override void ToggleBreakpointAt(ITextEditor editor, int lineNumber)
		{
		}
	}
}
