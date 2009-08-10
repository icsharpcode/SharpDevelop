// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Threading;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// Invokes an action when it is disposed.
	/// </summary>
	/// <remarks>
	/// This class ensures the callback is invoked at most once,
	/// even when Dispose is called on multiple threads.
	/// </remarks>
	sealed class CallbackOnDispose : IDisposable
	{
		Action action;
		
		public CallbackOnDispose(Action action)
		{
			Debug.Assert(action != null);
			this.action = action;
		}
		
		public void Dispose()
		{
			Action a = Interlocked.Exchange(ref action, null);
			if (a != null) {
				a();
			}
		}
	}
}
