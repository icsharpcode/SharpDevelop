// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Invokes a callback when this class is dispsed.
	/// </summary>
	sealed class CallbackOnDispose : IDisposable
	{
		// TODO: in 4.0, use System.Action and make this class public
		System.Threading.ThreadStart callback;
		
		public CallbackOnDispose(System.Threading.ThreadStart callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			this.callback = callback;
		}
		
		public void Dispose()
		{
			System.Threading.ThreadStart action = Interlocked.Exchange(ref callback, null);
			if (action != null)
				action();
		}
	}
}
