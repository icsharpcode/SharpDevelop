using System;
using ICSharpCode.SharpDevelop.Project;
using System.Diagnostics;

namespace ICSharpCode.Core 
{
	public interface IDebugger : IDisposable
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
		
		bool CanDebug(IProject project);

		/// <summary>
		/// Starts process and attaches debugger
		/// </summary>
		void Start(ProcessStartInfo processStartInfo);

		bool SupportsStart {
			get;
		}

		/// <summary>
		/// Starts process and does not attach debugger
		/// </summary>
		void StartWithoutDebugging(ProcessStartInfo processStartInfo);

		bool SupportsStartWithoutDebugging {
			get;
		}

		/// <summary>
		/// Stops/terminates attached process
		/// </summary>
		void Stop();

		bool SupportsStop {
			get;
		}
		
		// ExecutionControl:
		
		void Break();
		
		void Continue();

		bool SupportsExecutionControl {
			get;
		}

		// Stepping:

		void StepInto();

		void StepOver();

		void StepOut();

		bool SupportsStepping {
			get;
		}
		
		/// <summary>
		/// Gets the current value of the variable as string that can be displayed in tooltips.
		/// </summary>
		string GetValueAsString(string variable);

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
