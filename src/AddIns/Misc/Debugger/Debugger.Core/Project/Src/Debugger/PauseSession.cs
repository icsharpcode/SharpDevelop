// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// Holds information about the state of paused debugger.
	/// Expires when when Continue is called on debugger.
	/// </summary>
	public class PauseSession: IExpirable
	{
		PausedReason pausedReason;
		
		bool hasExpired = false;
		
		public event EventHandler Expired;
		
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
		
		public PausedReason PausedReason {
			get {
				return pausedReason;
			}
		}
		
		public PauseSession(PausedReason pausedReason)
		{
			this.pausedReason = pausedReason;
		}
	}
}
