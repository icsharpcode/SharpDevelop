// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public interface IDebugger : IDisposable, ITextAreaToolTipProvider
	{
		/// <summary>
		/// Returns true if debuger is attached to a process
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
}
