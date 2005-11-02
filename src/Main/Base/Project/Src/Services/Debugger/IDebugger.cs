// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

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
		/// Gets the current value of the variable as string that can be displayed in tooltips.
		/// </summary>
		string GetValueAsString(string variable);
		
		/// <summary>
		/// Gets the tooltip control that shows the value of given variable.
		/// Return null if no tooltip is available.
		/// </summary>
		DebuggerGridControl GetTooltipControl(string variable);
		
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
