// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	/// <summary>
	/// Unique identifier of the state of the debugee.
	/// Changes when debuggee is stepped, but not when properity is evaluated.
	/// </summary>
	public class DebugeeState: IExpirable, IMutable
	{
		Process process;
		bool hasExpired = false;
		
		public event EventHandler Expired;
		
		public event EventHandler<ProcessEventArgs> Changed {
			add {
				process.DebuggeeStateChanged += value;
			}
			remove {
				process.DebuggeeStateChanged -= value;
			}
		}
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		public bool HasExpired {
			get {
				return hasExpired;
			}
		}
		
		internal void NotifyHasExpired()
		{
			if(!hasExpired) {
				hasExpired = true;
				if (Expired != null) {
					Expired(this, EventArgs.Empty);
				}
			}
		}
		
		public DebugeeState(Process process)
		{
			this.process = process;
		}
	}
}
