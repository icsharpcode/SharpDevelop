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
			if (action != null)
				action();
		}
	}
}
