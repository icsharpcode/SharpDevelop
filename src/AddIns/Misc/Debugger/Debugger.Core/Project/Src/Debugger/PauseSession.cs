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
	public class PauseSession
	{
		PausedReason pausedReason;
		
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
