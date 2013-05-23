// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// IDisposable implementation that disposes multiple disposables.
	/// </summary>
	public class CompositeDisposable : IDisposable
	{
		IDisposable[] disposables;
		
		public CompositeDisposable(params IDisposable[] disposables)
		{
			if (disposables == null)
				throw new ArgumentNullException("disposables");
			this.disposables = disposables;
		}
		
		public void Dispose()
		{
			foreach (var disposable in disposables) {
				if (disposable != null)
					disposable.Dispose();
			}
		}
	}
}
