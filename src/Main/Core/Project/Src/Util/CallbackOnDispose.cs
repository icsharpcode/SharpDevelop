// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Threading;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Invokes a callback when this class is disposed.
	/// </summary>
	public sealed class CallbackOnDispose : IDisposable
	{
		Action callback;
		
		public CallbackOnDispose(Action callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			this.callback = callback;
		}
		
		public void Dispose()
		{
			Action action = Interlocked.Exchange(ref callback, null);
			if (action != null) {
				action();
				#if DEBUG
				GC.SuppressFinalize(this);
				#endif
			}
		}
		
		#if DEBUG
		~CallbackOnDispose()
		{
			Debug.Fail("CallbackOnDispose was finalized without being disposed.");
		}
		#endif
	}
}
